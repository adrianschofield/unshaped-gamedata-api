# This script gets all the data from Steam and then uploads it to the API

# Globals
$SteamBaseUri = "https://api.steampowered.com/"
$ApiBaseUri = "https://app-gamedata-westeurope-dev-001.azurewebsites.net/api/"

# Functions

# Steam

# It appears that even with large sets of data no pagination is required
# I have 480 games in steam and they are all returned at once
function GetDataFromSteam($key, $steamId) {

    $uri = $SteamBaseUri + "IPlayerService/GetOwnedGames/v0001/?key=" + $key + "&steamid=" + $steamId + "&include_appinfo=1&format=json"
    $response = $null
    try {
        $response = Invoke-WebRequest -Uri $uri -Method Get
    } catch {
        Write-Host "Failed with an error " + $_.ErrorDetails.Message
    }
    if ($null -ne $response) {
        $result = $response.Content | ConvertFrom-Json -Depth 5
        return $result
    } else {
        return $null
    }
}

# API

function GetGameData($apiKey) {

    $uri = $ApiBaseUri + "gamedata"
    $headers = @{"Content-Type" = "application/json"; "x-api-key" = $apiKey}
    $results = $null

    try {
        $results = Invoke-RestMethod -Uri $uri -Method Get -Headers $headers
    } catch {
        Write-Host "Failed with an error " + $_.ErrorDetails.Message
    }
    return $results

}

function CreateJsonObjectForApi($name, $platform, $timePlayed, $like) {
    
    $jsonBase = @{}
    $jsonBase.Add("name", $name)
    $jsonBase.Add("platform", $platform)
    $jsonBase.Add("timePlayed", $timePlayed)
    $jsonBase.Add("like", $like)

    # We need to calculate some values
    $minutesPlayed = $timePlayed % 60
    $hoursPlayed = ($timePlayed - $minutesPlayed) / 60

    $jsonBase.Add("hours", $hoursPlayed)
    $jsonBase.Add("minutes", $minutesPlayed)

    # Now set some defaults
    $jsonBase.Add("current", $false)
    $jsonBase.Add("completed", $false)
    $jsonBase.Add("multiplayer", $false)
    
    return $jsonBase

}

function UpdateJsonObjectForApi($entry) {
    # Using this function means that the time played data needs to be updated
    $entry.minutes = $entry.timePlayed % 60
    $entry.hours = ($entry.timePlayed - $entry.minutes) / 60
}

# Create a new entry in the API
function PostGameDataApi($entry, $apiKey) {
    $uri = $ApiBaseUri + "gamedata"
    # And we need Headers making sure to add API Key
    $headers = @{"Content-Type" = "application/json"; "x-api-key" = $apiKey}

    try {
        Invoke-RestMethod -Uri $uri -Method Post -Body ($entry | ConvertTo-Json -Depth 2) -Header $headers
    } catch {
        Write-Host "Failed create new entry with error " + $_.ErrorDetails.Message
    }
}

# Update an existing entry in the API
function PutGameDataApi($entry, $apiKey) {
    # Note uri requires the id of the object to be updated
    $uri = $ApiBaseUri + "gamedata/" + $entry.id

    # And we need Headers making sure to add API Key
    $headers = @{"Content-Type" = "application/json"; "x-api-key" = $apiKey}

    try {
        Invoke-RestMethod -Uri $uri -Method Put -Body ($entry | ConvertTo-Json -Depth 2) -Header $headers
    } catch {
        Write-Host "Failed update existing entry with error " + $_.ErrorDetails.Message
    }
}

# Utils
function LoadConfig() {
    $result = Get-Content -Path .\config.json | ConvertFrom-Json
    return $result
}

# Main

# First thing I need to do is load the configuration for the API Token
$config = LoadConfig

# We need to retrieve the api key from Azure Key Vault rather than the configuration file
$apiKey = Get-AzKeyVaultSecret -VaultName $config.azKeyVault -Name $config.azKeyName -AsPlainText

# Let's get the info from Steam
$steamResults = GetDataFromSteam -key $config.steamKey -steamId $config.steamId

# And read data from the API
$apiResults = GetGameData -apiKey $apiKey

# Now I need to loop through the steam data
# Find each game in the API Data
# If the game doesn't exist add it to the API
# If the game is different then update the API

foreach ($steamResult in $steamResults.response.games) {
    $matched = $false
    foreach($apiResult in $apiResults){
        if ($steamResult.name -eq $apiResult.name) {
            # Found a match
            $matched = $true
            # Check for changes, the only thing that would really change is Time Played
            if ($steamResult.playtime_forever -gt $apiResult.timePlayed) {
                # DBG
                Write-Host "Would update"  $apiResult.name "because times are different:"  $apiResult.timePlayed " :" $steamResult.playtime_forever
                $apiResult.timePlayed = $steamResult.playtime_forever
                UpdateJsonObjectForApi -entry $apiResult
                $result = PutGameDataApi -entry $apiResult -apiKey $apiKey
            }
        }
    }

    if ($matched -eq $false) {
        # DBG
        Write-Host "Didn't find an entry in the API for" $steamResult.name
        # Update the API as this Steam game was not found
        $apiObject = CreateJsonObjectForApi -name $steamResult.name -platform "PC" -timePlayed $steamResult.playtime_forever -like $true
        $result = PostGameDataApi -entry $apiObject -apiKey $apiKey
    }
}


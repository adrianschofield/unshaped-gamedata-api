# This script gets all the data from Steam and then uploads it to the API

# Globals
$SteamBaseUri = "https://api.steampowered.com/"
$ApiBaseUri = "https://localhost:7011/api/"

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

function PutGameDataApi($entry, $apiKey) {
    $uri = $ApiBaseUri + "gamedata"
    # And we need Headers making sure to add Notion-Version
    $headers = @{"Content-Type" = "application/json"; "x-api-key" = $apiKey}

    try {
        Invoke-RestMethod -Uri $uri -Method Post -Body ($entry | ConvertTo-Json -Depth 2) -Header $headers
    } catch {
        Write-Host "Failed with an error " + $_.ErrorDetails.Message
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

# I also need the api key get it from AWS in this case
$awsSecret = Get-SECSecretValue -SecretId Development_unshaped.gamedata.api_Authentication__ApiKey
$apiKey = $awsSecret.SecretString

# Let's get the info from Steam
$steamResults = GetDataFromSteam -key $config.steamKey -steamId $config.steamId

# Now I need to loop through that lot and create an API object for each one
# and then PUT it into the database

foreach ($steamResult in $steamResults.response.games) {
    $apiObject = CreateJsonObjectForApi -name $steamResult.name -platform "PC" -timePlayed $steamResult.playtime_forever -like $true
    $result = PutGameDataApi -entry $apiObject -apiKey $apiKey
}


# PowerShell script to get data from this REST API

# Globals
$BaseUri = "https://localhost:7011/api/"

# Functions

function LoadConfig() {
    $result = Get-Content -Path .\config.json | ConvertFrom-Json
    return $result
}

function GetGameData($apiKey) {

    $uri = $BaseUri + "gamedata"
    $headers = @{"Content-Type" = "application/json"; "x-api-key" = $apiKey}
    $results = $null

    try {
        $results = Invoke-RestMethod -Uri $uri -Method Get -Headers $headers
    } catch {
        Write-Host "Failed with an error " + $_.ErrorDetails.Message
    }
    return $results

}

function CreateJsonObject($name, $platform, $timePlayed, $like) {
    $jsonBase = @{}
    $jsonBase.Add("name", $name)
    $jsonBase.Add("platform", $platform)
    $jsonBase.Add("timePlayed", $timePlayed)
    $jsonBase.Add("like", $like)

    # We need to caluclate some values
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

function PutGameData($entry, $apiKey) {
    $uri = $BaseUri + "gamedata"
    # And we need Headers making sure to add Notion-Version
    $headers = @{"Content-Type" = "application/json"; "x-api-key" = $apiKey}

    try {
        Invoke-RestMethod -Uri $uri -Method Post -Body ($entry | ConvertTo-Json -Depth 2) -Header $headers
    } catch {
        Write-Host "Failed with an error " + $_.ErrorDetails.Message
    }
}

# Main

$config = LoadConfig

# Just read data from the API
$results = GetGameData -apiKey $config.apiKey
foreach ($result in $results) {
    Write-Host $result.name
}

# Let's try writing an entry into the database
$name = "Hogwarts Legacy"
$platform = "PC"
$timePlayed = 350
$like = $true

# $entry = CreateJsonObject -name $name -platform $platform -timePlayed $timePlayed -like $like
# $results = PutGameData -entry $entry -apiKey - $config.apiKey
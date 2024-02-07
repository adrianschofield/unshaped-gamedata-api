# PowerShell script to get data from Steam

# TODO
# Hmmm there's a weird issue where this script spits out s1` every time it runs

# Globals

$BaseUri = "https://api.steampowered.com/"

# Functions

function LoadConfig() {
    $result = Get-Content -Path .\config.json | ConvertFrom-Json
    return $result
}

# It appears that even with large sets of data no pagination is required
# I have 480 games in steam and they are all returned at once
function GetDataFromSteam($key, $steamId) {
    $uri = $BaseUri + "IPlayerService/GetOwnedGames/v0001/?key=" + $key + "&steamid=" + $steamId + "&include_appinfo=1&format=json"
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

# Main

# First thing I need to do is load the configuration for the API Token
$config = LoadConfig

$results = GetDataFromSteam -key $config.steamKey -steamId $config.steamId

if ($null -ne $results) {
    foreach ($result in $results.response.games) {
        Write-Host $result.name
    }
} else {
    Write-Host "Response was null"
}

# DBG
Write-Host $results.response.games.count
Write-Host $results.response.game_count

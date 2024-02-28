namespace unshaped_gamedata_api.Authentication;

public static class AuthConstants
{
    // Path to the apikey in configuration
    public const string ApiKeySectionName = "Authentication:ApiKey";
    // Path to the apiKey from Azure Key Vault
    public const string AzureApiKeySectionName = "gamedata-apikey";
    // Header name - actually case insensitive
    public const string ApiKeyHeaderName = "X-Api-Key";
}

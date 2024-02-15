using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace unshaped_gamedata_api.Authentication;

public class ApiKeyAuthFilter : IAuthorizationFilter
{
    private readonly IConfiguration _configuration;

    public ApiKeyAuthFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    void IAuthorizationFilter.OnAuthorization(AuthorizationFilterContext context)
    {
        // Check if the header is present, if not return 401 and an error
        if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey)) {
            context.Result = new UnauthorizedObjectResult("API Key missing");
            return;
        }

        // Get the actual api key from AWS from configuration
        var apiKey = _configuration.GetValue<string>(AuthConstants.AzureApiKeySectionName);

        //DBG - this should never happen - here for troubleshooting
        if (null == apiKey) {
            context.Result = new UnauthorizedObjectResult("Invalid Configuration");
            return;
        }

        // Check if the api keys match, if not return 401
        if (null != apiKey && !apiKey.Equals(extractedApiKey)) {
            context.Result = new UnauthorizedObjectResult("Invalid API Key");
            return;
        }
    }
}

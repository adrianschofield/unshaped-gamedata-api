namespace unshaped_gamedata_api.Authentication;

public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public ApiKeyAuthMiddleware(RequestDelegate next, IConfiguration configuration) {
        // This is standard practce and denotes the next item in the middleware pipeline
        _next = next;
        // We need configuration, AWS secrets are injected to make access easy
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context) {

        // Check if the header is present, if not return 401 and an error
        if (!context.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey)) {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key missing");
            return;
        }

        // Get the actual api key from AWS from configuration
        var apiKey = _configuration.GetValue<string>(AuthConstants.ApiKeySectionName);
        // Check if the api keys match, if not return 401
        if (null != apiKey && !apiKey.Equals(extractedApiKey)) {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid API Key");
            return;
        }

        // If all is good continue middleware
        await _next(context);
    }
}

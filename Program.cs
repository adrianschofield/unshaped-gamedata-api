using Amazon;
using Microsoft.Build.Framework;
using unshaped_gamedata_api.Authentication;
using unshaped_gamedata_api.Data;



var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Games") ?? "Data Source=Data/Games.db";
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSqlite<DatabaseContext>(connectionString);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add support for AWS Secrets Manager
var env = builder.Environment.EnvironmentName;
var appName = builder.Environment.ApplicationName;

// replace dashes with dots (AWS Secrets manager does not allow dash)
appName = appName.Replace("-", ".");

// add the secrets manager to the Configuration to allow the secrets to be
// accessed via simple configuration calls

builder.Configuration.AddSecretsManager(region: RegionEndpoint.EUWest2,
    configurator: options =>
    {
        // Filter out any unrequired secrets
        options.SecretFilter = entry => entry.Name.StartsWith($"{env}_{appName}_");
        // provide secrets in Configuration format e.g. Database:ConnectionString
        options.KeyGenerator = (entry, s) =>    s
            .Replace($"{env}_{appName}_", string.Empty)
            .Replace("__", ":");
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// This is my api key middleware
app.UseMiddleware<ApiKeyAuthMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

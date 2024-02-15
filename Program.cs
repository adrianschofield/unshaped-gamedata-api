using Microsoft.Build.Framework;
using unshaped_gamedata_api.Authentication;
using unshaped_gamedata_api.Data;
using System.Security.Cryptography.X509Certificates;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// ConnectionString for access to SQLite database
var connectionString = builder.Configuration.GetConnectionString("Games") ?? "Data Source=Data/Games.db";

// Add Azure Key Vault secrets to configuration

using var x509Store = new X509Store(StoreLocation.CurrentUser);

x509Store.Open(OpenFlags.ReadOnly);

var x509Certificate = x509Store.Certificates
    .Find(
        X509FindType.FindByThumbprint,
        builder.Configuration["Authentication:AzureADCertThumbprint"],
        validOnly: false)
    .OfType<X509Certificate2>()
    .Single();

builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{builder.Configuration["Authentication:KeyVaultName"]}.vault.azure.net/"),
    new ClientCertificateCredential(
        builder.Configuration["Authentication:AzureADDirectoryId"],
        builder.Configuration["Authentication:AzureADApplicationId"],
        x509Certificate));

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

builder.Services.AddScoped<ApiKeyAuthFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// This is my api key middleware
// app.UseMiddleware<ApiKeyAuthMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

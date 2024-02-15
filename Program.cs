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

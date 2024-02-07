# Game Data API 

This is a simple REST API built with .Net to store information about Games I own in Steam
and other platforms.

The data is stored in an SQLite database for ease of use.


## Manual testing

To make a GET you can use this URL

https://localhost:7011/api/gamedata

/Tools/Powershell contains PowerShell scripts for more detailed testing

## Authentication

This is the first time I've implemented this and it's been an interesting journey

Step 1

Create a Middleware (everything under /Authentication) to handle managing an API request
and ensure that the header x-api-key is present and has the correct value. Initially the correct
value is stored in appsettings. The way I have this set up it applies to all routes on all controllers,
it's possible to filter this

Step 2

Update the powershell scripts to handle the authentication method, pulling the key from a local
file

Step 3 - AWS

- Create the key in AWS secrets manager
- Create a group that has access to Secrets Manager using the default policy
- Create a new user in this group and create an Access Key for this user
- Install AWS Cli
- run aws configure and supply the relevant values

```
aws configure
AWS Access Key ID [****************E2U5]: 
AWS Secret Access Key [****************sYwy]: 
Default region name [eu-west-2]:
Default output format [None]:
```
Step 4 

Allow PowerShell to access secrets from AWS Secrets Manager

```
Install-Module -Name AWS.Tools.SecretsManager
Get-SECSecretValue -SecretId Development_unshaped.gamedata.api_Authentication__ApiKey
```

## Steps to initialise project

Just for my notes that are some of the steps I used to create the project

dotnet new webapi --use-controllers -o unshaped-gamedata-api

dotnet add package Microsoft.EntityFrameworkCore.Sqlite

dotnet tool install --global dotnet-ef

or to upgrade

dotnet tool update --global dotnet-ef

dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools

I had to manually add the DBContext

dotnet ef migrations add InitialCreate
dotnet ef database update

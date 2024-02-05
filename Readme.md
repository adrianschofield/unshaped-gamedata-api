## Steps to initialise project

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

## Manual testing
http://localhost:5094/api/gamedata
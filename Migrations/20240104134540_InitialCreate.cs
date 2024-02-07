using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace unshaped_gamedata_api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Platform = table.Column<string>(type: "TEXT", nullable: true),
                    TimePlayed = table.Column<int>(type: "INTEGER", nullable: true),
                    Hours = table.Column<int>(type: "INTEGER", nullable: true),
                    Minutes = table.Column<int>(type: "INTEGER", nullable: true),
                    Like = table.Column<bool>(type: "INTEGER", nullable: true),
                    Current = table.Column<bool>(type: "INTEGER", nullable: true),
                    Completed = table.Column<bool>(type: "INTEGER", nullable: true),
                    MultiPlayer = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameData", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameData");
        }
    }
}

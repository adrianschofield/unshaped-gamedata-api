using Microsoft.EntityFrameworkCore;
using unshaped_gamedata_api.Models;

namespace unshaped_gamedata_api.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options) { }
        public DbSet<GameData> GameData { get; set; }

        public string DbPath { get; }

        /*public DatabaseContext() {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = "api/Data/UserData.db";

        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
             => options.UseSqlite($"Data Source={DbPath}");*/
    }
}
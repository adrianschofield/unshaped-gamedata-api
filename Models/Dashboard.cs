using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace unshaped_gamedata_api.Models 
{
    public class Dashboard
    {
        public int GamesTotal { get; set; }
        public int? GamesPC { get; set; }
        public int? GamesXbox { get; set; }
        public int? GamesPlayStation  { get; set; }
        public int? GamesLessThanOne { get; set; }
        public int? GamesLessThanTen { get; set; }
        public int? GamesMoreThanTen { get; set; }
        public List<string> CurrentGames { get; set; }

        // Constructor

        public Dashboard() {
            // Initialise everything to zero
            GamesTotal = 0;
            GamesPC = 0;
            GamesXbox = 0;
            GamesPlayStation = 0;
            GamesLessThanOne = 0;
            GamesLessThanTen = 0;
            GamesMoreThanTen = 0;
            CurrentGames = new List<string>();
        }
    }
}

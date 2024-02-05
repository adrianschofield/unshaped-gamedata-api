using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace unshaped_gamedata_api.Models 
{
    public class GameData
    {
          public int Id { get; set; }
          [Required]
          public string? Name { get; set; }
          [Required]
          [AllowedValues("PC", "Xbox", "Playstation")]
          public string? Platform { get; set; }
          [Required]
          public int? TimePlayed { get; set; }
          public int? Hours { get; set; }
          public int? Minutes { get; set; }
          [Required]
          public bool? Like { get; set; }
          public bool? Current { get; set; } = false;
          public bool? Completed { get; set; } = false;
          public bool? MultiPlayer { get; set; } = false;
    }
}
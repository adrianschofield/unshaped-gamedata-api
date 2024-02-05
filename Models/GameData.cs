namespace unshaped_gamedata_api.Models 
{
    public class GameData
    {
          public int Id { get; set; }
          public string? Name { get; set; }
          public string? Platform { get; set; }
          public int? TimePlayed { get; set; }
          public int? Hours { get; set; }
          public int? Minutes { get; set; }
          public bool? Like { get; set; }
          public bool? Current { get; set; }
          public bool? Completed { get; set; }
          public bool? MultiPlayer { get; set; }
    }
}
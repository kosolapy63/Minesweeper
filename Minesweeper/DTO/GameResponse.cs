using Newtonsoft.Json;
namespace Minesweeper.DTO;

public class GameResponse
{
    [JsonProperty("game_id")]
    public Guid GameId { get; set; }

    [JsonProperty("width")]
    public int Width { get; set; }

    [JsonProperty("height")]
    public int Height { get; set; }

    [JsonProperty("mines_count")]
    public int MinesCount { get; set; }

    [JsonProperty("field")]
    public List<List<string>> Field { get; set; }

    [JsonProperty("completed")]
    public bool Completed { get; set; }
}

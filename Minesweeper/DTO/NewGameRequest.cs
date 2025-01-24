using Newtonsoft.Json;

namespace Minesweeper.DTO
{
    public record NewGameRequest
    {
        [JsonProperty("width")]
        public int Width { get; init; }

        [JsonProperty("height")]
        public int Height { get; init; }

        [JsonProperty("mines_count")]
        public int MinesCount { get; init; }
    }
}

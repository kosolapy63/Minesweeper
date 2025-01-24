using Newtonsoft.Json;

namespace Minesweeper.DTO
{
    public record GameTurnRequest
    {
        [JsonProperty("game_id")]
        public Guid GameId { get; init; }

        [JsonProperty("col")]
        public int Collumn { get; init; }

        [JsonProperty("row")]
        public int Row { get; init; }
    }
}

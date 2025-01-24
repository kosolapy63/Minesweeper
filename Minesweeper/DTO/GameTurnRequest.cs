namespace Minesweeper.DTO
{
    public record GameTurnRequest
    {
        public Guid game_id { get; init; }
        public int col { get; init; }
        public int row { get; init; }
    }
}

namespace Minesweeper.DTO
{
    public record NewGameRequest
    {
        public int width { get; init; }
        public int height { get; init; }
        public int mines_count { get; init; }
    }
}

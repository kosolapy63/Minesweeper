namespace Minesweeper.DTO
{
    public record NewGameRequest
    {
        public int width;
        public int height;
        public int mines_count;
    }
}

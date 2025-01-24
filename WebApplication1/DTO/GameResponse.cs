namespace Minesweeper.DTO;

public class GameResponse
{
    public Guid game_id { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public int mines_count { get; set; }
    public char[,] field { get; set; }
    public bool completed { get; set; }
}

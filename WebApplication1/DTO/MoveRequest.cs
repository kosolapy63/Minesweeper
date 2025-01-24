namespace Minesweeper.DTO;

public class MoveRequest
{
    public Guid game_id { get; set; }
    public int row { get; set; }
    public int col { get; set; }
}

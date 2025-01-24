namespace Minesweeper.DTO;

public class Game
{
    public Guid GameId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Mines_count { get; set; }
    public Cell[,] Field {  get; set; }
    public int CountNotOpenedCell { get; set; }
    public bool Completed { get; set; }
    public DateTime DateAdd { get; set; }

    public Game(int width, int height, int mines_count, Cell[,] field)
    {
        GameId = Guid.NewGuid();
        this.Width = width;
        this.Height = height;
        this.Mines_count= mines_count;
        this.Field = field;
        Completed = false;
        CountNotOpenedCell = width * height - mines_count;
        DateAdd = DateTime.UtcNow;
    }    
}




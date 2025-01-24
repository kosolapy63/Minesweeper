namespace Minesweeper.DTO;

public class Game
{
    public Guid game_id { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public int mines_count { get; set; }
    public Cell[,] field {  get; set; }
    public int countNotOpenedCell { get; set; }
    public bool completed { get; set; }
    public DateTime DateAdd { get; set; }

    public Game(int width, int height, int mines_count, Cell[,] field)
    {
        game_id = Guid.NewGuid();
        this.width = width;
        this.height = height;
        this.mines_count= mines_count;
        this.field = field;
        completed = false;
        countNotOpenedCell = width * height - mines_count;
        DateAdd = DateTime.UtcNow;
    }    
}




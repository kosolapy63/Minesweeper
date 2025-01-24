namespace Minesweeper.DTO;

public class Game
{
    public Guid game_id { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public int mines_count { get; set; }
    public char[,] fieldOpened { get; set; }
    public char[,] fieldCalculated { get; set; }
    public int countNotOpenedCell { get; set; }
    public bool completed { get; set; }
    
    public Game(int width, int height, int mines_count, char[,] fieldCalculated)
    {
        game_id = Guid.NewGuid();
        this.width = width;
        this.height = height;
        this.mines_count= mines_count;
        this.fieldCalculated = fieldCalculated;
        fieldOpened = new char[width,height];
        completed = false;
        countNotOpenedCell = width * height - mines_count;
    }    
}




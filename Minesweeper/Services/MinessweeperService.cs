using Microsoft.Extensions.Caching.Memory;
using Minesweeper.DTO;

namespace Minesweeper.Services;

public class MinessweeperService : IMinessweeperService
{
    private const int MaxFieldSize = 30;
    private const int MinFieldSize = 2;    
    private readonly IMemoryCache _cache;
    private static readonly (int dx, int dy)[] Directions = 
   {
        (0, 1),   // Вниз
        (-1, 1),  // Вниз-влево
        (1, 1),   // Вниз-вправо
        (-1, 0),  // Влево
        (1, 0),   // Вправо
        (0, -1),  // Вверх
        (1, -1),  // Вверх-вправо
        (-1, -1)  // Вверх-влево
   };

    public MinessweeperService(IMemoryCache cache)
    {
        _cache = cache;
    }

    private List<List<string>> Convert2DCellArrayTo2DList(Cell[,] field)
    {
        List<List<string>> result = new List<List<string>>();

        for (int i = 0; i < field.GetLength(0); i++)
        {
            List<string> row = new List<string>();
            for (int j = 0; j < field.GetLength(1); j++)
            {
                Cell cell = field[i, j];
                if (cell.Opened)
                {
                    row.Add(cell.Value.ToString()); // Преобразуем каждый символ в строку
                }
                else
                {
                    row.Add(" ");
                }

            }
            result.Add(row); // Добавляем строку в список
        }

        return result;
    }

    public GameResponse StartNewGame(int width, int height, int count_mines)
    {
        if (width > MaxFieldSize || height > MaxFieldSize)
            throw new ArgumentException("Max map size 30x30");
        if (width < MinFieldSize || height < MinFieldSize)
            throw new ArgumentException("Min map size 2x2");

        if (count_mines > width * height - 1)
            throw new ArgumentException("Number of mines > width * height-1");

        Game newGame = new Game(width, height, count_mines, InitField(width, height, count_mines));

        _cache.Set(newGame.GameId, newGame, TimeSpan.FromDays(1));        

        return
            new GameResponse
            {
                GameId = newGame.GameId,
                Width = newGame.Width,
                Height = newGame.Height,
                MinesCount = newGame.Mines_count,
                Field = Convert2DCellArrayTo2DList(newGame.Field),
                Completed = newGame.Completed
            };
    }

    private Cell[,] InitField(int width, int height, int count_mines)
    {
        var field = new Cell[height, width];
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                field[i, j] = new Cell { Value = '0', Opened = false };
            }

        Random random = new Random();
        
        width--;//тк массив от 0 до 9 при длине 10
        height--;
        while (count_mines > 0)
        {
            int randomX = random.Next(width);
            int randomY = random.Next(height);
            if (field[randomX, randomY].Value != 'X')
            {
                field[randomX, randomY].Value = 'X';
                CalculateDigitsAroundMine(width, height, field, randomX, randomY);
                count_mines--;
            }
        }
        return field;
    }

    private void CalculateDigitsAroundMine(int width, int height, Cell[,] field, int x, int y)
    {
        // Проходим по всем соседним клеткам
        foreach (var (dx, dy) in Directions)
        {
            int newX = x + dx;
            int newY = y + dy;

            // Проверяем, что соседняя клетка находится в пределах поля и не содержит мину
            if (newX >= 0 && newX < width && newY >= 0 && newY < height && field[newX, newY].Value != 'X')
            {
                field[newX, newY].Value = IncrementChar(field[newX, newY].Value);
            }
        }
    }

    public GameResponse GameTurn(Guid gameId, int col, int row)
    {
        if (!_cache.TryGetValue(gameId, out Game game))
        {
            throw new ArgumentException("Игра не найдена");
        }             

        if (game.Completed)
            throw new ArgumentException("Игра завершена");

        var turnedChar = game.Field[row, col];
        if (turnedChar.Opened)
            throw new ArgumentException("Ячейка уже открыта");

        if (turnedChar.Value == 'X')
        {
            game.Completed = true;
            PrepareWinOrLoseField(game.Field);
        }

        if (turnedChar.Value != '0')
        {
            turnedChar.Opened = true;
            game.CountNotOpenedCell = game.CountNotOpenedCell - 1;
        }
        else
            OpenThisAndAround(row, col, game);

        if (game.CountNotOpenedCell == 0)
        {
            game.Completed = true;
            PrepareWinOrLoseField(game.Field, true);
            //открыть всё поле
        }

        return
          new GameResponse
          {
              GameId = game.GameId,
              Width = game.Width,
              Height = game.Height,
              MinesCount = game.Mines_count,
              Field = Convert2DCellArrayTo2DList(game.Field),
              Completed = game.Completed
          };
    }

    private void OpenThisAndAround(int row, int col, Game game)
    {
        int width = game.Width - 1;
        int height = game.Height - 1;
        var field = game.Field;

        var currentCell = field[row, col];

        // Открываем текущую ячейку
        if (!currentCell.Opened)
        {
            currentCell.Opened = true;
            game.CountNotOpenedCell = game.CountNotOpenedCell - 1;
        }

        // Рекурсивно обрабатываем соседей
        foreach (var (dx, dy) in Directions)
        {
            int newX = col + dx;
            int newY = row + dy;

            if (newX >= 0 && newX <= width && newY >= 0 && newY <= height)
            {
                var neighbor = field[newY, newX];

                //проваливаемся дальше, если снова ноль, если нет, то открываем цифру(если это цифра)
                if (!neighbor.Opened && neighbor.Value == '0')
                    OpenThisAndAround(newY, newX, game);
                else
                {
                    if (!neighbor.Opened && neighbor.Value != 'X')
                    {
                        neighbor.Opened = true;
                        game.CountNotOpenedCell = game.CountNotOpenedCell - 1;
                    }
                }

            }
        }
    }

    private char IncrementChar(char ch)
    {
        // Если символ является цифрой, увеличиваем его на 1
        if (ch >= '0' && ch <= '7')
        {
            return (char)(ch + 1);
        }
        return ' ';  // Для остальных случаев возвращаем пробел
    }

    private void PrepareWinOrLoseField(Cell[,] field, bool finishByMine = false)
    {
        foreach (var row in field)
        {
            row.Opened = true;
            if (finishByMine)
                if (row.Value == 'X')
                    row.Value = 'M';
        }
    }
}


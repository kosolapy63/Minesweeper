using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Minesweeper.DTO;
using System;
using System.Text;

namespace Minesweeper;

public interface IMinessweeperService
{  
    GameResponse StartNewGame(int width, int height, int count_mines);
    GameResponse GameTurn(Guid gameId, int row, int col);
}

public class MinessweeperService : IMinessweeperService
{
    private const int MaxFieldSize = 30;
    private const int MinFieldSize = 2;
    private readonly Dictionary<Guid, Game> _games;

    public MinessweeperService(Dictionary<Guid, Game> games)
    {
        _games = games;
    }

    public List<List<string>> Convert2DCellArrayTo2DList(Cell[,] field)
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

        _games[newGame.game_id] = newGame;

        return
            new GameResponse
            {
                game_id = newGame.game_id,
                width = newGame.width,
                height = newGame.height,
                mines_count = newGame.mines_count,
                field = Convert2DCellArrayTo2DList(newGame.field),
                completed = newGame.completed
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

        int randomX;
        int randomY;
        width--;//тк массив от 0 до 9 при длине 10
        height--;
        while (count_mines > 0)
        {
            randomX = random.Next(width);
            randomY = random.Next(height);
            if (field[randomX, randomY].Value != 'X')
            {
                field[randomX, randomY].Value = 'X';
                CalculateDigitsAroundMine(width, height, field, randomX, randomY);
                count_mines--;
            }
        }
        return field;
    }
    public void CalculateDigitsAroundMine(int width, int height, Cell[,] field, int x, int y)
    {
        var directions = new (int dx, int dy)[]
        {
            (0, 1),   // Вправо
            (-1, 1),  // Вверх-вправо
            (1, 1),   // Вниз-вправо
            (-1, 0),  // Вверх
            (1, 0),   // Вниз
            (0, -1),  // Влево
            (1, -1),  // Вниз-влево
            (-1, -1)  // Вверх-влево
        };
        // Проходим по всем соседним клеткам
        foreach (var (dx, dy) in directions)
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
        if (!_games.ContainsKey(gameId))
            throw new ArgumentException("Игра не найдена");

        var game = _games[gameId];
        if (game.completed)
            throw new ArgumentException("Игра завершена");

        var turnedChar = game.field[row, col];
        if (turnedChar.Opened)
            throw new ArgumentException("Ячейка уже открыта");

        if (turnedChar.Value == 'X')
        {
            game.completed = true;
            prepareWinOrLoseField(game.field);
        }

        if (turnedChar.Value != '0')
        {
            turnedChar.Opened = true;
            game.countNotOpenedCell = game.countNotOpenedCell - 1;
        }
        else
            OpenThisAndAround(row, col, game);

        if (game.countNotOpenedCell == 0)
        {
            game.completed = true;
            prepareWinOrLoseField(game.field,true);
            //открыть всё поле
        }

        return
          new GameResponse
          {
              game_id = game.game_id,
              width = game.width,
              height = game.height,
              mines_count = game.mines_count,
              field = Convert2DCellArrayTo2DList(game.field),
              completed = game.completed
          };
    }
    public void OpenThisAndAround(int row, int col, Game game)
    {
        int width = game.width - 1;
        int height = game.height - 1;
        var field = game.field;

        //// Проверка границ поля и состояния ячейки
        //if (col < 0 || col > width || row < 0 || row > height || field[col, row].Opened)
        //    return;

        var currentCell = field[row, col];

        // Открываем текущую ячейку
        if (!currentCell.Opened)
        {
            currentCell.Opened = true;
            game.countNotOpenedCell = game.countNotOpenedCell - 1;
        }

        // Направления для соседних ячеек
        var directions = new (int dx, int dy)[]
        {
        (0, 1),   // вниз
        (0, -1),  // вверх
        (1, 0),   // вправо
        (-1, 0),  // влево
        (1, 1),   // вправо-вниз
        (1, -1),  // вправо-вверх
        (-1, 1),  // влево-вниз
        (-1, -1)  // влево-вверх
        };

        // Рекурсивно обрабатываем соседей
        foreach (var (dx, dy) in directions)
        {
            int newX = col + dx;
            int newY = row + dy;

            if (newX >= 0 && newX <= width && newY >= 0 && newY <= height)
            {
                var neighbor = field[newY, newX];

                if (!neighbor.Opened)
                {
                    //проваливаемся дальше, если снова ноль, если нет, то открываем цифру(если это цифра)
                    if (neighbor.Value == '0')
                        OpenThisAndAround(newY, newX, game);
                    else
                    {
                        if (neighbor.Value != 'X')
                        {
                            neighbor.Opened = true;
                            game.countNotOpenedCell = game.countNotOpenedCell - 1;
                        }
                    }


                }

            }
        }
    }
    public char IncrementChar(char ch)
    {
        // Если символ является цифрой, увеличиваем его на 1
        if (ch >= '0' && ch <= '7')
        {
            return (char)(ch + 1);
        }
        return ' ';  // Для остальных случаев возвращаем пробел
    }

    public void prepareWinOrLoseField(Cell[,] field, bool finishByMine=false)
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


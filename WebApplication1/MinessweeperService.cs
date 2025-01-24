using Minesweeper.DTO;
using System;

namespace Minesweeper;

public interface IMinessweeperService
{  
    GameResponse StartNewGame(int width, int height, int count_mines);
}

public class MinessweeperService : IMinessweeperService
{
    private Dictionary<Guid, Game> _games = new Dictionary<Guid, Game>();
    public GameResponse StartNewGame(int width, int height, int count_mines)
    {
        if (width > 30 || height > 30)
            throw new ArgumentException("Max map size 30x30");
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
                field = newGame.fieldCalculated,
                completed = newGame.completed
            };

    }
    private char[,] InitField(int width, int height, int count_mines)
    {
        var field = new char[height, width];
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                field[i, j] = ' ';
        Random random = new Random();

        int randomX;
        int randomY;

        while (count_mines > 0)
        {
            randomX = random.Next(width);
            randomY = random.Next(height);
            if (field[randomX, randomY] == '0')
            {
                field[randomX, randomY] = 'X';
                CalculateDigitsAroundMine(width, height, field, randomX, randomY);
                count_mines--;
            }
        }
        return field;
    }
    public void CalculateDigitsAroundMine(int width, int height, char[,] field, int x, int y)
    {
        if (x < width && (y + 1) < height && field[x, y + 1] != 'X')
            field[x, y + 1] = IncrementChar(field[x, y + 1]);

        if ((x - 1) < width && (y + 1) < height && field[x - 1, y + 1] != 'X')
            field[x - 1, y + 1] = IncrementChar(field[x - 1, y + 1]);

        if ((x + 1) < width && (y + 1) < height && field[x + 1, y + 1] != 'X')
            field[x + 1, y + 1] = IncrementChar(field[x + 1, y + 1]);

        if ((x - 1) < width && y < height && field[x - 1, y] != 'X')
            field[x - 1, y] = IncrementChar(field[x - 1, y]);

        if ((x + 1) < width && y < height && field[x + 1, y] != 'X')
            field[x + 1, y] = IncrementChar(field[x + 1, y]);

        if (x < width && (y - 1) < height && field[x, y - 1] != 'X')
            field[x, y - 1] = IncrementChar(field[x, y - 1]);

        if ((x + 1) < width && (y - 1) < height && field[x + 1, y - 1] != 'X')
            field[x + 1, y - 1] = IncrementChar(field[x + 1, y - 1]);

        if ((x - 1) < width && (y - 1) < height && field[x - 1, y - 1] != 'X')
            field[x - 1, y - 1] = IncrementChar(field[x - 1, y - 1]);
    }


    public Game GameTurn(Guid gameId, int row, int col)
    {
        if (!_games.ContainsKey(gameId))
            throw new ArgumentException("Game not found.");

        var game = _games[gameId];
        if (game.completed == false)
            throw new ArgumentException("Игра завершена");

        var turnedChar = game.fieldCalculated[col, row];

        if (turnedChar == 'X')
        {
            game.completed = true;
            //открыть все мины
        }

        if (turnedChar != '0')
        {
            game.fieldOpened[row, col] = game.fieldCalculated[row, col];
            game.countNotOpenedCell--;
        }
        else
            OpenThisAndAround(row, col, game);

        if (game.countNotOpenedCell == 0)
        {
            game.completed = true;
            //открыть всё поле
        }

        return game;
    }

    public void OpenThisAndAround(int row, int col, Game game)
    {
        int width = game.width;
        int height = game.height;
        var field = game.fieldCalculated;
        game.fieldOpened[row, col] = game.fieldCalculated[row, col];
        game.countNotOpenedCell--;

        if (row < width && (col + 1) < height)
        {
            if (field[row, (col + 1)] != 'X')
            {
                if (field[row, (col + 1)] != '0')
                {
                    game.fieldOpened[row, (col + 1)] = game.fieldCalculated[row, (col + 1)];
                    game.countNotOpenedCell--;
                }
                else
                    OpenThisAndAround(row, (col + 1), game);
            }
        }
        if ((row - 1) < width && (col + 1) < height)
        {
            if (field[(row - 1), (col + 1)] != 'X')
            {
                if (field[(row - 1), (col + 1)] != '0')
                {
                    game.fieldOpened[(row - 1), (col + 1)] = game.fieldCalculated[(row - 1), (col + 1)];
                    game.countNotOpenedCell--;
                }
                else
                    OpenThisAndAround((row - 1), (col + 1), game);
            }
        }
        if ((row + 1) < width && (col + 1) < height)
        {
            if (field[row + 1, (col + 1)] != 'X')
            {
                if (field[row + 1, (col + 1)] != '0')
                {
                    game.fieldOpened[(row + 1), (col + 1)] = game.fieldCalculated[(row + 1), (col + 1)];
                    game.countNotOpenedCell--;
                }
                else
                    OpenThisAndAround((row + 1), (col + 1), game);
            }
        }
        if ((row - 1) < width && col < height)
        {
            if (field[(row - 1), col] != 'X')
            {
                if (field[(row - 1), col] != '0')
                {
                    game.fieldOpened[(row - 1), col] = game.fieldCalculated[(row - 1), col];
                    game.countNotOpenedCell--;
                }
                else
                    OpenThisAndAround((row - 1), col, game);
            }
        }
        if ((row + 1) < width && col < height)
        {
            if (field[(row + 1), col] != 'X')
            {
                if (field[(row + 1), col] != '0')
                {
                    game.fieldOpened[(row + 1), col] = game.fieldCalculated[(row + 1), col];
                    game.countNotOpenedCell--;
                }
                else
                    OpenThisAndAround((row + 1), col, game);
            }
        }
        if (row < width && (col - 1) < height)
        {
            if (field[row, (col - 1)] != 'X')
            {
                if (field[row, (col - 1)] != '0')
                {
                    game.fieldOpened[row, (col - 1)] = game.fieldCalculated[row, (col - 1)];
                    game.countNotOpenedCell--;
                }
                else
                    OpenThisAndAround(row, (col - 1), game);
            }
        }
        if ((row + 1) < width && (col - 1) < height)
        {
            if (field[(row + 1), (col - 1)] != 'X')
            {
                if (field[(row + 1), (col - 1)] != '0')
                {
                    game.fieldOpened[(row + 1), (col - 1)] = game.fieldCalculated[(row + 1), (col - 1)];
                    game.countNotOpenedCell--;
                }
                else
                    OpenThisAndAround((row + 1), (col - 1), game);
            }
        }
        if ((row - 1) < width && (col - 1) < height)
        {
            if (field[(row - 1), (col - 1)] != 'X')
            {
                if (field[(row - 1), (col - 1)] != '0')
                {
                    game.countNotOpenedCell--;
                    game.fieldOpened[(row - 1), (col - 1)] = game.fieldCalculated[(row - 1), (col - 1)];
                }
                else
                    OpenThisAndAround((row - 1), (col - 1), game);
            }
        }

    }

    public char IncrementChar(char ch)
    {
        if (ch == '0')
            return '1';
        if (ch == '1')
            return '2';
        if (ch == '2')
            return '3';
        if (ch == '3')
            return '4';
        if (ch == '4')
            return '5';
        if (ch == '5')
            return '6';
        if (ch == '6')
            return '7';
        if (ch == '7')
            return '8';
        return ' ';
    }


}


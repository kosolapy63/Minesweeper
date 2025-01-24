using Minesweeper.DTO;

namespace Minesweeper.Services;

public interface IMinessweeperService
{
    GameResponse StartNewGame(int width, int height, int count_mines);
    GameResponse GameTurn(Guid gameId, int row, int col);
}


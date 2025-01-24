using Minesweeper.DTO;
using Microsoft.Extensions.Hosting;

public class GameCleanupService : BackgroundService
{
    private readonly Dictionary<Guid, Game> _games;
    private readonly TimeSpan _maxAge = TimeSpan.FromDays(7); 

    public GameCleanupService(Dictionary<Guid, Game> games)
    {
        _games = games;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Периодически проверяем и удаляем старые игры
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);  

            CleanUpOldGames();
        }
    }

    private void CleanUpOldGames()
    {
        var gamesToRemove = _games.Where(g => DateTime.UtcNow - g.Value.DateAdd > _maxAge).ToList();

        foreach (var game in gamesToRemove)
        {
            _games.Remove(game.Key);
            Console.WriteLine($"Удалена старая игра: {game.Key}");
        }
    }
}


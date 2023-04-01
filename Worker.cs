using Dapper;

namespace MapleRoyalsPlayerCount;

public class Worker : BackgroundService
{
    private readonly AppConfig _appConfig;
    private readonly ILogger<Worker> _logger;
    private readonly MapleRoyalsService _mapleRoyalsService;

    public Worker(AppConfig appConfig, ILogger<Worker> logger, MapleRoyalsService mapleRoyalsService)
    {
        _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapleRoyalsService = mapleRoyalsService ?? throw new ArgumentNullException(nameof(mapleRoyalsService));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await WaitForNextSchedule(_appConfig.Cron);
            _logger.LogInformation("Worker running at: {Time}", DateTime.UtcNow);
            
            var response = await _mapleRoyalsService.GetPlayerCount();
            if (response is null)
            {
                _logger.LogInformation("No response received from MapleRoyals api");
                continue;
            }
            
            _logger.LogInformation("Currently there are {OnlineCount} players and the server is {OnlineStatus}", 
                response.OnlineCount, response.Online ? "online" : "offline");
            await InsertRecord(response.OnlineCount, response.Online);
        }
    }
    
    private async Task WaitForNextSchedule(string cronExpression)
    {
        var parsedExp = Cronos.CronExpression.Parse(cronExpression);
        var occurenceTime = parsedExp.GetNextOccurrence(DateTime.UtcNow);

        var delay = occurenceTime.GetValueOrDefault() - DateTime.UtcNow;
        _logger.LogInformation("The run is delayed for {Delay}, current time: {CurrentTime}", delay, DateTime.UtcNow);

        await Task.Delay(delay);
    }

    private static async Task InsertRecord(int players, bool online)
    {
        await using var cnn = DataContext.GetSqliteConnection();
        await cnn.OpenAsync();
        await cnn.ExecuteAsync(@"INSERT INTO Players (players, server_online) VALUES (@players, @online)", new { players, online });
        cnn.Close();
    }
}
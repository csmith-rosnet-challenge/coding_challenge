using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class PeriodicStatusFetcher : BackgroundService
{
    private readonly IExternalStatusService _statusService;
    private readonly ExternalStatusHistoryService _historyService;
    private readonly ILogger<PeriodicStatusFetcher> _logger;
    private readonly IConfiguration _configuration;

    public PeriodicStatusFetcher(
        IExternalStatusService statusService,
        ExternalStatusHistoryService historyService,
        ILogger<PeriodicStatusFetcher> logger,
        IConfiguration configuration)
    {
        _statusService = statusService;
        _historyService = historyService;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalSeconds = _configuration.GetValue<int>("StatusFetcher:IntervalSeconds", 60);
        var urlList = _configuration.GetSection("StatusFetcher:Urls").Get<List<string>>() ?? new List<string>();

        if (!urlList.Any())
        {
            _logger.LogWarning("No StatusFetcher:Urls configured. Periodic fetcher will not run.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Running periodic status fetch for {Count} URLs.", urlList.Count);

                var request = new ExternalStatusRequest
                {
                    Urls = urlList,
                    TimeoutMs = _configuration.GetValue<int>("StatusFetcher:TimeoutMs", 5000),
                    MaxConcurrency = _configuration.GetValue<int>("StatusFetcher:MaxConcurrency", 5)
                };

                var results = await _statusService.CheckStatusAsync(request, stoppingToken);
                await _historyService.SaveAsync(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Periodic status fetch failed.");
            }

            await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
        }
    }
}

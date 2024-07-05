using Microsoft.Extensions.Options;

namespace PrinterTester
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly List<TestData> _printerData;
        private int _currentIndex;

        public Worker(ILogger<Worker> logger, IOptions<List<TestData>> printerDataOptions)
        {
            _logger = logger;
            _printerData = printerDataOptions.Value;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_printerData.Count == 0)
                {
                    _logger.LogWarning("No printer data available.");
                    await Task.Delay(5000, stoppingToken);
                    continue;
                }

                foreach (var data in _printerData.TakeWhile(data => !stoppingToken.IsCancellationRequested))
                {
                    data.UpdateZplProgram();
                    data.Printer(_logger);
                    _logger.LogInformation($"ZplProgram: {data.ZplProgram}, Value: {data.Value}, RgbColor: {data.RgbColor}");

                    await Task.Delay(5000, stoppingToken); // Wait for 5 seconds before processing the next item
                }

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Completed a full cycle of printer data. Waiting for 60 seconds before restarting the cycle.");
                }

                await Task.Delay(60000, stoppingToken); // Wait for 60 seconds before starting the next cycle
            }
        }
    }
}


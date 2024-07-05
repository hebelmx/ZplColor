using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZplColor.Printers;

namespace ZplColor;

public class PrinterWorker(ILogger<PrinterWorker> logger, IPrinter printer) : BackgroundService
{
    private bool isRunning = false;


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (isRunning)
        {
            logger.LogInformation("Service is already running.");
            return;
        }

        isRunning = true;

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var resultModbus = new ResultTest();
                printer.StartCommunications();
                resultModbus = await printer.PrintTaskAsync(stoppingToken, resultModbus);
                await Task.Delay(500, stoppingToken);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in PrinterWorker");
        }
        finally
        {
            isRunning = false;
        }
    }



}


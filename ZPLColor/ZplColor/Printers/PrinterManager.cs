using Microsoft.Extensions.Logging;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using ZplColor.Common;

namespace ZplColor.Printers
{
    public class PrinterManager(ILogger<PrinterManager> logger, GatewayConfig gatewayConfig)
    {
        public GatewayConfig GatewayConfig { get; } = gatewayConfig;



        public async Task SendZplProgram(string zplProgram, CancellationToken stoppingToken)
        {
            if (!GatewayConfig.SendProgram)
            {
                logger.LogInformation("Printing Disabled");
                return;
            }

            var connection = await CheckConnection(GatewayConfig.IpAddressPrinter, logger, stoppingToken);
            if (!connection)
            {
                logger.LogError($"Printer {GatewayConfig.IpAddressPrinter} not reachable.");
                return;
            }

            try
            {
                var zplBytes = Encoding.UTF8.GetBytes(zplProgram);
                using var printer = new TcpClient(GatewayConfig.IpAddressPrinter, GatewayConfig.PrinterPort);
                await using var stream = printer.GetStream();
                await stream.WriteAsync(zplBytes, 0, zplBytes.Length, stoppingToken);

                logger.LogInformation("ZPL program sent to the printer successfully.");
            }
            catch (SocketException e)
            {
                logger.LogError($"Error while printing: {e.Message}");
            }
        }

        private async Task<bool> CheckConnection(string ipAddress, ILogger logger, CancellationToken token)
        {
            for (var i = 0; i < GatewayConfig.PingSettings.MaxRetries; i++)
            {
                try
                {
                    using var pingSender = new Ping();
                    var reply = pingSender.Send(ipAddress, GatewayConfig.PingSettings.PingTimeout);

                    if (reply.Status == IPStatus.Success)
                    {
                        logger.LogInformation($"Ping to {ipAddress} successful; time: {reply.RoundtripTime} ms");
                        return true;
                    }

                    await Task.Delay(GatewayConfig.PingSettings.PingRetryTime, token);

                    logger.LogError($"Ping to {ipAddress} failed: {reply.Status}. Attempt {i + 1} of {GatewayConfig.PingSettings.MaxRetries}");
                }
                catch (PingException ex)
                {
                    logger.LogError($"A ping error occurred: {ex.StackTrace}. Attempt {i + 1} of {GatewayConfig.PingSettings.MaxRetries}");
                }
                catch (Exception ex)
                {
                    logger.LogError($"An unexpected error occurred: {ex.StackTrace}. Attempt {i + 1} of {GatewayConfig.PingSettings.MaxRetries}");
                }
            }

            logger.LogError($"Ping to {ipAddress} failed after {GatewayConfig.PingSettings.MaxRetries} attempts.");
            return false;
        }
    }
}

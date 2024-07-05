// Include to use Color type for RGB

using System.IO.Ports;
using ZplColor.Common;

namespace PrinterTester;

public class TestData
{
    public string ZplProgram { get; set; }
    public double Value { get; set; }
    public RgbColor RgbColor { get; set; }

    public void UpdateZplProgram()
    {
        if (ZplProgram.Contains("{value}"))
        {
            ZplProgram = ZplProgram.Replace("{value}", Value.ToString("F2"));
        }
    }

    public void Printer(ILogger logger)
    {
        // Detect available serial ports
        var availablePorts = DetectSerialPorts();

        if (availablePorts.Length == 0)
        {
            logger.LogError("No serial ports detected.");
            return;
        }

        // Use the first available port for demonstration
        var selectedPort = availablePorts.First();
        logger.LogInformation($"Using serial port: {selectedPort}");

        // Send ZplProgram to the serial port
        SendZplToSerialPort(selectedPort, ZplProgram, logger);
    }

    private string[] DetectSerialPorts()
    {
        // Get the list of available serial ports
        return SerialPort.GetPortNames();
    }
    private void SendZplToSerialPort(string portName, string zplProgram, ILogger logger)
    {
        try
        {
            using (var serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One))
            {
                serialPort.Open();
                serialPort.WriteLine(zplProgram);
                logger.LogInformation("ZPL Program sent successfully.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Error: {ex.Message}");
        }
    }
}


// Custom converter to handle RGB string to Color conversion
using System.IO.Ports;
using System.Text;
using Microsoft.Extensions.Logging;
using ZplColor.Common;

namespace ZplColor.Printers
{
    public class SerialCommunicator
    {
        private readonly ILogger<SerialCommunicator> _logger;
        private readonly SerialPortConfig _serialPortConfig;
        private SerialPort _serialPort;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public SerialCommunicator(ILogger<SerialCommunicator> logger, SerialPortConfig serialPortConfig)
        {
            _logger = logger;
            _serialPortConfig = serialPortConfig;
        }

        public void StartCommunications()
        {
            var portName = _serialPortConfig.PortName;
            if (string.IsNullOrWhiteSpace(portName))
            {
                _logger.LogError("Serial port name is not specified in the configuration.");
                throw new ArgumentNullException(nameof(portName));
            }

            if (!_isInitialized || _serialPort == null)
            {
                _serialPort = new SerialPort(portName)
                {
                    BaudRate = _serialPortConfig.BaudRate,
                    Parity = _serialPortConfig.Parity,
                    StopBits = _serialPortConfig.StopBits,
                    DataBits = _serialPortConfig.DataBits,
                    Handshake = _serialPortConfig.Handshake,
                    ReadTimeout = _serialPortConfig.ReadTimeout,
                    WriteTimeout = _serialPortConfig.WriteTimeout,
                };
                _isInitialized = true;
            }

            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
                _logger.LogInformation($"Serial port {portName} opened successfully.");
                FlushSerialPortBuffer();
                _logger.LogInformation($"Serial port {portName} data flushed.");
            }
            else
            {
                _logger.LogDebug($"Serial port {portName} was already opened.");
            }
        }

        public async Task<string> ReadSerialDataAsync()
        {
            var stringSerialPort = new StringBuilder();
            const int maxAttempts = 20000;
            const int delayMilliseconds = 10;

            var buffer = new byte[1024];
            int bytesRead;
            var attempts = 0;

            while (!stringSerialPort.ToString().Contains("^XZ") && attempts < maxAttempts)
            {
                attempts++;
                if (_serialPort.BytesToRead > 0)
                {
                    bytesRead = await _serialPort.BaseStream.ReadAsync(buffer, 0, buffer.Length);
                    stringSerialPort.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));
                }
                else
                {
                    await Task.Delay(delayMilliseconds);
                }
            }

            var message = stringSerialPort.ToString();
            LogReadAttempt(attempts, message);

            return message;
        }

        private void FlushSerialPortBuffer()
        {
            _logger.LogInformation("Flushing serial port buffer...");
            while (_serialPort.BytesToRead > 0)
            {
                _serialPort.ReadLine(); // Read and discard data
            }
            _logger.LogInformation("Serial port buffer flushed.");
        }

        private void LogReadAttempt(int attempts, string message)
        {
            if (message.Length < 1) return;

            var information = $"Message took {attempts} cycles. Length: {message.Length}\nMessage:\n{message}";
            _logger.LogInformation(information);
        }
    }
}

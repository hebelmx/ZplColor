using Microsoft.Extensions.Logging;
using ZplColor.Common;
using ZplColor.Interfaces;

namespace ZplColor.Printers
{
    public class PrinterSerial : IPrinter
    {
        private readonly ILogger<PrinterSerial> _logger;
        private readonly SerialCommunicator _serialCommunicator;
        private readonly DataProcessor _dataProcessor;
        private readonly PrinterManager _printerManager;
        private readonly CounterService _counterService;
        private int _counter;

        public ResultTest LastResult { get; set; } = new();
        public ModelConfig Model { get; set; }

        public PrinterSerial(ILogger<PrinterSerial> logger, SerialCommunicator serialCommunicator,
            DataProcessor dataProcessor, PrinterManager printerManager, CounterService counterService)
        {
            _logger = logger;
            _serialCommunicator = serialCommunicator;
            _dataProcessor = dataProcessor;
            _printerManager = printerManager;
            _counterService = counterService;
        }

        public async Task<ResultTest> PrintTaskAsync(CancellationToken stoppingToken, ResultTest lastResult)
        {
            LastResult = lastResult?.DeepCopy() ?? new ResultTest();

            var newResult = await GetMessageToPrint(LastResult);

            if (newResult.Retransmit)
            {
                await _printerManager.SendZplProgram(newResult.Data, stoppingToken);
                return newResult;
            }

            if (!_dataProcessor.Analyzers.ContainsKey(_dataProcessor.Model.ModelClassHandler))
            {
                _logger.LogError("Color analyzer not found: {Model}", _dataProcessor.Model.ModelClassHandler);
                return newResult;
            }

            var analyzer = _dataProcessor.Analyzers[_dataProcessor.Model.ModelClassHandler];

            if (!newResult.Print) return newResult;

            analyzer.DetermineColorsAndCalculateRGB(newResult.MaxiCruceH);

            _logger.LogInformation(analyzer.RgbColor.ToString());
            _logger.LogInformation("PrinterWorker running at: {time} {msec}", DateTimeOffset.Now, DateTimeOffset.Now.Millisecond);

            var zplProgram = SetParametersToZplProgram(analyzer, newResult);

            _counter++;
            _counterService.IncrementCounter();

            _logger.LogInformation($"Printing {_counter} label from the session");
            _logger.LogInformation($"Printing {_counterService.Counter} label from the beginning");
            _logger.LogInformation($"Program to print: \n{zplProgram}");


            await _printerManager.SendZplProgram(zplProgram, stoppingToken);


            return newResult;
        }

        public void StartCommunications()
        {
            _serialCommunicator.StartCommunications();
        }

        private async Task<ResultTest> GetMessageToPrint(ResultTest lastResult)
        {
            if (!_serialCommunicator.IsInitialized)
            {
                _serialCommunicator.StartCommunications();
            }

            var completeData = await _serialCommunicator.ReadSerialDataAsync();

            return string.IsNullOrWhiteSpace(completeData) ? lastResult.DeepCopy() : _dataProcessor.ProcessData(completeData, lastResult);
        }

        private string SetParametersToZplProgram(IColorAnalyzer analyzer, ResultTest newResult)
        {
            var zplProgram = _printerManager.GatewayConfig.ZPLProgram;
            zplProgram = zplProgram.Replace("{{RED}}", analyzer.RgbColor.Red.ToString());
            zplProgram = zplProgram.Replace("{{BLUE}}", analyzer.RgbColor.Blue.ToString());
            zplProgram = zplProgram.Replace("{{GREEN}}", analyzer.RgbColor.Green.ToString());
            zplProgram = zplProgram.Replace("{{MaxiCruceV}}", newResult.MaxiCruceV.ToString("F2"));
            zplProgram = zplProgram.Replace("{{MaxiCruceH}}", newResult.MaxiCruceH.ToString("F2"));
            zplProgram = zplProgram.Replace("{{Model}}", _dataProcessor.Model.Model);
            zplProgram = zplProgram.Replace("{{NumberPart}}", _dataProcessor.Model.NumberPart);

            return zplProgram;
        }

        public override string ToString()
        {
            return $"PrinterSerial: [Model={_dataProcessor.Model}, LastResult={LastResult}]";
        }
    }
}

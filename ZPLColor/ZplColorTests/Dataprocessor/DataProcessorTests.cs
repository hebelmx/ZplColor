using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;
using ZplColor;
using ZplColor.Common;
using ZplColor.Interfaces;
using ZplColor.Printers;

namespace PrinterCSharpTests.Serial
{
    public class DataProcessorTests
    {
        private readonly ILogger<DataProcessor> _logger;
        private readonly Dictionary<string, IColorAnalyzer> _analyzers;
        private readonly ModelConfigurations _modelConfigurations;
        private readonly DataProcessor _dataProcessor;
        private readonly ITestOutputHelper _output;

        public DataProcessorTests(ITestOutputHelper output)
        {
            _output = output;
            _logger = Substitute.For<ILogger<DataProcessor>>();
            _analyzers = new Dictionary<string, IColorAnalyzer>();

            // Ensure ModelConfigurations is properly initialized with a non-null Models dictionary
            _modelConfigurations = new ModelConfigurations
            {
                Models = new Dictionary<string, ModelConfig>()
            };

            _dataProcessor = new DataProcessor(_logger, _modelConfigurations, _analyzers);
        }

        [Theory]
        [InlineData("^XA^PW300^FT30,30^A0N,13,13^FDBRP HIGH BEAM PREMIUM^FS^FT30,45^A0N,12,12^FD\\^FDCOORDENADAS MAXI^FS^FT30,62^ABN,11,11^FV,{#^FV{0.00  1.23 }^FS^XZ", 0.00, 1.23)]
        [InlineData("^XA^PW300^FT30,30^A0N,13,13^FDBRP HIGH BEAM BASE^FS^FT30,45^A0N,12,12^FD\\^FDCOORDENADAS MAXI^FS^FT30,62^ABN,11,11^FV,{#^FV{0.00  0.39 }^FS^XZ", 0.00, 0.39)]
        [InlineData("^XA\n^PW300\n^FT45,30^A0N,13,13^FDBRP LOW BEAM PREMIUM^FS\n^FT45,50^A0N,12,12^FD\\^FDCOORDENADAS CORTE^FS\n^FT31,70^ABN,11,11^FV,{#^FV{-2.90  -0.86 }^FS\n^XZ", -2.90, -0.86)]
        [InlineData("^XA\n^PW300\n^FT40,30^A0N,13,13^FDBRP LOW BEAM BASE^FS\n^FT40,50^A0N,12,12^FD\\^FDCOORDENADAS CORTE^FS\n^FT40,70^ABN,11,11^FV,{#^FV{-2.90  -0.86}^FS\n^XZ", -2.90, -0.86)]
        public void SearchValuesOnData_ValidValues_UpdatesResult(string data, double expectedMaxiCruceV, double expectedMaxiCruceH)
        {
            // Arrange
            var lines = data.Split(new[] { "\n", "^FS" }, StringSplitOptions.RemoveEmptyEntries);
            var newResult = new ResultTest();

            // Act
            _dataProcessor.SearchValuesOnData(lines, newResult);

            // Assert
            newResult.Print.Should().BeTrue();
            newResult.MaxiCruceV.Should().BeApproximately(expectedMaxiCruceV, 0.01);
            newResult.MaxiCruceH.Should().BeApproximately(expectedMaxiCruceH, 0.01);

            // Log captured output
            LogCapturedOutput();
        }

        [Fact]
        public void SearchValuesOnData_WithValidValues_ShouldUpdateResult()
        {
            // Arrange
            var lines = new[]
            {
                "^FT31,70^ABN,11,11^FV,{#^FV{-2.90  -0.86 }"
            };
            var newResult = new ResultTest();

            // Act
            _dataProcessor.SearchValuesOnData(lines, newResult);

            // Assert
            newResult.Print.Should().BeTrue();
            newResult.MaxiCruceV.Should().BeApproximately(-2.90, 0.01);
            newResult.MaxiCruceH.Should().BeApproximately(-0.86, 0.01);

            // Log captured output
            LogCapturedOutput();
        }

        [Fact]
        public void SearchValuesOnData_WithMinimumString_ShouldUpdateResult()
        {
            // Arrange
            var lines = new[]
            {
                "ABN FV 0.0  1.23"
            };
            var newResult = new ResultTest();

            // Act
            _dataProcessor.SearchValuesOnData(lines, newResult);

            // Assert
            newResult.Print.Should().BeTrue();
            newResult.MaxiCruceV.Should().BeApproximately(0, 0.01);
            newResult.MaxiCruceH.Should().BeApproximately(1.23, 0.01);

            // Log captured output
            LogCapturedOutput();
        }

        [Fact]
        public void SearchValuesOnData_WithNoValidValues_ShouldNotUpdateResult()
        {
            // Arrange
            var lines = new[]
            {
                "^FT31,70^ABN,11,11^FV,{#^FV{ABC  DEF }"
            };
            var newResult = new ResultTest();

            // Act
            _dataProcessor.SearchValuesOnData(lines, newResult);

            // Assert
            newResult.Print.Should().BeFalse();

            // Log captured output
            LogCapturedOutput();
        }

        [Fact]
        public void ProcessData_WithValidData_ShouldReturnNewResult()
        {


            // Arrange
            var lines = new[]
            {
                "^FT31,70^ABN,11,11^FV,{#^FV{-2.90  -0.86 }^FS"
            };
            var result = new ResultTest();

            // Act
            _dataProcessor.SearchValuesOnData(lines, result);

            // Assert
            result.Print.Should().BeTrue();
            result.MaxiCruceV.Should().BeApproximately(-2.90, 0.01);
            result.MaxiCruceH.Should().BeApproximately(-0.86, 0.01);

            // Log captured output
            LogCapturedOutput();
        }

        [Fact]
        public void ProcessData_WithNoValidData_ShouldReturnLastResult()
        {
            // Arrange
            // Arrange
            var lines = new[]
            {
                "^FT31,70^ABN,11,11^FV,{#^FV{ }^FS"
            };

            var result = new ResultTest { Print = true, MaxiCruceV = 1.23, MaxiCruceH = 4.56 };

            // Act
            _dataProcessor.SearchValuesOnData(lines, result);

            // Assert
            result.Print.Should().BeFalse();
            result.MaxiCruceV.Should().BeApproximately(1.23, 0.01); // Ensure it retains the last result
            result.MaxiCruceH.Should().BeApproximately(4.56, 0.01); // Ensure it retains the last result

            // Log captured output
            LogCapturedOutput();
        }

        private void LogCapturedOutput()
        {
            var logEvents = _logger
                .ReceivedCalls()
                .Select(call => call.GetArguments()[1] as LogLevel? ?? LogLevel.None);

            foreach (var logEvent in logEvents)
            {
                _output.WriteLine(logEvent.ToString());
            }
        }
    }
}

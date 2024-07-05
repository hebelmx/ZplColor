using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using ZplColor.Common;
using ZplColor.Interfaces;

namespace ZplColor.Printers
{
    public class DataProcessor
    {
        private readonly ILogger<DataProcessor> _logger;
        private readonly ModelConfigurations _modelConfigurations;
        public Dictionary<string, IColorAnalyzer> Analyzers { get; }

        public ModelConfig Model { get; set; }

        public DataProcessor(ILogger<DataProcessor> logger, ModelConfigurations modelConfigurations, Dictionary<string, IColorAnalyzer> analyzers)
        {
            _logger = logger;
            _modelConfigurations = modelConfigurations;
            Analyzers = analyzers;
        }

        public ResultTest ProcessData(string data, ResultTest lastResult)
        {
            var lines = data.Split(new[] { "^FS" }, StringSplitOptions.RemoveEmptyEntries);

            var newResult = lastResult.DeepCopy();
            Model = ModelConfig.CreateDefault();

            newResult.Data = data;
            SearchModelOnData(data);

            switch (Model.MarcaCromatica)
            {
                case false when data.Contains("^XA") && data.Contains("^XZ"):
                    newResult.Retransmit = true;
                    return newResult;
                case false:
                    return newResult;
                default:
                    SearchValuesOnData(lines, newResult);
                    return newResult;
            }
        }

        private void SearchModelOnData(string data)
        {
            foreach (var (ModelName, modelData) in _modelConfigurations.Models)
            {
                if (data.Contains(ModelName))
                {
                    Model.DeepCopy(modelData);
                    break;
                }
            }
        }

        public void SearchValuesOnData(string[] lines, ResultTest result)
        {

            result.Print = false;

            foreach (var line in lines)
            {
                if (!line.Contains("ABN") || !line.Contains("FV")) continue;

                var matches = Regex.Matches(line, @"-?\d+\.\d+");
                if (matches.Count >= 2)
                {
                    var lastTwoMatches = matches.Cast<Match>().Skip(matches.Count - 2).Select(m => double.Parse(m.Value)).ToArray();
                    result.MaxiCruceV = Math.Round(lastTwoMatches[0], 2);
                    result.MaxiCruceH = Math.Round(lastTwoMatches[1], 2);

                    _logger.LogInformation($"Type: {Model} code {result.Model}, First Value MaxiCruceV: {result.MaxiCruceV}, Second Value MaxiCruceH: {result.MaxiCruceH}");

                    result.Print = true;
                    break;
                }
                else
                {
                    _logger.LogInformation("No valid numbers found in the expected format.");
                }
            }
        }
    }
}

using ZplColor.Common;

namespace ZplColor.Printers
{
    public interface IPrinter
    {
        ResultTest LastResult { get; set; }
        ModelConfig Model { get; set; }

        Task<ResultTest> PrintTaskAsync(CancellationToken stoppingToken, ResultTest lastResult);

        void StartCommunications();

    }
}
using ZplColor.Common;

namespace ZplColor.Interfaces;

public interface IColorAnalyzer
{
    double ResultAnalyze { get; }

    double Adjust { get; }
    string Name { get; }
    IColorThresholds ColorThresholds { get; }
    RgbColor RgbColor { get; }
    RgbColor DetermineColorsAndCalculateRGB(double dataToAnalyze);
}
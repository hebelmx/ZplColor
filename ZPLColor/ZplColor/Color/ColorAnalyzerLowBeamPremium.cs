using ZplColor.Common;
using ZplColor.Interfaces;
using static ZplColor.Common.ColorConstants;

namespace ZplColor.Color;


public class ColorAnalyzerLowBeamPremium(IColorThresholds colorThresholds, string name, double adjust) : IColorAnalyzer
{
    public double ResultAnalyze { get; private set; }
    public double Adjust { get; private set; } = adjust;

    public RgbColor RgbColor { get; private set; } = new(0, 0, 0);

    public string Name { get; } = name;
    public IColorThresholds ColorThresholds { get; } = colorThresholds;
    private void CalculateResultAnalyze(double dataToAnalyze)
    {
        ResultAnalyze = dataToAnalyze + Adjust;
    }

    public override string ToString()
    {
        return $"ResultAnalyze: {ResultAnalyze}";
    }
    public RgbColor DetermineColorsAndCalculateRGB(double dataToAnalyze)
    {

        CalculateResultAnalyze(dataToAnalyze);

        ResultAnalyze = Math.Round(ResultAnalyze, 2); // 2.50

        var resGreen = ResultAnalyze <= ColorThresholds.LimGreenYellow && ResultAnalyze > ColorThresholds.LimInfGreen ? 1 : 0;
        var resYellow = ResultAnalyze <= ColorThresholds.LimYellowOrange && ResultAnalyze > ColorThresholds.LimGreenYellow ? 1 : 0;
        var resOrange = ResultAnalyze <= ColorThresholds.LimOrangeWhite && ResultAnalyze > ColorThresholds.LimYellowOrange ? 1 : 0;
        var resPurple = ResultAnalyze <= ColorThresholds.LimSupPurple && ResultAnalyze > ColorThresholds.LimWhitePurple ? 1 : 0;

        var rgbRed = Full * resYellow + Full * resOrange + Middle * resPurple;
        var rgbGreen = Full * resGreen + Full * resYellow + Middle * resOrange;
        var rgbBlue = (Middle * resPurple);

        RgbColor = new RgbColor(rgbRed, rgbGreen, rgbBlue);
        return RgbColor;
    }





}

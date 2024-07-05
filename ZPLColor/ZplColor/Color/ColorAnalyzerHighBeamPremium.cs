using ZplColor.Common;
using ZplColor.Interfaces;
using static ZplColor.Common.ColorConstants;

namespace ZplColor.Color;

public class ColorAnalyzerHighBeamPremium(IColorThresholds colorThresholds, string name, double adjust) : IColorAnalyzer
{
    public double ResultAnalyze { get; private set; }
    public double Adjust { get; private set; } = adjust;

    public RgbColor RgbColor { get; private set; } = new(0, 0, 0);

    public string Name { get; } = name;
    public IColorThresholds ColorThresholds { get; } = colorThresholds;


    public RgbColor DetermineColorsAndCalculateRGB(double dataToAnalyze)
    {

        ResultAnalyze = Math.Round(dataToAnalyze, 2); // 2.50


        var resGreen = (ResultAnalyze <= ColorThresholds.LimInfGreen) ? 1 : 0;
        var resBlue = (ResultAnalyze > ColorThresholds.LimInfGreen && ResultAnalyze <= ColorThresholds.LimGreenBlue) ? 1 : 0;
        var resYellow = (ResultAnalyze > ColorThresholds.LimGreenBlue && ResultAnalyze <= ColorThresholds.LimBlueYellow) ? 1 : 0;
        var resOrange = (ResultAnalyze >= ColorThresholds.LimYellowOrange) ? 1 : 0;

        var rgbRed = (Full * resYellow) + (Full * resOrange);
        var rgbGreen = (Full * resGreen) + (Full * resYellow) + (Middle * resOrange);
        var rgbBlue = (Full * resBlue);

        RgbColor = new RgbColor(rgbRed, rgbGreen, rgbBlue);
        return RgbColor;
    }


}
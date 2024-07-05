using ZplColor.Common;
using ZplColor.Interfaces;

namespace ZplColor.Color;

public class ColorAnalyzerLowBeamBase(IColorThresholds colorThresholds, string name, double adjust) : IColorAnalyzer
{
    public double ResultAnalyze { get; private set; }
    public double Adjust { get; private set; } = adjust;

    public RgbColor RgbColor { get; private set; } = new(0, 0, 0);

    public string Name { get; } = name;
    public IColorThresholds ColorThresholds { get; } = colorThresholds;


    public RgbColor DetermineColorsAndCalculateRGB(double dataToAnalyze)
    {

        ResultAnalyze = Math.Round(dataToAnalyze, 2); // 2.50

        var resGreen = (ResultAnalyze < ColorThresholds.LimInfGreen) ? 1 : 0;
        var resBlue = (ResultAnalyze > ColorThresholds.LimInfGreen && ResultAnalyze <= ColorThresholds.LimGreenBlue) ? 1 : 0;
        var resYellow = (ResultAnalyze > ColorThresholds.LimGreenBlue && ResultAnalyze <= ColorThresholds.LimBlueYellow) ? 1 : 0;
        var resOrange = (ResultAnalyze > ColorThresholds.LimBlueYellow && ResultAnalyze <= ColorThresholds.LimYellowOrange) ? 1 : 0;
        var resPink = (ResultAnalyze >= ColorThresholds.LimOrangePink) ? 1 : 0;

        var rgbRed = (ColorConstants.Full * resYellow) + (ColorConstants.Full * resOrange) + (ColorConstants.Full * resPink);
        var rgbGreen = (ColorConstants.Full * resGreen) + (ColorConstants.Full * resYellow) + (ColorConstants.Middle * resOrange);
        var rgbBlue = (ColorConstants.Full * resBlue) + (ColorConstants.Middle * resPink);

        RgbColor = new RgbColor(rgbRed, rgbGreen, rgbBlue);

        return RgbColor;
    }


}
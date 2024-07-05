using ZplColor.Interfaces;

namespace ZplColor.Common;

public class ColorThresholds : IColorThresholds
{
    public double Adjust { get; set; }
    public string Name { get; set; }
    public double LimInfGreen { get; set; }
    public double LimGreenBlue { get; set; }
    public double LimBlueYellow { get; set; }
    public double LimYellowOrange { get; set; }
    public double LimOrangePink { get; set; }
    public double LimGreenYellow { get; set; }
    public double LimOrangeWhite { get; set; }
    public double LimWhitePurple { get; set; }
    public double LimSupPurple { get; set; }



}


// Constants equivalent to the #define directives in C++
// Uncomment and adjust the values based on your original code if needed
// private const double Audit_AimY = -0.26f;
// private const double PrintY = Audit_AimY + 1.56f;

// private const double LimGreenYellow = -0.8f;
// private const double LimInfGreen = 1.23f;
// private const double LimYellowOrange = -0.64f;
// private const double LimOrangeWhite = -0.48f;
// private const double LimWhitePurple = -0.32f;
// private const double LimSupPurple = 0.5f;



namespace ZplColor.Interfaces;

public interface IColorThresholds
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
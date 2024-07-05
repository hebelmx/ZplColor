namespace ZplColor.Common;

public class RgbColor(int rGbRed, int rGbGreen, int rGbBlue)
{

    // Constructor for initializing directly

    public int Red { get; set; } = rGbRed;
    public int Green { get; set; } = rGbGreen;
    public int Blue { get; set; } = rGbBlue;


    public override bool Equals(object? obj)
    {
        // Check for null and compare run-time types.
        if (obj is null || GetType() != obj.GetType())
        {
            return false;
        }
        else
        {
            var p = (RgbColor)obj;
            return Red == p.Red && Green == p.Green && Blue == p.Blue;
        }
    }

    public override int GetHashCode()
    {
        // Use hash code of aggregated fields (Red, Green, Blue)
        // HashCode.Combine is available in .NET Core 2.1 and later versions
        return HashCode.Combine(Red, Green, Blue);
    }

    public override string ToString()
    {
        return $"RGBColor: Red:{Red}; Green:{Green} ; Blue:{Blue}  ";
    }

}
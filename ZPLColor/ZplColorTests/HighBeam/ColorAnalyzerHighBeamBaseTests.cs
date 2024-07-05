using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using ZplColor.Color;
using ZplColor.Common;

namespace PrinterCSharpTests.HighBeam
{

    public class ColorAnalyzerHighBeamBaseTests(ITestOutputHelper output)
    {
        private readonly ITestOutputHelper _output = output;


        public ColorThresholds ColorThresholdsBase { get; private set; } = new()
        {
            // Initialize properties here
            LimInfGreen = 0.39,
            LimGreenBlue = 0.69, // Assuming LimGreenYellow is a typo and you meant LimGreenBlue
            LimBlueYellow = 0.79,
            LimYellowOrange = 1.30,
            LimOrangePink = 1.31,
            Adjust = 0.0

        };

        // Initialize properties here
        // Assuming LimGreenYellow is a typo and you meant LimGreenBlue


        public static IEnumerable<object[]> RgbColorTestData => new List<object[]>
        {
            new object[] { 0.26, new RgbColor(0, 255, 0) },
            new object[] { 0.30, new RgbColor(0, 255, 0) },
            new object[] { 0.36, new RgbColor(0, 255, 0) },
            new object[] { 0.39, new RgbColor(0, 255, 0) },
            new object[] { 0.41, new RgbColor(0, 0, 255) },
            new object[] { 0.45, new RgbColor(0, 0, 255) },
            new object[] { 0.50, new RgbColor(0, 0, 255) },
            new object[] { 0.55, new RgbColor(0, 0, 255) },
            new object[] { 0.60, new RgbColor(0, 0, 255) },
            new object[] { 0.62, new RgbColor(0, 0, 255) },
            new object[] { 0.69, new RgbColor(0, 0, 255) },
            new object[] { 0.70, new RgbColor(255, 255, 0) },
            new object[] { 0.72, new RgbColor(255, 255, 0) },
            new object[] { 0.74, new RgbColor(255, 255, 0) },
            new object[] { 0.76, new RgbColor(255, 255, 0) },
            new object[] { 0.78, new RgbColor(255, 255, 0) },
            new object[] { 0.79, new RgbColor(255, 255, 0) },
            new object[] { 0.80, new RgbColor(255, 127, 0) },
            new object[] { 0.90, new RgbColor(255, 127, 0) },
            new object[] { 1.00, new RgbColor(255, 127, 0) },
            new object[] { 1.20, new RgbColor(255, 127, 0) },
            new object[] { 1.22, new RgbColor(255, 127, 0) },
            new object[] { 1.28, new RgbColor(255, 127, 0) },
            new object[] { 1.30, new RgbColor(255, 127, 0) },
            new object[] { 1.31, new RgbColor(255, 0, 127) },
            new object[] { 1.35, new RgbColor(255, 0, 127) },
            new object[] { 1.40, new RgbColor(255, 0, 127) },
            new object[] { 1.45, new RgbColor(255, 0, 127) },
            new object[] { 1.47, new RgbColor(255, 0, 127) },
            new object[] { 1.50, new RgbColor(255, 0, 127) },
            // Add more test cases as needed
        };

        [Theory]
        [MemberData(nameof(RgbColorTestData))]
        public void RgbColorMustBeCorrect(double auditAimY, RgbColor expectedRgbColor)
        {
            // Arrange
            var colorAnalyzerHighBeamBase = new ColorAnalyzerHighBeamBase(ColorThresholdsBase, "High Beam Base", 0.0f);

            // Act
            var result = colorAnalyzerHighBeamBase.DetermineColorsAndCalculateRGB(auditAimY);
            _output.WriteLine(expectedRgbColor.ToString());
            _output.WriteLine(result.ToString());

            // Assert
            expectedRgbColor.Should().Be(result);
        }





    }
}
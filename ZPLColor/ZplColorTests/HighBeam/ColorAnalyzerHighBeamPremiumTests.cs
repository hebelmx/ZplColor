using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using ZplColor.Color;
using ZplColor.Common;

namespace PrinterCSharpTests.HighBeam
{

    public class ColorAnalyzerHighBeamPremiumTests(ITestOutputHelper output)
    {
        private readonly ITestOutputHelper _output = output;

        public ColorThresholds ColorThresholdsColorAnalyzerHighBeamPremium { get; private set; } = new()
        {
            // Initialize properties here
            LimInfGreen = 1.23,
            LimGreenBlue = 2.18, // Assuming LimGreenYellow is a typo and you meant LimGreenBlue
            LimBlueYellow = 2.49,
            LimYellowOrange = 2.50,
            Adjust = 0.0

        };

        // Initialize properties here
        // Assuming LimGreenYellow is a typo and you meant LimGreenBlue


        public static IEnumerable<object[]> RgbColorTestData => new List<object[]>
        {
            new object[] { 0.00, new RgbColor(0, 255, 0) },
            new object[] { 1.23, new RgbColor(0, 255, 0) },
            new object[] { 1.22, new RgbColor(0, 255, 0) },
            new object[] { 1.25, new RgbColor(0, 0, 255) },
            new object[] { 2.15, new RgbColor(0, 0, 255) },
            new object[] { 2.20, new RgbColor(255, 255, 0) },
            new object[] { 2.30, new RgbColor(255, 255, 0) },
            new object[] { 2.35, new RgbColor(255, 255, 0) },
            new object[] { 2.40, new RgbColor(255, 255, 0) },
            new object[] { 2.45, new RgbColor(255, 255, 0) },
            new object[] { 2.49, new RgbColor(255, 255, 0) },
            new object[] { 2.494, new RgbColor(255, 255, 0) },
            new object[] { 2.495, new RgbColor(255, 127, 0) },
            new object[] { 2.50, new RgbColor(255, 127, 0) },
            new object[] { 2.55, new RgbColor(255, 127, 0) },
            new object[] { 2.60, new RgbColor(255, 127, 0) },
            // Add more test cases as needed
        };

        [Theory]
        [MemberData(nameof(RgbColorTestData))]
        public void RgbColorMustBeCorrect(double auditAimY, RgbColor expectedRgbColor)
        {
            // Arrange
            var colorAnalyzerHighBeamPremium = new ColorAnalyzerHighBeamPremium(ColorThresholdsColorAnalyzerHighBeamPremium, "High Beam Premium", 0.0f);

            // Act
            var result = colorAnalyzerHighBeamPremium.DetermineColorsAndCalculateRGB(auditAimY);

            _output.WriteLine($"ResutlAim: {auditAimY}");
            _output.WriteLine($"Expected: {expectedRgbColor}");
            _output.WriteLine($"Result: {result}");
            // Assert
            expectedRgbColor.Should().Be(result);
        }





    }



}
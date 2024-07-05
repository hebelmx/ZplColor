using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using ZplColor.Color;
using ZplColor.Common;


namespace PrinterCSharpTests.LowBeam
{

    public class ColorAnalyzerLowBeamPremiumTests(ITestOutputHelper output)
    {
        private readonly ITestOutputHelper _output = output;

        public ColorThresholds ColorThresholdsModel3 { get; private set; } = new()
        {
            // Initialize properties here
            LimInfGreen = -1.87,
            LimGreenYellow = -0.8,
            LimYellowOrange = -0.6,
            LimOrangeWhite = -0.48,
            LimWhitePurple = -0.32,
            LimSupPurple = 0.50,
            Adjust = 0.00
        };

        // Initialize properties here
        // Assuming LimGreenYellow is a typo and you meant LimGreenBlue


        public static IEnumerable<object[]> RgbColorTestData => new List<object[]>
        {

            new object[] { -0.90, new RgbColor(0, 255, 0) },//Green
            new object[] { -0.70, new RgbColor(255, 255, 0) },//Yellow
            new object[] { -0.50, new RgbColor(255, 127, 0) },//Orange
            new object[] { -0.40, new RgbColor(0, 0, 0) },//Black
            new object[] { -0.39, new RgbColor(0, 0, 0) },//Black
            new object[] { 0.39, new RgbColor(127, 0, 127) },//Purple
            new object[] { 0.45, new RgbColor(127, 0, 127) },//Purple
           
            // Add more test cases as needed
        };

        [Theory]
        [MemberData(nameof(RgbColorTestData))]
        public void RgbColorMustBeCorrect(double auditAimY, RgbColor expectedRgbColor)
        {
            // Arrange
            var colorAnalyzerLowBeamPremium = new ColorAnalyzerLowBeamPremium(ColorThresholdsModel3, "Low Beam Premium", 0.0f);

            // Act
            var result = colorAnalyzerLowBeamPremium.DetermineColorsAndCalculateRGB(auditAimY);

            _output.WriteLine($"Result Aim: {auditAimY}");
            _output.WriteLine($"Expected: {expectedRgbColor}");
            _output.WriteLine($"Result: {result}");
            // Assert
            expectedRgbColor.Should().Be(result);
        }





    }



}
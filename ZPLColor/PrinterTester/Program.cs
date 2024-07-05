namespace PrinterTester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);


            // Configure the options
            var configuration = builder.Configuration;
            var printerDataSection = configuration.GetSection("PrinterData");

            builder.Services.Configure<List<TestData>>(printerDataSection);
            builder.Services.AddHostedService<Worker>();





            var host = builder.Build();
            host.Run();
        }
    }
}
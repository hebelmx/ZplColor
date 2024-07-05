using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System.Diagnostics;
using System.IO.Ports;
using ZplColor.Common;
using ZplColor.Interfaces;
using ZplColor.Printers;

namespace ZplColor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            // Configure app configuration
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            // Configure logging
            builder.Logging.ClearProviders();
            var logger = ProgramExtensions.InitializeSerilog(builder.Configuration);
            logger.Information("Configuring");

            builder.Logging.AddSerilog(logger);

            // Configure services
            builder.Services.ConfigureServices(builder.Configuration, builder.Environment, logger);

            try
            {
                logger.Information("Starting up");

                var host = builder.Build();
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }

    public static class ProgramExtensions
    {
        public static Serilog.ILogger InitializeSerilog(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.WithProperty("ProcessName", "PrinterCSharp")
                .CreateLogger();
        }

        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment, Serilog.ILogger logger)
        {
            services.Configure<SerialPortConfig>(configuration.GetSection("SerialPort"));
            services.Configure<GatewayConfig>(configuration.GetSection("GatewayConfig"));
            services.Configure<ModelConfigurations>(configuration.GetSection("ModelConfigurations"));

            var modelConfigurations = new ModelConfigurations();
            configuration.GetSection("ModelConfigurations").Bind(modelConfigurations);
            logger.Information($"ModelConfigurations: {modelConfigurations}");

            // Register services
            services.AddSingleton(modelConfigurations);
            services.AddSingleton<CounterService>();
            services.AddSingleton<SerialCommunicator>();
            services.AddSingleton<DataProcessor>();
            services.AddSingleton<PrinterManager>();
            services.AddSingleton<IPrinter, PrinterSerial>();

            services.AddSingleton(provider =>
            {
                var config = provider.GetRequiredService<IOptions<SerialPortConfig>>().Value;
                ConfigureSerialPort(config, logger);
                return config;
            });

            services.AddSingleton(provider =>
            {
                var config = provider.GetRequiredService<IOptions<GatewayConfig>>().Value;
                ConfigureGatewayConfig(config, environment, logger);
                return config;
            });

            services.AddSingleton(provider =>
            {
                var colorThresholdsData = new Dictionary<string, ColorThresholds>();
                configuration.GetSection("ColorThresholds").Bind(colorThresholdsData);
                var analyzers = GetColorAnalyzersFromThresholdsData(colorThresholdsData, logger);
                return analyzers;
            });

            services.AddHostedService<PrinterWorker>();

            return services;
        }

        public static void ConfigureSerialPort(SerialPortConfig config, Serilog.ILogger logger)
        {
            string[] ports = SerialPort.GetPortNames();
            config.PortFound = ports.Any(port => port == config.PortName);

            if (config.PortFound)
            {
                logger.Information($"Serial port {config.PortName} found and configured");
                return;
            }

            logger.Error($"Serial port {config.PortName} not found");
            ports.ToList().ForEach(port => logger.Information($"Available port: {port}"));

            var fallbackPort = ports.FirstOrDefault(p => p.Contains("tty") && p.Contains("USB")) ??
                               ports.FirstOrDefault(p => p.Contains("COM"));

            config.PortName = config.UseFoundedSerialPort && fallbackPort is not null
                ? fallbackPort
                : throw new InvalidDataException($"Serial port {config.PortName} not found");

            config.PortFound = true;
            logger.Information($"Serial port {config.PortName} found and configured");
        }

        public static void ConfigureGatewayConfig(GatewayConfig config, IHostEnvironment environment, Serilog.ILogger logger)
        {
            var isDevelopment = environment.IsDevelopment();
            var isDebuggerAttached = Debugger.IsAttached;

            logger.Information($"Environment: {environment.EnvironmentName}");
            logger.Information($"Debugger Attached: {isDebuggerAttached}");

            if (!isDebuggerAttached)
            {
                config.SendProgram = true;
            }

            logger.Information($"GatewayConfig: {config}");
        }

        public static Dictionary<string, IColorAnalyzer> GetColorAnalyzersFromThresholdsData(Dictionary<string, ColorThresholds> colorThresholdsData, Serilog.ILogger logger)
        {
            var colorAnalyzer = new Dictionary<string, IColorAnalyzer>();

            foreach (var (key, value) in colorThresholdsData)
            {
                var className = key;

                var classType = Type.GetType(className);
                if (classType != null)
                {
                    var constructorInfo = classType.GetConstructor(new[] { typeof(ColorThresholds), typeof(string), typeof(double) });
                    if (constructorInfo != null)
                    {
                        var classInstance = constructorInfo.Invoke(new object[] { value, value.Name, value.Adjust });
                        colorAnalyzer.Add(key, (IColorAnalyzer)classInstance);
                        logger.Information($"ColorAnalyzer {key} created and configured.");
                    }
                }
            }

            return colorAnalyzer;
        }
    }
}

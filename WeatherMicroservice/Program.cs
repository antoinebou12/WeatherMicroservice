using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Elasticsearch;


namespace WeatherMicroservice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
            {
                AutoRegisterTemplate = true,
            })
            .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog() // <-- Add this line
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddUserSecrets<Program>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        var configuration = config.Build(); // Create the configuration

                        webBuilder.UseSentry(o =>
                        {
                            // Read the DSN from user secrets
                            o.Dsn = "";
                            // When configuring for the first time, to see what the SDK is doing:
                            o.Debug = true;
                            // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
                            // We recommend adjusting this value in production.
                            o.TracesSampleRate = 1.0;
                        });
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}

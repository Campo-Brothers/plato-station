using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;
using System;

namespace plato.server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.File("VAR/LOG/plato-server.log")
            .WriteTo.Console()
            .CreateLogger();

            try
            {
                Log.Information("Starting up...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception err)
            {
                Log.Fatal(err, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel().UseUrls("http://*:5000").UseStartup<Startup>();
                });
    }
}

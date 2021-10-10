using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Debugging;

using System;
using System.Diagnostics;

namespace PokemonCRUD.API
{
    public class Program
    {
        public static IConfiguration BuiltConfiguration { get; } = new ConfigurationBuilder()
                                                                       .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                                                       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                                       .AddEnvironmentVariables()
                                                                       .Build();
        public static void Main(string[] args)
        {
            //Initialize Logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(BuiltConfiguration)             
                .CreateLogger();

            SelfLog.Enable(msg =>
            {
                Debug.Print(msg);
                Debugger.Break();
            });

            try
            {
                Log.Information("Application Starting.");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The Application failed to start.");
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
                    webBuilder.UseStartup<Startup>();
                });


    }
}

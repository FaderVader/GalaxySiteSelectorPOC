using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Logging;

namespace GalaxyWeb
{
    public class Program
    {
        public static string programDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData); //%PROGRAMDATA%/
        public static string basePath = Path.Combine(programDataPath, "GalaxyWeb");
        // We expext to get basepath defined as  -> @"C:\ProgramData\GalaxyWeb\";
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseContentRoot(basePath)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();

                    // service-version needs write-access to the log-location - %PROGRAMDATA% provides that
                    var logPath = Path.Combine(basePath, "GalaxyPOC-{Date}.log");
                    logging.AddFile(logPath); 
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("https://*:5050");
                });
    }
}

using System.IO;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Elastic.Search.Web
{
    /// <summary>
    /// You should know who is <see cref="Program"/>
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main
        /// </summary>
        public static void Main(string[] args)
        {
            TelemetryConfiguration.Active.DisableTelemetry = true;

            BuildWebHost(args)
                .Run();
        }

        /// <summary>
        /// Build WebHost
        /// </summary>
        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    builder.AddConsole();
                    builder.AddDebug();
                })
                .Build();
    }
}

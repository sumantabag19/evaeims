using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using NLog.Web;

namespace AuthorizationServer.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //BuildWebHost(args).Run();
            var host = BuildWebHost(args).Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Seeded the database.");

            host.Run();
        }

        public static IWebHostBuilder BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseNLog()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    // Requires `using Microsoft.Extensions.Logging;`
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                });

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DeleteLoggedOutTokensScheduler.Common;
using DeleteLoggedOutTokensScheduler.ScheduledTask;
using DeleteLoggedOutTokensScheduler.SchedulerConfiguration.Scheduling;
using EVA.EIMS.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DeleteLoggedOutTokensScheduler
{
    public class Startup
    {
        const string settingFolderName = "Appsettings";
        readonly IHostingEnvironment _hostingEnvironment;
        public IConfigurationRoot Configuration { get; }
        public Startup(IHostingEnvironment env)
        {
            _hostingEnvironment = env;

            var builder = new ConfigurationBuilder()
                 .SetBasePath(GetRootPath())
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SchedulerSettings>(Configuration.GetSection("SchedulerSettings"));
            services.AddMvc();
            services.AddSingleton<IScheduledTask, DeleteLoggedOutTokenScheduler>();
            services.AddSingleton<EVA.EIMS.Logging.ILogger, EVA.EIMS.Logging.Logger>();

            services.AddScheduler((sender, args) =>
            {
                Console.Write(args.Exception.Message);
                args.SetObserved();
            });

            services.Configure<LoggerConfig>(Configuration.GetSection("LoggerConfig"));

            services.AddHttpClient();

            // Initialize looger configuration oject
            var config = new NLog.Config.LoggingConfiguration();
            // Targets where to log to: File and Console
            NLog.Targets.Target logTarget = null;
            if (Configuration["LoggerConfig:LogWriteType"] == LogConstants.File)
                logTarget = new NLog.Targets.FileTarget("logfile") { FileName = Configuration["LoggerConfig:FileName"].Replace("/",Path.DirectorySeparatorChar.ToString()), Layout = Configuration["LoggerConfig:LoggerLayout"], ArchiveEvery = NLog.Targets.FileArchivePeriod.Month };
            else
                logTarget = new NLog.Targets.ConsoleTarget("logconsole") { Layout = Configuration["LoggerConfig:LoggerLayout"] };
            // Rules for mapping loggers to targets            
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logTarget, Configuration["LoggerConfig:LoggerNamePattern"]);
            // Apply config           
            NLog.LogManager.Configuration = config;

            //Adding Healt Checks in services
            services.AddHealthChecks().AddSqlServer(string.Empty, "Select 1");

        }

        private object BuildWebHost(object args)
        {
            throw new NotImplementedException();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();
            }
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        public static string GetRootPath()
        {
            var path = Directory.GetCurrentDirectory();
            var rootPath = Path.Combine(path, settingFolderName);
            return rootPath;
        }
    }
}

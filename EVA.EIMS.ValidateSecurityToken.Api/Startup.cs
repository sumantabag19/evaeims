using EVA.EIMS.Common;
using EVA.EIMS.Data;
using EVA.EIMS.Helper;
using EVA.EIMS.ValidateSecurityToken.Api.Filter;
using EVA.EIMS.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Targets;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace EVA.EIMS.ValidateSecurityToken.Api
{
    public class Startup
    {
        const string settingFolderName = "Appsettings";
        readonly IHostingEnvironment _hostingEnvironment;
        public IConfigurationRoot Configuration { get; }
        readonly string AllowedSpecificOrigins = "CorsPolicy";
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


            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));

            DIContainer.Common(services);

            switch (Configuration["ApplicationSettings:UseDatabase"])
            {
                case "SqlDatabase":
                    //Add SQl connection string settings
                    services.AddDbContextPool<DataContext>(Options => Options.UseSqlServer(Configuration.GetConnectionString("SQLConnection"), providerOptions => providerOptions.EnableRetryOnFailure(3)));
                    DIContainer.SqlInjector(services);
                    break;
                case "MySqlDatabase":
                    // Add Mysql connection string settings
                    services.AddDbContext<DataContext>(Options => Options.UseMySql(Configuration.GetConnectionString("MySQLConnection"), providerOptions => providerOptions.EnableRetryOnFailure(3)));
                    DIContainer.MySqlInjector(services);
                    break;
            }
            //services.AddDistributedRedisCache(options =>
            //{
            //    options.Configuration = Configuration["ApplicationSettings:RedisQueue"];
            //});
            //services.AddCors();
            //services.AddCors();
            string[] origins = Configuration["ApplicationSettings:AllowedDomains"].Split(',');
            services.AddCors(o => o.AddPolicy(AllowedSpecificOrigins, builder =>
            {
                builder.WithOrigins(origins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .SetPreflightMaxAge(TimeSpan.FromSeconds(2520));
            }));
            //Added versioning for API, default is 1.0

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                //Set API default version as 1.0
                options.DefaultApiVersion = new ApiVersion(1, 0);
                //Accept version details in request header
                options.ApiVersionReader = new HeaderApiVersionReader(ApiVersionConstant.ApiVersion);
            });

            // Register the Swagger generator, defining one or more Swagger documents  
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "IMS Validation API", Version = "v1" });
                c.OperationFilter<AddAuthTokenHeaderParameter>();
            });
            services.AddMvc(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory(AllowedSpecificOrigins));
            });

            //services.AddMvc(options =>
            //{
            //    //Add Token validation filter
            //    //options.Filters.Add(new AuthorizationTokenFilter());

            //    ////Add Role access permission validation filter
            //    //options.Filters.Add(new RolePermissionFilter());

            //    ////Add Exception filter
            //    //options.Filters.Add(new CustomExceptionFilter());

            //});

            services.Configure<LoggerConfig>(Configuration.GetSection("LoggerConfig"));

            services.AddHttpClient();

            // Initialize looger configuration oject
            var configLogger = new NLog.Config.LoggingConfiguration();
            // Targets where to log to: File and Console
            NLog.Targets.Target logTarget = null;
            if (Configuration["LoggerConfig:LogWriteType"] == LogConstants.File)
                logTarget = new NLog.Targets.FileTarget("logfile") { FileName = Configuration["LoggerConfig:FileName"].Replace("/", Path.DirectorySeparatorChar.ToString()), Layout = Configuration["LoggerConfig:LoggerLayout"], ArchiveEvery = NLog.Targets.FileArchivePeriod.Month };
            else
                logTarget = new NLog.Targets.ConsoleTarget("logconsole") { Layout = Configuration["LoggerConfig:LoggerLayout"] };
            // Rules for mapping loggers to targets            
            configLogger.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logTarget, Configuration["LoggerConfig:LoggerNamePattern"]);

            string sqlConnectionstring = Configuration["ConnectionStrings:SQLConnection"];
            //Reconfigure NLog to set environment specific database connection string
            var config = LogManager.Configuration;
            var dbTarget = (DatabaseTarget)config.FindTargetByName("database");
            dbTarget.ConnectionString = Configuration.GetConnectionString("SQLConnection");

            configLogger.AddRule(NLog.LogLevel.Error, NLog.LogLevel.Fatal, dbTarget);

            // Apply configLogger           
            NLog.LogManager.Configuration = configLogger;
            LogManager.ReconfigExistingLoggers();
        

            services.AddHealthChecks().AddSqlServer(sqlConnectionstring, "Select 1");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            // Enable middleware to serve generated Swagger as a JSON endpoint.  
            app.UseSwagger();

            //Set cros origin 
            app.UseCors(AllowedSpecificOrigins);

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.  
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "EIMS Validation API");
            });
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("api-supported-versions");
                await next();
            });
            app.UseMvc();
        }
        public static string GetRootPath()
        {
            var path = Directory.GetCurrentDirectory();
            var rootPath = Path.Combine(path, settingFolderName);
            return rootPath;
        }
    }
}

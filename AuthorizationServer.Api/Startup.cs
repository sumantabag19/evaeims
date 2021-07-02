using AuthorizationServer.Api.Filter;
using AuthorizationServer.Api.Formats;
using AuthorizationServer.Api.Providers;
using EVA.EIMS.Common;
using EVA.EIMS.Data;
using EVA.EIMS.Helper;
using EVA.EIMS.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Security.Cryptography;

namespace AuthorizationServer.Api
{
    public class Startup
    {
        readonly IHostingEnvironment _hostingEnvironment;
        public IConfigurationRoot Configuration { get; }
        readonly string AllowedSpecificOrigins = "CorsPolicy";

        static string settingFolderName = "Appsettings";
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
            services.Configure<LoggerConfig>(Configuration.GetSection("LoggerConfig"));

            DIContainer.Common(services);

            switch (Configuration["ApplicationSettings:UseDatabase"])
            {
                case "SqlDatabase":
                    //Add SQl connection string settings
                    //services.AddDbContextPool<DataContext>(Options => Options.UseSqlServer(Configuration.GetConnectionString("SQLConnection"), providerOptions => providerOptions.EnableRetryOnFailure(3)), 300);
                    services.AddDbContext<DataContext>(c =>
                     c.UseSqlServer(Configuration.GetConnectionString("SQLConnection"),
                     sqlServerOptionsAction: sqlOptions =>
                     {
                         sqlOptions.EnableRetryOnFailure(
                         maxRetryCount: 2,
                         maxRetryDelay: TimeSpan.FromSeconds(30),
                         errorNumbersToAdd: null);
                     }), ServiceLifetime.Transient);
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


            var provider = services.BuildServiceProvider();
            //

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    //options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //    options.OnAppendCookie = cookieContext =>
            //        CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            //    options.OnDeleteCookie = cookieContext =>
            //        CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            //});
            services.AddAuthentication(options => { options.DefaultAuthenticateScheme = "JWT"; options.DefaultSignOutScheme = "JWT"; }).AddOAuthValidation().AddOpenIdConnectServer(options =>
            {
                options.AuthorizationEndpointPath = new PathString("/authorize");
                options.AuthorizationCodeFormat = new CustomJwtFormat(Configuration["ApplicationSettings:IMSEndPoint"], provider);
                options.LogoutEndpointPath = new PathString("/account/logout");


                options.AllowInsecureHttp = !Convert.ToBoolean(Configuration["ApplicationSettings:EnableSSL"]);
                options.TokenEndpointPath = new PathString("/connect/token");
                options.AccessTokenLifetime = TimeSpan.FromMinutes(Convert.ToInt32(Configuration["ApplicationSettings:AccessTokenExpireTimeSpanInMinutes"]));
                options.Provider = new CustomOAuthProvider(provider);

                //options.LogoutEndpointPath = new PathString("/connect/logout");
                options.AccessTokenFormat = new CustomJwtFormat(Configuration["ApplicationSettings:IMSEndPoint"], provider);
                options.RefreshTokenLifetime = TimeSpan.FromTicks(Convert.ToInt64(Configuration["ApplicationSettings:RefreshTokenExpireTimeSpanInMinutes"]));
                string privateKeyPath = _hostingEnvironment.ContentRootPath + Path.DirectorySeparatorChar + Configuration["ApplicationSettings:PrivateKeyPath"];
                RSA privateRsa = RSAHelper.PrivateKeyFromPemFile(privateKeyPath);
                var privateKey = new RsaSecurityKey(privateRsa);
                SigningCredentials signingCredentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);
                options.SigningCredentials.Add(signingCredentials);
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory(AllowedSpecificOrigins));
                options.Filters.Add(typeof(ApiExceptionFilter));
            });

            // Register the Swagger generator, defining one or more Swagger documents  
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Authorization API", Version = "v1" });
                c.OperationFilter<AddAuthTokenHeaderParameter>();
            });

            services.AddHttpClient();

            // Initialize looger configuration oject
            var config = new NLog.Config.LoggingConfiguration();
            // Targets where to log to: File and Console
            NLog.Targets.Target logTarget = null;
            if (Configuration["LoggerConfig:LogWriteType"] == LogConstants.File)
                logTarget = new NLog.Targets.FileTarget("logfile") { FileName = Configuration["LoggerConfig:FileName"].Replace("/", Path.DirectorySeparatorChar.ToString()), Layout = Configuration["LoggerConfig:LoggerLayout"], ArchiveEvery = NLog.Targets.FileArchivePeriod.Month };
            else
                logTarget = new NLog.Targets.ConsoleTarget("logconsole") { Layout = Configuration["LoggerConfig:LoggerLayout"] };
            // Rules for mapping loggers to targets            
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logTarget, Configuration["LoggerConfig:LoggerNamePattern"]);
            // Apply config           
            NLog.LogManager.Configuration = config;

            string sqlConnectionstring = Configuration["ConnectionStrings:SQLConnection"];
            services.AddHealthChecks().AddSqlServer(sqlConnectionstring, "Select 1");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //Set cros origin 
            app.UseCors(AllowedSpecificOrigins);
            // Enable middleware to serve generated Swagger as a JSON endpoint.  
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.  
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(Path.DirectorySeparatorChar + "swagger" + Path.DirectorySeparatorChar + "v1" + Path.DirectorySeparatorChar + "swagger.json", "EIMS Authorization API");
            });


            //app.UseCookiePolicy();

            app.UseAuthentication();

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("api-supported-versions");
                await next();

            });
            app.UseMvc();

        }

        //private void CheckSameSite(HttpContext httpContext, CookieOptions options)
        //{
        //    if (options.SameSite == SameSiteMode.None)
        //    {
        //        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
        //        if (DisallowsSameSiteNone(userAgent))
        //        {
        //            options.SameSite = (SameSiteMode)(-1);
        //        }
        //    }
        //}


        public static string GetRootPath()
        {
            var path = Directory.GetCurrentDirectory();
            var rootPath = Path.Combine(path, settingFolderName);
            return rootPath;
        }
    }
}

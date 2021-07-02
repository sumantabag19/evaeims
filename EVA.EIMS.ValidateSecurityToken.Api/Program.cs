using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace EVA.EIMS.ValidateSecurityToken.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            NLogBuilder.ConfigureNLog("nlog.config");
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseNLog()
                .UseStartup<Startup>()
                .UseNLog()
                .Build();
    }
}

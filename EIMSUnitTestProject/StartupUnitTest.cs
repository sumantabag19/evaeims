using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xunit;
using EVA.EIMS.Security.API.Controllers;
using EVA.EIMS.Entity;
using Microsoft.AspNetCore.Http;
using EVA.EIMS.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EVA.EIMS.Security.API;
using EVA.EIMS.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using EVA.EIMS.Entity.ViewModel;
using EVA.EIMS.Helper;
using System;


namespace EIMSUnitTestProject
{
    public class StartupUnitTest<T> where T:Controller
    {
        private T _tController;
        public T ConfigureServices()
        {
            var configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", false)
              .Build();
            IServiceCollection services = new ServiceCollection();
            services.Configure<ApplicationSettings>(configuration.GetSection("ApplicationSettings"));
            services.AddTransient<T>();

            DIContainer.Common(services);
            switch (configuration["ApplicationSettings:UseDatabase"])
            {
                case "SqlDatabase":
                    //Add SQl connection string settings
                    services.AddDbContext<DataContext>(Options => Options.UseSqlServer(configuration["ConnectionStrings:SQLConnection"]));
                    DIContainer.SqlInjector(services);
                    break;
                case "MySqlDatabase":
                    // Add Mysql connection string settings
                    services.AddDbContext<DataContext>(Options => Options.UseMySql(configuration["ConnectionStrings:MySQLConnection"]));
                    DIContainer.MySqlInjector(services);
                    break;
            }
            var serviceProvider = services.BuildServiceProvider();

            _tController = serviceProvider.GetService<T>();

            _tController.ControllerContext.HttpContext = new DefaultHttpContext();
            _tController.ControllerContext.HttpContext.Items.Add(KeyConstant.OrgId, TestConstant.OrgId);
            _tController.ControllerContext.HttpContext.Items.Add(KeyConstant.Client_Type, TestConstant.Client_Type);
            _tController.ControllerContext.HttpContext.Items.Add(KeyConstant.UserName, TestConstant.UserName);
            _tController.ControllerContext.HttpContext.Items.Add(KeyConstant.ClientId, TestConstant.ClientId);
            _tController.ControllerContext.HttpContext.Items.Add(TestConstant.Authorization, TestConstant.Token);
            List<string> rlist = new List<string>();
            rlist.AddRange(TestConstant.Role);
            _tController.ControllerContext.HttpContext.Items.Add(KeyConstant.Role, TestConstant.Role);
            return _tController;
        }
    }
}

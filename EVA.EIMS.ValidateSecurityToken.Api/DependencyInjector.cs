using EVA.EIMS.Business;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVA.EIMS.Data;
using EVA.EIMS.Common;
using EVA.EIMS.Helper;
using Microsoft.AspNetCore.Http;
using EVA.EIMS.Repository.CommonRepository;


namespace EVA.EIMS.ValidateSecurityToken.Api
{
    public static class DIContainer
    {
        public static void SqlInjector(IServiceCollection services)
        {
            #region Repository for Sql Server specific operations           
            services.AddScoped(typeof(IExecuterStoreProc<>), typeof(SqlProcExecuterRepository<>));
            #endregion
        }

        public static void MySqlInjector(IServiceCollection services)
        {
            #region Repository for MySql specific operations      
            services.AddScoped(typeof(IExecuterStoreProc<>), typeof(MySqlProcExecuterRepository<>));
            #endregion
        }


        public static void Common(IServiceCollection services)
        {

            #region Add Context And UnitOfWork
            //Register Context
            services.AddTransient<DbContext, DataContext>();

            //Register UnitOfWork
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            #endregion
            services.AddMvc();
            //services.AddScoped<ClassRequestInformationAttribute>();

            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<IClientBusiness, ClientBusiness>();

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserBusiness, UserBusiness>();

            services.AddTransient<IApplicationRepository, ApplicationRepository>();
            services.AddTransient<IApplicationBusiness, ApplicationBusiness>();

            services.AddTransient<IOrganizationRepository, OrganizationRepository>();
            services.AddTransient<IOrganizationBusiness, OrganizationBusiness>();

            services.AddTransient<IIPTableRepository, IPTableRepository>();
            services.AddTransient<IIPTableBusiness, IPTableBusiness>();

            services.AddScoped<IIMSLogOutRepository, IMSLogOutRepository>();
            services.AddScoped<IIMSLogOutBusiness, IMSLogOutBusiness>();

            services.AddTransient<IServiceProvider, ServiceProvider>();

            services.AddTransient<ICustomPasswordHash, CustomPasswordHash>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IClientTypeRepository, ClientTypeRepository>();
            services.AddTransient<IClientTypeBusiness, ClientTypeBusiness>();

            services.AddSingleton<ILogging, Logger>();

            services.AddScoped<IUserRoleMappingRepository, UserRoleMappingRepository>();

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleBusiness, RoleBusiness>();

            services.AddScoped<EVA.EIMS.Logging.ILogger, EVA.EIMS.Logging.Logger>();
        }
    }
}

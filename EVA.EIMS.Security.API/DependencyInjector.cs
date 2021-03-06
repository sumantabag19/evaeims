using EVA.EIMS.Business;
using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Data;
using EVA.EIMS.Helper;
using EVA.EIMS.Repository;
using EVA.EIMS.Repository.CommonRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace EVA.EIMS.Security.API
{
    /// <summary>
    /// Use below class to add dependancies
    /// </summary>
    public static class DIContainer
    {
        /// <summary>
        /// We can add three type of dependancies
        /// 1. Transient :
        ///     Transient lifetime services are created each time they are requested.This lifetime works best for lightweight, stateless services.
        /// 2. Scoped :
        ///     Scoped lifetime services are created once per request.
        /// 3. Singleton :
        ///     Singleton lifetime services are created the first time they are requested (or when ConfigureServices is run if you specify an instance there) 
        ///     and then every subsequent request will use the same instance.
        /// </summary>
        /// <param name="services"></param>
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
            services.AddScoped<DbContext, DataContext>();

            //Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion

            services.AddScoped<ICustomPasswordHash, CustomPasswordHash>();

            #region Add Repository And Business
            services.AddScoped<IAuthProviderRepository, AuthProviderRepository>();
            services.AddScoped<IAuthProviderBusiness, AuthProviderBusiness>();

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductBusiness, ProductBusiness>();

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleBusiness, RoleBusiness>();

            services.AddScoped<IUserRoleMappingRepository, UserRoleMappingRepository>();
            //  services.AddScoped<IUserRoleMappingBusiness, IUserRoleMappingBusiness>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserBusiness, UserBusiness>();

            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IClientBusiness, ClientBusiness>();

            services.AddScoped<IClientTypeRepository, ClientTypeRepository>();
            services.AddScoped<IClientTypeBusiness, ClientTypeBusiness>();

            services.AddScoped<IUserOTPRepository, UserOTPRepository>();

            services.AddScoped<IUserAnswerBusiness, UserAnswerBusiness>();
            services.AddScoped<IUserAnswerRepository, UserAnswerRepository>();

            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRefreshTokenBusiness, RefreshTokenBusiness>();

            services.AddScoped<IDeviceRepository, DeviceRepository>();
            services.AddScoped<IDeviceBusiness, DeviceBusiness>();

            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IOrganizationBusiness, OrganizationBusiness>();

            services.AddScoped<IApplicationRepository, ApplicationRepository>();
            services.AddScoped<IApplicationBusiness, ApplicationBusiness>();

            services.AddScoped<ISecurityQuestionRepository, SecurityQuestionRepository>();
            services.AddScoped<ISecurityQuestionBusiness, SecurityQuestionBusiness>();

            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddScoped<IEmailTemplateBusiness, EmailTemplateBusiness>();

            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<ILanguageBusiness, LanguageBusiness>();

            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRefreshTokenBusiness, RefreshTokenBusiness>();

            services.AddScoped<IApplicationUserMappingRepository, ApplicationUserMappingRepository>();
            services.AddScoped<IApplicationUserMappingBusiness, ApplicationUserMappingBusiness>();

            services.AddScoped<IOrganizationApplicationmappingRepository, OrganizationApplicationmappingRepository>();
            services.AddScoped<IOrganizationApplicationmappingBusiness, OrganizationApplicationmappingBusiness>();

            services.AddScoped<IApplicationRoleMappingRepository, ApplicationRoleMappingRepository>();
            services.AddScoped<IApplicationRoleMappingBusiness, ApplicationRoleMappingBusiness>();

            services.AddScoped<IPasswordHistoryRepository, PasswordHistoryRepository>();
            services.AddScoped<IPasswordHistoryBusiness, PasswordHistoryBusiness>();

            services.AddScoped<IIPTableRepository, IPTableRepository>();
            services.AddScoped<IIPTableBusiness, IPTableBusiness>();

            services.AddScoped<ISendEmailNotificationBusiness, SendEmailNotificationBusiness>();

            services.AddScoped<IOrganizationTenantMappingRepository, OrganizationTenantMappingRepository>();
            services.AddScoped<IOrganizationTenantMappingDomainRepository, OrganzationTenantMappingDomainRepository>();
            services.AddScoped<IOrganizationTenantMapping, OrganizationTenantBusiness>();

            services.AddScoped<IForgotPasswordFlowManagementRepository, ForgotPasswordFlowManagementRepository>();

            services.AddScoped<IUserClientTypeMappingRepository, UserClientTypeMappingRepository>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IEmailSender, EmailSender>();


            services.AddScoped<ILockAccountRepository, LockAccountRepository>();

            #region Role and Client Type Base Access Repository And Business
            services.AddScoped<IModuleRepository, ModuleRepository>();

            services.AddScoped<IClientTypeAccessExceptionRepository, ClientTypeAccessExceptionRepository>();

            services.AddScoped<IRoleAccessExceptionRepository, RoleAccessExceptionRepository>();

            services.AddScoped<IActionsRepository, ActionsRepository>();

            services.AddScoped<IAccessTypeRepository, AccessTypeRepository>();

            services.AddScoped<IRoleModuleAccessRepository, RoleModuleAccessRepository>();
            services.AddScoped<IRoleModuleAccessBusiness, RoleModuleAccessBusiness>();

            services.AddScoped<IClientTypeModuleAccessRepository, ClientTypeModuleAccessRepository>();
            services.AddScoped<IClientTypeModuleAccessBusiness, ClientTypeModuleAccessBusiness>();

            services.AddScoped<IUserOrganizationMappingRepository, UserOrganizationMappingRepository>();

            services.AddScoped<IIMSLogOutRepository, IMSLogOutRepository>();
            services.AddScoped<IIMSLogOutBusiness, IMSLogOutBusiness>();

            services.AddScoped<IUserRoleMappingRepository, UserRoleMappingRepository>();
            #endregion
            #endregion

            services.AddScoped<EVA.EIMS.Logging.ILogger, EVA.EIMS.Logging.Logger>();
        }
    }
}

using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ComplexEntities;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
    public class ApplicationUserMappingBusiness : IApplicationUserMappingBusiness
    {
        private readonly IApplicationUserMappingRepository _applicationUserMappingRepository;
        private bool _disposed;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        public ApplicationUserMappingBusiness(IApplicationUserMappingRepository applicationUserMappingRepository, IServiceProvider serviceProvider, ILogger logger)
        {
            _applicationUserMappingRepository = applicationUserMappingRepository;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _disposed = false;
        }

        /// <summary>
        /// Method to get user mapping with application
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="ClientId"></param>
        /// <param name="ClientSecret"></param>
        /// <returns></returns>
        public async Task<ApplicationUserDetails> GetUserMappingWithApplicationId(string UserId, string ClientId, string ClientSecret)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IExecuterStoreProc<ApplicationUserDetails> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<ApplicationUserDetails>>();
                List<Parameters> param = new List<Parameters>() {
                new Parameters("p_UserId", UserId),
                new Parameters("p_ClientId", ClientId),
                new Parameters("p_ClientSecret", ClientSecret),
                new Parameters("p_OrgName",DBNull.Value)
                };
                var appUserMapping = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetUserMappingWithApplicationId.ToString(), param);
                return appUserMapping.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error("ApplicationUserMappingBusiness", "GetUserMappingWithApplicationId", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ExceptionLogger.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Method to get client id mapping with application based on organization
        /// </summary>
        /// <param name="ClientId"></param>
        /// <param name="ClientSecret"></param>
        /// <param name="OrgName"></param>
        /// <returns></returns>
        public async Task<ApplicationUserDetails> GetClientIdMappingWithApplicationId(string ClientId, string ClientSecret, string OrgName)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IExecuterStoreProc<ApplicationUserDetails> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<ApplicationUserDetails>>();
                List<Parameters> param = new List<Parameters>() {
                new Parameters("p_UserId", DBNull.Value),
                new Parameters("p_ClientId", ClientId),
                new Parameters("p_ClientSecret", ClientSecret),
                new Parameters("p_OrgName", OrgName)

                };
                var appUserMapping = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetUserMappingWithApplicationId.ToString(), param);
				return appUserMapping.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error("ApplicationUserMappingBusiness", "GetClientIdMappingWithApplicationId", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ExceptionLogger.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// This method is used to execute the stored procedure to check if the application is present in the organization and if the user has access to it.
        /// </summary>
        ///<param name = "userId">userId</param>
        /// <param name="appId">userId</param>
        /// <returns>returns a boolean value</returns>
        public async Task<bool> ValidateUserApplicationAccess(Guid userId, int appId)
        {
            try
            {
                IExecuterStoreProc<User> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<User>>();

                List<Parameters> param = new List<Parameters>();
                param.Add(new Parameters("UserId", userId));
                param.Add(new Parameters("AppId", appId));

                var validateUserAccess = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procValidateUserApplicationAccess.ToString(), param);

                if (validateUserAccess != null && validateUserAccess.Count() > 0)
                    return true;

                return false;

            }
            catch (Exception ex)
            {
                _logger.Error("ApplicationUserMappingBusiness", "ValidateUserApplicationAccess", ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _applicationUserMappingRepository.Dispose();
            }

            _disposed = true;
        }

        /// <summary>
        /// Method to dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

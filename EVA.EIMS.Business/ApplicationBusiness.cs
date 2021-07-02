using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ViewModel;
using EVA.EIMS.Logging;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
    public class ApplicationBusiness : IApplicationBusiness
    {
        #region Private Variable
        private readonly IApplicationRepository _applicationRepository;
        IServiceProvider _serviceProvider;
        private bool _disposed;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public ApplicationBusiness(IServiceProvider serviceProvider, IApplicationRepository applicationRepository, ILogger logger)
        {
            _applicationRepository = applicationRepository;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _disposed = false;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// This method is used get the applications.
        /// </summary>
        /// <returns>returns multiple application details</returns>
        public async Task<ReturnResult> Get(TokenData tokenData)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IExecuterStoreProc<Application> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<Application>>();
                List<Parameters> param = new List<Parameters>() {
                    new Parameters("p_role", tokenData.Role.FirstOrDefault()),
                    new Parameters("p_UserName", tokenData.UserName)};

                var requiredApplicationInfo = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetApplicationByRole, param);

                if (requiredApplicationInfo != null && requiredApplicationInfo.Count() > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = requiredApplicationInfo;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ResourceInformation.GetResValue("Application")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return await Task.FromResult(returnResult);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ApplicationBusiness", "Get", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used get the application details by id.
        /// </summary>
        /// <param name="appId">applicationId</param>
        /// <returns>returns single application details</returns>
        public async Task<ReturnResult> GetById(int appId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var requiredApplicationInfo = await _applicationRepository.SelectFirstOrDefaultAsync(a => a.AppId.Equals(appId));
                if (requiredApplicationInfo != null)
                {
                    returnResult.Success = true;
                    returnResult.Data = requiredApplicationInfo;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")} " +
                                          $"{ResourceInformation.GetResValue("Application")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ApplicationBusiness", "GetById", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to save the application details
        /// </summary>
        /// <param name="application">application object</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Save(string userName, Application application)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (application == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(application.AppName))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideApplicationName");
                    returnResult.Success = false;
                    return returnResult;
                }

                if (await _applicationRepository.SelectFirstOrDefaultAsync(a => a.AppName.Equals(application.AppName) && !a.IsActive.Value) != null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ExistsAndInActive");
                    returnResult.Success = false;
                    return returnResult;
                }

                if (await _applicationRepository.SelectFirstOrDefaultAsync(a => a.AppName.Equals(application.AppName) && a.IsActive.Value) != null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("Application")} {application.AppName} { ResourceInformation.GetResValue("AlreadyExist")}";
                    returnResult.Success = false;
                    return returnResult;
                }
                IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                Guid userId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive == true)).UserId;

                if (application.IsActive == null)
                {
                    application.IsActive = true;
                }

                if (application.IsPwdExpNotify == null || application.IsPwdExpNotify == false)
                {
                    application.IsPwdExpNotify = false;
                    application.PwdExpNotifyDays = 0;
                }

                if (application.IsPwdExpNotify == true && application.PwdExpNotifyDays == 0)
                {
                    application.PwdExpNotifyDays = 7;
                }

                application.CreatedBy = userId;
                application.CreatedOn = DateTime.Now;
                application.ModifiedBy = userId;
                application.ModifiedOn = DateTime.Now;

                var result = await _applicationRepository.AddAsync(application);

                if (result.State.Equals(EntityState.Added))
                {
                    await _applicationRepository.UnitOfWork.SaveChangesAsync();

                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("DataSavedSuccess");
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DataSavedFailure");
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("ApplicationBusiness", "Save", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to update the application details
        /// </summary>
        /// <param name="appId">applicationId</param>
        /// <param name="application">application object</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Update(string userName, int appId, Application application)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {
                if (appId == 0)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideAppId");
                    return returnResult;
                }

                var updateApplication = await _applicationRepository.SelectFirstOrDefaultAsync(a => a.AppId == appId);

                if (updateApplication == null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("Application")} {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                if (application.IsActive == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("IsActiveRequired");
                    return returnResult;
                }
                if (application.IsPwdExpNotify == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("IsPwdExpNotifyRequired");
                    return returnResult;
                }

                if (application.IsPwdExpNotify == true && application.PwdExpNotifyDays == 0)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("PwdExpNotifyDaysRequired");
                    return returnResult;
                }
                updateApplication.IsPwdExpNotify = application.IsPwdExpNotify.Value;
                updateApplication.IsActive = application.IsActive.Value;
                updateApplication.AppName = application.AppName;
                updateApplication.AppUrl = application.AppUrl;
                updateApplication.Description = application.Description;
                updateApplication.PwdExpNotifyDays = application.PwdExpNotifyDays;
                IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                updateApplication.ModifiedBy = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive == true)).UserId;
                updateApplication.ModifiedOn = DateTime.Now;
                updateApplication.AzureAppId = application.AzureAppId;

                var result = await _applicationRepository.UpdateAsync(updateApplication);

                if (result.State.Equals(EntityState.Modified))
                {
                    await _applicationRepository.UnitOfWork.SaveChangesAsync();

                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("DataUpdateSuccess");
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DataUpdateFailure");
                }

                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("ApplicationBusiness", "Update", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to delete the application details
        /// </summary>
        /// <param name="appId">application</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Delete(string userName, int appId)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {
                var deleteapplication = await _applicationRepository.SelectFirstOrDefaultAsync(a => a.AppId == appId && a.IsActive.Value);

                if (deleteapplication == null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("Application")} {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                deleteapplication.IsActive = false;

                IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                deleteapplication.ModifiedBy = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive == true)).UserId;
                deleteapplication.ModifiedOn = DateTime.Now;
                var result = await _applicationRepository.UpdateAsync(deleteapplication);

                if (result.State.Equals(EntityState.Modified))
                {
                    await _applicationRepository.UnitOfWork.SaveChangesAsync();

                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("DataDeleteSuccess");
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DataDeleteFailure");
                }

                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("ApplicationBusiness", "Delete", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used get the application Id by appName.
        /// </summary>
        /// <param name="appId">applicationId</param>
        /// <returns>returns single application Id/returns>
        public async Task<int> GetByAppName(string appName)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var requiredApplicationId = (await _applicationRepository.SelectFirstOrDefaultAsync(a => a.AppName.Equals(appName, StringComparison.OrdinalIgnoreCase) && a.IsActive.Value)).AppId;
                if (requiredApplicationId != 0)
                {
                    return requiredApplicationId;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ApplicationBusiness", "GetByAppName", ex.Message, ex.StackTrace);
                return 0;
            }
        }

    /// <summary>
    /// This method is used get the application Id by appName.
    /// </summary>
    /// <param name="appId">applicationId</param>
    /// <returns>returns single application Id/returns>
    public async Task<int> GetByApplicationName(string appName)
    {
      ReturnResult returnResult = new ReturnResult();
      try
      {
        var requiredApplicationId = (await _applicationRepository.SelectFirstOrDefaultAsync(a => a.AppName.Equals(appName, StringComparison.OrdinalIgnoreCase)));
        if (requiredApplicationId != null)
        {
          return requiredApplicationId.AppId;
        }
        else
        {
          return 0;
        }
      }
      catch (Exception ex)
      {
        _logger.Error("ApplicationBusiness", "GetByApplicationName", ex.Message, ex.StackTrace);
        return 0;
      }
    }

        /// <summary>
        /// Method to return application name to which client has access
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<string> GetAppNameFromClientId(string clientId)
        {
            IClientRepository clientRepository = _serviceProvider.GetRequiredService<IClientRepository>();

            var result = await clientRepository.UnitOfWork.DbContext.Set<OauthClient>()
                 .Join(_applicationRepository.UnitOfWork.DbContext.Set<Application>(),
                c => c.AppId, a => a.AppId, (c, a) => new { c, a })
                .Where(x => x.c.ClientId.Equals(clientId))
                .Select(x => new Application { AppName = x.a.AppName })
                .FirstOrDefaultAsync();

            return result.AppName;

        }

        /// <summary>
        /// Method to return client data by azure app id
        /// </summary>
        /// <param name="azureAppId"></param>
        /// <returns></returns>
        public async Task<OauthClient> GetClientIdByAzureAppId(string azureAppId)
        {
            IClientRepository clientRepository = _serviceProvider.GetRequiredService<IClientRepository>();
            Guid appid = Guid.Empty;
            Guid.TryParse(azureAppId, out appid);
            var result = await clientRepository.UnitOfWork.DbContext.Set<OauthClient>()
                 .Join(_applicationRepository.UnitOfWork.DbContext.Set<Application>(),
                c => c.AppId, a => a.AppId, (c, a) => new { c, a })
                .Where(x => x.a.AzureAppId.Equals(appid) && x.c.Flow.Equals(KeyConstant.AuthCode))
                .Select(x => new OauthClient
                {
                    ClientId = x.c.ClientId,
                    ClientSecret = x.c.ClientSecret,
                    TokenValidationPeriod = x.c.TokenValidationPeriod,
                    AppId = x.c.AppId,
                })
                .FirstOrDefaultAsync();

            return result;

        }


        /// <summary>
        /// Method to return application based on appid to which client has access
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<Application> GetAppById(int appid)
        {
            try
            {
                return await _applicationRepository.SelectFirstOrDefaultAsync(a => a.AppId == appid && a.IsActive.Value);
            }
            catch (Exception ex)
            {
                _logger.Error("ApplicationBusiness", "GetAppById", ex.Message, ex.StackTrace);
                return null;
            }
        }
        /// <summary>
        /// This Method return application list as per given user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>return application list</returns>
        public async Task<ReturnResult> GetAllApplicationByUserId(Guid userId, TokenData tokenData)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IExecuterStoreProc<UserApplicationDetails> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<UserApplicationDetails>>();
                List<Parameters> param = new List<Parameters>() {
                    new Parameters("p_UserId", userId),
                    new Parameters("p_role", tokenData.Role.FirstOrDefault()),
                    new Parameters("p_UserName", tokenData.UserName)};

                var requiredApplicationInfo = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetApplicationByUserId, param);

                if (requiredApplicationInfo != null && requiredApplicationInfo.Count > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = requiredApplicationInfo;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")} " +
                                          $"{ResourceInformation.GetResValue("Application")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ApplicationBusiness", "GetAllApplicationByUserId", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }
        public async Task<ReturnResult> SetAzureAppId(int appid, Guid adid)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var app = await _applicationRepository.SelectFirstOrDefaultAsync(a => a.AppId == appid);
                if (app == null)
                {
                    throw new Exception("App ID not Found");
                }
                app.AzureAppId = adid;
                await _applicationRepository.UpdateAsync(app);
                await _applicationRepository.UnitOfWork.SaveChangesAsync();
                returnResult.Success = true;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateSuccess")}";
                return returnResult;
            }
            catch (Exception ex)
            {
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Method to dispose by parameter.
        /// </summary>
        /// <param name="disposing"></param>
        /// 
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _applicationRepository.Dispose();
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
        #endregion
    }
}

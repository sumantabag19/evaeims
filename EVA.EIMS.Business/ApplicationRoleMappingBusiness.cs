using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ComplexEntities;
using EVA.EIMS.Entity.ViewModel;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
    public class ApplicationRoleMappingBusiness : IApplicationRoleMappingBusiness
    {
        #region Private Variable
        private readonly IApplicationRoleMappingRepository _applicationRoleMappingRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private bool _disposed;
        #endregion

        #region Constructor
        public ApplicationRoleMappingBusiness(IServiceProvider serviceProvider, IApplicationRoleMappingRepository applicationRoleMappingRepository, ILogger logger)
        {
            _applicationRoleMappingRepository = applicationRoleMappingRepository;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _disposed = false;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// To get all Role Details Assingn to an Application
        /// </summary>
        /// <returns>Returns a list of RoleModel </returns>
        public async Task<ReturnResult> GetApplicationRoles(int appId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (appId == 0)
                {
                    returnResult.Success = false;
                    returnResult.Data = ResourceInformation.GetResValue("InvalidAppId");
                    return returnResult;
                }
                IExecuterStoreProc<RoleModel> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<RoleModel>>();

                List<Parameters> param = new List<Parameters>() {
                            new Parameters("p_AppId", appId) };

                var listOfApplicationRoles = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetApplicationRoles.ToString(), param);
                if (listOfApplicationRoles != null && listOfApplicationRoles.Count() > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = listOfApplicationRoles;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")} " +
                        $"{ResourceInformation.GetResValue("ApplicationRoles")} " +
                        $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("applicationRoleMappingBusiness", "GetApplicationRoles", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// Get Application details by Application Role Id
        /// </summary>
        /// <param name="applicationRoleId"></param>
        /// <returns></returns>
        public async Task<ReturnResult> GetApplicationRolesById(int applicationRoleId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (applicationRoleId == 0)
                {
                    returnResult.Success = false;
                    returnResult.Data = ResourceInformation.GetResValue("InvalidAppRoleId");
                    return returnResult;
                }

                var result = await _applicationRoleMappingRepository.SelectFirstOrDefaultAsync(x => x.ApplicationRoleId == applicationRoleId && x.IsActive.Value);

                if (result != null)
                {
                    returnResult.Success = true;
                    returnResult.Data = result;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("NoRecordsFound");
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("applicationRoleMappingBusiness", "GetApplicationRolesById", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// To get Application Roles
        /// </summary>
        /// <returns>Returns a list of RoleModel </returns>
        public async Task<ReturnResult> GetAllApplicationRoles(int appRoleId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IExecuterStoreProc<ApplicationRoleModel> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<ApplicationRoleModel>>();
                List<Parameters> param;
                if (appRoleId == 0)
                {
                     param = new List<Parameters>() {
                            new Parameters("p_AppRoleId", DBNull.Value) };
                }
                else
                {
                     param = new List<Parameters>() {
                            new Parameters("p_AppRoleId", appRoleId) };
                }

                var result = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllApplicationRoleMapping.ToString(), param);

                if (result != null && result.Count() > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = result;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("NoRecordsFound");
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("applicationRoleMappingBusiness", "GetApplicationRoles", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }



        /// <summary>
        /// To save new Application Roles Mapping
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="appId">appId</param>
        /// <param name="roleIds">roleIds</param>
        /// <returns>Returns responce message</returns>
        public async Task<ReturnResult> SaveApplicationRoles(string userName, ApplicationRoleMapping applicationRoleMapping)
        {
            ReturnResult returnResult = new ReturnResult();
            //int successCount = 0;
            try
            {
                if (applicationRoleMapping == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                    return returnResult;
                }

                var existingInactiveAppRole = await _applicationRoleMappingRepository.SelectFirstOrDefaultAsync(arm => arm.AppId == applicationRoleMapping.AppId && arm.RoleId == applicationRoleMapping.RoleId && !arm.IsActive.Value);
                if (existingInactiveAppRole != null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ExistsAndInActive");
                    returnResult.Success = false;
                    return returnResult;
                }
                var appRole = await _applicationRoleMappingRepository.SelectFirstOrDefaultAsync(arm => arm.AppId == applicationRoleMapping.AppId && arm.RoleId == applicationRoleMapping.RoleId && arm.IsActive.Value);
                if (appRole != null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("ApplicationRoleMapping")} { ResourceInformation.GetResValue("AlreadyExist")}";
                    return returnResult;
                }

                IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();

                var userDetails = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
                if (userDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("UserDetailsNotFound");
                    return returnResult;
                }
                var userId = userDetails.UserId;

                if (applicationRoleMapping.IsActive == null)
                {
                    applicationRoleMapping.IsActive = true;
                }
                var result = await _applicationRoleMappingRepository.AddAsync(new ApplicationRoleMapping
                {
                    AppId = applicationRoleMapping.AppId,
                    RoleId = applicationRoleMapping.RoleId,
                    IsActive = applicationRoleMapping.IsActive,
                    CreatedBy = userId,
                    CreatedOn = DateTime.Now,
                    ModifiedBy = userId,
                    ModifiedOn = DateTime.Now

                });

                if (result.State.Equals(EntityState.Added))
                {
                    await _applicationRoleMappingRepository.UnitOfWork.SaveChangesAsync();
                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("DataSavedSuccess");
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DataSavedFailure");
                    return returnResult;
                }

                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("applicationRoleMappingBusiness", "SaveApplicationRoles", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// Save application role from ui
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="applicationRole"></param>
        /// <returns></returns>
        public async Task<ReturnResult> SaveApplicationRolesMapping(string userName, ApplicationRoleViewModel applicationRole)
        {
            ReturnResult returnResult = new ReturnResult();
            IApplicationRepository applicationRepository = _serviceProvider.GetRequiredService<IApplicationRepository>();
            IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            List<ApplicationRoleMapping> appRoleList = new List<ApplicationRoleMapping>();
            try
            {
                if (applicationRole == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                    return returnResult;
                }

                List<MultiSelect> roleDetails = applicationRole.RoleName.ToList();

                var roleId = roleDetails.Where(x => x.selected == true).Select(x => x.id);

                var appDetails = await applicationRepository.SelectFirstOrDefaultAsync(x => x.AppName.Equals(applicationRole.AppName,StringComparison.OrdinalIgnoreCase) && x.IsActive.Value);
                if (appDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ApplicationDetailsNotFound");
                    return returnResult;
                }
                if (applicationRole.IsActive == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("IsActiveRequired");
                    return returnResult;
                }

                var userDetails = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
                if (userDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("UserDetailsNotFound");
                    return returnResult;
                }

                foreach (var item in roleId)
                {
                    appRoleList.Add(new ApplicationRoleMapping()
                    {
                        AppId = appDetails.AppId,
                        RoleId = item,
                        IsActive = applicationRole.IsActive,
                        ModifiedBy = userDetails.UserId,
                        ModifiedOn = DateTime.Now,
                        CreatedBy = userDetails.UserId,
                        CreatedOn = DateTime.Now

                    });
                }

                if (appRoleList != null)
                {
                    await _applicationRoleMappingRepository.AddRange(appRoleList);
                    await _applicationRoleMappingRepository.UnitOfWork.SaveChangesAsync();
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
                _logger.Error("applicationRoleMappingBusiness", "SaveApplicationRolesMapping", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataSavedFailure");
                return returnResult;
            }
        }
        /// <summary>
        /// To get all the roles which are not assigns to particular application
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public async Task<ReturnResult> GetAllRolesByApplicationName(string appName)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (string.IsNullOrEmpty(appName))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ApplicationDetailsNotFound");
                    return returnResult;
                }

                IExecuterStoreProc<RoleModel> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<RoleModel>>();

                List<Parameters> param = new List<Parameters>() {
                            new Parameters("p_appName", appName) };
                var result = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllExceptionRolesByApplicationName, param);

                if (result != null && result.Count > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = result;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("NoRecordsFound");
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("applicationRoleMappingBusiness", "GetAllRolesByApplicationName", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }
        /// <summary>
        /// Method to delete application role by ApplicationRoleId
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="applicationRoleId"></param>
        /// <returns></returns>
        public async Task<ReturnResult> DeleteApplicationRoles(string userName, int applicationRoleId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var deleteApplicationRole = await _applicationRoleMappingRepository.SelectFirstOrDefaultAsync(a => a.ApplicationRoleId == applicationRoleId && a.IsActive.Value);

                if (deleteApplicationRole == null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                    return returnResult;
                }

                deleteApplicationRole.IsActive = false;

                IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                deleteApplicationRole.ModifiedBy = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive == true)).UserId;
                deleteApplicationRole.ModifiedOn = DateTime.Now;
                var result = await _applicationRoleMappingRepository.UpdateAsync(deleteApplicationRole);

                if (result.State.Equals(EntityState.Modified))
                {
                    await _applicationRoleMappingRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("applicationRoleMappingBusiness", "DeleteApplicationRoles", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// Update application role
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="applicationRoleModel"></param>
        /// <returns></returns>
        public async Task<ReturnResult> UpdateApplicationRoles(string userName, ApplicationRoleModel applicationRoleModel)
        {
            ReturnResult returnResult = new ReturnResult();
            IUserRepository userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            IApplicationRepository applicationRepository = _serviceProvider.GetRequiredService<IApplicationRepository>();
            IRoleRepository roleRepository = _serviceProvider.GetRequiredService<IRoleRepository>();
            ApplicationRoleMapping applicationRoleMapping = new ApplicationRoleMapping();
            try
            {
                if (applicationRoleModel == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                    return returnResult;
                }

                var userDetails = await userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
                if (userDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("UserDetailsNotFound");
                    return returnResult;
                }

                if (applicationRoleModel.IsActive == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("IsActiveRequired");
                    return returnResult;
                }
                applicationRoleMapping.IsActive = applicationRoleModel.IsActive;
                var appRoleDetail = await _applicationRoleMappingRepository.SelectFirstOrDefaultAsync(x => x.ApplicationRoleId == applicationRoleModel.ApplicationRoleId);
                if(appRoleDetail == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("NotExists");
                    return returnResult;
                }

                var appdetail = await applicationRepository.SelectFirstOrDefaultAsync(x => x.AppName.Equals(applicationRoleModel.AppName, StringComparison.OrdinalIgnoreCase) && x.IsActive.Value);
                if(appdetail != null)
                {
                    applicationRoleMapping.AppId = appdetail.AppId;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ApplicationDetailsNotFound");
                    return returnResult;
                }

                var roledetail = await roleRepository.SelectFirstOrDefaultAsync(x => x.RoleName.Equals(applicationRoleModel.RoleName, StringComparison.OrdinalIgnoreCase) && x.IsActive.Value);
                if(roledetail !=null)
                {
                    applicationRoleMapping.RoleId = roledetail.RoleId;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("RoleDetailsNotFound");
                    return returnResult;
                }
                applicationRoleMapping.ApplicationRoleId = appRoleDetail.ApplicationRoleId;
                applicationRoleMapping.CreatedBy = userDetails.UserId;
                applicationRoleMapping.ModifiedBy = userDetails.UserId;
                applicationRoleMapping.CreatedOn = DateTime.Now;
                applicationRoleMapping.ModifiedOn = DateTime.Now;

                var result = await _applicationRoleMappingRepository.UpdateAsync(applicationRoleMapping);
                if(result.State.Equals(EntityState.Modified))
                {
                    await _applicationRoleMappingRepository.UnitOfWork.SaveChangesAsync();
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
                _logger.Error("applicationRoleMappingBusiness", "UpdateApplicationRoles", ex.Message, ex.StackTrace);
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
                _applicationRoleMappingRepository.Dispose();
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


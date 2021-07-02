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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
    public class RoleModuleAccessBusiness : IRoleModuleAccessBusiness
    {
        #region Private Variable
        private readonly IRoleModuleAccessRepository _roleModuleAccessRepository;
        IServiceProvider _serviceProvider;
        private bool _disposed;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public RoleModuleAccessBusiness(IServiceProvider serviceProvider, IRoleModuleAccessRepository roleModuleAccessRepository, ILogger logger)
        {
            _roleModuleAccessRepository = roleModuleAccessRepository;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _disposed = false;
        }
        #endregion

        #region Public Methods
        #region RoleModuleAccess Public Methods
        /// <summary>
        /// This method is used get the Role Module Access Details for all roles.
        /// </summary>
        /// <returns>returns multiple RoleModuleAccess details</returns>
        public async Task<ReturnResult> Get()
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var listOfRoleModuleAccess = await _roleModuleAccessRepository.SelectAsync(rma => rma.IsActive.Value);
                if (listOfRoleModuleAccess != null && listOfRoleModuleAccess.Count() > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = listOfRoleModuleAccess;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ResourceInformation.GetResValue("RoleModuleAccess")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error( "RoleModuleAccessBusiness", "Get", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used get the RoleModuleAccess details by roleAccessId.
        /// </summary>
        /// <param name="roleAccessId">roleAccessId</param>
        /// <returns>returns single RoleModuleAccess details</returns>
        public async Task<ReturnResult> GetById(int roleAccessId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var requiredRoleModuleAccessInfo = await _roleModuleAccessRepository.SelectFirstOrDefaultAsync(rma => rma.RoleAccessId.Equals(roleAccessId) && rma.IsActive.Value);
                if (requiredRoleModuleAccessInfo != null)
                {
                    returnResult.Success = true;
                    returnResult.Data = requiredRoleModuleAccessInfo;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")} " +
                                          $"{ResourceInformation.GetResValue("RoleModuleAccess")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("RoleModuleAccessBusiness", "GetById", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used get the RoleModuleAccess details by id.
        /// </summary>
        /// <param name="roleId">roleId</param>
        /// <returns>returns all role RoleModuleAccess assign to specific role</returns>
        public async Task<ReturnResult> GetByRoleId(int roleId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IExecuterStoreProc<ModuleAccessDetails> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<ModuleAccessDetails>>();

                // Pass Role name Controller and Action to store procedure
                List<Parameters> param = new List<Parameters>() { new Parameters("p_RoleId", roleId) };

                // procGetRoleModuleDetails returns RoleModuleAccessDetails if Role exists
                List<ModuleAccessDetails> roleModuleDetails = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetRoleModuleDetails, param);

                if (roleModuleDetails != null)
                {
                    returnResult.Success = true;
                    returnResult.Data = roleModuleDetails;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")} " +
                                          $"{ResourceInformation.GetResValue("RoleModuleAccess")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("RoleModuleAccessBusiness", "GetByRoleId", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to save single RoleModuleAccess details
        /// </summary>
        /// <param name="userName">userName object</param>
        ///  /// <param name="roleModuleAccessModel">roleModuleAccessModel object</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Save(string userName, RoleModuleAccessModel roleModuleAccessModel)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                returnResult = await VerifyRoleModuleAccessData(roleModuleAccessModel);
                if (returnResult.Success != true)
                {
                    return returnResult;
                }

                RoleModuleAccess roleModuleAccess = (RoleModuleAccess)returnResult.Data;

                IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                Guid userId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive == true)).UserId;

                roleModuleAccess.CreatedBy = userId;
                roleModuleAccess.CreatedOn = DateTime.Now;
                roleModuleAccess.ModifiedBy = userId;
                roleModuleAccess.ModifiedOn = DateTime.Now;

                var result = await _roleModuleAccessRepository.AddAsync(roleModuleAccess);

                if (result.State.Equals(EntityState.Added))
                {
                    await _roleModuleAccessRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("RoleModuleAccessBusiness", "Save", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to save multiple RoleModuleAccess details
        /// </summary>
        /// <param name="userName">userName object</param>
        /// <param name="roleModuleAccess">roleModuleAccess object</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> SaveRange(string userName, IEnumerable<RoleModuleAccessModel> roleModuleAccessModelList)
        {
            ReturnResult returnResult = new ReturnResult();
            List<RoleModuleAccess> roleModuleAccessList = new List<RoleModuleAccess>();
            try
            {
                RoleModuleAccess roleModuleAccess;

                IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                Guid userId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value)).UserId;

                foreach (RoleModuleAccessModel roleModuleAccessModel in roleModuleAccessModelList)
                {
                    returnResult = await VerifyRoleModuleAccessData(roleModuleAccessModel);
                    if (returnResult.Success != true)
                    {
                        return returnResult;
                    }
                    roleModuleAccess = (RoleModuleAccess)returnResult.Data;

                    roleModuleAccess.CreatedBy = userId;
                    roleModuleAccess.CreatedOn = DateTime.Now;
                    roleModuleAccess.ModifiedBy = userId;
                    roleModuleAccess.ModifiedOn = DateTime.Now;
                    roleModuleAccessList.Add(roleModuleAccess);
                }
                bool result = await _roleModuleAccessRepository.AddRange(roleModuleAccessList);

                if (result)
                {
                    await _roleModuleAccessRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("RoleModuleAccessBusiness", "SaveRange", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to update single RoleModuleAccess details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="roleAccessId">roleAccessId</param>
        /// <param name="roleModuleAccessModel">roleModuleAccessModel </param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Update(string userName, int roleAccessId, RoleModuleAccessModel roleModuleAccessModel)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (roleAccessId == 0)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                    return returnResult;
                }

                RoleModuleAccess updateRoleModuleAccess = await _roleModuleAccessRepository.SelectFirstOrDefaultAsync(rma => rma.RoleAccessId == roleAccessId);

                if (updateRoleModuleAccess == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RoleModuleAccess")} { ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                if (roleModuleAccessModel.IsActive == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("IsActiveRequired");
                    return returnResult;
                }

                updateRoleModuleAccess.IsActive = roleModuleAccessModel.IsActive.Value;
                updateRoleModuleAccess.ReadAccess = roleModuleAccessModel.ReadAccess;
                updateRoleModuleAccess.WriteAccess = roleModuleAccessModel.WriteAccess;
                updateRoleModuleAccess.EditAccess = roleModuleAccessModel.EditAccess;
                updateRoleModuleAccess.DeleteAccess = roleModuleAccessModel.DeleteAccess;

                IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                updateRoleModuleAccess.ModifiedBy = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value)).UserId;
                updateRoleModuleAccess.ModifiedOn = DateTime.Now;

                var result = await _roleModuleAccessRepository.UpdateAsync(updateRoleModuleAccess);

                if (result.State.Equals(EntityState.Modified))
                {
                    await _roleModuleAccessRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("RoleModuleAccessBusiness", "Update", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to update multiple RoleModuleAccess details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="roleId">roleId</param>
        /// <param name="roleModuleAccessModelList">roleModuleAccessModelList </param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> UpdateRange(string userName, int roleId, IEnumerable<RoleModuleAccessModel> roleModuleAccessModelList)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (roleId == 0)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideRoleDetails");
                    return returnResult;
                }
                var updateRoleModuleAccessList =await _roleModuleAccessRepository.SelectAsync(rma => rma.RoleId == roleId);

                if (await _roleModuleAccessRepository.DeleteRange(updateRoleModuleAccessList) != true)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DataUpdateFailure");
                    return returnResult;
                }

                returnResult = await SaveRange(userName, roleModuleAccessModelList);

                if (returnResult.Success != true)
                {
                    returnResult.Success = false;
                    return returnResult;
                }

                returnResult.Success = true;
                returnResult.Result = ResourceInformation.GetResValue("DataUpdateSuccess");
                return returnResult;

            }
            catch (Exception ex)
            {
                _logger.Error("RoleModuleAccessBusiness", "UpdateRange", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to soft delete single RoleModuleAccess details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="roleAccessId">RoleModuleAccess</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Delete(string userName, int roleAccessId)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {
                var deleteRoleModuleAccess = await _roleModuleAccessRepository.SelectFirstOrDefaultAsync(rma => rma.RoleAccessId == roleAccessId && rma.IsActive.Value);

                if (deleteRoleModuleAccess == null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("RoleModuleAccess")} {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                deleteRoleModuleAccess.IsActive = false;

                IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                deleteRoleModuleAccess.ModifiedBy = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value)).UserId;
                deleteRoleModuleAccess.ModifiedOn = DateTime.Now;

                var result = await _roleModuleAccessRepository.UpdateAsync(deleteRoleModuleAccess);

                if (result.State.Equals(EntityState.Modified))
                {
                   await _roleModuleAccessRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("RoleModuleAccessBusiness", "Delete", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to delete specific role's RoleModuleAccess details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="roleId">RoleModuleAccess</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> DeleteByRole(string userName, int roleId)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {
                List<RoleModuleAccess> deleteRoleModuleAccessList = (await _roleModuleAccessRepository.SelectAsync(rma => rma.RoleAccessId == roleId && rma.IsActive.Value)).ToList();

                if (deleteRoleModuleAccessList == null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("RoleModuleAccess")} {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
                IUserRepository userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                Guid modifiedBy = (await userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value)).UserId;
                foreach (RoleModuleAccess deleteRoleModuleAccess in deleteRoleModuleAccessList)
                {
                    deleteRoleModuleAccess.IsActive = false;
                    deleteRoleModuleAccess.ModifiedBy = modifiedBy;
                    deleteRoleModuleAccess.ModifiedOn = DateTime.Now;
                    deleteRoleModuleAccessList.Add(deleteRoleModuleAccess);
                }

                bool result = await _roleModuleAccessRepository.UpdateRange(deleteRoleModuleAccessList);
                if (result)
                {
                    await _roleModuleAccessRepository.UnitOfWork.SaveChangesAsync();
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
                _logger.Error("RoleModuleAccessBusiness", "DeleteByRole", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }
        #endregion

        #region RoleAccessException Public Methods

        /// <summary>
        /// This method is used get all RoleAccessException details.
        /// </summary>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> GetRoleAccessException()
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IRoleAccessExceptionRepository roleAccessExceptionRepository = _serviceProvider.GetRequiredService<IRoleAccessExceptionRepository>();
                var roleAccessExceptiondetails = await roleAccessExceptionRepository.SelectAllAsync();

                if (roleAccessExceptiondetails != null)
                {
                    returnResult.Success = true;
                    returnResult.Data = roleAccessExceptiondetails;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("RoleModuleAccessBusiness", "GetRoleAccessException", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to save  RoleAccessException details
        /// </summary>
        /// <param name="userName">userName</param>
        ///  /// <param name="accessExceptionModel">accessExceptionModel object</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> SaveRoleAccessException(string userName, AccessExceptionModel accessExceptionModel)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                RoleAccessException roleAccessException = new RoleAccessException();
                IRoleRepository roleRepository = _serviceProvider.GetRequiredService<IRoleRepository>();
                Role roleData = await roleRepository.SelectFirstOrDefaultAsync(r => r.RoleName.Equals(accessExceptionModel.AccessorTypeName) && r.IsActive.Value);
                // Check AccessorTypeName is not null 
                if (accessExceptionModel.AccessorTypeName.Equals(null) || roleData == null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideRoleDetails");
                    returnResult.Success = false;
                    return returnResult;
                }

                IModuleRepository moduleRepository = _serviceProvider.GetRequiredService<IModuleRepository>();
                Module moduleData = await moduleRepository.SelectFirstOrDefaultAsync(m => m.ModuleName.Equals(accessExceptionModel.ModuleName) && m.IsActive == true);
                // Check ModuleName is not null and exist
                if (accessExceptionModel.ModuleName.Equals(null) || moduleData == null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideModuleDetails");
                    returnResult.Success = false;
                    return returnResult;
                }

                IActionsRepository actionsRepository = _serviceProvider.GetRequiredService<IActionsRepository>();
                Actions actionsData = await actionsRepository.SelectFirstOrDefaultAsync(a => a.ActionName.Equals(accessExceptionModel.ActionName) && a.IsActive == true);
                // Check ActionName is not null and exist
                if (accessExceptionModel.ActionName.Equals(null) || actionsData == null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                    returnResult.Success = false;
                    return returnResult;
                }

                IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                Guid userId = ( await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive == true)).UserId;

                roleAccessException.RoleId = roleData.RoleId;
                roleAccessException.ModuleId = moduleData.ModuleId;
                roleAccessException.ActionId = actionsData.ActionId;
                roleAccessException.CreatedBy = userId;
                roleAccessException.CreatedOn = DateTime.Now;

                IRoleAccessExceptionRepository roleAccessExceptionRepository = _serviceProvider.GetRequiredService<IRoleAccessExceptionRepository>();

                var result = await roleAccessExceptionRepository.AddAsync(roleAccessException);

                if (result.State.Equals(EntityState.Added))
                {
                    await _roleModuleAccessRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("RoleModuleAccessBusiness", "SaveRoleAccessException", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to soft delete single RoleModuleAccess details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="roleAccessId">RoleModuleAccess</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> DeleteRoleAccessException(int accessExceptionId)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {
                IRoleAccessExceptionRepository roleAccessExceptionRepository = _serviceProvider.GetRequiredService<IRoleAccessExceptionRepository>();
                var roleAccessExceptiondetails = await roleAccessExceptionRepository.SelectFirstOrDefaultAsync(rae => rae.AccessExceptionId == accessExceptionId);

                if(roleAccessExceptiondetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ResourceInformation.GetResValue("roleAccessExceptionId")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                var result = await roleAccessExceptionRepository.DeleteAsync(roleAccessExceptiondetails);

                if (result.State.Equals(EntityState.Deleted))
                {
                    await _roleModuleAccessRepository.UnitOfWork.SaveChangesAsync();
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
                _logger.Error("RoleModuleAccessBusiness", "DeleteRoleAccessException", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        #endregion

        #region Action Public Method
        
        /// <summary>
        /// This method is used get all Actions details.
        /// </summary>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> GetAllActions()
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IActionsRepository actionsRepository = _serviceProvider.GetRequiredService<IActionsRepository>();
                var actionsdetails = await actionsRepository.SelectAsync(ar =>  ar.IsActive.Equals(true));

                if (actionsdetails != null)
                {
                    returnResult.Success = true;
                    returnResult.Data = actionsdetails;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("RoleModuleAccessBusiness", "GetAllActions", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }
        #endregion
        #endregion

        #region Private Methods
        /// <summary>
        /// This method validate RoleModuleAccess details sent by user
        /// </summary>
        /// <param name="roleModuleAccessModel">roleModuleAccessModel</param>
        /// <returns>returns response message</returns>
        private async Task<ReturnResult> VerifyRoleModuleAccessData(RoleModuleAccessModel roleModuleAccessModel)
        {
            ReturnResult returnResult = new ReturnResult();
            RoleModuleAccess roleModuleAccess = new RoleModuleAccess();

            if (roleModuleAccessModel == null)
            {
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                return returnResult;
            }

            if (roleModuleAccessModel.IsActive == null)
            {
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("IsActiveRequired");
                return returnResult;
            }

            IRoleRepository roleRepository = _serviceProvider.GetRequiredService<IRoleRepository>();
            // Check RoleId is not equal to 0 and exist
            if (roleModuleAccessModel.RoleId != 0 && (await roleRepository.SelectAsync(r => r.RoleId == roleModuleAccessModel.RoleId && r.IsActive.Value)).Any())
            {
                roleModuleAccess.RoleId = roleModuleAccessModel.RoleId;
            }
            // Check RoleName is not equal to null and exist
            else if (!roleModuleAccessModel.RoleName.Equals(null) && (await roleRepository.SelectAsync(r => r.RoleName.Equals(roleModuleAccessModel.RoleName) && r.IsActive.Value)).Any())
            {
               var roleDetails = await roleRepository.SelectFirstOrDefaultAsync(r => r.RoleName.Equals(roleModuleAccessModel.RoleName) && r.IsActive.Value);
                if (roleDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("RoleDetailsNotFound");
                    return returnResult;
                }
                roleModuleAccess.RoleId = roleDetails.RoleId;
            }
            else
            {
                returnResult.Result = ResourceInformation.GetResValue("ProvideRoleDetails");
                returnResult.Success = false;
                return returnResult;
            }
            IModuleRepository moduleRepository = _serviceProvider.GetRequiredService<IModuleRepository>();
            // Check ModuleId is not equal to 0 and exist
            if (roleModuleAccessModel.ModuleId != 0 && (await moduleRepository.SelectAsync(r => r.ModuleId == roleModuleAccessModel.ModuleId && r.IsActive == true)).Any())
            {
                roleModuleAccess.ModuleId = roleModuleAccessModel.ModuleId;
            }
            // Check ModuleName is not equal to null and exist
            else if (!roleModuleAccessModel.RoleName.Equals(null) && (await moduleRepository.SelectAsync(r => r.ModuleName.Equals(roleModuleAccessModel.ModuleName) && r.IsActive == true)).Any())
            {
                roleModuleAccess.ModuleId = (await moduleRepository.SelectFirstOrDefaultAsync(r => r.ModuleName.Equals(roleModuleAccessModel.ModuleName) && r.IsActive == true)).ModuleId;
            }
            else
            {
                returnResult.Result = ResourceInformation.GetResValue("ProvideModuleDetails");
                returnResult.Success = false;
                return returnResult;
            }

            // check wether Role and Module has existing active or inactive Role Module Access Entry 
            var existingRoleModuleAccess = await _roleModuleAccessRepository.SelectFirstOrDefaultAsync(rma => rma.RoleId == roleModuleAccess.RoleId && rma.ModuleId == roleModuleAccess.ModuleId);
            if (existingRoleModuleAccess != null)
            {
                if (!existingRoleModuleAccess.IsActive.Value)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("RoleModuleAccess")} {" "} { ResourceInformation.GetResValue("ExistsAndInActive")}";
                    returnResult.Success = false;
                    return returnResult;
                }
                else
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("RoleModuleAccess")} { ResourceInformation.GetResValue("AlreadyExist")}";
                    returnResult.Success = false;
                    return returnResult;
                }
            }
            roleModuleAccess.ReadAccess = roleModuleAccessModel.ReadAccess;
            roleModuleAccess.WriteAccess = roleModuleAccessModel.WriteAccess;
            roleModuleAccess.EditAccess = roleModuleAccessModel.EditAccess;
            roleModuleAccess.DeleteAccess = roleModuleAccessModel.DeleteAccess;
            roleModuleAccess.IsActive = roleModuleAccessModel.IsActive;

            returnResult.Success = true;
            returnResult.Data = roleModuleAccess;
            return returnResult;
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
                _roleModuleAccessRepository.Dispose();
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

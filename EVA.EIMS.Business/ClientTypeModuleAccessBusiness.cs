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
    public class ClientTypeModuleAccessBusiness : IClientTypeModuleAccessBusiness
    {
        #region Private Variable
        private readonly IClientTypeModuleAccessRepository _clientTypeModuleAccessRepository;
        IServiceProvider _serviceProvider;
        private bool _disposed;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public ClientTypeModuleAccessBusiness(IServiceProvider serviceProvider, IClientTypeModuleAccessRepository clientTypeModuleAccessRepository, ILogger logger)
        {
            _clientTypeModuleAccessRepository = clientTypeModuleAccessRepository;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _disposed = false;
        }
        #endregion

        #region Public Methods
        #region ClientTypeModuleAccess Methods
        /// <summary>
        /// This method is used get the ClientType Module Access Details for all ClientTypes.
        /// </summary>
        /// <returns>returns multiple ClientTypeModuleAccess details</returns>
        public async Task<ReturnResult> Get()
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var listOfClientTypeModuleAccess = await _clientTypeModuleAccessRepository.SelectAsync(rma => rma.IsActive == true);
                if (listOfClientTypeModuleAccess != null && listOfClientTypeModuleAccess.Count() > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = listOfClientTypeModuleAccess;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ResourceInformation.GetResValue("ClientTypeModuleAccess")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ClientTypeModuleAccessBusiness", "Get", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used get the ClientTypeModuleAccess details by clientTypeAccessId.
        /// </summary>
        /// <param name="clientTypeAccessId">clientTypeAccessId</param>
        /// <returns>returns single ClientTypeModuleAccess details</returns>
        public async Task<ReturnResult> GetById(int clientTypeAccessId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var requiredClientTypeModuleAccessInfo = await _clientTypeModuleAccessRepository.SelectFirstOrDefaultAsync(rma => rma.ClientTypeAccessId.Equals(clientTypeAccessId) && rma.IsActive);
                if (requiredClientTypeModuleAccessInfo != null)
                {
                    returnResult.Success = true;
                    returnResult.Data = requiredClientTypeModuleAccessInfo;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")} " +
                                          $"{ResourceInformation.GetResValue("ClientTypeModuleAccess")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ClientTypeModuleAccessBusiness", "GetById", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used get the ClientTypeModuleAccess details by id.
        /// </summary>
        /// <param name="clientTypeId">clientTypeId</param>
        /// <returns>returns all clientType ClientTypeModuleAccess assign to specific clientType</returns>
        public async Task<ReturnResult> GetByClientTypeId(int clientTypeId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IExecuterStoreProc<ModuleAccessDetails> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<ModuleAccessDetails>>();

                // Pass ClientType name Controller and Action to store procedure
                List<Parameters> param = new List<Parameters>() { new Parameters("p_ClientTypeId", clientTypeId) };

                // procGetClientTypeModuleDetails returns ModuleAccessDetails if ClientType exists
                List<ModuleAccessDetails> clientTypeModuleDetails = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetClientTypeModuleDetails.ToString(), param);

                if (clientTypeModuleDetails != null)
                {
                    returnResult.Success = true;
                    returnResult.Data = clientTypeModuleDetails.ToList();
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")} " +
                                          $"{ResourceInformation.GetResValue("ClientTypeModuleAccess")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ClientTypeModuleAccessBusiness", "GetByClientTypeId", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to save single ClientTypeModuleAccess details
        /// </summary>
        /// <param name="userName">userName object</param>
        ///  /// <param name="ClientTypeModuleAccessModel">ClientTypeModuleAccessModel object</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Save(string userName, ClientTypeModuleAccessModel clientTypeModuleAccessModel)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                returnResult = await VerifyClientTypeModuleAccessData(clientTypeModuleAccessModel);
                if (returnResult.Success != true)
                {
                    return returnResult;
                }

                ClientTypeModuleAccess clientTypeModuleAccess = (ClientTypeModuleAccess)returnResult.Data;

                IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                Guid userId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive == true)).UserId;

                clientTypeModuleAccess.CreatedBy = userId;
                clientTypeModuleAccess.CreatedOn = DateTime.Now;
                clientTypeModuleAccess.ModifiedBy = userId;
                clientTypeModuleAccess.ModifiedOn = DateTime.Now;

                var result = await _clientTypeModuleAccessRepository.AddAsync(clientTypeModuleAccess);

                if (result.State.Equals(EntityState.Added))
                {
                    await _clientTypeModuleAccessRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("ClientTypeModuleAccessBusiness", "Save", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to save multiple ClientTypeModuleAccess details
        /// </summary>
        /// <param name="userName">userName object</param>
        /// <param name="clientTypeModuleAccess">ClientTypeModuleAccess object</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> SaveRange(string userName, IEnumerable<ClientTypeModuleAccessModel> clientTypeModuleAccessModelList)
        {
            ReturnResult returnResult = new ReturnResult();
            List<ClientTypeModuleAccess> ClientTypeModuleAccessList = new List<ClientTypeModuleAccess>();
            try
            {
                ClientTypeModuleAccess ClientTypeModuleAccess;

                IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                Guid userId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive == true)).UserId;

                foreach (ClientTypeModuleAccessModel clientTypeModuleAccessModel in clientTypeModuleAccessModelList)
                {
                    returnResult = await VerifyClientTypeModuleAccessData(clientTypeModuleAccessModel);
                    if (returnResult.Success != true)
                    {
                        return returnResult;
                    }
                    ClientTypeModuleAccess = (ClientTypeModuleAccess)returnResult.Data;

                    ClientTypeModuleAccess.IsActive = clientTypeModuleAccessModel.IsActive.Value;
                    ClientTypeModuleAccess.CreatedBy = userId;
                    ClientTypeModuleAccess.CreatedOn = DateTime.Now;
                    ClientTypeModuleAccess.ModifiedBy = userId;
                    ClientTypeModuleAccess.ModifiedOn = DateTime.Now;
                    ClientTypeModuleAccessList.Add(ClientTypeModuleAccess);
                }
                bool result = await _clientTypeModuleAccessRepository.AddRange(ClientTypeModuleAccessList);

                if (result)
                {
                    await _clientTypeModuleAccessRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("ClientTypeModuleAccessBusiness", "SaveRange", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to update single ClientTypeModuleAccess details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="clientTypeAccessId">clientTypeAccessId</param>
        /// <param name="ClientTypeModuleAccessModel">ClientTypeModuleAccessModel </param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Update(string userName, int clientTypeAccessId, ClientTypeModuleAccessModel clientTypeModuleAccessModel)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (clientTypeAccessId == 0)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                    return returnResult;
                }

                ClientTypeModuleAccess updateClientTypeModuleAccess = await _clientTypeModuleAccessRepository.SelectFirstOrDefaultAsync(rma => rma.ClientTypeAccessId == clientTypeAccessId);

                if (updateClientTypeModuleAccess == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("ClientTypeModuleAccess")} { ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                if (clientTypeModuleAccessModel.IsActive == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("IsActiveRequired");
                    return returnResult;
                }

                updateClientTypeModuleAccess.IsActive = clientTypeModuleAccessModel.IsActive.Value;
                updateClientTypeModuleAccess.ReadAccess = clientTypeModuleAccessModel.ReadAccess;
                updateClientTypeModuleAccess.WriteAccess = clientTypeModuleAccessModel.WriteAccess;
                updateClientTypeModuleAccess.EditAccess = clientTypeModuleAccessModel.EditAccess;
                updateClientTypeModuleAccess.DeleteAccess = clientTypeModuleAccessModel.DeleteAccess;

                IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                updateClientTypeModuleAccess.ModifiedBy = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive == true)).UserId;
                updateClientTypeModuleAccess.ModifiedOn = DateTime.Now;

                var result = await _clientTypeModuleAccessRepository.UpdateAsync(updateClientTypeModuleAccess);

                if (result.State.Equals(EntityState.Modified))
                {
                    await _clientTypeModuleAccessRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("ClientTypeModuleAccessBusiness", "Update", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to update multiple ClientTypeModuleAccess details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="clientTypeId">clientTypeId</param>
        /// <param name="clientTypeModuleAccessModelList">clientTypeModuleAccessModelList </param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> UpdateRange(string userName, int clientTypeId, IEnumerable<ClientTypeModuleAccessModel> clientTypeModuleAccessModelList)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (clientTypeId == 0)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideClientTypeDetails");
                    return returnResult;
                }
                var updateClientTypeModuleAccessList = await _clientTypeModuleAccessRepository.SelectAsync(rma => rma.ClientTypeId == clientTypeId);

                if (await _clientTypeModuleAccessRepository.DeleteRange(updateClientTypeModuleAccessList) != true)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DataUpdateFailure");
                    return returnResult;
                }

                returnResult = await SaveRange(userName, clientTypeModuleAccessModelList);

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
                _logger.Error("ClientTypeModuleAccessBusiness", "UpdateRange", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to soft delete single ClientTypeModuleAccess details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="clientTypeAccessId">clientTypeAccessId</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Delete(string userName, int clientTypeAccessId)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {

                var deleteClientTypeModuleAccess = await _clientTypeModuleAccessRepository.SelectFirstOrDefaultAsync(rma => rma.ClientTypeAccessId == clientTypeAccessId && rma.IsActive);

                if (deleteClientTypeModuleAccess == null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("ClientTypeModuleAccess")} {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                deleteClientTypeModuleAccess.IsActive = false;

                IUserRepository _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                deleteClientTypeModuleAccess.ModifiedBy = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive == true)).UserId;
                deleteClientTypeModuleAccess.ModifiedOn = DateTime.Now;

                var result = await _clientTypeModuleAccessRepository.UpdateAsync(deleteClientTypeModuleAccess);

                if (result.State.Equals(EntityState.Modified))
                {
                    await _clientTypeModuleAccessRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("ClientTypeModuleAccessBusiness", "Delete", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to delete specific clientType's ClientTypeModuleAccess details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="clientTypeId">ClientTypeModuleAccess</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> DeleteByClientType(string userName, int clientTypeId)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {
                List<ClientTypeModuleAccess> deleteClientTypeModuleAccessList = (await _clientTypeModuleAccessRepository.SelectAsync(rma => rma.ClientTypeAccessId == clientTypeId && rma.IsActive)).ToList();

                if (deleteClientTypeModuleAccessList == null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("ClientTypeModuleAccess")}  {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
                IUserRepository userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                Guid modifiedBy = (await userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive == true)).UserId;
                foreach (ClientTypeModuleAccess deleteClientTypeModuleAccess in deleteClientTypeModuleAccessList)
                {
                    deleteClientTypeModuleAccess.IsActive = false;
                    deleteClientTypeModuleAccess.ModifiedBy = modifiedBy;
                    deleteClientTypeModuleAccess.ModifiedOn = DateTime.Now;
                    deleteClientTypeModuleAccessList.Add(deleteClientTypeModuleAccess);
                }

                bool result = await _clientTypeModuleAccessRepository.UpdateRange(deleteClientTypeModuleAccessList);
                if (result)
                {
                    await _clientTypeModuleAccessRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("ClientTypeModuleAccessBusiness", "DeleteByClientType", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }
        #endregion

        #region  ClientTypeAccessException Public Methods

        /// <summary>
        /// This method is used get all ClientTypeAccessException details.
        /// </summary>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> GetClientTypeAccessException()
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IClientTypeAccessExceptionRepository clientTypeAccessExceptionRepository = _serviceProvider.GetRequiredService<IClientTypeAccessExceptionRepository>();
                var clientTypeAccessExceptiondetails = await clientTypeAccessExceptionRepository.SelectAllAsync();

                if (clientTypeAccessExceptiondetails != null)
                {
                    returnResult.Success = true;
                    returnResult.Data = clientTypeAccessExceptiondetails.ToList();
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
                _logger.Error("ClientTypeModuleAccessBusiness", "GetClientTypeAccessException", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to save  ClientTypeAccessException details
        /// </summary>
        /// <param name="userName">userName</param>
        ///  /// <param name="accessExceptionModel">accessExceptionModel object</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> SaveClientTypeAccessException(string userName, AccessExceptionModel accessExceptionModel)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                ClientTypeAccessException clientTypeAccessException = new ClientTypeAccessException();
                IClientTypeRepository clientTypeRepository = _serviceProvider.GetRequiredService<IClientTypeRepository>();
                ClientType clientTypeData = await clientTypeRepository.SelectFirstOrDefaultAsync(r => r.ClientTypeName.Equals(accessExceptionModel.AccessorTypeName) && r.IsActive.Value);
                // Check AccessorTypeName is not null 
                if (accessExceptionModel.AccessorTypeName.Equals(null) || clientTypeData == null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideClientTypeDetails");
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
                Guid userId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive == true)).UserId;

                clientTypeAccessException.ClientTypeId = clientTypeData.ClientTypeId;
                clientTypeAccessException.ModuleId = moduleData.ModuleId;
                clientTypeAccessException.ActionId = actionsData.ActionId;
                clientTypeAccessException.CreatedBy = userId;
                clientTypeAccessException.CreatedOn = DateTime.Now;

                IClientTypeAccessExceptionRepository clientTypeAccessExceptionRepository = _serviceProvider.GetRequiredService<IClientTypeAccessExceptionRepository>();

                var result = await clientTypeAccessExceptionRepository.AddAsync(clientTypeAccessException);

                if (result.State.Equals(EntityState.Added))
                {
                    await _clientTypeModuleAccessRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("ClientTypeModuleAccessBusiness", "SaveClientTypeAccessException", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to soft delete single ClientTypeModuleAccess details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="clientTypeAccessId">clientTypeAccessId</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> DeleteClientTypeAccessException(int accessExceptionId)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {
                IClientTypeAccessExceptionRepository  clientTypeAccessExceptionRepository = _serviceProvider.GetRequiredService<IClientTypeAccessExceptionRepository>();
                var  clientTypeAccessExceptiondetails = await clientTypeAccessExceptionRepository.SelectFirstOrDefaultAsync(rae => rae.AccessExceptionId == accessExceptionId);

                if ( clientTypeAccessExceptiondetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ResourceInformation.GetResValue(" ClientTypeAccessExceptionId")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                var result = await clientTypeAccessExceptionRepository.DeleteAsync( clientTypeAccessExceptiondetails);

                if (result.State.Equals(EntityState.Deleted))
                {
                    await _clientTypeModuleAccessRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("ClientTypeModuleAccessBusiness", "DeleteClientTypeAccessException", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        #endregion
        #endregion

        #region Private Methods
        /// <summary>
        /// This method validate ClientTypeModuleAccess details sent by user
        /// </summary>
        /// <param name="ClientTypeModuleAccessModel">ClientTypeModuleAccessModel</param>
        /// <returns>returns response message</returns>
        private async Task<ReturnResult> VerifyClientTypeModuleAccessData(ClientTypeModuleAccessModel clientTypeModuleAccessModel)
        {
            ReturnResult returnResult = new ReturnResult();
            ClientTypeModuleAccess clientTypeModuleAccess = new ClientTypeModuleAccess();

            if (clientTypeModuleAccessModel == null)
            {
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                return returnResult;
            }

            if (clientTypeModuleAccessModel.IsActive == null)
            {
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("IsActiveRequired");
                return returnResult;
            }

            IClientTypeRepository clientTypeRepository = _serviceProvider.GetRequiredService<IClientTypeRepository>();
            // Check ClientTypeId is not equal to 0 and exist
            if (clientTypeModuleAccessModel.ClientTypeId != 0 && (await clientTypeRepository.SelectAsync(r => r.ClientTypeId == clientTypeModuleAccessModel.ClientTypeId && r.IsActive == true)).Any())
            {
                clientTypeModuleAccess.ClientTypeId = clientTypeModuleAccessModel.ClientTypeId;
            }
            // Check ClientTypeName is not equal to null and exist
            else if (!clientTypeModuleAccessModel.ClientTypeName.Equals(null) && (await clientTypeRepository.SelectAsync(r => r.ClientTypeName.Equals(clientTypeModuleAccessModel.ClientTypeName) && r.IsActive == true)).Any())
            {
                clientTypeModuleAccess.ClientTypeId = (await clientTypeRepository.SelectFirstOrDefaultAsync(r => r.ClientTypeName.Equals(clientTypeModuleAccessModel.ClientTypeName) && r.IsActive == true)).ClientTypeId;
            }
            else
            {
                returnResult.Result = ResourceInformation.GetResValue("ProvideclientTypeDetails");
                returnResult.Success = false;
                return returnResult;
            }
            IModuleRepository moduleRepository = _serviceProvider.GetRequiredService<IModuleRepository>();
            // Check ModuleId is not equal to 0 and exist
            if (clientTypeModuleAccessModel.ClientTypeId != 0 && (await moduleRepository.SelectAsync(r => r.ModuleId == clientTypeModuleAccessModel.ModuleId && r.IsActive == true)).Any())
            {
                clientTypeModuleAccess.ModuleId = clientTypeModuleAccessModel.ModuleId;
            }
            // Check ModuleName is not equal to null and exist
            else if (!clientTypeModuleAccessModel.ClientTypeName.Equals(null) && (await moduleRepository.SelectAsync(r => r.ModuleName.Equals(clientTypeModuleAccessModel.ModuleName) && r.IsActive == true)).Any())
            {
                clientTypeModuleAccess.ModuleId = (await moduleRepository.SelectFirstOrDefaultAsync(r => r.ModuleName.Equals(clientTypeModuleAccessModel.ModuleName) && r.IsActive == true)).ModuleId;
            }
            else
            {
                returnResult.Result = ResourceInformation.GetResValue("ProvideModuleDetails");
                returnResult.Success = false;
                return returnResult;
            }

            // check wether ClientType and Module has existing active or inactive ClientType Module Access Entry 
            var existingClientTypeModuleAccess = await _clientTypeModuleAccessRepository.SelectFirstOrDefaultAsync(rma => rma.ClientTypeId == clientTypeModuleAccess.ClientTypeId && rma.ModuleId == clientTypeModuleAccess.ModuleId);
            if (existingClientTypeModuleAccess != null)
            {
                if (!existingClientTypeModuleAccess.IsActive)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("ClientTypeModuleAccess")} {" "} { ResourceInformation.GetResValue("ExistsAndInActive")}";
                    returnResult.Success = false;
                    return returnResult;
                }
                else
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("ClientTypeModuleAccess")} { ResourceInformation.GetResValue("AlreadyExist")}";
                    returnResult.Success = false;
                    return returnResult;
                }
            }
            clientTypeModuleAccess.ReadAccess = clientTypeModuleAccessModel.ReadAccess;
            clientTypeModuleAccess.WriteAccess = clientTypeModuleAccessModel.WriteAccess;
            clientTypeModuleAccess.EditAccess = clientTypeModuleAccessModel.EditAccess;
            clientTypeModuleAccess.DeleteAccess = clientTypeModuleAccessModel.DeleteAccess;
            clientTypeModuleAccess.IsActive = clientTypeModuleAccessModel.IsActive.Value;

            returnResult.Success = true;
            returnResult.Data = clientTypeModuleAccess;
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
                _clientTypeModuleAccessRepository.Dispose();
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

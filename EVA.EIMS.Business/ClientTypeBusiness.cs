using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
    public class ClientTypeBusiness : IClientTypeBusiness
    {
        #region Private Variable
        private readonly IClientTypeRepository _clientTypeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        private Guid userId;
        private bool _disposed;
        #endregion

        #region Constructor
        public ClientTypeBusiness(IClientTypeRepository clientTypeRepository, IUserRepository userRepository, ILogger logger)
        {
            _clientTypeRepository = clientTypeRepository;
            _userRepository = userRepository;
            _logger = logger;
            _disposed = false;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// This method is used get the client types.
        /// </summary>
        /// <returns>returns multiple client types details</returns>
        public async Task<ReturnResult> Get()
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var listOfClientTypes = await _clientTypeRepository.SelectAsync(c => c.IsActive.Value);
                if (listOfClientTypes != null && listOfClientTypes.Count() > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = listOfClientTypes;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ResourceInformation.GetResValue("ClientType")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ClientTypeBusiness", "Get", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used get the client type details by id.
        /// </summary>
        /// <param name="clientTypeId">client type id</param>
        /// <returns>returns single client type details</returns>
        public async Task<ReturnResult> GetById(int clientTypeId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var requiredClientInfo = await _clientTypeRepository.SelectFirstOrDefaultAsync(c => c.ClientTypeId == clientTypeId);
                if (requiredClientInfo != null)
                {
                    returnResult.Success = true;
                    returnResult.Data = requiredClientInfo;
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
                _logger.Error("ClientTypeBusiness", "GetById", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to save the client type details.
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="clientType">client type object</param>
        /// <returns>returns response  message</returns>
        public async Task<ReturnResult> Save(string userName, ClientType clientType)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (clientType == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                    return returnResult;
                }


                if (string.IsNullOrEmpty(clientType.ClientTypeName))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideClientTypeName");
                    return returnResult;
                }

                if ( await _clientTypeRepository.SelectFirstOrDefaultAsync(c => c.ClientTypeName.Equals(clientType.ClientTypeName) && !c.IsActive.Value) != null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ExistsAndInActive");
                    return returnResult;
                }

                if ( await _clientTypeRepository.SelectFirstOrDefaultAsync(c => c.ClientTypeName.Equals(clientType.ClientTypeName) && c.IsActive.Value) != null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("ClientType")} {clientType.ClientTypeName} { ResourceInformation.GetResValue("AlreadyExist")}";
                    return returnResult;
                }

                if (clientType.IsActive == null)
                {
                    clientType.IsActive = true;

                }
                userId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value)).UserId;
                clientType.CreatedBy = userId;
                clientType.ModifiedBy = userId;

                var result = await _clientTypeRepository.AddAsync(clientType);

                if (result.State.Equals(EntityState.Added))
                {
                    await _clientTypeRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("ClientTypeBusiness", "Save", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to update the client type details.
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="clientTypeId">clientTypeId</param>
        /// <param name="clientType">client type object</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Update(string userName, int clientTypeId, ClientType clientType)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {

                if (clientType == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                    return returnResult;
                }


                if (clientTypeId == 0)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideClientId");
                    return returnResult;
                }

                var updateClientType = await _clientTypeRepository.SelectFirstOrDefaultAsync(c => c.ClientTypeId == clientTypeId);

                if (updateClientType == null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("ClientType")} { ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                if (clientType.IsActive == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("IsActiveRequired");
                    return returnResult;
                }
                userId = _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value).GetAwaiter().GetResult().UserId;
                updateClientType.ClientTypeName = clientType.ClientTypeName;
                updateClientType.Description = clientType.Description;
                updateClientType.IsActive = clientType.IsActive;
                updateClientType.ModifiedBy = userId;


                var result = await _clientTypeRepository.UpdateAsync(updateClientType);

                if (result.State.Equals(EntityState.Modified))
                {
                    await _clientTypeRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("ClientTypeBusiness", "Update", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to delete the client type details.
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="clientTypeId">client type id</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Delete(string userName, int clientTypeId)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {
                userId = _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value).GetAwaiter().GetResult().UserId;
                if (clientTypeId == 0)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideClientId");
                    return returnResult;
                }

                var deleteClientType = await _clientTypeRepository.SelectFirstOrDefaultAsync(c => c.ClientTypeId == clientTypeId);

                if (deleteClientType == null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("ClientType")} {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                deleteClientType.IsActive = false;
                deleteClientType.ModifiedBy = userId;
                var result = await _clientTypeRepository.UpdateAsync(deleteClientType);

                if (result.State.Equals(EntityState.Modified))
                {
                    await _clientTypeRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("ClientTypeBusiness", "Delete", ex.Message, ex.StackTrace);
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
    public async Task<int> GetByClientTypeName(string clientTypeName)
    {
      ReturnResult returnResult = new ReturnResult();
      try
      {
        var requiredClientTypeId = (await _clientTypeRepository.SelectFirstOrDefaultAsync(a => a.ClientTypeName.Equals(clientTypeName, StringComparison.OrdinalIgnoreCase)));
        if (requiredClientTypeId != null)
        {
          return requiredClientTypeId.ClientTypeId;
        }
        else
        {
          return 0;
        }
      }
      catch (Exception ex)
      {
        _logger.Error("ClientTypeBusiness", "GetByClientTypeName", ex.Message, ex.StackTrace);
        return 0;
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
                _clientTypeRepository.Dispose();
                _userRepository.Dispose();
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

using EVA.EIMS.Common;
using EVA.EIMS.Common.Constants;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ViewModel;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
    public class ClientBusiness : IClientBusiness
    {
        #region Private Variable
        private readonly IClientRepository _clientRepository;
        private readonly IUserRepository _userRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly ICustomPasswordHash _customPasswordHash;
        private readonly ILogger _logger;
        private Guid userId;
        private bool _disposed;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IClientTypeRepository _clientTypeRepository;
        private readonly IOptions<ApplicationSettings> _applicationSettings;
        private readonly double _clientExpirationHour;
        #endregion

        #region Constructor
        public ClientBusiness(IClientRepository clientRepository, IUserRepository userRepository, ICustomPasswordHash customPasswordHash, ILogger logger, IServiceProvider serviceProvider, IApplicationRepository applicationRepository, IClientTypeRepository clientTypeRepository, IOptions<ApplicationSettings> applicationSettings)
        {
            _clientRepository = clientRepository;
            _clientExpirationHour = applicationSettings.Value.ClientExpirationHour;
            _userRepository = userRepository;
            _customPasswordHash = customPasswordHash;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _applicationRepository = applicationRepository;
            _clientTypeRepository = clientTypeRepository;
            _disposed = false;
            _applicationSettings = applicationSettings;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// This method is used get the clients
        /// </summary>
        /// <returns>returns multiple clients details</returns>
        public async Task<ReturnResult> Get()
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var listOfClients = await _clientRepository.SelectAsync(c => c.IsActive.Value);
                if (listOfClients != null && listOfClients.Count() > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = listOfClients;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ResourceInformation.GetResValue("Client")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ClientBusiness", "Get", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to get the multiple client details
        /// <param name="tokenData">tokenData object</param>
        /// </summary>
        /// <returns>returns multiple organization details</returns>
        public async Task<ReturnResult> GetClient(int oauthClientId)
        {
            try
            {
                ReturnResult returnResult = new ReturnResult();
                List<Parameters> param;
                IExecuterStoreProc<OauthClientModel> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<OauthClientModel>>();
                if (oauthClientId == 0)
                {
                    param = new List<Parameters>() {
                    new Parameters("p_OauthClientId", DBNull.Value)
          };
                }
                else
                {
                    param = new List<Parameters>() {
                    new Parameters("p_OauthClientId", oauthClientId)
          };
                }

                var listOfCllients = (await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllClient.ToString(), param));

                if (listOfCllients != null && listOfCllients.Count > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = listOfCllients;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("NotExists");
                }
                return returnResult;
            }

            catch (Exception ex)
            {
                _logger.Error("ClientBusiness", "GetClient", ex.Message, ex.StackTrace);
                return null;

            }
        }

        /// <summary>
        /// This method is used get the client details by id
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <returns>returns single client details</returns>
        public async Task<OauthClient> GetById(string clientId)
        {
            try
            {
                return (await _clientRepository.SelectAsync(c => c.ClientId.Equals(clientId) && c.IsActive.Value)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error("ClientBusiness", "GetById", ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// This method is used get the client details by id for inactive client
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <returns>returns single client details</returns>
        public async Task<OauthClient> GetByIdForInActiveClient(string clientId)
        {
            try
            {
                return (await _clientRepository.SelectAsync(c => c.ClientId.Equals(clientId))).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error("ClientBusiness", "GetByIdForInActiveClient", ex.Message, ex.StackTrace);
                throw ex;
            }
        }


        

        /// <summary>
        /// This method is used to save the client details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="client">client object</param>
        /// <returns>returns response  message</returns>
        public async Task<ReturnResult> Save(string userName, OauthClient client)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (client == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(client.ClientId))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideClientId");
                    return returnResult;
                }

                if (client.AppId == 0)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidAppId");
                    return returnResult;
                }

                if (client.ClientTypeId == 0)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideClientType");
                    return returnResult;
                }

                if (client.ClientValidationPeriod == null || client.ClientValidationPeriod == 0)
                {
                    client.ClientValidationPeriod = null;
                    client.ClientExpireOn = null;
                }

                if (string.IsNullOrEmpty(client.ClientSecret))
                {
                    client.ClientSecret = _customPasswordHash.CreateClientSecret();
                }

                if (await _clientRepository.SelectFirstOrDefaultAsync(c => c.ClientId.Equals(client.ClientId) && !c.IsActive.Value) != null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ExistsAndInActive");
                    return returnResult;
                }

                if (await _clientRepository.SelectFirstOrDefaultAsync(c => c.ClientId.Equals(client.ClientId) && c.IsActive.Value) != null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("Client")} {client.ClientId} { ResourceInformation.GetResValue("AlreadyExist")}";
                    return returnResult;
                }

                if (client.IsActive == null)
                {
                    client.IsActive = true;
                }

                if (client.DeleteRefreshToken == null)
                {
                    client.DeleteRefreshToken = false;
                }

                if (client.ClientValidationPeriod != null && client.ClientValidationPeriod != 0)
                {
                    if ((client.ClientValidationPeriod.Value != Math.Floor(client.ClientValidationPeriod.Value)) || client.ClientValidationPeriod < 0)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("ProperClientValidateFor");
                        return returnResult;
                    }

                    client.ClientExpireOn = DateTime.UtcNow.AddDays(client.ClientValidationPeriod.Value);
                }

                client.ClientGuid = Guid.NewGuid();
                var userDetails = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
                client.CreatedBy = userDetails.UserId;
                client.ModifiedBy = userDetails.UserId;

                var result = await _clientRepository.AddAsync(client);

                if (result.State.Equals(EntityState.Added))
                {
                    await _clientRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("ClientBusiness", "Save", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to update the client details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="clientId">clientId</param>
        /// <param name="client">client object</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Update(string userName, string clientId, OauthClient client)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {
                if (client == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(clientId))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideClientId");
                    return returnResult;
                }

                if (client.ClientId != clientId)
                {
                    returnResult.Result = ResourceInformation.GetResValue("MismatchClientId");
                    return returnResult;
                }

                var updateClient = await _clientRepository.SelectFirstOrDefaultAsync(c => c.ClientId.Equals(clientId));

                if (updateClient == null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("Client")} {clientId} { ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                if (client.AppId == 0)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidAppId");
                    return returnResult;
                }

                if (client.ClientTypeId == 0)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideClientType");
                    return returnResult;
                }

                if (client.IsActive == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("IsActiveRequired");
                    return returnResult;
                }

                if (client.DeleteRefreshToken == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DeletRefreshTokenRequired");
                    return returnResult;
                }

                if (client.ClientValidationPeriod == null || client.ClientValidationPeriod == 0)
                {
                    updateClient.ClientValidationPeriod = null;
                    updateClient.ClientExpireOn = null;
                }

                if (client.ClientValidationPeriod != null && client.ClientValidationPeriod != 0)
                {
                    if ((client.ClientValidationPeriod.Value != Math.Floor(client.ClientValidationPeriod.Value)) || client.ClientValidationPeriod < 0)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("ProperClientValidateFor");
                        return returnResult;
                    }

                    updateClient.ClientValidationPeriod = client.ClientValidationPeriod;
                    updateClient.ClientExpireOn = DateTime.UtcNow.AddDays(client.ClientValidationPeriod.Value);
                }

                if (string.IsNullOrEmpty(client.AllowedScopes))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideScopes");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(client.Flow))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideFlow");
                    return returnResult;
                }

                userId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value)).UserId;

                updateClient.AppId = client.AppId;
                updateClient.AllowedScopes = client.AllowedScopes;
                updateClient.ClientId = client.ClientId;
                updateClient.ClientName = client.ClientName;
                updateClient.ClientTypeId = client.ClientTypeId;
                updateClient.Flow = client.Flow;
                updateClient.IsActive = client.IsActive;
                updateClient.ModifiedBy = userId;
                updateClient.DeleteRefreshToken = client.DeleteRefreshToken;
                updateClient.TokenValidationPeriod = client.TokenValidationPeriod;

                var result = await _clientRepository.UpdateAsync(updateClient);

                if (result.State.Equals(EntityState.Modified))
                {
                    await _clientRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("ClientBusiness", "Update", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }
        /// <summary>
        /// This method is used to delete the client details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="clientId">client</param>
        /// <returns>returns response  message</returns>
        public async Task<ReturnResult> Delete(string userName, string clientId)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {
                userId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value)).UserId;
                if (string.IsNullOrEmpty(clientId))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideClientId");
                    return returnResult;
                }

                var deleteClient = await _clientRepository.SelectFirstOrDefaultAsync(c => c.ClientId.Equals(clientId));

                if (deleteClient == null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("Client")} {clientId} {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                deleteClient.IsActive = false;
                deleteClient.ModifiedBy = userId;
                var result = await _clientRepository.UpdateAsync(deleteClient);

                if (result.State.Equals(EntityState.Modified))
                {
                    await _clientRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("ClientBusiness", "Delete", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to create client dynamically
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="dynamicClient"></param>
        /// <returns></returns>
        public async Task<ReturnResult> DynamicClientCreation(TokenData tokenData, OauthClient dynamicClient)
        {
            ReturnResult returnResult = new ReturnResult();
            OAuthClientViewModel oAuthClientViewModel = new OAuthClientViewModel();
            try
            {
                if (dynamicClient == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(dynamicClient.ClientId))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideClientId");
                    return returnResult;
                }

                // If SuperAdmin is accessing dynamic client creation

                if (tokenData.Role.Length != 0)
                {
                    var clientDetails = await _clientRepository.SelectFirstOrDefaultAsync(a => a.ClientId.Equals(dynamicClient.ClientId));

                    if (clientDetails != null && !clientDetails.IsActive.Value)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("ExistsAndInActive");
                        return returnResult;
                    }

                    if (clientDetails != null && clientDetails.IsActive.Value)
                    {
                        returnResult.Result = $"{ResourceInformation.GetResValue("Client")} {dynamicClient.ClientId} { ResourceInformation.GetResValue("AlreadyExist")}";
                        return returnResult;
                    }

                    if (dynamicClient.AppId == 0)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("InvalidAppId");
                        return returnResult;
                    }

                    if (dynamicClient.ClientTypeId == 0)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("ProvideClientType");
                        return returnResult;
                    }

                    if (dynamicClient.ClientValidationPeriod == null || dynamicClient.ClientValidationPeriod == 0)
                    {
                        dynamicClient.ClientValidationPeriod = null;
                        dynamicClient.ClientExpireOn = null;
                    }

                    if (string.IsNullOrEmpty(dynamicClient.ClientSecret))
                    {
                        dynamicClient.ClientSecret = _customPasswordHash.CreateClientSecret();
                    }

                    if (dynamicClient.IsActive == null)
                    {
                        dynamicClient.IsActive = true;
                    }

                    if (dynamicClient.DeleteRefreshToken == null)
                    {
                        dynamicClient.DeleteRefreshToken = false;
                    }

                    if (dynamicClient.ClientValidationPeriod != null && dynamicClient.ClientValidationPeriod != 0)
                    {
                        if ((dynamicClient.ClientValidationPeriod.Value != Math.Floor(dynamicClient.ClientValidationPeriod.Value)) || dynamicClient.ClientValidationPeriod < 0)
                        {
                            returnResult.Success = false;
                            returnResult.Result = ResourceInformation.GetResValue("ProperClientValidateFor");
                            return returnResult;
                        }

                        dynamicClient.ClientExpireOn = DateTime.UtcNow.AddDays(dynamicClient.ClientValidationPeriod.Value);
                    }

                    dynamicClient.ClientGuid = Guid.NewGuid();
                    userId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(tokenData.UserName) && u.IsActive.Value)).UserId;
                    dynamicClient.CreatedBy = userId;
                    dynamicClient.ModifiedBy = userId;

                    var result = await _clientRepository.AddAsync(dynamicClient);

                    if (result.State.Equals(EntityState.Added))
                    {
                        await _clientRepository.UnitOfWork.SaveChangesAsync();
                        oAuthClientViewModel.ClientId = dynamicClient.ClientId;
                        oAuthClientViewModel.ClientSecret = dynamicClient.ClientSecret;
                        returnResult.Success = true;
                        returnResult.Data = oAuthClientViewModel;
                    }
                    else
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("DataSavedFailure");
                    }
                    return returnResult;


                }
                //If client is trying to create new client using client credentials flow 
                else
                {
                    //Validate client details, who want to generate new client
                    var clientDetails = await _clientRepository.SelectFirstOrDefaultAsync(a => a.ClientId.Equals(tokenData.ClientId) & a.IsActive.Value);

                    if (clientDetails == null)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("UnauthorizedAccessException");
                        return returnResult;
                    }

                    if (clientDetails.ClientTypeId != (int)ClientTypes.ServiceClient)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("MismatchClientTypeId");
                        return returnResult;
                    }

                    //If client expired then he cant create new client
                    if (clientDetails.ClientExpireOn != null && clientDetails.ClientExpireOn < DateTime.UtcNow)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("ClientExpired");
                        return returnResult;
                    }

                    if (clientDetails.ClientExpireOn != null && ((clientDetails.ClientExpireOn.Value.Subtract(DateTime.UtcNow)).TotalHours>= _clientExpirationHour))
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("NotValidRenewablePeriod");
                        return returnResult;
                    }
                    


                   //Validate if provided new client Id is already existed
                   var existingClient = await _clientRepository.SelectFirstOrDefaultAsync(a => a.ClientId.Equals(dynamicClient.ClientId));

                    if (existingClient != null && !existingClient.IsActive.Value)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("ExistsAndInActive");
                        return returnResult;
                    }

                    if (existingClient != null && existingClient.IsActive.Value)
                    {
                        returnResult.Result = $"{ResourceInformation.GetResValue("Client")} {dynamicClient.ClientId} { ResourceInformation.GetResValue("AlreadyExist")}";
                        return returnResult;
                    }

                    //Only below details of new client are different from crater client
                    dynamicClient.ClientSecret = _customPasswordHash.CreateClientSecret();
                    dynamicClient.ClientName = dynamicClient.ClientId;
                    dynamicClient.ClientGuid = Guid.NewGuid();

                    //Below details of new client will be same as creaters client
                    dynamicClient.ClientTypeId = clientDetails.ClientTypeId;
                    dynamicClient.Flow = clientDetails.Flow;
                    dynamicClient.DeleteRefreshToken = clientDetails.DeleteRefreshToken;
                    dynamicClient.AllowedScopes = clientDetails.AllowedScopes;
                    dynamicClient.AppId = clientDetails.AppId;
                    dynamicClient.IsActive = clientDetails.IsActive;
                    dynamicClient.TokenValidationPeriod = clientDetails.TokenValidationPeriod;
                    dynamicClient.CreatedBy = clientDetails.ClientGuid;
                    dynamicClient.ModifiedBy = clientDetails.ClientGuid;

                    if (clientDetails.ClientValidationPeriod != null)
                    {
                        dynamicClient.ClientValidationPeriod = clientDetails.ClientValidationPeriod;
                        dynamicClient.ClientExpireOn = DateTime.UtcNow.AddDays(clientDetails.ClientValidationPeriod.Value);
                    }
                    else
                    {
                        dynamicClient.ClientValidationPeriod = _applicationSettings.Value.ClientValidationPeriod;
                        dynamicClient.ClientExpireOn = DateTime.UtcNow.AddDays(_applicationSettings.Value.ClientValidationPeriod);
                    }

                    var result = await _clientRepository.AddAsync(dynamicClient);

                    if (result.State.Equals(EntityState.Added))
                    {
                        await _clientRepository.UnitOfWork.SaveChangesAsync();
                        oAuthClientViewModel.ClientId = dynamicClient.ClientId;
                        oAuthClientViewModel.ClientSecret = dynamicClient.ClientSecret;
                        returnResult.Success = true;
                        returnResult.Data = oAuthClientViewModel;
                    }
                    else
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("DataSavedFailure");
                    }
                    return returnResult;
                }

            }


            catch (Exception ex)
            {
                _logger.Error("ClientBusiness", "DynamicClientCreation", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataSavedFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// update client secret of expired client and set client expiraton time
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="updateClientModel"></param>
        /// <returns></returns>
        public async Task<ReturnResult> UpdateClientSecret(TokenData tokenData, OAuthClientViewModel updateClientModel)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (updateClientModel == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(updateClientModel.ClientId) || string.IsNullOrEmpty(updateClientModel.ClientSecret))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                    return returnResult;
                }

                var clientDetails = await _clientRepository.SelectFirstOrDefaultAsync(a => a.ClientId.Equals(tokenData.ClientId) & a.IsActive.Value);

                if (clientDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("UnauthorizedAccessException");
                    return returnResult;
                }

                if (clientDetails.ClientTypeId != (int)ClientTypes.ServiceClient)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("MismatchClientTypeId");
                    return returnResult;
                }

                if (clientDetails.ClientExpireOn != null && clientDetails.ClientExpireOn < DateTime.UtcNow)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ClientExpired");
                    return returnResult;
                }
                var existingClient = await _clientRepository.SelectFirstOrDefaultAsync(a => a.ClientId.Equals(updateClientModel.ClientId, StringComparison.OrdinalIgnoreCase) && a.IsActive.Value);
                if (existingClient == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("NotExists");
                    return returnResult;
                }

                if(existingClient.ClientValidationPeriod == null || existingClient.ClientExpireOn == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ClientNotExpire");
                    return returnResult;
                }

                if(existingClient.ClientExpireOn != null && ((existingClient.ClientExpireOn.Value.Subtract(DateTime.UtcNow)).TotalHours >= _clientExpirationHour))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("NotValidRenewablePeriod");
                    return returnResult;
                }

                existingClient.ClientSecret = _customPasswordHash.CreateClientSecret();
                existingClient.ClientExpireOn = DateTime.UtcNow.AddDays(existingClient.ClientValidationPeriod.Value);

                var result = await _clientRepository.UpdateAsync(existingClient);

                if (result.State.Equals(EntityState.Modified))
                {
                    await _clientRepository.UnitOfWork.SaveChangesAsync();
                    updateClientModel.ClientSecret = existingClient.ClientSecret;
                    returnResult.Success = true;
                    returnResult.Data = updateClientModel;
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
                _logger.Error("ClientBusiness", "UpdateClientSecret", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataSavedFailure");
                return returnResult;
            }
        }

        public async Task<List<ClientApplicationDetails>> GetAzureAppIdByClientId(string clientId)
        {
            try
            {
                List<Parameters> param = new List<Parameters>() { new Parameters("ClientId", clientId) };
                IExecuterStoreProc<ClientApplicationDetails> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<ClientApplicationDetails>>();
                var result=await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAzureAppIdByClientId.ToString(), param);
                return result;
            }
            catch (Exception e)
            {
                throw e;
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
                _clientRepository.Dispose();
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

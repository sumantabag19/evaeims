using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
    public class IMSLogOutBusiness : IIMSLogOutBusiness
    {
        #region Private Variables
        private bool _disposed;
        private readonly IServiceProvider _serviceProvider;
        private readonly IIMSLogOutRepository _iMSLogOutRepository;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public IMSLogOutBusiness(IIMSLogOutRepository iIMSLogOutRepository, ILogger logger, IServiceProvider serviceProvider)
        {
            _iMSLogOutRepository = iIMSLogOutRepository;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _disposed = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method is used to get all logged out token from database.
        /// </summary>
        /// <returns>All logged out tokens</returns>
        public async Task<ReturnResult> GetLoggedOutUserToken()
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var result = await _iMSLogOutRepository.SelectAllAsync();
                if (result != null && result.Count() > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = result;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                }
                return returnResult;

            }
            catch (Exception ex)
            {
                _logger.Error("IMSLogOutBusiness", "GetLoggedOutUserToken", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;

            }
        }

        /// <summary>
        /// This method returns responce message with success true if the token exist in Logout table else false.
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>responce message</returns>
        public async Task<ReturnResult> IsUserLoggedOut(string token)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidToken");
                    return returnResult;
                }
                else
                {
                    if (await _iMSLogOutRepository.SelectFirstOrDefaultAsync(i => i.LogOutToken.Equals(token)) != null)
                    {
                        returnResult.Success = true;
                        returnResult.Result = ResourceInformation.GetResValue("InvalidToken");
                    }
                    else
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("ValidToken");
                    }

                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("IMSLogOutBusiness", "IsUserLoggedOut", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;

            }

        }

        /// <summary>
        /// This method is used to save the token for blacklisting when user logout. 
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns>responce message</returns>
        public async Task<ReturnResult> SaveLogOutUserToken(string token)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (token.Contains("Bearer"))
                {
                    token = token.Replace("Bearer ", String.Empty);
                }
                JwtSecurityToken tokenDetails = null;
                if (string.IsNullOrWhiteSpace(token))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidToken");
                    return returnResult;
                }
                var jwtHandler = new JwtSecurityTokenHandler();
                if (!jwtHandler.CanReadToken(token))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidToken");
                    return returnResult;
                }
                else
                {
                    tokenDetails = jwtHandler.ReadToken(token) as JwtSecurityToken;

                }

                var isExisting = await _iMSLogOutRepository.SelectFirstOrDefaultAsync(i => i.LogOutToken.Equals(token));
                if (isExisting != null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("TokenAlreadyExists");
                    return returnResult;
                }
                IMSLogOutToken iMSLogOut = new IMSLogOutToken
                {
                    LogOutTokenId = new Guid(),
                    LogOutToken = token,
                    LogoutOn = DateTime.Now,
                    TokenValidationPeriod = tokenDetails.ValidTo

                };
                var result = await _iMSLogOutRepository.AddAsync(iMSLogOut);
                if (result.State.Equals(EntityState.Added))
                {
                    await _iMSLogOutRepository.UnitOfWork.SaveChangesAsync();
                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("DataSavedSuccess");
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DataSavedFailure");
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("IMSLogOutBusiness", "SaveLogOutUserToken", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataSavedFailure");
                return returnResult;

            }
        }

        /// <summary>
        /// This method is used to delete all logged out tokens from Database
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public async Task DeleteLoggedOutToken(TokenData tokenData)
        {
            try
            {
                if (tokenData != null)
                {
                    int deletedRecordsCount = await _iMSLogOutRepository.ExecuteQueryAsync(ProcedureConstants.proc_DeleteLoggedOutToken);
                    _logger.Info("IMSLogOutBusiness", "DeleteLoggedOutToken", "Record deleted count = " + deletedRecordsCount, "");
                }
                else
                    _logger.Error("IMSLogOutBusiness", "DeleteLoggedOutToken", "Invalid token", "Scheduler failed at : " + DateTime.Now);

            }
            catch (Exception ex)
            {
                _logger.Error("IMSLogOutBusiness", "DeleteLoggedOutToken", ex.Message, ex.StackTrace);
            }
        }
        #endregion


        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) { return; }
            else
            {
                _iMSLogOutRepository.Dispose();
            }
            _disposed = true;

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }



        #endregion
    }


}

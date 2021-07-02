using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ComplexEntities;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
    public class PasswordHistoryBusiness : IPasswordHistoryBusiness
    {
        #region Private variables
        private bool _disposed;
        private readonly IPasswordHistoryRepository _passwordHistoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICustomPasswordHash _customPasswordHash;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<ApplicationSettings> _applicationSettings;
        #endregion

        #region Public Constructor
        public PasswordHistoryBusiness(IPasswordHistoryRepository passwordHistoryRepository, ILogger logger, IServiceProvider serviceProvider, ICustomPasswordHash customPasswordHash, IUserRepository userRepository, IOptions<ApplicationSettings> applicationSettings)
        {
            _passwordHistoryRepository = passwordHistoryRepository;
            _applicationSettings = applicationSettings;
            _userRepository = userRepository;
            _customPasswordHash = customPasswordHash;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _disposed = false;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Method to manage password into passhistory table
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPwd"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public async Task<ReturnResult> ManagePassword(string userId, string newPwd, string createdBy)
        {
            PasswordHistory passwordHistory = new PasswordHistory();
            ReturnResult returnResult = new ReturnResult()
            {
                Success = false
            };
            try
            {
                var userDetails = await _userRepository.SelectFirstOrDefaultAsync(x => x.UserId.ToString().Equals(userId) && x.IsActive.Value);

                if (_customPasswordHash.ScryptHashStringVerify(userDetails.PasswordHash, newPwd))
                {             
                    returnResult.Result = ResourceInformation.GetResValue("PasswordSame");
                    return returnResult;
                }

                var passwordHashList = (await _passwordHistoryRepository.SelectAsync(x=>x.UserId.ToString().Equals(userId))).Select(u=> u.PasswordHash).ToList();

                var lastPasswordChangeOnList = (await _passwordHistoryRepository.SelectAsync(x => x.UserId.ToString().Equals(userId))).Select(u => u.LastPasswordChangeDate).ToList();

                if (passwordHashList != null)
                {
                    foreach (var passwordHash in passwordHashList)
                    {
                        if (_customPasswordHash.ScryptHashStringVerify(passwordHash,newPwd))
                        {
            
                            returnResult.Result = ResourceInformation.GetResValue("PasswordAlreadyPresent");
                            return returnResult;
                        }
                    }

                    if (passwordHashList.Count() < _applicationSettings.Value.PasswordCount)
                    {
                        passwordHistory.UserId = new Guid(userId);
                        passwordHistory.PasswordHash = userDetails.PasswordHash;
                        passwordHistory.LastPasswordChangeDate = DateTime.Now;
                        passwordHistory.CreateBy = new Guid(createdBy);

                        var result = await _passwordHistoryRepository.AddAsync(passwordHistory);

                        if (result.State.Equals(EntityState.Added))
                        {
                            await _passwordHistoryRepository.UnitOfWork.SaveChangesAsync();
                            returnResult.Success = true;
                        }
                    }

                    if (passwordHashList.Count() == _applicationSettings.Value.PasswordCount)
                    {
                        DateTime smallestDate = lastPasswordChangeOnList.Min(p => p);
                        var deletePassword = await _passwordHistoryRepository.SelectFirstOrDefaultAsync(x => x.LastPasswordChangeDate.ToString("yyyy-MM-dd HH:mm:ss.fff") == smallestDate.ToString("yyyy-MM-dd HH:mm:ss.fff") && x.UserId.ToString().Equals(userId));
                        var result = await _passwordHistoryRepository.DeleteAsync(deletePassword);

                        if (result.State.Equals(EntityState.Deleted))
                        {
                            await _passwordHistoryRepository.UnitOfWork.SaveChangesAsync();

                            passwordHistory.UserId = new Guid(userId);
                            passwordHistory.PasswordHash = userDetails.PasswordHash;
                            passwordHistory.LastPasswordChangeDate = DateTime.Now;
                            passwordHistory.CreateBy = new Guid(createdBy);

                            result = await _passwordHistoryRepository.AddAsync(passwordHistory);

                            if (result.State.Equals(EntityState.Added))
                            {
                                await _passwordHistoryRepository.UnitOfWork.SaveChangesAsync();
                                returnResult.Success = true;
                            }
                        }                                      
                    }

                    if (passwordHashList.Count() > _applicationSettings.Value.PasswordCount)
                    {
                      
                        var resultReturn = await _passwordHistoryRepository.ExcecuteQueryAsync(ProcedureConstants.procGetAllOldPassword.ToString() +" '" +userId +"',"+ _applicationSettings.Value.PasswordCount);
                        if (resultReturn != 0 )
                        {
                            await _passwordHistoryRepository.UnitOfWork.SaveChangesAsync();
                            returnResult.Success = true;
                        }
                        else
                        {
                            return returnResult;
                        }                  

                        passwordHistory.UserId = new Guid(userId);
                        passwordHistory.PasswordHash = userDetails.PasswordHash;
                        passwordHistory.LastPasswordChangeDate = DateTime.Now;
                        passwordHistory.CreateBy = new Guid(createdBy);

                        var result = await _passwordHistoryRepository.AddAsync(passwordHistory);
                        if (result.State.Equals(EntityState.Added))
                        {
                            await _passwordHistoryRepository.UnitOfWork.SaveChangesAsync();
                            returnResult.Success = true;
                        }                        
                    }
                    return returnResult;
                }
                else
                {
                    passwordHistory.UserId = new Guid(userId);
                    passwordHistory.PasswordHash = userDetails.PasswordHash;
                    passwordHistory.LastPasswordChangeDate = DateTime.Now;
                    passwordHistory.CreateBy = new Guid(createdBy);

                    var result = await _passwordHistoryRepository.AddAsync(passwordHistory);

                    if (result.State.Equals(EntityState.Added))
                    {
                        await _passwordHistoryRepository.UnitOfWork.SaveChangesAsync();
                        returnResult.Success = true;
                    }
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                returnResult.Success = false;
                _logger.Error("PasswordHistoryBusiness", "ManagePassword", ex.Message, ex.StackTrace);
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
                _passwordHistoryRepository.Dispose();
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

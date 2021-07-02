using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sodium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
    public class UserAnswerBusiness : IUserAnswerBusiness
    {
        #region Private Variable

        private readonly IUserAnswerRepository _userAnswerRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly int _totalSecurityQuestions;
        private readonly ILogger _logger;
        private bool _disposed;

        #endregion

        #region Constructor
        public UserAnswerBusiness(IUserAnswerRepository userAnswerRepository, IServiceProvider serviceProvider, IOptions<ApplicationSettings> applicationSettings, ILogger logging)
        {
            _userAnswerRepository = userAnswerRepository;
            _serviceProvider = serviceProvider;
            _totalSecurityQuestions = applicationSettings.Value.TotalSecurityQuestions;
            _logger = logging;
            _disposed = false;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Add or Updates user's security questions and/or answers
        /// <param name="userName">UserId</param>
        /// <param name="userAns">List of user's question and answer</param>
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnResult> AddOrUpdate(string userName, List<UserAnswer> userAns)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (userAns != null && userAns.Count() == _totalSecurityQuestions)
                {

                    IUserRepository userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                    ICustomPasswordHash customPasswordHash = _serviceProvider.GetRequiredService<ICustomPasswordHash>();
                    var userDetails = await userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
                    if (userDetails == null)
                    {
                        returnResult.Success = false;
                        returnResult.Result = $"{ResourceInformation.GetResValue("User")} { ResourceInformation.GetResValue("NotExists")}";
                        return returnResult;
                    }
                    Guid userIdFromToken = userDetails.UserId;

                    List<UserAnswer> userAnswersCrypto = new List<UserAnswer>();

                    foreach (var item in userAns)
                    {
                        if (item.UserId.Equals(userIdFromToken))
                        {
                            userAnswersCrypto.Add(new UserAnswer
                            {
                                QuestionId = item.QuestionId,
                                UserAnswerText = customPasswordHash.ScryptHash(item.UserAnswerText.ToUpper()),
                                UserId = userIdFromToken,
                                CreatedBy = userIdFromToken,
                                ModifiedBy = userIdFromToken

                            });
                        }
                        else
                        {
                            returnResult.Result = ResourceInformation.GetResValue("ProvideValidUserId");
                            return returnResult;
                        }
                    }
                    var existingQuesAns = await _userAnswerRepository.SelectAsync(u => u.UserId.Equals(userIdFromToken));

                    if (existingQuesAns != null && existingQuesAns.Count() == _totalSecurityQuestions)
                    {
                        userDetails.RequiredSecurityQuestion = false;
                        await _userAnswerRepository.DeleteRange(existingQuesAns);
                        await _userAnswerRepository.AddRange(userAnswersCrypto);
                        await userRepository.UpdateAsync(userDetails);
                        await userRepository.UnitOfWork.SaveChangesAsync();
                        await _userAnswerRepository.UnitOfWork.SaveChangesAsync();
                    }

                    else if (existingQuesAns != null && existingQuesAns.Count() != _totalSecurityQuestions && existingQuesAns.Count() != 0)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("ProvideRequiredSecQues");
                        return returnResult;
                    }
                    else
                    {
                        await _userAnswerRepository.AddRange(userAnswersCrypto);
                        userDetails.RequiredSecurityQuestion = false;
                        await userRepository.UpdateAsync(userDetails);
                        await userRepository.UnitOfWork.SaveChangesAsync();
                        await _userAnswerRepository.UnitOfWork.SaveChangesAsync();
                    }

                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("DataSavedSuccess");
                }
                else
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideAnsDetails");
                    returnResult.Success = false;
                }

                return returnResult;

            }

            catch (Exception ex)
            {
                _logger.Error("UserAnswerBusiness", "AddOrUpdate", ex.Message, ex.StackTrace);
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                returnResult.Success = false;
                return returnResult;
            }
        }

        /// <summary>
        /// Updates user's security questions and/or answers
        /// <param name="userName">UserId</param>
        /// <param name="userAns">List of user's question and answer</param>
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnResult> UpdateUserQuestionAnswer(string userName, List<UserAnswer> userAns)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (userAns != null && userAns.Count() == _totalSecurityQuestions)
                {

                    IUserRepository userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                    ICustomPasswordHash customPasswordHash = _serviceProvider.GetRequiredService<ICustomPasswordHash>();
                    var userDetails = await userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
                    if (userDetails == null)
                    {
                        returnResult.Success = false;
                        returnResult.Result = $"{ResourceInformation.GetResValue("User")} { ResourceInformation.GetResValue("NotExists")}";
                        return returnResult;
                    }
                    Guid userIdFromToken = userDetails.UserId;

                    List<UserAnswer> userAnswersCrypto = new List<UserAnswer>();
                    var existingQuesAns = await _userAnswerRepository.SelectAsync(u => u.UserId.Equals(userIdFromToken));

                    foreach (var item in userAns)
                    {
                        if (item.UserId.Equals(userIdFromToken))
                        {
                            if (!string.IsNullOrEmpty(item.UserAnswerText.Trim()))
                            {
                                userAnswersCrypto.Add(new UserAnswer
                                {
                                    QuestionId = item.UpdatedQuestionId,
                                    UserAnswerText = customPasswordHash.ScryptHash(item.UserAnswerText.ToUpper()),
                                    UserId = userIdFromToken,
                                    CreatedBy = userIdFromToken,
                                    ModifiedBy = userIdFromToken
                                });
                            }
                            else
                            {
                                userAnswersCrypto.Add(new UserAnswer
                                {
                                    QuestionId = item.QuestionId,
                                    UserAnswerText = existingQuesAns.FirstOrDefault(e => e.QuestionId == item.QuestionId).UserAnswerText,
                                    UserId = userIdFromToken,
                                    CreatedBy = userIdFromToken,
                                    ModifiedBy = userIdFromToken
                                });
                            }
                        }
                        else
                        {
                            returnResult.Result = ResourceInformation.GetResValue("ProvideValidUserId");
                            return returnResult;
                        }
                    }

                    if (existingQuesAns != null && existingQuesAns.Count() == _totalSecurityQuestions)
                    {
                        userDetails.RequiredSecurityQuestion = false;
                        await _userAnswerRepository.DeleteRange(existingQuesAns);
                        await _userAnswerRepository.AddRange(userAnswersCrypto);
                        await userRepository.UpdateAsync(userDetails);
                        await userRepository.UnitOfWork.SaveChangesAsync();
                        await _userAnswerRepository.UnitOfWork.SaveChangesAsync();
                    }

                    else if (existingQuesAns != null && existingQuesAns.Count() != _totalSecurityQuestions && existingQuesAns.Count() != 0)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("ProvideRequiredSecQues");
                        return returnResult;
                    }
                    else
                    {
                        await _userAnswerRepository.AddRange(userAnswersCrypto);
                        userDetails.RequiredSecurityQuestion = false;
                        await userRepository.UpdateAsync(userDetails);
                        await userRepository.UnitOfWork.SaveChangesAsync();
                        await _userAnswerRepository.UnitOfWork.SaveChangesAsync();
                    }

                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("DataSavedSuccess");
                }
                else
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideAnsDetails");
                    returnResult.Success = false;
                }

                return returnResult;

            }

            catch (Exception ex)
            {
                _logger.Error("UserAnswerBusiness", "UpdateUserQuestionAnswer", ex.Message, ex.StackTrace);
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                returnResult.Success = false;
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
                _userAnswerRepository.Dispose();

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

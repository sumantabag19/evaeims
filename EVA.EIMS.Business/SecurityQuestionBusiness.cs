using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVA.EIMS.Common;
using EVA.EIMS.Common.Constants;
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
using Microsoft.Extensions.Options;
using Sodium;

namespace EVA.EIMS.Business
{
    public class SecurityQuestionBusiness : ISecurityQuestionBusiness
    {
        #region Private Variable

        private readonly ISecurityQuestionRepository _securityQuestionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly int _randomQuestionCount;
        private readonly int _totalSecurityQuestions;
        private readonly int _forgotPasswordSession;
        private Guid _userId;
        private bool _disposed;
        private readonly string _encryptionKey;
        private readonly ICustomPasswordHash _customPasswordHash;
        #endregion

        #region Constructor
        public SecurityQuestionBusiness(ISecurityQuestionRepository securityQuestionRepository, IUserRepository userRepository, IApplicationRepository applicationRepository, IServiceProvider serviceProvider, IOptions<ApplicationSettings> applicationSettings, ILogger logger,ICustomPasswordHash customPasswordHash)
        {
            _securityQuestionRepository = securityQuestionRepository;
            _userRepository = userRepository;
            _serviceProvider = serviceProvider;
            _randomQuestionCount = applicationSettings.Value.RandomSecurityQuestion;
            _totalSecurityQuestions = applicationSettings.Value.TotalSecurityQuestions;
            _forgotPasswordSession = applicationSettings.Value.ForgotPasswordSession;
            _encryptionKey = applicationSettings.Value.Eck;
            _disposed = false;
            _logger = logger;
            _customPasswordHash = customPasswordHash;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// This method is used to get the multiple security questions
        /// </summary>
        /// <returns>returns  multiple security question details</returns>
        public async Task<ReturnResult> Get()
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var listofQuestions = await _securityQuestionRepository.SelectAsync(s => s.IsActive.Value);
                if (listofQuestions != null && listofQuestions.Count() > 0)
                {
                    foreach(var item in listofQuestions)
                    {
                        item.Question = _customPasswordHash.Decrypt(item.Question, _encryptionKey);
                    }
                    returnResult.Success = true;
                    returnResult.Data = listofQuestions;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DetailsNotFound");
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("SecurityQuestionBusiness", "Get", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to get the security question details by id
        /// </summary>
        /// <param name="questionId">questionId</param>
        /// <returns>returns single security question details</returns>
        public async Task<ReturnResult> GetQuestionById(int questionId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (questionId == 0)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideQuesId");
                    return returnResult;
                }

                var requiredQuestionInfo = await _securityQuestionRepository.SelectFirstOrDefaultAsync(s => s.QuestionId == questionId && s.IsActive.Value);
                if (requiredQuestionInfo != null)
                {
                    requiredQuestionInfo.Question = _customPasswordHash.Decrypt(requiredQuestionInfo.Question, _encryptionKey);
                    returnResult.Success = true;
                    returnResult.Data = requiredQuestionInfo;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DetailsNotFound");
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("SecuritQuestionBusiness", "GetbyQuestionId", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;

            }
        }

        /// <summary>
        /// This method is used to save the security question details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="question">security question object</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Save(string userName, SecurityQuestion question)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
              
                if (question == null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideQuestionDetails");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(question.Question))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideSecurityQuestion");
                    return returnResult;
                }

                var encryptQuestion = _customPasswordHash.Encrypt(question.Question, _encryptionKey);
                var existQuestion = await _securityQuestionRepository.SelectFirstOrDefaultAsync(s => s.Question.Equals(encryptQuestion) && !s.IsActive.Value);
                if (existQuestion != null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ExistsAndInActive");
                    return returnResult;
                }

                var existingQuestion = await _securityQuestionRepository.SelectFirstOrDefaultAsync(s => s.Question.Equals(encryptQuestion) && s.IsActive.Value);
                if (existingQuestion != null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("SecurityQuestion")} {question.Question} {ResourceInformation.GetResValue("AlreadyExist")}";
                    return returnResult;
                }

                question.Question = encryptQuestion;
                if (question.IsActive == null)
                {
                    question.IsActive = true;
                }
                var userDetails = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
                if (userDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("UserDetailsNotFound");
                    return returnResult;
                }
                _userId = userDetails.UserId;
                question.CreatedBy = _userId;
                question.ModifiedBy = _userId;
                var result = await _securityQuestionRepository.AddAsync(question);

                if (result.State.Equals(EntityState.Added))
                {
                    await _securityQuestionRepository.UnitOfWork.SaveChangesAsync();
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
                _logger.Error("OrganizationBusiness", "Save", ex.Message, ex.StackTrace);
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                returnResult.Success = false;
                return returnResult;
            }

        }

        /// <summary>
        /// This method is used update the security question details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="questionId">questionId</param>
        /// <param name="question">security question object</param>
        /// <returns>returns response message</returns>
        /// <summary>
        public async Task<ReturnResult> Update(string userName, int questionId, SecurityQuestion question)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
               
                if (question == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideQuestionDetails");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(question.Question))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideSecurityQuestion");
                    return returnResult;
                }
                if (questionId == 0)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideQuesId");
                    return returnResult;
                }
                var updateQuestion = await _securityQuestionRepository.SelectFirstOrDefaultAsync(s => s.QuestionId == questionId);
                if (updateQuestion == null)
                {
                    returnResult.Success = false;
                    returnResult.Result =
                        $"{ResourceInformation.GetResValue("SecurityQuestion")} {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
                
                if (question.IsActive == null)
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
                _userId = userDetails.UserId;
                updateQuestion.Question = _customPasswordHash.Encrypt(question.Question,_encryptionKey);
                updateQuestion.ModifiedBy = _userId;
                updateQuestion.IsActive = question.IsActive;

                var result = await _securityQuestionRepository.UpdateAsync(updateQuestion);
                if (result.State.Equals(EntityState.Modified))
                {
                    await _securityQuestionRepository.UnitOfWork.SaveChangesAsync();
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
                _logger.Error("SecurityQuestionBusiness", "Update", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result =
                    $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";

                return returnResult;
            }

        }

        /// <summary>
        /// This method is used to delete the security question details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="questionId">questionId</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Delete(string userName, int questionId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (questionId == 0)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideQuesId");
                    return returnResult;
                }
                var userDetails = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
                if (userDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("UserDetailsNotFound");
                    return returnResult;
                }
                _userId = userDetails.UserId;
                var deleteQuestion = await _securityQuestionRepository.SelectFirstOrDefaultAsync(s => s.QuestionId == questionId);
                if (deleteQuestion == null)
                {
                    returnResult.Result =
                        $"{ResourceInformation.GetResValue("SecurityQuestion")} {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                deleteQuestion.IsActive = false;
                deleteQuestion.ModifiedBy = _userId;

                var result = await _securityQuestionRepository.UpdateAsync(deleteQuestion);
                if (result.State.Equals(EntityState.Modified))
                {
                    await _securityQuestionRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("SecurityQuestionBusiness", "Delete", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result =
                    $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// Get random User questions using stored procedure
        /// <param name="userId">UserId</param>
        /// </summary>
        /// <returns>UserQuestions</returns>
        public async Task<ReturnResult> GetRandomSecurityQuestions(Guid userId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (userId.Equals(Guid.Empty))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidData");
                    return returnResult;
                }
                ReturnResult result = await VerifyFlow(userId);
                if (result.Success == false)
                {
                    return result;
                }
                IExecuterStoreProc<GetUserQuestions> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<GetUserQuestions>>();
                List<Parameters> param = new List<Parameters>
                    {
                        new Parameters("UserId", userId),
                        new Parameters("randomQuestionCount", _randomQuestionCount)
                    };
                var getTwoRandomQuestion = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.getTwoRandomSecurityQuestion.ToString(), param);
                if (getTwoRandomQuestion != null && getTwoRandomQuestion.Count() == _randomQuestionCount)
                {
                    foreach(var item in getTwoRandomQuestion)
                    {
                        item.Question =  _customPasswordHash.Decrypt(item.Question, _encryptionKey);
                    }
                    returnResult.Success = true;
                    returnResult.Data = getTwoRandomQuestion;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                    return returnResult;
                }

            }
            catch (Exception ex)
            {
                _logger.Error("SecurityQuestionBusiness", "GetRandomSecurityQuestions", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;

            }
        }



        /// <summary>
        /// Verify security questions answer by User
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userAnswers"></param>
        /// <returns>true or false</returns>
        public async  Task<ReturnResult> VerifySecurityQuestionsAnswer(List<SecurityAnswerFromUserModel> userAnswers)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IUserBusiness userBusiness = _serviceProvider.GetRequiredService<IUserBusiness>();
                ICustomPasswordHash customPasswordHash = _serviceProvider.GetRequiredService<ICustomPasswordHash>();

                if (userAnswers != null && userAnswers.Count() == _randomQuestionCount)
                {
                    Guid userId = userAnswers.Select(u => u.UserId).FirstOrDefault();
                    var userDetails = await _userRepository.SelectFirstOrDefaultAsync(x => x.UserId == userId && x.IsActive.Value);

                    if(userDetails == null)
                    {
                        returnResult.Success = false;
                        returnResult.Result = $"{ResourceInformation.GetResValue("User")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                        return returnResult;
                    }

                    if (userDetails.IsAccLock)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("AccountLocked");
                        return returnResult;
                    }
                    ReturnResult result = await VerifyFlow(userId);
                    if (result.Success == false)
                    {
                        return result;
                    }

                    IExecuterStoreProc<SecurityAnswerFromUserModel> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<SecurityAnswerFromUserModel>>();
                    List<Parameters> param = new List<Parameters>
                    {
                    new Parameters("UserId", userId)
                    };

                    var securityAnswers = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.getUserSecurityQuestionInfo.ToString(), param);
                    if (securityAnswers.Count != 0 && securityAnswers.Count() == _totalSecurityQuestions)
                    {
                        //check if the answers provided by user and the answers saved by him/her are same.
                        var anslist = (from a in securityAnswers
                                       join b in userAnswers
                                       on new { a.QuestionId } equals
                                       new { b.QuestionId }
                                       where
                                       customPasswordHash.ScryptHashStringVerify(a.UserAnswerText, b.UserAnswerText.ToUpper())
                                       select a.QuestionId);


                        if (anslist != null && anslist.Count() == _randomQuestionCount)
                        {
                            await userBusiness.ResetLock(userDetails.UserName.ToString(), (int)LockType.QuestionAnswerLock);
                            returnResult.Success = true;
                            IForgotPasswordFlowManagementRepository forgotPasswordFlowManagementRepository = _serviceProvider.GetRequiredService<IForgotPasswordFlowManagementRepository>();
                            var flowManagement = await forgotPasswordFlowManagementRepository.SelectFirstOrDefaultAsync(u => u.UserId.Equals(userId));

                            flowManagement.VerifiedSecurityQuestions = true;
                            flowManagement.VerifiedSecurityQuestionsOn = DateTime.Now;
                            await forgotPasswordFlowManagementRepository.UpdateAsync(flowManagement);
                            await forgotPasswordFlowManagementRepository.UnitOfWork.SaveChangesAsync();
                            returnResult.Result = ResourceInformation.GetResValue("SecurityQuesAnsVerified");
                        }
                        else
                        {
                            returnResult = await userBusiness.LockAccount(userDetails.UserName.ToString(), (int)LockType.QuestionAnswerLock);
                            returnResult.Success = false;
                            returnResult.Result = ResourceInformation.GetResValue("WrongQuestionAnswer") + " " + returnResult.Result;
                        }
                    }
                    else
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("UsersSecurityDataNotPresent");
                    }
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidInput");
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("SecurityQuestionBusiness", "VerifySecurityQuestionsAnswerByUser", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result =
                    $"{ResourceInformation.GetResValue("DataLoadFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// Get all the User questions using stored procedure
        /// <param name="userId">UserId</param>
        /// </summary>
        /// <returns>UserQuestions</returns>
        public async Task<ReturnResult> GetbyUserId(string userNamefromToken, Guid userId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (userId.Equals(Guid.Empty))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidData");
                    return returnResult;
                }

                var userDetailsfromToken = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userNamefromToken) && u.IsActive.Value);
                if (userDetailsfromToken == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("UserNotFound");
                    return returnResult;
                }

                if (userDetailsfromToken.IsAccLock)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("AccountLocked");
                    return returnResult;
                }

                if (!userDetailsfromToken.IsActive.Value)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InactiveRecord");
                    return returnResult;
                }

                if (userDetailsfromToken.UserId != userId)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("TokenUserMismatch");
                    return returnResult;
                }
                IExecuterStoreProc<GetUserQuestions> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<GetUserQuestions>>();
                List<Parameters> param = new List<Parameters>
                {
                    new Parameters("UserId", userId)
                };

                var userSecurityQuestionInfo = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.getUserSecurityQuestionInfo.ToString(), param);

                var userQuestions = userSecurityQuestionInfo.Select(us => new GetUserQuestions { QuestionId = us.QuestionId, Question = us.Question }).ToList();
                if (userSecurityQuestionInfo != null && userSecurityQuestionInfo.Count() == _totalSecurityQuestions)
                {
                    foreach(var item in userQuestions)
                    {
                        item.Question = _customPasswordHash.Decrypt(item.Question, _encryptionKey);
                    }
                    returnResult.Success = true;
                    returnResult.Data = userQuestions;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("QuestionsNotConfigued");
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("SecurityQuestionBusiness", "GetByUserId", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Verify Forgot Password Flow
        /// <param name="userId">UserId</param>
        /// </summary>
        /// <returns>response message</returns>
        private async Task<ReturnResult> VerifyFlow(Guid userId)
        {
            ReturnResult returnResult = new ReturnResult { Success = true };

            var existingUser = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserId.Equals(userId) && u.IsActive.Value);
            if (existingUser == null)
            {
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("User")} " + $"{ ResourceInformation.GetResValue("NotExists")}";
                return returnResult;
            }

            IForgotPasswordFlowManagementRepository forgotPasswordFlowManagementRepository = _serviceProvider.GetRequiredService<IForgotPasswordFlowManagementRepository>();

            ForgotPasswordFlowManagement flowManagement = await forgotPasswordFlowManagementRepository.SelectFirstOrDefaultAsync(fpm => fpm.UserId.Equals(userId));

            if (flowManagement == null)
            {
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("AccountVerificationRequired")}";
                return returnResult;
            }

            if (flowManagement.VerifiedEmailOn.AddMinutes(_forgotPasswordSession) < DateTime.Now)
            {
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("ForgotPasswordTimeOut");
                return returnResult;
            }

            if (existingUser.TwoFactorEnabled == true)
            {
                if (flowManagement.VerifiedOTP == false )
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("OTPVerificationRequired");
                    return returnResult;
                }

            }
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
                _securityQuestionRepository.Dispose();
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





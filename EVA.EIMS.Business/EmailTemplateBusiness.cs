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
    public class EmailTemplateBusiness : IEmailTemplateBusiness
    {
        #region Private variables
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;
        private Guid userId;
        private bool _disposed;
        #endregion

        #region Constructor
        public EmailTemplateBusiness(IEmailTemplateRepository emailTemplateRepository, IUserRepository userRepository, ILogger logger)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _userRepository = userRepository;
            _logger = logger;
            _disposed = false;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method is used get email templates
        /// </summary>
        /// <returns>returns multiple email templates details</returns>
        public async Task<ReturnResult> Get()
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var listOfEmailTemplates = await _emailTemplateRepository.SelectAllAsync();
                if (listOfEmailTemplates != null && listOfEmailTemplates.Count() > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = listOfEmailTemplates;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ResourceInformation.GetResValue("EmailTemplate")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("EmailTemplateBusiness", "Get", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used get the email template details by id
        /// </summary>
        /// <param name="emailTemplateId">emailTemplateId</param>
        /// <returns>returns single email template details</returns>
        public async Task<ReturnResult> GetById(int emailTemplateId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var requiredEmailTemplateInfo = await _emailTemplateRepository.SelectFirstOrDefaultAsync(e => e.EmailTemplateId == emailTemplateId);
                if (requiredEmailTemplateInfo != null)
                {
                    returnResult.Success = true;
                    returnResult.Data = requiredEmailTemplateInfo;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                        $" {ResourceInformation.GetResValue("EmailTemplate")} " +
                        $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("EmailTemplateBusiness", "GetById", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to save the email template details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="emailTemplate">emailTemplate object</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Save(string userName, EmailTemplate emailTemplate)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {
                userId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value)).UserId;
                emailTemplate.ModifiedBy = userId;

                var result = await _emailTemplateRepository.AddAsync(emailTemplate);

                if (result.State.Equals(EntityState.Added))
                {
                    await _emailTemplateRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("EmailTemplateBusiness", "Save", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to update the emaliTemplate details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="emailTemplateId">emailTemplateId</param>
        /// <param name="emailTemplate">emaliTemplate object</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Update(string userName, int emailTemplateId, EmailTemplate emailTemplate)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {
				userId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value)).UserId;
				var updateEmailTempate = await _emailTemplateRepository.SelectFirstOrDefaultAsync(e => e.EmailTemplateId == emailTemplateId);

                if (updateEmailTempate == null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("EmailTemplate")} { ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                updateEmailTempate.AppId = emailTemplate.AppId;
                updateEmailTempate.EmailBody = emailTemplate.EmailBody;
                updateEmailTempate.EmailConfidentialMsg = emailTemplate.EmailConfidentialMsg;
                updateEmailTempate.EmailFooter = emailTemplate.EmailFooter;
                updateEmailTempate.EmailFrom = emailTemplate.EmailFrom;
                updateEmailTempate.LanguageId = emailTemplate.LanguageId;
                updateEmailTempate.EmailSubject = emailTemplate.EmailSubject;
                updateEmailTempate.ModifiedBy = userId;

                var result = await _emailTemplateRepository.UpdateAsync(updateEmailTempate);

                if (result.State.Equals(EntityState.Modified))
                {
                    await _emailTemplateRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("EmailTemplateBusiness", "Update", ex.Message, ex.StackTrace); 
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to delete the email template details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="emailTemplateId">emailTemplateId</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Delete(string userName, int emailTemplateId)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {
				userId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value)).UserId;

				var deleteEmailTemplate = await _emailTemplateRepository.SelectFirstOrDefaultAsync(e => e.EmailTemplateId == emailTemplateId);
                deleteEmailTemplate.ModifiedBy = userId;

                if (deleteEmailTemplate == null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("EmailTemplate")} {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                var result = await _emailTemplateRepository.DeleteAsync(deleteEmailTemplate);

                if (result.State.Equals(EntityState.Deleted))
                {
                    await _emailTemplateRepository.UnitOfWork.SaveChangesAsync();
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
                _logger.Error("EmailTemplateBusiness", "Delete", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
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
                _emailTemplateRepository.Dispose();
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

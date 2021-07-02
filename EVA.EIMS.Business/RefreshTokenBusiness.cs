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
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
    public class RefreshTokenBusiness : IRefreshTokenBusiness
    {
        #region Private Variable
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly IOptions<ApplicationSettings> _applicationSettings;
        private bool _disposed;
        #endregion

        #region Constructor
        public RefreshTokenBusiness(IRefreshTokenRepository refreshTokenRepository, IServiceProvider serviceProvider, ILogger logger, IOptions<ApplicationSettings> applicationSettings)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _disposed = false;
            _applicationSettings = applicationSettings;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// This method is used whether refreshtoken with id of provided token's id exists or not
        /// </summary>
        /// <param name="refreshToken">RefreshToken</param>
        /// <returns>returns true if token with provided token's id exist else returns false</returns>
        public async Task<ReturnResult> Get(Guid refreshTokenId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                returnResult.Success = true;
                returnResult.Data = await _refreshTokenRepository.SelectFirstOrDefaultAsync(r => r.RefreshTokenId.Equals(refreshTokenId));
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("RefreshTokenBusiness", "Get", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                return returnResult;
            }
        }

        /// <summary>
        /// Fetch refresh token count client wise
        /// </summary>
        /// <param name="refreshToken">RefreshToken</param>
        /// <returns>returns true if token with provided token's id exist else returns false</returns>
        public async Task<ReturnResult> GetClientwiseTokenCount()
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IExecuterStoreProc<RefreshTokenCountModel> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<RefreshTokenCountModel>>();
                var tokenCount = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetClientwiseTokenCount.ToString());
                returnResult.Success = true;
                returnResult.Data = tokenCount;
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("RefreshTokenBusiness", "Get", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to save the RefreshToken details
        /// </summary>
        /// <param name="refreshToken">RefreshToken object</param>
        /// <returns>returns response  message</returns>
        public async Task<ReturnResult> Save(RefreshToken refreshToken)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {

                var result = await _refreshTokenRepository.AddAsync(refreshToken);
                if (result.State.Equals(EntityState.Added))
                {
                    await _refreshTokenRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("RefreshTokenBusiness", "Save", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to delete the RefreshToken details
        /// </summary>
        /// <param name="refreshTokenId">refreshTokenId</param>
        /// <returns>returns response  message</returns>
        public async Task<ReturnResult> Delete(Guid refreshTokenId)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {

                var resultReturn = await _refreshTokenRepository.ExecuteQueryAsync(ProcedureConstants.procDeleteRefreshTokenById.ToString() + " '" + refreshTokenId.ToString() + "'");

                if (resultReturn != 0)
                {
                    await _refreshTokenRepository.UnitOfWork.SaveChangesAsync();
                    returnResult.Success = true;
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
                _logger.Error("RefreshTokenBusiness", "Delete", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to Clear RefreshToken Data
        /// </summary>
        /// <returns>returns response  message</returns>
        public async Task<ReturnResult> DeleteExpiredRefreshTokens()
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                int deleteCount = await _refreshTokenRepository.ExecuteQueryAsync(ProcedureConstants.procClearRefreshTokenData);
                returnResult.Success = true;
                returnResult.Result = ResourceInformation.GetResValue("DataDeleteSuccess");
                _logger.Info("RefreshTokenBusiness", "DeleteExpiredRefreshTokens", "Record deleted count = " + deleteCount, "");
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("RefreshTokenBusiness", "DeleteExpiredRefreshTokens", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// Send email notification when token request for specific clients exceeds allocated threshold requests
        /// </summary>
        /// <returns>true or false</returns>
        public async Task SendTokenRequestCountEmail(TokenData tokenData)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (tokenData != null)
                {
                    var emailSubject = ResourceInformation.GetResValue("TokenRequestCountEmail");
                    var emailFlag = new ReturnResult();

                    var emails = new List<EmailAddress>();
                    IEmailSender _mailSender = _serviceProvider.GetRequiredService<IEmailSender>();
                    IExecuterStoreProc<RefreshTokenCountModel> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<RefreshTokenCountModel>>();
                    var tokenCount = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetClientwiseTokenCount.ToString());

                    if (tokenCount != null && tokenCount.Count() != 0)
                    {
                        StringBuilder emailBody = new StringBuilder();
                        try
                        {
                            emailBody.Append("<html><head>");
                            emailBody.Append("<style type='text/css'>.tg{border-collapse:collapse;border-spacing:0;}.tg td{border-color:black;border-style:solid;border-width:1px;font-family:Arial, sans-serif;font-size:14px; overflow:hidden;padding:10px 5px;word-break:normal;}.tg th{border-color:black;border-style:solid;border-width:1px;font-family:Arial, sans-serif;font-size:14px; font-weight:normal;overflow:hidden;padding:10px 5px;word-break:normal;}.tg .tg-0lax{text-align:left;vertical-align:top}</style>");
                            emailBody.Append("</head><body>");
                            emailBody.Append("<div style='font-size:14px;'> <p>Hi,</p><p>Greetings from EVA support.</p><p>This is to inform you that excessive requests being sent by below clients. To avoid deactivation of below clients, please stop respective services.</p></div>");

                            emailBody.Append("<div style='font-size: 13px;'>");
                            emailBody.Append("<table class='tg'>");
                            emailBody.Append("<thead><tr><th class='tg-0lax'>Client Id</th><th class='tg-0lax'>Request Threshold</th><th class='tg-0lax'>Token Request Count</th></tr></thead>");
                            emailBody.Append("<tbody>");
                            foreach (var item in tokenCount)
                            {
                                emailBody.Append("<tr>");
                                emailBody.Append("<td class='tg-0lax'>" + item.ClientId + "</td>");
                                emailBody.Append("<td class='tg-0lax'>" + Convert.ToString(item.RequestThreshold) + "</td>");
                                emailBody.Append("<td class='tg-0lax'>" + Convert.ToString(item.TokenCount) + "</td>");
                                emailBody.Append("</tr>");
                            }
                            emailBody.Append("</tbody></table></div>");

                            emailBody.Append("<div style='font-size:14px;'><p>If you have any questions please contact our Support team at <a href='mailto:BTS-DS-IEC-DEVOPS@eva.com'>BTS-DS-IEC-DEVOPS@eva.com</a></p><p>&nbsp;</p><p><strong>Note:</strong> This is auto generated Email. Do not reply to it.</p></div>");
                            emailBody.Append("<div style='font-size:14px;'> <p>Thank you.</p></div>");
                            emailBody.Append("</body></html>");

                            var tomail = _applicationSettings.Value.NotifyTokenCountEmails.Split(';')?.Select(e => new EmailAddress() { Email = e }).ToList();

                            if (tomail != null && tomail.Count() > 0)
                                emailFlag = await _mailSender.SendEmail(tomail, ApplicationLevelConstants.FromEmailID, emailSubject, emailBody.ToString());
                            if (!emailFlag.Success)
                                _logger.Info("RefreshTokenBusiness", "SendTokenRequestCountEmail", "Email not sent to" + emailFlag.Data, "Mail failed at" + DateTime.Now);
                        }
                        finally
                        {
                            emailBody.Clear();
                        }
                    }
                    else
                    {
                        _logger.Info("RefreshTokenBusiness", "SendTokenRequestCountEmail", "No emails to send", "No emails sent" + DateTime.Now);
                    }
                }
                else
                {
                    _logger.Info("RefreshTokenBusiness", "SendTokenRequestCountEmail", "Invalid Token", "Mail failed at" + DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("RefreshTokenBusiness", "SendTokenRequestCountEmail", ex.Message, ex.StackTrace);
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
                _refreshTokenRepository.Dispose();
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

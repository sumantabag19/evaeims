using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using EVA.EIMS.Entity.ComplexEntities;
using System.Web;
using System.Threading.Tasks;
using EVA.EIMS.Logging;

namespace EVA.EIMS.Common
{
    public interface IEmailSender
    {
        Task<ReturnResult> SendEmail(string toEmail, string fromEmail, string subject, string body);
        Task<ReturnResult> SendEmail(List<EmailAddress> toEmail, string fromEmail, string subject, string body);
        string ComposeOTPMailBody(string userName, string otp);
        string ComposePwdExpMailBody(string userName);
        string ComposePwdExpMailBodyCustomize(string userName,string name,DateTime expdateTime,string EnvUrl);
    }
    /// <summary>
    /// Generic class to send email
    /// </summary>
    public class EmailSender : SendGridMessage, IEmailSender
    {
        #region Private Variable
        // Define supported password characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private readonly IOptions<ApplicationSettings> _applicationSettings;
        private readonly ILogger _logger;
        #endregion
        
        #region Constructor

        public EmailSender(IOptions<ApplicationSettings> applicationSettings, ILogger logger)
        {
            _applicationSettings = applicationSettings;
            _logger = logger;
        }
        #endregion

        #region Public Method
        /// <summary>
        /// This methos is used to send email
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="fromEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<ReturnResult> SendEmail(string toEmail, string fromEmail, string subject, string body)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                string plainText = subject;
                var apiKey = _applicationSettings.Value.EmailPassword;
                var client = new SendGridClient(apiKey);

                var fromAddress = new EmailAddress(fromEmail);
                var toAddress = new EmailAddress(toEmail);
                var msg = MailHelper.CreateSingleEmail(fromAddress, toAddress, subject, plainText, body);
                
                var response = await client.SendEmailAsync(msg);
                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    returnResult.Success = true;
                    returnResult.Data = toAddress;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    return returnResult;
                }
            }

            catch (Exception ex)
            {
                _logger.Error("EmailSender", "SendEmail", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                return returnResult;
            }
        }

        public async Task<ReturnResult> SendEmail(List<EmailAddress> toEmail, string fromEmail, string subject, string body)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                string plainText = subject;
                var apiKey = _applicationSettings.Value.EmailPassword;
                var client = new SendGridClient(apiKey);

                var fromAddress = new EmailAddress(fromEmail);
                //var toAddress = new EmailAddress(toEmail);
                var msg = MailHelper.CreateSingleEmailToMultipleRecipients(fromAddress, toEmail, subject, plainText, body);

                var response = await client.SendEmailAsync(msg);
                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    returnResult.Success = true;
                    returnResult.Data = toEmail;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    return returnResult;
                }
            }

            catch (Exception ex)
            {
                _logger.Error("EmailSender", "SendEmail", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                return returnResult;
            }
        }

        /// <summary>
        /// Compose email body for OTP
        /// </summary>
        /// <param name="otp"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string ComposeOTPMailBody(string userName, string otp)
        {
            string emailBody = string.Empty;
            emailBody = "<html><head></head><body><div style=\"font-size:13px; font-family:Calibri;\"><p style=\"line-height:10px;\">Dear<b> " + userName + "</b>,</p><p style=\"line-height:10px;\"> Your One Time Password for Password Recovery is:" + otp + "</p><p style=\"line-height:10px;\">Expire time of One Time Password: <b>15 minutes.</b></p><p style=\"line-height:10px;\"></br></br></br></br></br>This is auto generated email, please do not reply.</p><p style=\"line-height:10px;\">Thank You</p></div></body></html>";

            return emailBody;
        }

        /// <summary>
        /// Compose email body for Password Expiration
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string ComposePwdExpMailBody(string userName)
        {
            string emailBody = string.Empty;
            emailBody = "<html><head></head><body><div style=\"font-size:13px; font-family:Calibri;\"><p style=\"line-height:10px;\">Hi,</p><p style=\"line-height:10px;\">The password for the account, <b>" + userName +"</b>  is about to expire.</p><p style=\"line-height:10px;\">Please change your password before it expires otherwise you may not be able to access any application.</p><p style=\"line-height:10px;\">If you have changed your password after this email was delivered to your mailbox, please disregard this message.</p></div><div style=\"font-size:13px; font-family:Calibri;\"><p style=\"line-height:10px;\"></br></br></br></br></br>This is auto generated Email. Do not reply to it.</p><p style=\"line-height:10px;\">Thank you.</p></div></body></html>";

            return emailBody;
        }

        public string ComposePwdExpMailBodyCustomize(string userName, string name, DateTime expdateTime,string EnvUrl)
        {
            string emailBody = string.Empty;
            int days = (expdateTime - DateTime.UtcNow).Days;
            emailBody = "<div style=\"0font-size: 13px;\"><p>Hello&nbsp;<strong>" + name + "</strong>,</p><p><strong>Your password associated to User Name \""+userName+"\" will expire in " + days + " days.</strong></p><p>To avoid being locked out of your account, please change the password as soon as possible.</p><p>To change your password, follow the method below:&nbsp;</p><ol><li>Login to EVA Application <a href=\""+ EnvUrl + "\">"+ EnvUrl + "</a></li><li>Click on User Name located at upper right corner of the screen</li><li>Select Change Password</li><li>Fill in your old password and set a new password</li><li>Click on Change Password</li></ol><p>If you have any questions please contact our Support team at <a href=\"mailto:BTS-DS-IEC-DEVOPS@eva.com\">BTS-DS-IEC-DEVOPS@eva.com</a></p><p>&nbsp;</p><p><strong>Note:</strong> This is auto generated Email. Do not reply to it.</p><p>Thank you.</p></div>";
            return emailBody;
        }

        #region Sending mails through bcc
        //public bool SendMultipleEmailAsync(List<EmailAddress> toEmail, string fromEmail, string subject, string body)
        //{
        //    try
        //    {
        //        var apiKey = _applicationSettings.Value.EmailPassword;
        //        var userName = _applicationSettings.Value.EmailUserName;
        //        var client = new SendGridClient(apiKey);

        //        List<Personalization> personal = new List<Personalization>();

        //        var msg = new SendGridMessage();

        //        var personalization = new Personalization();
        //        personalization.Subject = subject;
        //        personalization.Tos = toEmail;
        //        personalization.Bccs = toEmail;
        //        personalization.Tos = toEmail;
        //        personal.Add(personalization);
        //        msg.HtmlContent = body;

        //        var response = client.SendEmailAsync(msg);
        //        return true;

        //        //var response = client.SendEmailAsync(msg);
        //        //return true;
        //        //var msg = new SendGridMessage();
        //        //msg.From = new EmailAddress (fromEmail) ;
        //        //msg.HtmlContent = body;
        //        ////msg.AddBccs(toEmail, 0, null);
        //        //msg.SetBypassListManagement(true);
        //        //msg.SetBccSetting(true, fromEmail);
        //        //msg.AddSubstitution("%name1%", "Example User1");
        //        //msg.Subject = subject;
        //        //var personalization = new Personalization()
        //        //{
        //        //    Bccs = toEmail,

        //        //};
        //        ////msg.AddBccs(toEmail);
        //        //msg.AddBccs(toEmail, 0, personalization);

        //        //msg.Serialize();
        //        ////var msgs = MailHelper.CreateSingleEmailToMultipleRecipients(msg.From, , subject, null, body);
        //        //var response = client.SendEmailAsync(msg);
        //        //return true;
        //    }

        //    catch (Exception ex)
        //    {
        //        _logger.Log(LogType.ERROR, "SendEmail", "SendEmailToVerifyUserEmail", ex.Message, ex.StackTrace);
        //        return false;
        //    }
        //}
        #endregion


        #endregion
    }
}

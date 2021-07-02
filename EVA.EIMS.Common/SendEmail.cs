using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EVA.EIMS.Common
{
    public class SendEmail
    {
        private readonly IOptions<ApplicationSettings> _applicationSettings;

        public SendEmail()
        {
        }

        public SendEmail(IOptions<ApplicationSettings> applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }
        public async Task<bool> SendRequestAccessNotification(List<User> listOfSiteAdmin, string username, string emailid)
        {
            bool result = false;
            try
            {
                MailEntity mailEntity = new MailEntity();
                StringBuilder emailMsgBody = new StringBuilder();
                emailMsgBody.Append("<html><head></head><body><div style=");
                emailMsgBody.Append("font - size:13px; font - family:Calibri; ");
                emailMsgBody.Append("><p style=line - height:10px; >");
                emailMsgBody.Append("Hi, </p>");
                emailMsgBody.Append("<p style=line - height:10px;>");
                emailMsgBody.Append("The user " + username + "   with email ID : " + emailid + " has submitted a request to create user as Active Directory user in EVA. Please activate this user in the setup -> User -> Inactive Active Directory user by selecting the user with Login type as “Active Directory” & clicking submit button. </ p >");
                emailMsgBody.Append("<p style = line - height:10px; >Thanks, </ p ><p style = line - height:10px; >EVA Management.</ p ></ div > ");
                emailMsgBody.Append("<p style = line - height:7px; > This is an automated email. Please don’t reply to this email.</ p ></ body ></ html > ");

                mailEntity.Body = emailMsgBody.ToString();
                mailEntity.MailFrom = EmailConstants.mailId;
                List<EmailAddress> emailTos = new List<EmailAddress>();
                foreach (var siteAdmin in listOfSiteAdmin)
                {
                    emailTos.Add(new EmailAddress(siteAdmin.EmailId.ToString().Trim(), siteAdmin.UserName.ToString().Trim()));
                }


                mailEntity.Subject = "User creation request for active directory in EVA requested for user " + username;
                result = await SendMail(mailEntity, emailTos);
            }
            catch (Exception ex)
            {

                return false;
            }
            return result;
        }

        public async Task<bool> SendUserRegistrationNotification(MailEntity mailEntity)
        {
            try
            {
                var client = new System.Net.Mail.SmtpClient(_applicationSettings.Value.EmailConfigurationServer, Convert.ToInt32(_applicationSettings.Value.EmailConfigurationPort))
                {
                    Credentials = new NetworkCredential(_applicationSettings.Value.EmailUserName, _applicationSettings.Value.EmailPassword),
                    EnableSsl = true
                };
                await client.SendMailAsync(mailEntity.MailFrom, mailEntity.MailTo, mailEntity.Subject, mailEntity.Body);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> SendMail(MailEntity mailEntity, List<EmailAddress> emailAddressesTo)
        {
            var apiKey = "";
            var client = new SendGridClient(apiKey);
            try
            {
                var msg1 = MailHelper.CreateSingleEmailToMultipleRecipients(new EmailAddress(mailEntity.MailFrom.ToString().Trim()), emailAddressesTo, mailEntity.Subject, "Email Notification", mailEntity.Body);
                var response = await client.SendEmailAsync(msg1);
                var status = response.StatusCode;
                if (status == HttpStatusCode.Accepted)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

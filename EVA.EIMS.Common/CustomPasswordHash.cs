using EVA.EIMS.Helper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sodium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace EVA.EIMS.Common
{
    #region ICustomPasswordHash Interface
    public interface ICustomPasswordHash
    {
        /// <summary>
        /// Method to generate hash 
        /// </summary>
        /// <param name="plainText">plainText</param>
        /// <returns>returns string</returns>
        string ScryptHash(string plainText);

        bool ScryptHashStringVerify(string hash, string clearTextPassword);

        /// <summary>
        /// This method is used to entrypt the password
        /// </summary>
        /// <param name="clearTextPassword">cleartext password</param>
        /// <param name="subject">subject</param>
        /// <returns>reruns encrypted  string value</returns>
        string EncryptAndPHash(string clearTextPassword, string subject);

        bool EncryptAndScryptHashStringVerify(string hash, string clearTextPassword, string subject);

        /// <summary>
        /// This method is used to entrypt the password
        /// </summary>
        /// <param name="clearTextPassword">cleartext password</param>
        /// <param name="subject">subject</param>
        /// <returns>reruns encrypted  string value</returns>
        string Encrypt(string clearTextPassword, string subject);

        /// <summary>
        /// This method is used decrypt the password
        /// </summary>
        /// <param name="pHashEncryptText">pHashEncryptText</param>
        /// <param name="subject">subject</param>
        /// <returns></returns>
        string Decrypt(string pHashEncryptText, string subject);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pHashEncryptText"></param>
        /// <param name="id"></param>
        /// <param name="subject"></param>
        /// <returns>returns encrypted string</returns>
        string DecryptEncryptAndPHash(string pHashEncryptText, string id, string subject);

        /// <summary>
        /// Method to generate client secret
        /// </summary>
        /// <returns>returns secret key </returns>
        string CreateClientSecret();

        /// <summary>
        /// This method is used to validate the password
        /// </summary>
        /// <param name="password">password</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>returns error message</returns>
        bool ValidatePassword(string password, out string errorMessage);

        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        string[] GenerateRandomPassword(string subject);

        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        string GenerateRandomPasswordForDeviceUser(string subject);

        /// <summary>
        /// This method is used to send the  mail 
        /// </summary>
        /// <param name="newPwd">new password</param>
        /// <param name="emailFrom">From emailId</param>
        /// <param name="emailTo">To emailId </param>
        /// <param name="emailSubject">Email Subject</param>
        /// <param name="emailBody">Email body</param>
        /// <param name="emailConfidentialMsg">confidential message</param>
        /// <param name="emailFooter">Email footer message</param>
        void SendEmail(string newPwd, string emailFrom, string emailTo, string emailSubject, string emailBody, string emailConfidentialMsg, string emailFooter);
    }
    #endregion

    #region CustomPasswordHash Class
    public class CustomPasswordHash : ICustomPasswordHash,IDisposable
    {
        #region Private Variable
        // Define supported password characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private readonly IOptions<ApplicationSettings> _applicationSettings;
        private string _minPwdLenth = string.Empty;
        private string _maxPwdLength = string.Empty;
        #endregion

        #region Constructor
        public CustomPasswordHash()
        {

        }
        public CustomPasswordHash(IOptions<ApplicationSettings> applicationSettings)
        {
            _applicationSettings = applicationSettings;
            _minPwdLenth = _applicationSettings.Value.minPwdLenth;
            _maxPwdLength = _applicationSettings.Value.maxPwdLenth;

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to generate hash 
        /// </summary>
        /// <param name="plainText">plainText</param>
        /// <returns>returns string</returns>
        public string ScryptHash(string plainText)
        {
            //this will produce a 32 byte hash
            return PasswordHash.ScryptHashString(plainText, PasswordHash.Strength.Medium);
        }

        public bool ScryptHashStringVerify(string hash, string clearTextPassword)
        {
            return PasswordHash.ScryptHashStringVerify(hash, clearTextPassword);
        }

        /// <summary>
        /// This method is used to entrypt the password
        /// </summary>
        /// <param name="clearTextPassword">cleartext password</param>
        /// <param name="subject">subject</param>
        /// <returns>reruns encrypted  string value</returns>
        public string EncryptAndPHash(string clearTextPassword, string subject)
        {
            var result = Encrypt(clearTextPassword, subject);
            return ScryptHash(result);
        }

        public bool EncryptAndScryptHashStringVerify(string hash, string clearTextPassword, string subject)
        {
            var encryptText = Encrypt(clearTextPassword, subject);
            return PasswordHash.ScryptHashStringVerify(hash, encryptText);
        }

        /// <summary>
        /// This method is used to entrypt the password
        /// </summary>
        /// <param name="clearTextPassword">cleartext password</param>
        /// <param name="subject">subject</param>
        /// <returns>reruns encrypted  string value</returns>
        public string Encrypt(string clearTextPassword, string subject)
        {
            try
            {
                var splitSubject = subject.ToLower().Split('-');
                if (string.IsNullOrEmpty(clearTextPassword) || string.IsNullOrEmpty(subject)) return string.Empty;

                string encryptionKey = $"{splitSubject[1]}{splitSubject[2]}{splitSubject[3]}";

                var clearBytes = Encoding.Unicode.GetBytes(splitSubject.First() + "-" + clearTextPassword + "-" + splitSubject.Last());
                var result = string.Empty;
                using (var encryptor = Aes.Create())
                {
                    var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    if (encryptor != null)
                    {
                        encryptor.Key = pdb.GetBytes(32);
                        encryptor.IV = pdb.GetBytes(16);
                        using (var ms = new MemoryStream())
                        {
                            using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(clearBytes, 0, clearBytes.Length);
                                cs.Close();
                            }
                            result = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
                return result;
            }
            catch(Exception)
            {
                throw new System.ArgumentException("Input Value", clearTextPassword +":"+ subject);
            }
        }

        /// <summary>
        /// This method is used decrypt the password
        /// </summary>
        /// <param name="pHashEncryptText">pHashEncryptText</param>
        /// <param name="subject">subject</param>
        /// <returns></returns>
        public string Decrypt(string pHashEncryptText, string subject)
        {
            var splitSubject = subject.ToLower().Split('-');
            if (string.IsNullOrEmpty(pHashEncryptText) || string.IsNullOrEmpty(subject)) return string.Empty;

            string encryptionKey = $"{splitSubject[1]}{splitSubject[2]}{splitSubject[3]}";

            var cipherBytes = Convert.FromBase64String(pHashEncryptText);
            var result = string.Empty;
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                if (encryptor != null)
                {
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        result = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
            }
            result = result.Replace(splitSubject.First() + "-", "");
            result = result.Replace("-" + splitSubject.Last(), "");

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pHashEncryptText"></param>
        /// <param name="id"></param>
        /// <param name="subject"></param>
        /// <returns>returns encrypted string</returns>
        public string DecryptEncryptAndPHash(string pHashEncryptText, string id, string subject)
        {
            #region Decrypt using Id

            var splitId = id.ToLower().Split('-');
            if (string.IsNullOrEmpty(pHashEncryptText) || string.IsNullOrEmpty(id) || string.IsNullOrEmpty(subject)) return string.Empty;

            string encryptionKey = $"{splitId[1]}{splitId[2]}{splitId[3]}";

            var cipherBytes = Convert.FromBase64String(pHashEncryptText);
            var plainText = string.Empty;
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                if (encryptor != null)
                {
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        plainText = Encoding.Unicode.GetString(ms.ToArray()).Split('-')[1];
                    }
                }
            }

            #endregion

            #region Encrypt using subject

            var splitSubject = subject.ToLower().Split('-');
            if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(subject)) return string.Empty;

            encryptionKey = $"{splitSubject[1]}{splitSubject[2]}{splitSubject[3]}";

            var clearBytes = Encoding.Unicode.GetBytes(splitSubject.First() + "-" + plainText + "-" + splitSubject.Last());
            var encryptResult = string.Empty;
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                if (encryptor != null)
                {
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        encryptResult = Convert.ToBase64String(ms.ToArray());
                    }
                }
            }

            #endregion

            return ScryptHash(encryptResult);
        }

        /// <summary>
        /// Method to generate client secret
        /// </summary>
        /// <returns>returns secret key </returns>
        public string CreateClientSecret()
        {
            var key = new byte[32];
            RNGCryptoServiceProvider.Create().GetBytes(key);
            //var base64Secret = TextEncodings.Base64Url.Encode(key);
            var base64Secret = Base64UrlEncoder.Encode(key);
            return base64Secret;
        }

        /// <summary>
        /// This method is used to validate the password
        /// </summary>
        /// <param name="password">password</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>returns error message</returns>
        public bool ValidatePassword(string password, out string errorMessage)
        {
            var input = password;
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                errorMessage = $"{ResourceInformation.GetResValue("PwdEmpty")}";
                return false;
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            //var hasMiniMaxChars = new Regex(@".{8,16}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
            {
                errorMessage = $"{ResourceInformation.GetResValue("PwdLoweCase")}";
                return false;
            }
            else if (!hasUpperChar.IsMatch(input))
            {
                errorMessage = $"{ResourceInformation.GetResValue("PwdUpperCase")}";
                return false;
            }
            else if (input.Length < int.Parse(_minPwdLenth))
            {
                errorMessage = $"{ResourceInformation.GetResValue("PwdMin")} {_minPwdLenth} {ResourceInformation.GetResValue("Characters")}";
                return false;
            }
            else if (input.Length > int.Parse(_maxPwdLength))
            {
                errorMessage = $"{ResourceInformation.GetResValue("PwdMax")} {_maxPwdLength} {ResourceInformation.GetResValue("Characters")}";
                return false;
            }

            else if (!hasNumber.IsMatch(input))
            {
                errorMessage = $"{ResourceInformation.GetResValue("PwdNumericCase")}";
                return false;
            }

            else if (!hasSymbols.IsMatch(input))
            {
                errorMessage = "Password should contain at least one special case characters";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        public string[] GenerateRandomPassword(string subject)
        {
            var pwdDetails = new string[2];
            var randomPwd = Generate(int.Parse(_minPwdLenth), int.Parse(_maxPwdLength));
            var newPwd = ScryptHash(randomPwd);// EncryptAndPHash(randomPwd, subject);
            pwdDetails[0] = randomPwd;
            pwdDetails[1] = newPwd;
            return pwdDetails;
        }

        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        public string GenerateRandomPasswordForDeviceUser(string subject)
        {
            var randomPwd = Generate(int.Parse(_minPwdLenth), int.Parse(_maxPwdLength));
            var newPwd = Encrypt(randomPwd, subject);
            return newPwd;
        }

        /// <summary>
        /// This method is used to send the  mail 
        /// </summary>
        /// <param name="newPwd">new password</param>
        /// <param name="emailFrom">From emailId</param>
        /// <param name="emailTo">To emailId </param>
        /// <param name="emailSubject">Email Subject</param>
        /// <param name="emailBody">Email body</param>
        /// <param name="emailConfidentialMsg">confidential message</param>
        /// <param name="emailFooter">Email footer message</param>
        public void SendEmail(string newPwd, string emailFrom, string emailTo, string emailSubject, string emailBody, string emailConfidentialMsg, string emailFooter)
        {
            var htmlBody = string.Empty;
            var mailmassage = new MailMessage();
            htmlBody += "</br><p style ='background-color:#D4E2F4;'>" + "<span style='font-family:\"Calibri\";font-size:14.0pt;'>" + emailBody + " " + newPwd + " </span></p>";
            htmlBody += "<p style ='background-color: rgb(250, 250, 250);'>" + "<span style='font-family:\"Calibri\";font-size:9.0pt;'>" + emailConfidentialMsg + "</br>&nbsp; " + "</span></p>";
            htmlBody += "<p style ='text-align:center'>" + "<span style='font-family:\"Calibri\";font-size:10.0pt;'>©" + DateTime.Now.Year.ToString() + emailFooter + "</span></p>";
            htmlBody += "</body></html>";
            var avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            mailmassage.AlternateViews.Add(avHtml);
            SendingEmail(emailFrom, emailTo, emailSubject, htmlBody, mailmassage);
        }

        public void SendingEmail(string emailFrom, string emailTo, string emailSubject, string emailBody, MailMessage mailMessage)
        {
            if (string.IsNullOrEmpty(emailTo))
            {
                emailTo = emailFrom;
            }
            else
            {
                var emailIDs = emailTo.Split(new char[] { ',' }).ToList().Distinct().ToList();
                emailTo = string.Join(",", emailIDs);
            }

            var fromAddress = new MailAddress(emailFrom);
            mailMessage.From = fromAddress;
            mailMessage.To.Add(emailTo);
            mailMessage.Subject = emailSubject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = emailBody;
            var emailSmtpServer = _applicationSettings.Value.EmailConfigurationServer;
            var emailSmtpPort = Convert.ToInt32(_applicationSettings.Value.EmailConfigurationPort);
            var userName = _applicationSettings.Value.EmailUserName;
            var pwd = _applicationSettings.Value.EmailPassword;

            var client = new SmtpClient(emailSmtpServer, emailSmtpPort)
            {
                Credentials = new NetworkCredential(userName, pwd),
                EnableSsl = Convert.ToBoolean(_applicationSettings.Value.EmailConfigurationEnableSSL)
            };
            client.Send(mailMessage);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <param name="minLength">
        /// Minimum password length.
        /// </param>
        /// <param name="maxLength">
        /// Maximum password length.
        /// </param>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        private string Generate(int minLength, int maxLength)
        {
            // Make sure that input parameters are valid.
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                return null;

            // Create a local array containing supported password characters
            // grouped by types. You can remove character groups from this
            // array, but doing so will weaken the password strength.
            var charGroups = new[]
            {
                PasswordConstants.CharactorsLowerCase.ToCharArray(),
                PasswordConstants.CharactorsUpperCase.ToCharArray(),
                PasswordConstants.CharactorsNumeric.ToCharArray(),
                PasswordConstants.CharactorsSpecial.ToCharArray()
            };

            // Use this array to track the number of unused characters in each
            // character group.
            var charsLeftInGroup = new int[charGroups.Length];

            // Initially, all characters in each group are not used.
            for (var i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            // Use this array to track (iterate through) unused character groups.
            var leftGroupsOrder = new int[charGroups.Length];

            // Initially, all character groups are not used.
            for (var i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            var randomBytes = new byte[4];

            // Generate 4 random bytes.
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert 4 bytes into a 32-bit integer value.
            var seed = BitConverter.ToInt32(randomBytes, 0);

            // Now, this is real randomization.
            var random = new Random(seed);

            // This array will hold password characters.

            // Allocate appropriate memory for the password.
            var password = minLength < maxLength ? new char[random.Next(minLength, maxLength + 1)] : new char[minLength];

            // Index of the last non-processed group.
            var lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            // Generate password characters one at a time.
            for (var i = 0; i < password.Length; i++)
            {
                // If only one character group remained unprocessed, process it;
                // otherwise, pick a random character group from the unprocessed
                // group list. To allow a special character to appear in the
                // first position, increment the second parameter of the Next
                // function call by one, i.e. lastLeftGroupsOrderIdx + 1.

                // Index which will be used to track not processed character groups.
                int nextLeftGroupsOrderIdx;
                if (lastLeftGroupsOrderIdx == 0)
                    nextLeftGroupsOrderIdx = 0;
                else
                    nextLeftGroupsOrderIdx = random.Next(0,
                                                         lastLeftGroupsOrderIdx);

                // Get the actual index of the character group, from which we will
                // pick the next character.
                // Index of the next character group to be processed.
                var nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                // Get the index of the last unprocessed characters in this group.
                // Index of the last non-processed character in a group.
                var lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                // If only one unprocessed character is left, pick it; otherwise,
                // get a random character from the unused character list.
                // Index of the next character to be added to password.
                var nextCharIdx = lastCharIdx == 0 ? 0 : random.Next(0, lastCharIdx + 1);

                // Add this character to the password.
                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                // If we processed the last character in this group, start over.
                if (lastCharIdx == 0)
                    charsLeftInGroup[nextGroupIdx] =
                                              charGroups[nextGroupIdx].Length;
                // There are more unprocessed characters left.
                else
                {
                    // Swap processed character with the last unprocessed character
                    // so that we don't pick it until we process all characters in
                    // this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        var temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                                    charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    // Decrement the number of unprocessed characters in
                    // this group.
                    charsLeftInGroup[nextGroupIdx]--;
                }

                // If we processed the last group, start all over.
                if (lastLeftGroupsOrderIdx == 0)
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                // There are more unprocessed groups left.
                else
                {
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        var temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                    leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    // Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx--;
                }
            }

            // Convert password characters into a string and return the result.
            return new string(password);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    #endregion
}

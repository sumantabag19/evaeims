//using EVA.EIMS.Common;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using Microsoft.Extensions.Options;
using System;
using Sodium;
using EVA.EIMS.Logging;

namespace CustomPasswordHashCheck
{
    public class CustomPasswordCheck :  IDisposable
    {
        private readonly IOptions<ApplicationSettings> _applicationSettings;
        private readonly ILogger _logger;
        public CustomPasswordCheck(IOptions<ApplicationSettings> applicationSettings, ILogger logger)
        {
            _applicationSettings = applicationSettings;
            _logger = logger;
        }

        /// <summary>
        /// To check password hash with plain password
        /// </summary>
        /// <param name="hashpassword"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CustomPwdCheck(string hashpassword, string password)
        {
            try
            {
                object thisLock = new object();
                //  CustomPasswordHash customPasswordHash = new CustomPasswordHash(_applicationSettings);
                bool result;
                //  using(CustomPasswordHash customPasswordHash = new CustomPasswordHash(_applicationSettings))
                // {
                lock (thisLock)
                {
                     result = PasswordHash.ScryptHashStringVerify(hashpassword, password);
                }
                return result;
               // }

            }
            catch(Exception ex)
            {
                _logger.Error("CustomPasswordCheck", "CustomPwdCheck", ex.Message, ex.StackTrace);
                return false;
            }
           
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
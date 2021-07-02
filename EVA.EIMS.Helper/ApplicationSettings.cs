using System;

namespace EVA.EIMS.Helper
{
    public class ApplicationSettings
    {
        public string IMSEndPoint { get; set; }
        public string MstOrg { get; set; }
        public string MstClientTypeId { get; set; }
        public string minPwdLenth { get; set; }
        public string maxPwdLenth { get; set; }
        public string EmailConfigurationEnableSSL { get; set; }
        public string EmailConfigurationServer { get; set; }
        public string EmailConfigurationPort { get; set; }
        public string EmailUserName { get; set; }
        public string EmailPassword { get; set; }
        public int ForgotPasswordSession { get; set; }
        public LoggerStorageClass LoggerStorage { get; set; }
        public string UseDatabase { get; set; }
        public string UseLogging { get; set; }
        public int AccessTokenExpireTimeSpanInMinutes { get; set; }
        public int RefreshTokenExpireTimeSpanInMinutes { get; set; }
        public int RandomSecurityQuestion { get; set; }
        public int TotalSecurityQuestions { get; set; }
        public int PasswordCount { get; set; }
        public int NumberOfAttempts { get; set; }
        public int PasswordExpirationDays { get; set; }
        public bool EnableIPWhitelisting { get; set; }
        public string Eck { get; set; }
        public int OTPLength { get; set; }
        public int ClientValidationPeriod { get; set; }
        public AzurAdSettings AzurAdSettings { get; set; }
        public double ClientExpirationHour { get; set; }
        public int CodeExpire { get; set; }
        public string LocalRedirectBase { get; set; }
        public bool IsHttpsRequired { get; set; }
        public bool EnableTokenLogging { get; set; }
        public string SSOLocalUrl { get; set; }
        public string SSOReleaseUrl { get; set; }
        public string AllowedDomains { get; set; }
        public string NotifyTokenCountEmails { get; set; }
        public string PrivateKeyPath { get; set; }
    }
}

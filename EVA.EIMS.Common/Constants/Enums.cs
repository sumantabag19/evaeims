using System.ComponentModel;

namespace EVA.EIMS.Common.Constants
{
    /// <summary>
    /// Define Log Types
    /// </summary>
    public enum LogType1
    {
        INFO,
        WARNING,
        ERROR
    }

    public enum ClientTypes
    {
        [Description("SecurityApi.Client")]
        SecurityApiClient = 1,

        [Description("UI.Web.Client")]
        UiWebClient = 2,

        [Description("Device.Client")]
        DeviceClient = 3,

        [Description("Service.Client")]
        ServiceClient = 4,

        [Description("Hadoop.Client")]
        HadoopClient = 5,

        [Description("Kiosk.Client")]
        KioskClient = 6
    }

    public enum UserRoles
    {
        [Description("SuperAdmin")]
        SuperAdmin = 0,

        [Description("SiteAdmin")]
        SiteAdmin = 1,

        [Description("SiteUser")]
        SiteUser = 2,

        [Description("Support")]
        Support = 3,
    }
    public enum EIMSRoles
    {
        [Description("SuperAdmin")]
        SuperAdmin = 6,

        [Description("SiteAdmin")]
        SiteAdmin = 4,

        [Description("SiteUser")]
        SiteUser = 5,

        [Description("Support")]
        Support = 7,
    }


    public enum LockType
    {
        [Description("OTPLock")]
        OTPLock = 1,

        [Description("LoginLock")]
        LoginLock = 2,

        [Description("VerificationLock")]
        VerificationLock = 3,

        [Description("QuestionAnswerLock")]
        QuestionAnswerLock = 4,

        [Description("ResetPasswordLock")]
        ResetPasswordLock=5       
    }
    public enum OTPTypeEnum
    { 
        [Description("Admin Login")]
        AdminLogin = 1,
        [Description("Forgot Password")]
        ForgotPassword = 2,
        [Description("Mobile Login")]
        MobileLogin = 3

    }
}

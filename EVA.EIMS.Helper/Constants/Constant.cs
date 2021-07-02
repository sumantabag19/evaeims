using System;
using System.Collections.Generic;
using System.Text;

namespace EVA.EIMS.Helper.Constants
{
    /// <summary>
    /// Define Log Types
    /// </summary>
    public enum LogType
    {
        INFO,
        WARNING,
        ERROR
    }

    /// <summary>
    ///Application Level Define Constants 
    /// </summary>
    public static class ApplicationLevelConstants
    {
        public const string ApplicationName = "EIMS";
        public const string AzureAdGrant_Type = "password";
        public const string AzureAdScope = "openid";
        public const string AzureAdAuthProvider = "AzureAD";
        public const string IMSAuthProvider = "IMS";
        public const string FromEmailID = "info-eva@eva.com";
        public const string logtype = "ERROR";

    }
    public enum MonthTables
    {
        z_AuditsJan,
        z_AuditsFeb,
        z_AuditsMar,
        z_AuditsApr,
        z_AuditsMay,
        z_AuditsJun,
        z_AuditsJul,
        z_AuditsAug,
        z_AuditsSept,
        z_AuditsOct,
        z_AuditsNov,
        z_AuditsDec
    }
}

using System;

namespace EVA.EIMS.Common
{
    /// <summary>
    /// Define constatants for URLs
    /// </summary>
    public class URLConstants
    {
        #region IMSURL
        //public const string iMSEndPointAPIUrl = "https://eimsd-api.azurewebsites.net/api/claims";
        #endregion
    }

    /// <summary>
    /// Define constants for messages
    /// </summary>
    public class MessageConstants
    {
        #region Log Message
        public const string StartMethodExecution = "--- Start Execution Of {0} ---";
        public const string EndMethodExecution = "--- End Execution Of {0} ---";
        #endregion

        #region Exception Messages
        public const string UnauthorizedAccessException = "You are not an authorized user, Please contact super admin";
        public const string InvalidAccessTokenException = "Invalid Token Details";
        #endregion

        #region Product Messages
        public const string AddedSuccessfully = " Product Added Successfully";
        public const string ModifiedSuccessfully = " Product Modified Successfully";
        public const string DeletedSuccessfully = " Product Deleted successfully";
        #endregion

        #region ExceptionType
        public const string UnauthorizedAccess = "UnauthorizedAccess";
        public const string InvalidClient = "InvalidClient";
        public const string AccountLocked = "AccountLocked";
        public const string InvalidAccessToken = "InvalidAccessToken";
        public const string InternalServerError = "InternalServerError";
        #endregion
    }

    /// <summary>
    /// Define Constants for keys
    /// </summary>
    public static class KeyConstant
    {
        #region Token Details
        public const string ImsClaim = "ImsClaim";
        public const string OrgId = "org";
        public const string Client_Type = "client_type";
        public const string UserName = "username";
        public const string Role = "role";
        public const string AppName = "app";
        public const string RefreshToken = "refresh_token";
        public const string Subid = "subid";
        public const string AppUserId = "auid";
        public const string AppOrgId = "aoid";
        public const string Sub = "sub";
        public const string Client = "Client";
        public const string GrantType = "grant_type";
        public const string Passwordexpireat = "passwordexpireat";
        public const string RemainingDays = "passwordexpireindays";
        public const string Audience = "audience";
        public const string AudienceKey = "audienceKey";
        public const string JWT = "JWT";
        public const string ClientId = "client_id";
        public const string ClientExpireOn = "client_expireon";
        public const string Aud = "aud";
        public const string IsMultiOrg = "is_multiorg";
        public const string TokenValidationPeriod = "tokenValidationPeriod";
        public const string sts = "ClientSecret";
        public const string AuthCode = "authorization_code";
        public const string LocalRedirect = "/api/oauth/auth";
        public const string LocalHost = "http://localhost:";
        public const string IsRefreshTokenFlow = "IsRefreshTokenFlow";
        public const string InvalidUserCode = "wHjhhjsd54GFGD84545DGDFGkjhsjf%DsffgDFGFGHGFHdsfsdfjkkjjfnsd4343DFGD32432jbjbdmvmxvZJorazNF4bec%Hjhhjsd54GFGD84545DGDFGkjhsjf%DsffgDFGFGHGFHdsfsdfjkkjjfnsd4343DFGD32432jbjbdmvmxv5BAvaCHYs2OoCxHjhhjsd54GFGD84545DGDFGkjhsjf%DsffgDFGFGHGFHdsfsdfjkkjjfnsd4343DFGD32432jbjbdmvmxvAopxcX9%2B9T12r5zq9cbyVXvgMGD0jgRRcizWHjhhjsd54GFGD84545DGDFGkjhsjf%DsffgDFGFGHGFHdsfsdfjkkjjfnsd4343DFGD32432jbjbdmvmxv834sQwyzSwchbrUCSC4O7fblF4dtYrHjhhjsd54GFGD84545DGDFGkjhsjf%DsffgDFGFGHGFHdsfsdfjkkjjfnsd4343DFGD32432jbjbdmvmxvYH98o3IOhAEonMeoOFG8bf0j%2F5ckHjhhjsd54GFGD84545DGDFGkjhsjf%DsffgDFGFGHGFHdsfsdfjkkjjfnsd4343DFGD32432jbjbdmvmxvjZBfy24GK0TzhKm%2BnE2ZYmPOv85QacBflDm%2Bd63YHjhhjsd54GFGD84545DGDFGkjhsjf%DsffgDFGFGHGFHdsfsdfjkkjjfnsd4343DFGD32432jbjbdmvmxv";
        public const string InvalidForgotPasswordInput = "195CDC21-1EF2-4500-B09D-765053A065F6";
        public const string TwoFactorEnabled = "tf";
        public const string IsFreshLogin = "freshLogin";
        public const string EmailId = "email";
        public const string Scope = "scope";
        public const string Issuer = "iss";
        public const string ADUser = "aduser";
        public const string PreferredUserName = "preferred_username";
        public const string TenantId = "tid";
        public const string ObjectId = "oid";
        public const string GraphURL = "https://graph.microsoft.com/v1.0/me";
        public const string JWKS = ".well-known/jwks";
        public const string Superadmin = "superadmin";
        #endregion
    }

    /// <summary>
    /// Define Procedure and Entity names
    /// </summary>
    public class ProcedureConstants
    {
        #region Product Procedure Contsants
        public const string procGetAllProducts = "proc_GetAllProducts";
        public const string procGetAllProductsSales = "proc_GetAllProductsSales";
        public const string procGetAllProductsByProductId = "proc_GetAllProductsByProductId";
        public const string procInsertProduct = "proc_InsertProduct";

        #endregion

        #region User Procedure Contsants
        public const string procGetAllUsersForUI = "proc_GetAllUsersByRoleForUI";
        public const string procGetAllUsers = "proc_GetAllUsersByRole";
        public const string procGetUserById = "proc_GetUserById";
        public const string procGetUserByName = "proc_GetUserByName";
        public const string procGetAllUserDetailsByName = "proc_GetAllUserDetailsByName";
        public const string procGetAllUserDetailsByMobile = "proc_GetAllUserDetailsByMobile";
        public const string procGetUserMappingWithApplicationId = "proc_GetUserMappingWithApplicationId";
        public const string procValidateUserApplicationAccess = "proc_ValidateUserApplicationAccess";
        public const string procGetAllAppIdByOrganizationId = "proc_GetAllAppIdByOrganizationId";
        public const string procGetAllUserByOrgId = "proc_GetAllUserByOrgId";
        public const string procGetAllUserByUserId = "proc_GetAllUserByUserId";
        public const string procGetAllExistUserById = "proc_GetAllExistUserById";
        public const string procResetLockOfUser = "proc_ResetLockOfUser";
        //public const string procDeleteUserById = "proc_DeleteUserById";
        public const string procGetAllDeviceDetails = "proc_GetAllDeviceDetails";
        public const string proc_GetAllOrgAppDetailsByUserName = "proc_GetAllOrgAppDetailsByUserName";

        #endregion

        #region Security Question Procedure Constants

        public const string getTwoRandomSecurityQuestion = "proc_GetRandomSecurityQuestion";

        public const string getUserSecurityQuestionInfo = "proc_GetUserSecurityQuestionInfo";

        #endregion

        #region Schedular Procedure Constant
        public const string procClearRefreshTokenData = "proc_ClearRefreshTokenData";
        #endregion

        #region Application Role Contsants
        public const string procGetApplicationRoles = "proc_GetApplicationRoles";
        public const string procGetApplicationByUserId = "proc_GetAllApplicationByUserId";
        public const string procGetApplicationByRole = "proc_GetAllApplicationByRole";
        public const string procGetAllApplicationRoleMapping = "proc_GetAllApplicationRoleMapping";
        public const string procGetAllExceptionRolesByApplicationName = "proc_GetAllExceptionRolesByApplicationName";
        #endregion

        #region AutoEmailer Scheduler Constants
        public const string procGetUsersForPwdExpNotify = "proc_GetAllUsersForPwdExpNotify";
        #endregion

        #region User Application Contsants
        public const string procGetAllLockedUsers = "proc_GetAllLockedUsers";
        #endregion

        #region Password Procedure Constants
        public const string procGetAllOldPassword = "proc_GetAllOldPassword";
        #endregion

        #region IP Table Procedure Constants
        public const string procGetAllAuthorizedIPByAppId = "proc_GetAllAuthorizedIPByAppId";
        #endregion

        #region Role and ClientType Base Access Constants
        public const string procVerifyRoleBaseAccess = "proc_VerifyRoleBaseAccess";
        public const string procVerifyClientTypeBaseAccess = "proc_VerifyClientTypeBaseAccess";
        public const string procGetRoleModuleDetails = "proc_GetRoleModuleDetails";
        public const string procGetClientTypeModuleDetails = "proc_GetClientTypeModuleDetails";
        public const string procVerifyRoleAccessExceptions = "proc_VerifyRoleAccessExceptions";
        public const string procVerifyClientTypeAccessExceptions = "proc_VerifyClientTypeAccessExceptions";
        public const string procGetAllOrganization = "proc_GetAllOrganization";
        public const string procGetOrganizationApplicationDetails = "proc_GetAllOrgWithAppDetails";
        public const string procGetAllClient = "proc_GetAllClient";
        public const string procGetAzureAppIdByClientId = "proc_GetAzureAppIdByClientId";
        #endregion

        #region Table Constants
        public static string tablePurchaseorderSaleDetails = "PurchaseorderSaleDetails";
        #endregion

        #region RefreshToken
        public static string procDeleteRefreshTokenById = "proc_DeleteRefreshTokenById";
        public static string procGetClientwiseTokenCount = "proc_GetClientwiseTokenCount";
        #endregion

        #region Organization
        public const string procGetAllOrganizationByClientId = "proc_GetAllOrganizationByClientId";
        public const string procGetAllOrgNameAppNameMapping = "proc_GetAllOrgNameAppNameMapping";
        public const string procGetAllExceptionApplicationsByOrgName = "proc_GetAllExceptionApplicationsByOrgName";
        public const string procGetAllActiveOrganizationByClientId = "proc_GetAllActiveOrganizationByClientId";
        public const string procGetOrganizationByTenant = "proc_GetOrganizationByTenant";
        public const string procGetOrgAppDetails = "proc_GetOrgAppDetails";
        #endregion

        #region Device
        public const string proc_GetDeviceBasedOnRole = "proc_GetDeviceBasedOnRole";
        #endregion

        #region UserLogOut Token
        public const string proc_DeleteLoggedOutToken = "proc_DeleteLoggedOutToken";
        #endregion

        #region SiteAdmin
        public const string procGetAllSiteAdmin = "GetSiteAdminDetails";
        #endregion
    }

    /// <summary>
    /// Define Password related constants
    /// </summary>
    public class PasswordConstants
    {
        public const string CharactorsLowerCase = "abcdefgijkmnopqrstwxyz";
        public const string CharactorsUpperCase = "ABCDEFGHJKLMNPQRSTWXYZ";
        public const string CharactorsNumeric = "23456789";
        public const string CharactorsSpecial = "*$-+?_&=!%{}/";
    }

    public class EmailConstants
    {
        public const string mailId = "info@eva.com";
        public const string UserRegistrationTemplate = "UserRegistration";
    }

    public class ValidationConstants
    {
        public const string ApplicationName = "appName";
        public const string OrganizationName = "orgName";
    }

    public class ApiVersionConstant
    {
        public const string ApiVersion = "api-version";
    }

    public class StatusCodeConstants
    {
        public const int Status449RetryWith = 449;
    }
}

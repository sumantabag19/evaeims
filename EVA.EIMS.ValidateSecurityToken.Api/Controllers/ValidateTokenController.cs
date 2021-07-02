using EVA.EIMS.Common;
using EVA.EIMS.Common.Constants;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity.ComplexEntities;
using EVA.EIMS.Entity.ViewModel;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.ValidateSecurityToken.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/validatetoken/[action]")]

    public class ValidateTokenController : Controller, IDisposable
    {

        #region Private variables
        private readonly IClientBusiness _clientBusiness;
        private readonly IOrganizationBusiness _organizationBusiness;
        private readonly ILogger _logger;
        private readonly IUserBusiness _userBusiness;
        private readonly IOptions<ApplicationSettings> _applicationSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly ICustomPasswordHash _customPasswordHash;
        private readonly IDistributedCache _distributedCache;
        private readonly string _encryptionKey;

        #endregion

        #region Constructor
        public ValidateTokenController(IClientBusiness clientBusiness, ILogger logger, IUserBusiness userBusiness, IApplicationBusiness applicationBusiness, IOrganizationBusiness organizationBusiness, IOptions<ApplicationSettings> appsettings, IServiceProvider serviceProvider, IDistributedCache distributedCache)
        {
            _clientBusiness = clientBusiness;
            _organizationBusiness = organizationBusiness;
            _logger = logger;
            _userBusiness = userBusiness;
            _applicationSettings = appsettings;
            _serviceProvider = serviceProvider;
            _customPasswordHash = _serviceProvider.GetRequiredService<ICustomPasswordHash>();
            _encryptionKey = _applicationSettings.Value.Eck;
            _distributedCache = distributedCache;
        }

        #endregion

        #region Public Methods      
        /// <summary>
        /// This Get call is for testing API
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get([FromQuery] string token)
        {
            return Ok(token);
        }

        /// <summary>
        /// This method is used to get the token and check for its validity
        /// </summary>
        ///<param name="token">token</param>
        /// <returns>returns success or faulure based on token validation</returns>
        [HttpPost]
        [ActionName("")]
        public async Task<IActionResult> Post([FromBody] AccessTokenModel accessToken)
        {
            string appName = String.Empty;
            ReturnResult returnResult = new ReturnResult();
            string clientId = String.Empty;
            bool enableTokenLogging = false;
            string orgName = String.Empty;
            string userId = string.Empty;
            UserDetails userDetails = new UserDetails();
            try
            {
                SecurityToken tokenValues;
                var jwtHandler = new JwtSecurityTokenHandler();
                List<string> appNameArray = new List<string>();
                var request = Request;
                var header = request.Headers;
                bool allowedToAccess = false;
                var enableIPWhitelisting = _applicationSettings.Value.EnableIPWhitelisting;
                enableTokenLogging = _applicationSettings.Value.EnableTokenLogging;

                if (header.ContainsKey(ValidationConstants.ApplicationName) && header[ValidationConstants.ApplicationName].First() != String.Empty)
                {
                    appName = header[ValidationConstants.ApplicationName].ToString();
                }
                else
                {
                    LogUnauthorizedError(clientId, userId, "ValidateToken-" + ResourceInformation.GetResValue("ProvideApplicationName"), HttpStatusCode.Unauthorized, NLog.LogLevel.Info);
                    return new ContentResult { Content = ResourceInformation.GetResValue("ProvideApplicationName"), StatusCode = StatusCodes.Status401Unauthorized };
                }

                if (accessToken == null || !jwtHandler.CanReadToken(accessToken.token))
                {
                    //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("ProvideAccessToken"), "Usr-" + userId + " Cli-" + clientId);
                    LogUnauthorizedError(clientId, userId, "ValidateToken-" + ResourceInformation.GetResValue("ProvideAccessToken"), HttpStatusCode.Unauthorized, NLog.LogLevel.Info);
                    return new ContentResult { Content = ResourceInformation.GetResValue("ProvideAccessToken"), StatusCode = StatusCodes.Status401Unauthorized };
                }

                var tokenDetails = jwtHandler.ReadToken(accessToken.token) as JwtSecurityToken;
                //Decrypting ClientId
                var encryptedClientId = tokenDetails.Payload.Single(c => c.Key == KeyConstant.ClientId).Value.ToString();
                clientId = _customPasswordHash.Decrypt(encryptedClientId, _encryptionKey);

                if (tokenDetails == null)
                {
                    //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("ProvideAccessToken"), "Usr-" + userId + " Cli-" + clientId);
                    LogUnauthorizedError(clientId, userId, "ValidateToken-" + ResourceInformation.GetResValue("ProvideAccessToken"), HttpStatusCode.Unauthorized, NLog.LogLevel.Info);
                    return new ContentResult { Content = ResourceInformation.GetResValue("ProvideAccessToken"), StatusCode = StatusCodes.Status401Unauthorized };
                }

                if (tokenDetails.Payload.ContainsKey(KeyConstant.Sub))
                {
                    if (tokenDetails.Payload.ContainsKey(KeyConstant.Subid))
                        userId = tokenDetails.Payload.Single(c => c.Key == KeyConstant.Subid).Value.ToString();
                    else
                        userId = tokenDetails.Payload.Single(c => c.Key == KeyConstant.Sub).Value.ToString();

                    //userDetails = await _userBusiness.GetUserByUserName(tokenDetails.Payload.Single(c => c.Key == KeyConstant.Sub).Value.ToString());

                    //if (userDetails != null)
                    //{
                    //    if (userDetails.ProviderName == ApplicationLevelConstants.IMSAuthProvider)
                    //    {
                    //        var expiryDate = DateTime.Parse(tokenDetails.Payload.Single(c => c.Key == KeyConstant.Passwordexpireat).Value.ToString());

                    //        if (expiryDate.Date < DateTime.Now.Date)
                    //        {
                    //            //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("UserPasswordExpired"), "Usr-" + userId + " Cli-" + clientId);
                    //            LogUnauthorizedError(clientId, userId, "ValidateToken-" + ResourceInformation.GetResValue("UserPasswordExpired"), HttpStatusCode.Unauthorized, LogLevel.Info);
                    //            return new ContentResult
                    //            {
                    //                Content = ResourceInformation.GetResValue("UserPasswordExpired"),
                    //                StatusCode = StatusCodes.Status401Unauthorized
                    //            };
                    //        }
                    //    }
                    //    // Check for OrgApp Mapping for user with MultiOrg Access
                    //    if (userDetails.MultipleOrgAccess)
                    //    {
                    //        if (header.ContainsKey(ValidationConstants.OrganizationName) && header[ValidationConstants.OrganizationName].First() != String.Empty)
                    //        {
                    //            orgName = header[ValidationConstants.OrganizationName].ToString();
                    //            var allowedOrgApp = await _userBusiness.GetOrgAppMappingByUserName(userDetails.UserName);

                    //            //Get orgDetails of the org the user wants to access
                    //            var orgDetails = await _organizationBusiness.GetOrganizationIdByOrgName(orgName);

                    //            foreach (var mapping in allowedOrgApp)
                    //            {
                    //                var orgIds = mapping.OrgId.Split(',');
                    //                //Check if the given OrgApp mapping is allowed to user
                    //                if (mapping.AppName.Equals(appName) && orgIds.Contains(orgDetails.OrgId.ToString()))
                    //                {
                    //                    allowedToAccess = true;
                    //                    break;
                    //                }
                    //            }
                    //            if (!allowedToAccess)
                    //            {
                    //                //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("UnAuthorizedUser"), "Usr-" + userId + " Cli-" + clientId);
                    //                LogUnauthorizedError(clientId, userId, "ValidateToken-" + ResourceInformation.GetResValue("UnAuthorizedUser"), HttpStatusCode.Unauthorized, LogLevel.Info);
                    //                return new ContentResult { Content = ResourceInformation.GetResValue("UnAuthorizedUser"), StatusCode = StatusCodes.Status401Unauthorized };
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("ProvideOrganizationName"), "Usr-" + userId + " Cli-" + clientId);
                    //            LogUnauthorizedError(clientId, userId, "ValidateToken-" + ResourceInformation.GetResValue("ProvideOrganizationName"), HttpStatusCode.Unauthorized, LogLevel.Info);
                    //            return new ContentResult { Content = ResourceInformation.GetResValue("ProvideOrganizationName"), StatusCode = StatusCodes.Status401Unauthorized };
                    //        }
                    //    }
                    //}
                }
                if (tokenDetails.ValidTo <= DateTime.UtcNow)
                {
                    //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("TokenExpired"), "Usr-" + userId + " Cli-" + clientId);
                    LogUnauthorizedError(clientId, userId, "ValidateToken-" + ResourceInformation.GetResValue("TokenExpired")
                        + "|" + tokenDetails.ValidFrom.ToString("s") + "|" + tokenDetails.ValidTo.ToString("s") + "|" + DateTime.UtcNow.ToString("s"), HttpStatusCode.Unauthorized, NLog.LogLevel.Info);
                    return new ContentResult
                    {
                        Content = ResourceInformation.GetResValue("TokenExpired"),
                        StatusCode = StatusCodes.Status401Unauthorized
                    };

                }

                if (tokenDetails.Payload.ContainsKey(KeyConstant.TwoFactorEnabled))
                {
                    string twoFactorEnabled = tokenDetails.Payload.Single(c => c.Key == KeyConstant.TwoFactorEnabled).Value.ToString();
                    if (twoFactorEnabled.ToLower() == bool.TrueString.ToLower())
                    {
                        // Verify if the user is using a previously generated token without logging out
                        IIMSLogOutBusiness iMSLogOutBusiness = _serviceProvider.GetRequiredService<IIMSLogOutBusiness>();
                        var result = await iMSLogOutBusiness.IsUserLoggedOut(accessToken.token);
                        if (result.Success)
                        {
                            //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("InvalidToken"), "Usr-" + userId + " Cli-" + clientId);
                            LogUnauthorizedError(clientId, userId, "ValidateToken-" + ResourceInformation.GetResValue("InvalidToken"), HttpStatusCode.Unauthorized, NLog.LogLevel.Info);
                            return new ContentResult { Content = ResourceInformation.GetResValue("InvalidToken"), StatusCode = StatusCodes.Status401Unauthorized };
                        }
                    }
                }

                var client = await _clientBusiness.GetById(clientId);

                //if (client == null)
                //{
                //    //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("InvalidClient"), "Usr-" + userId + " Cli-" + clientId);
                //    LogUnauthorizedError(clientId, userId, "ValidateToken-" + ResourceInformation.GetResValue("InvalidClient"), HttpStatusCode.Unauthorized, LogLevel.Info);
                //    return new ContentResult { Content = $"Invalid_client", StatusCode = StatusCodes.Status401Unauthorized };
                //}

                //if (client.ClientExpireOn != null && client.ClientExpireOn < DateTime.Now)
                //{
                //    //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("ClientExpired"), "Usr-" + userId + " Cli-" + clientId);
                //    LogUnauthorizedError(clientId, userId, "ValidateToken-" + ResourceInformation.GetResValue("ClientExpired"), HttpStatusCode.Unauthorized, LogLevel.Info);
                //    return new ContentResult { Content = ResourceInformation.GetResValue("ClientExpired"), StatusCode = StatusCodes.Status401Unauthorized };
                //}
                //if (tokenDetails.Payload.Any(c => c.Key == KeyConstant.Role))
                //{
                //    if (!tokenDetails.Payload.Single(c => c.Key == KeyConstant.Role).Value.ToString().Contains(UserRoles.SuperAdmin.ToString()))
                //    {
                //        if (tokenDetails.Payload.Any(c => c.Key == KeyConstant.AppName))
                //        {
                //            if (tokenDetails.Payload[KeyConstant.AppName].GetType().ToString().Equals("System.String"))
                //            {
                //                //Decrypting Application
                //                var encryptedApplication = tokenDetails.Payload[KeyConstant.AppName].ToString();
                //                var decryptedApplication = _customPasswordHash.Decrypt(encryptedApplication, _encryptionKey);
                //                appNameArray.Add(decryptedApplication.ToLower());
                //            }
                //            else
                //            {
                //                foreach (var item in ((Newtonsoft.Json.Linq.JArray)tokenDetails.Payload[KeyConstant.AppName]).Children())
                //                {
                //                    //Decrypting Application
                //                    var encryptedApplication = item.ToString();
                //                    var decryptedApplication = _customPasswordHash.Decrypt(encryptedApplication, _encryptionKey);
                //                    appNameArray.Add(decryptedApplication.ToLower());
                //                }
                //            }
                //        }

                //        else
                //        {
                //            //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("NoAppNameFound"), "Usr-" + userId + " Cli-" + clientId);
                //            LogUnauthorizedError(clientId, userId, "ValidateToken-" + ResourceInformation.GetResValue("NoAppNameFound"), HttpStatusCode.Unauthorized, LogLevel.Info);
                //            return new ContentResult
                //            {
                //                Content = ResourceInformation.GetResValue("NoAppNameFound"),
                //                StatusCode = StatusCodes.Status401Unauthorized
                //            };
                //        }

                //        if (!appNameArray.Contains(appName.ToLower()))
                //        {
                //            //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("NoPermissionToApp"), "Usr-" + userId + " Cli-" + clientId);
                //            LogUnauthorizedError(clientId, userId, "ValidateToken-" + ResourceInformation.GetResValue("NoPermissionToApp"), HttpStatusCode.Unauthorized, LogLevel.Info);
                //            return new ContentResult
                //            {
                //                Content = ResourceInformation.GetResValue("NoPermissionToApp"),
                //                StatusCode = StatusCodes.Status401Unauthorized
                //            };
                //        }
                //    }
                //}

                //if (enableIPWhitelisting)
                //{
                //    if (client.ClientTypeId == (int)ClientTypes.DeviceClient || (client.ClientTypeId == (int)ClientTypes.ServiceClient))
                //    {
                //        var ipAddress = HttpContext.Connection.RemoteIpAddress;
                //        var encryptedOrganization = tokenDetails.Payload.Single(c => c.Key == KeyConstant.OrgId).Value.ToString();
                //        if (!string.IsNullOrEmpty(encryptedOrganization))
                //        {
                //            var isIpAllowed = await CheckIPValidation(tokenDetails, ipAddress, appName, encryptedOrganization);
                //            if (isIpAllowed.Success == false)
                //            {
                //                //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("UnauthorizedAccessException"), "Usr-" + userId + " Cli-" + clientId);
                //                LogUnauthorizedError(clientId, userId, "ValidateToken-" + ResourceInformation.GetResValue("UnauthorizedAccessException"), HttpStatusCode.Unauthorized, LogLevel.Info);
                //                return new ContentResult
                //                {
                //                    Content = ResourceInformation.GetResValue("UnauthorizedAccessException"),
                //                    StatusCode = StatusCodes.Status401Unauthorized
                //                };

                //            }
                //        }
                //    }

                //}
                var validationParams =
                    new TokenValidationParameters()
                    {
                        ValidAudience = clientId,
                        ValidIssuer = tokenDetails.Issuer,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                WebEncoders.Base64UrlDecode(client.ClientSecret))

                    };

                jwtHandler.ValidateToken(accessToken.token, validationParams, out tokenValues);
            }
            catch (Exception ex)
            {
                _logger.Error("ValidateToken", "Post", ex.Message, "Usr-" + userId + " Cli-" + clientId + "\n StackTrace : " + ex.StackTrace);
                LogUnauthorizedError(clientId, userId, "ValidateToken-" + ResourceInformation.GetResValue("UnauthorizedAccessException"), HttpStatusCode.Unauthorized, NLog.LogLevel.Error, ex.StackTrace);
                return new ContentResult
                {
                    Content = ResourceInformation.GetResValue(ex.Message + "     " + ex.StackTrace),
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            return new ContentResult
            {
                Content = ResourceInformation.GetResValue("ValidToken"),
                StatusCode = StatusCodes.Status200OK
            };


        }

        /// <summary>
        /// This method is used to get the token and check for its validity and return claims
        /// </summary>
        ///<param name="token">token</param>
        /// <returns>returns token claim values</returns>

        [HttpPost]
        [ActionName("ValidateAndGetClaim")]
        public async Task<IActionResult> ValidateAndGetClaim([FromBody] AccessTokenModel accessToken)
        {
            JwtSecurityToken tokenDetails;
            string appName = String.Empty;
            string clientId = String.Empty;
            string userId = string.Empty;
            bool enableTokenLogging = false;
            UserDetails userDetails = new UserDetails();
            try
            {
                SecurityToken tokenValues;
                var jwtHandler = new JwtSecurityTokenHandler();
                List<string> appNameArray = new List<string>();
                var request = Request;
                var header = request.Headers;
                string orgName = String.Empty;
                IIMSLogOutBusiness iMSLogOutBusiness = _serviceProvider.GetRequiredService<IIMSLogOutBusiness>();

                bool allowedToAccess = false;
                var enableIPWhitelisting = _applicationSettings.Value.EnableIPWhitelisting;
                enableTokenLogging = _applicationSettings.Value.EnableTokenLogging;
                //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", "ValidateAndGetClaim Start",  "Usr-" + userId+ " Cli-" + clientId );

                //Removed app name validation
                /*
                if (header.ContainsKey(ValidationConstants.ApplicationName) && header[ValidationConstants.ApplicationName].First() != String.Empty)
                {
                    appName = header[ValidationConstants.ApplicationName].ToString();
                    //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", "ValidateAndGetClaim App Name captured", "Usr-" + userId + " Cli-" + clientId);
                }
                else
                {
                    //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("ProvideApplicationName"), "Usr-" + userId + " Cli-" + clientId);
                    LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("ProvideApplicationName"), HttpStatusCode.Unauthorized, NLog.LogLevel.Info);

                    return new ContentResult { Content = ResourceInformation.GetResValue("ProvideApplicationName"), StatusCode = StatusCodes.Status401Unauthorized };
                }
                */

                if (accessToken == null || !jwtHandler.CanReadToken(accessToken.token))
                {
                    //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("ProvideAccessToken"), "Usr-" + userId + " Cli-" + clientId);
                    LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("ProvideAccessToken"), HttpStatusCode.Unauthorized, NLog.LogLevel.Info);
                    return new ContentResult { Content = ResourceInformation.GetResValue("ProvideAccessToken"), StatusCode = StatusCodes.Status401Unauthorized };
                }

                tokenDetails = jwtHandler.ReadToken(accessToken.token) as JwtSecurityToken;
                var isAdUser = tokenDetails.Claims.FirstOrDefault(c => c.Type == KeyConstant.ADUser);



                if (isAdUser != null)
                {
                    if (tokenDetails.ValidTo <= DateTime.UtcNow)
                    {
                        //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("TokenExpired"), "Usr-" + userId + " Cli-" + clientId);

                        LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("TokenExpired") + "|" + tokenDetails.ValidFrom.ToString("s")
                            + "|" + tokenDetails.ValidTo.ToString("s") + "|" + DateTime.UtcNow.ToString("s"), HttpStatusCode.Unauthorized,
                            NLog.LogLevel.Info);

                        return new ContentResult
                        {
                            Content = ResourceInformation.GetResValue("TokenExpired"),
                            StatusCode = StatusCodes.Status401Unauthorized
                        };

                    }
                    var invalidToken = await iMSLogOutBusiness.IsUserLoggedOut(accessToken.token);
                    if (invalidToken.Success)
                    {
                        LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("TokenAlreadyExists"), HttpStatusCode.Unauthorized, NLog.LogLevel.Info);

                        return new ContentResult { Content = ResourceInformation.GetResValue("TokenAlreadyExists"), StatusCode = StatusCodes.Status401Unauthorized };
                    }

                    return Ok(tokenDetails.Payload);

                }
                //Decrypting ClientId
                var encryptedClientId = tokenDetails.Payload.Single(c => c.Key == KeyConstant.ClientId).Value.ToString();
                clientId = _customPasswordHash.Decrypt(encryptedClientId, _encryptionKey);

                if (tokenDetails == null)
                {
                    //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("ProvideAccessToken"), "Usr-" + userId + " Cli-" + clientId);
                    LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("ProvideAccessToken"), HttpStatusCode.Unauthorized, NLog.LogLevel.Info);
                    return new ContentResult { Content = ResourceInformation.GetResValue("ProvideAccessToken"), StatusCode = StatusCodes.Status401Unauthorized };
                }
                if (tokenDetails.Payload.ContainsKey(KeyConstant.Sub))
                {
                    if (tokenDetails.Payload.ContainsKey(KeyConstant.Subid))
                        userId = tokenDetails.Payload.Single(c => c.Key == KeyConstant.Subid).Value.ToString();
                    else
                        userId = tokenDetails.Payload.Single(c => c.Key == KeyConstant.Sub).Value.ToString();

                    //userDetails = await _userBusiness.GetUserByUserName(tokenDetails.Payload.Single(c => c.Key == KeyConstant.Sub).Value.ToString());
                    //if (userDetails != null)
                    //{
                    //    if (userDetails.ProviderName == ApplicationLevelConstants.IMSAuthProvider)
                    //    {
                    //        var expiryDate = DateTime.Parse(tokenDetails.Payload.Single(c => c.Key == KeyConstant.Passwordexpireat).Value.ToString());

                    //        if (expiryDate.Date < DateTime.Now.Date)
                    //        {
                    //            //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("UserPasswordExpired"), "Usr-" + userId + " Cli-" + clientId);
                    //            LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("UserPasswordExpired"), HttpStatusCode.Unauthorized, LogLevel.Info);
                    //            return new ContentResult
                    //            {
                    //                Content = ResourceInformation.GetResValue("UserPasswordExpired"),
                    //                StatusCode = StatusCodes.Status401Unauthorized
                    //            };
                    //        }
                    //    }
                    //    // Check for OrgApp Mapping for user with MultiOrg Access
                    //    if (userDetails.MultipleOrgAccess)
                    //    {
                    //        if (header.ContainsKey(ValidationConstants.OrganizationName) && header[ValidationConstants.OrganizationName].First() != String.Empty)
                    //        {
                    //            orgName = header[ValidationConstants.OrganizationName].ToString();
                    //            var allowedOrgApp = await _userBusiness.GetOrgAppMappingByUserName(userDetails.UserName);

                    //            //Get orgDetails of the org the user wants to access
                    //            var orgDetails = await _organizationBusiness.GetOrganizationIdByOrgName(orgName);

                    //            //Removed app name validation
                    //            /*
                    //            foreach (var mapping in allowedOrgApp)
                    //            {
                    //                var orgIds = mapping.OrgId.Split(',');
                    //                //Check if the given OrgApp mapping is allowed to user
                    //                if (mapping.AppName.Equals(appName) && orgIds.Contains(orgDetails.OrgId.ToString()))
                    //                {
                    //                    allowedToAccess = true;
                    //                    break;
                    //                }
                    //            }
                    //            if (!allowedToAccess)
                    //            {
                    //                //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("UnAuthorizedUser"), "Usr-" + userId + " Cli-" + clientId);
                    //                LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("UnAuthorizedUser"), HttpStatusCode.Unauthorized, LogLevel.Info);
                    //                return new ContentResult { Content = ResourceInformation.GetResValue("UnAuthorizedUser"), StatusCode = StatusCodes.Status401Unauthorized };
                    //            }
                    //            */
                    //        }
                    //        else
                    //        {
                    //            //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("ProvideOrganizationName"), "Usr-" + userId + " Cli-" + clientId);
                    //            LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("ProvideOrganizationName"), HttpStatusCode.Unauthorized, LogLevel.Info);
                    //            return new ContentResult { Content = ResourceInformation.GetResValue("ProvideOrganizationName"), StatusCode = StatusCodes.Status401Unauthorized };
                    //        }
                    //    }
                    //}
                }
                if (tokenDetails.ValidTo <= DateTime.UtcNow)
                {
                    //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("TokenExpired"), "Usr-" + userId + " Cli-" + clientId);
                    LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("TokenExpired") + "|" + tokenDetails.ValidFrom.ToString("s")
                        + "|" + tokenDetails.ValidTo.ToString("s") + "|" + DateTime.UtcNow.ToString("s"), HttpStatusCode.Unauthorized, NLog.LogLevel.Info);
                    return new ContentResult
                    {
                        Content = ResourceInformation.GetResValue("TokenExpired"),
                        StatusCode = StatusCodes.Status401Unauthorized
                    };

                }

                if (tokenDetails.Payload.ContainsKey(KeyConstant.TwoFactorEnabled))
                {
                    string twoFactorEnabled = tokenDetails.Payload.Single(c => c.Key == KeyConstant.TwoFactorEnabled).Value.ToString();
                    if (twoFactorEnabled.ToLower() == bool.TrueString.ToLower())
                    {
                        // Verify if the user is using a previously generated token without logging out
                        var result = await iMSLogOutBusiness.IsUserLoggedOut(accessToken.token);
                        if (result.Success)
                        {
                            if (userDetails.TwoFactorEnabled)
                                return new ContentResult { Content = ResourceInformation.GetResValue("OtpValidationNotDone"), StatusCode = StatusCodeConstants.Status449RetryWith };
                            else
                            {
                                //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("TokenAlreadyExists"), "Usr-" + userId + " Cli-" + clientId);
                                LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("TokenAlreadyExists"), HttpStatusCode.Unauthorized, NLog.LogLevel.Info);
                                return new ContentResult { Content = ResourceInformation.GetResValue("TokenAlreadyExists"), StatusCode = StatusCodes.Status401Unauthorized };
                            }
                        }
                    }
                }

                var client = await _clientBusiness.GetById(clientId);

                //if (client == null)
                //{
                //    //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("InvalidClient"), "Usr-" + userId + " Cli-" + clientId);
                //    LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("InvalidClient"), HttpStatusCode.Unauthorized, LogLevel.Info);
                //    return new ContentResult { Content = $"Invalid_client", StatusCode = StatusCodes.Status401Unauthorized };
                //}
                //if (client.ClientExpireOn != null && client.ClientExpireOn < DateTime.Now)
                //{
                //    //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("ClientExpired"), "Usr-" + userId + " Cli-" + clientId);
                //    LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("ClientExpired"), HttpStatusCode.Unauthorized, LogLevel.Info);
                //    return new ContentResult { Content = ResourceInformation.GetResValue("ClientExpired"), StatusCode = StatusCodes.Status401Unauthorized };
                //}
                //if (tokenDetails.Payload.Any(c => c.Key == KeyConstant.Role))
                //{
                //    if (!tokenDetails.Payload.Single(c => c.Key == KeyConstant.Role).Value.ToString().Contains(UserRoles.SuperAdmin.ToString()))
                //    {
                //        if (tokenDetails.Payload.Any(c => c.Key == KeyConstant.AppName))
                //        {
                //            if (tokenDetails.Payload[KeyConstant.AppName].GetType().ToString().Equals("System.String"))
                //            {
                //                //Decrypting Application
                //                var encryptedApplication = tokenDetails.Payload[KeyConstant.AppName].ToString();
                //                var decryptedApplication = _customPasswordHash.Decrypt(encryptedApplication, _encryptionKey);
                //                appNameArray.Add(decryptedApplication.ToLower());
                //            }
                //            else
                //            {
                //                foreach (var item in ((Newtonsoft.Json.Linq.JArray)tokenDetails.Payload[KeyConstant.AppName]).Children())
                //                {
                //                    //Decrypting Application
                //                    var encryptedApplication = item.ToString();
                //                    var decryptedApplication = _customPasswordHash.Decrypt(encryptedApplication, _encryptionKey);
                //                    appNameArray.Add(decryptedApplication.ToLower());
                //                }
                //            }
                //        }

                //        else
                //        {
                //            //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("NoAppNameFound"), "Usr-" + userId + " Cli-" + clientId);
                //            LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("NoAppNameFound"), HttpStatusCode.Unauthorized, LogLevel.Info);
                //            return new ContentResult
                //            {
                //                Content = ResourceInformation.GetResValue("NoAppNameFound"),
                //                StatusCode = StatusCodes.Status401Unauthorized
                //            };
                //        }

                //        //Removed app name validation
                //        /*
                //        if (!appNameArray.Contains(appName.ToLower()))
                //        {
                //            //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("NoPermissionToApp"), "Usr-" + userId + " Cli-" + clientId);
                //            LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("NoPermissionToApp"), HttpStatusCode.Unauthorized, LogLevel.Info);
                //            return new ContentResult
                //            {
                //                Content = ResourceInformation.GetResValue("NoPermissionToApp"),
                //                StatusCode = StatusCodes.Status401Unauthorized
                //            };
                //        }
                //        */
                //    }
                //}

                //if (enableIPWhitelisting)
                //{
                //    if (client.ClientTypeId == (int)ClientTypes.DeviceClient || (client.ClientTypeId == (int)ClientTypes.ServiceClient))
                //    {
                //        var ipAddress = HttpContext.Connection.RemoteIpAddress;
                //        var encryptedOrganization = tokenDetails.Payload.Single(c => c.Key == KeyConstant.OrgId).Value.ToString();
                //        if (!string.IsNullOrEmpty(encryptedOrganization))
                //        {
                //            var isIpAllowed = await CheckIPValidation(tokenDetails, ipAddress, appName, encryptedOrganization);
                //            if (isIpAllowed.Success == false)
                //            {
                //                //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("UnauthorizedAccessException"), "Usr-" + userId + " Cli-" + clientId);
                //                LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("UnauthorizedAccessException"), HttpStatusCode.Unauthorized, LogLevel.Info);
                //                return new ContentResult
                //                {
                //                    Content = ResourceInformation.GetResValue("UnauthorizedAccessException"),
                //                    StatusCode = StatusCodes.Status401Unauthorized
                //                };

                //            }
                //        }
                //    }

                //}
                //var validationParams =
                //    new TokenValidationParameters()
                //    {
                //        ValidAudience = clientId,
                //        ValidIssuer = tokenDetails.Issuer,
                //        IssuerSigningKey =
                //            new SymmetricSecurityKey(
                //                WebEncoders.Base64UrlDecode(client.ClientSecret))

                //    };

                //jwtHandler.ValidateToken(accessToken.token, validationParams, out tokenValues);
                var tokenS = jwtHandler.ReadToken(accessToken.token) as JwtSecurityToken;

                return Ok(tokenS.Payload);
            }
            catch (Exception ex)
            {
                _logger.Error("ValidateAndGetClaim", "ValidateAndGetClaim", ex.Message, "Usr-" + userId + " Cli-" + clientId + "\n Stack Trace : " + ex.StackTrace);
                LogUnauthorizedError(clientId, userId, ex.Message, HttpStatusCode.InternalServerError, NLog.LogLevel.Error, ex.StackTrace);
                return new ContentResult
                {
                    Content = ResourceInformation.GetResValue(ex.Message + "     " + ex.StackTrace),
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }


        }

        /// <summary>
        /// This method is used to get the token and check for its validity for device only
        /// </summary>
        ///<param name="token">token</param>
        /// <returns>returns success or faulure based on token validation</returns>
        [HttpPost]
        [ActionName("DeviceValidation")]
        public async Task<IActionResult> PostDeviceValidation([FromBody] AccessTokenModel accessToken)
        {
            ReturnResult returnResult = new ReturnResult();
            string appName = string.Empty;
            string clientId = string.Empty;
            string userId = string.Empty;
            bool enableTokenLogging = false;
            try
            {
                SecurityToken tokenValues;
                var jwtHandler = new JwtSecurityTokenHandler();
                List<string> appNameArray = new List<string>();
                var request = Request;
                var header = request.Headers;
                string orgName = String.Empty;
                var enableIPWhitelisting = _applicationSettings.Value.EnableIPWhitelisting;
                enableTokenLogging = _applicationSettings.Value.EnableTokenLogging;

                if (accessToken == null || !jwtHandler.CanReadToken(accessToken.token))
                {
                    //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("ProvideAccessToken"), "Usr-" + userId + " Cli-" + clientId);
                    LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("ProvideAccessToken"), HttpStatusCode.Unauthorized, NLog.LogLevel.Info);
                    return new ContentResult { Content = ResourceInformation.GetResValue("ProvideAccessToken"), StatusCode = StatusCodes.Status401Unauthorized };
                }

                var tokenDetails = jwtHandler.ReadToken(accessToken.token) as JwtSecurityToken;

                //Decrypting ClientId
                var encryptedClientId = tokenDetails.Payload.Single(c => c.Key == KeyConstant.ClientId).Value.ToString();
                clientId = _customPasswordHash.Decrypt(encryptedClientId, _encryptionKey);
                userId = tokenDetails.Payload.Single(c => c.Key == KeyConstant.Subid).Value.ToString();

                if (tokenDetails == null)
                {
                    //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("ProvideAccessToken"), "Usr-" + userId + " Cli-" + clientId);
                    LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("ProvideAccessToken"), HttpStatusCode.Unauthorized, NLog.LogLevel.Info);
                    return new ContentResult { Content = ResourceInformation.GetResValue("ProvideAccessToken"), StatusCode = StatusCodes.Status401Unauthorized };
                }

                if (tokenDetails.ValidTo <= DateTime.UtcNow)
                {
                    //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("TokenExpired"), "Usr-" + userId + " Cli-" + clientId);
                    LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("TokenExpired") + "|" + tokenDetails.ValidFrom.ToString("s")
                        + "|" + tokenDetails.ValidTo.ToString("s") + "|" + DateTime.UtcNow.ToString("s"), HttpStatusCode.Unauthorized, NLog.LogLevel.Info);
                    return new ContentResult
                    {
                        Content = ResourceInformation.GetResValue("TokenExpired"),
                        StatusCode = StatusCodes.Status401Unauthorized
                    };

                }

                // Verify if the user is using a previously generated token without logging out
                IIMSLogOutBusiness iMSLogOutBusiness = _serviceProvider.GetRequiredService<IIMSLogOutBusiness>();
                var result = await iMSLogOutBusiness.IsUserLoggedOut(accessToken.token);
                if (result.Success)
                {
                    //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("OtpValidationNotDone"), "Usr-" + userId + " Cli-" + clientId);
                    LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("OtpValidationNotDone"), HttpStatusCode.Unauthorized, NLog.LogLevel.Info);
                    return new ContentResult { Content = ResourceInformation.GetResValue("OtpValidationNotDone"), StatusCode = StatusCodes.Status401Unauthorized };
                }

                var client = await _clientBusiness.GetById(clientId);

                if (client == null)
                {
                    //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("InvalidClient"), "Usr-" + userId + " Cli-" + clientId);
                    LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("InvalidClient"), HttpStatusCode.Unauthorized, NLog.LogLevel.Info);
                    return new ContentResult { Content = $"Invalid_client", StatusCode = StatusCodes.Status401Unauthorized };
                }

                if (client.ClientExpireOn != null && client.ClientExpireOn < DateTime.Now)
                {
                    //_logger.Log(LogType.ERROR, "ValidateAndGetClaim", "Post", ResourceInformation.GetResValue("ClientExpired"), "Usr-" + userId + " Cli-" + clientId);
                    LogUnauthorizedError(clientId, userId, ResourceInformation.GetResValue("ClientExpired"), HttpStatusCode.Unauthorized, NLog.LogLevel.Info);
                    return new ContentResult { Content = ResourceInformation.GetResValue("ClientExpired"), StatusCode = StatusCodes.Status401Unauthorized };
                }

                var validationParams =
                    new TokenValidationParameters()
                    {
                        ValidAudience = clientId,
                        ValidIssuer = tokenDetails.Issuer,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                WebEncoders.Base64UrlDecode(client.ClientSecret))

                    };

                jwtHandler.ValidateToken(accessToken.token, validationParams, out tokenValues);
            }
            catch (Exception ex)
            {
                _logger.Error("ValidateToken", "PostDeviceValidation", ex.Message, "Usr-" + userId + " Cli-" + clientId + "\n StackTrace : " + ex.StackTrace);
                //LogUnauthorizedError(clientId, userId, ex.Message, HttpStatusCode.InternalServerError, LogLevel.Error, ex.StackTrace);
                return new ContentResult
                {
                    Content = ResourceInformation.GetResValue(ex.Message + "     " + ex.StackTrace),
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            return new ContentResult
            {
                Content = ResourceInformation.GetResValue("ValidToken"),
                StatusCode = StatusCodes.Status200OK
            };


        }


        #endregion

        #region Private Methods

        private async void LogUnauthorizedError(string clientId, string userId, string message, HttpStatusCode statusCode,
            NLog.LogLevel logLevel, string exception = "")
        {
            var cachedData = await _distributedCache.GetAsync("IsDatabaseLogEnabled");
            if (cachedData != null)
            {
                var cachedMessage = Encoding.UTF8.GetString(cachedData);
                if (cachedMessage.ToLower() == "true")
                {
                    NLog.LogEventInfo theEvent = new NLog.LogEventInfo(NLog.LogLevel.Error, "", message);
                    theEvent.Properties["UserId"] = userId;
                    theEvent.Properties["ClientId"] = clientId;
                    theEvent.Properties["StatusCode"] = Convert.ToInt32(statusCode);
                    theEvent.Properties["exception"] = exception;
                    NLog.LogManager.GetCurrentClassLogger().Log(theEvent);
                }
            }
            _logger.Info("ValidateController", "ValidateAndGetClaim", message + " " + clientId, string.Empty);
        }

        private async Task<ReturnResult> CheckIPValidation(JwtSecurityToken tokenDetails, IPAddress iPAddress, string appName, string encryptedOrganization)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IOrganizationBusiness organizationBusiness = _serviceProvider.GetRequiredService<IOrganizationBusiness>();
                IIPTableBusiness iPTableBusiness = _serviceProvider.GetRequiredService<IIPTableBusiness>();
                IApplicationBusiness applicationBusiness = _serviceProvider.GetRequiredService<IApplicationBusiness>();

                var appId = await applicationBusiness.GetByAppName(appName);
                var orgName = _customPasswordHash.Decrypt(encryptedOrganization, _encryptionKey);
                var org = await organizationBusiness.GetOrganizationIdByOrgName(orgName);

                //Check for authorized IP for Device and Service Clients
                var isIpAllowed = await iPTableBusiness.IsIPAuthorized(iPAddress, org.OrgId, appId);
                return isIpAllowed;

            }
            catch (Exception ex)
            {
                _logger.Error("ValidateToken", "CheckIPValidation", ex.Message, ex.StackTrace);
                returnResult.Success = false;
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _clientBusiness.Dispose();
                _userBusiness.Dispose();
                _organizationBusiness.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }




}

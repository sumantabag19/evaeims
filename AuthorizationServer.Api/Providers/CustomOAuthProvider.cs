using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using CustomPasswordHashCheck;
using EVA.EIMS.Common;
using EVA.EIMS.Common.Constants;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ComplexEntities;
using EVA.EIMS.Helper.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using EVA.EIMS.Logging;
using EVA.EIMS.Helper;

namespace AuthorizationServer.Api.Providers
{
    public class CustomOAuthProvider : OpenIdConnectServerProvider
    {
        #region Private variables 
        //private readonly ICustomPasswordHash _customPasswordHash;
        private readonly IOptions<ApplicationSettings> _applicationSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly ServiceProvider _provider;
        private readonly EVA.EIMS.Logging.ILogger _logger;
        private readonly string _encryptionKey;
        private readonly ICustomPasswordHash _customPasswordHash;
        private readonly CookieOptions _cookieOptions;
        private readonly IHttpContextAccessor _httpContext;
        #endregion

        #region Public Constructor
        public CustomOAuthProvider(ServiceProvider provider)
        {
            _provider = provider;
            _applicationSettings = _provider.GetRequiredService<IOptions<ApplicationSettings>>();
            _serviceProvider = _provider.GetRequiredService<IServiceProvider>();
            _logger = _provider.GetRequiredService<EVA.EIMS.Logging.ILogger>();
            _customPasswordHash = provider.GetRequiredService<ICustomPasswordHash>();
            _encryptionKey = _applicationSettings.Value.Eck;
            _httpContext = _provider.GetRequiredService<IHttpContextAccessor>();

            _cookieOptions = new CookieOptions
            {
                //Domain = ".azurewebsites.net",
                Path = "/;SameSite = None",
                HttpOnly = true,
                Secure = true,
                //Expires = DateTimeOffset.Now.AddHours(6),
                IsEssential = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
            };
        }

        #endregion

        #region Public Methods       
        public override Task ValidateAuthorizationRequest(ValidateAuthorizationRequestContext context)
        {
            ReturnResult returnResult = new ReturnResult();
            string mobile = string.Empty;
            try
            {
                if (!context.Request.IsAuthorizationCodeFlow())
                {
                    returnResult.Success = false;
                    context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedResponseType,
                    description: ResourceInformation.GetResValue("InvalidAuthFlow"));
                    return Task.FromResult<object>(returnResult);

                }

                if (!string.IsNullOrEmpty(context.Request.ResponseMode) && !context.Request.IsFormPostResponseMode() &&
                                                                           !context.Request.IsFragmentResponseMode() &&
                                                                           !context.Request.IsQueryResponseMode())
                {
                    returnResult.Success = false;
                    context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedResponseType,
                    description: ResourceInformation.GetResValue("InvalidResponseMode"));
                    return Task.FromResult<object>(returnResult);
                }


                var clientBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IClientBusiness>();
                var clientData = clientBusiness.GetById(context.ClientId).GetAwaiter().GetResult();

                if (clientData == null)
                {
                    returnResult.Success = false;
                    context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidRequest,
                    description: ResourceInformation.GetResValue("InvalidClient"));
                    return Task.FromResult<object>(returnResult);
                }

                var allowedScopes = clientData.AllowedScopes.Split(' ');
                var Scope = context.Request.Scope.Split(' ');
                var contextScopes = Scope.Where(s => !string.IsNullOrEmpty(s)).ToList();
                if (allowedScopes.Length != contextScopes.Count ||
                    allowedScopes.Any(scope => !contextScopes.Contains(scope)))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidScope");
                    context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidScope,
                    description: ResourceInformation.GetResValue("InvalidScope"));
                    return Task.FromResult<object>(returnResult);
                }

                //context.HttpContext.Request.Headers.TryGetValue("Origin", out var originValues);
                //if (originValues.Count() < 1 || (!originValues[0].Contains(clientData.RequestURL)
                //    && !originValues[0].Contains(clientData.DebugURL)))
                //string referer = new Uri(Convert.ToString(context.HttpContext.Request.Headers["Origin"])).AbsoluteUri;

                //if (referer != clientData.RequestURL && referer != _applicationSettings.Value.SSOLocalUrl)
                //{
                //    returnResult.Success = false;
                //    context.Reject(
                //    error: OpenIdConnectConstants.Errors.InvalidRequest,
                //    description: ResourceInformation.GetResValue("InvalidRequestURL"));
                //    return Task.FromResult<object>(returnResult);
                //}

                string baseUrl = new Uri(context.HttpContext.Request.Scheme + "://" + context.HttpContext.Request.Host.Value).ToString();

                if (!context.HttpContext.Request.Host.Value.Contains("localhost"))
                {
                    baseUrl = _applicationSettings.Value.IMSEndPoint;
                }

                var client = new Uri(baseUrl).AbsolutePath.ToString() + KeyConstant.LocalRedirect;

                string internalRedirectUrl = new Uri(new Uri(baseUrl), client.Replace("//", "/")).ToString();

                if (!context.Request.RedirectUri.Contains(clientData.RedirectURL) && !context.Request.RedirectUri.Contains(clientData.DebugURL)
                    && !context.Request.RedirectUri.Contains(internalRedirectUrl)
                    )
                {
                    returnResult.Success = false;
                    context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidRequest,
                    description: ResourceInformation.GetResValue("InvalidRedirectURL"));
                    return Task.FromResult<object>(returnResult);
                }

                if (_applicationSettings.Value.IsHttpsRequired && !context.HttpContext.Request.IsHttps)
                {
                    returnResult.Success = false;
                    context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidRequest,
                    description: ResourceInformation.GetResValue("InvalidURIScheme"));
                    return Task.FromResult<object>(returnResult);
                }

                context.Request.RedirectUri = internalRedirectUrl;
                var userBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IUserBusiness>();

                if (Convert.ToString(context.HttpContext.Request.Headers["authenticator_type"]) == "otp")
                {
                    mobile = context.Request.Username;
                    var existingUser = userBusiness.GetUserByMobile(mobile).GetAwaiter().GetResult();

                    var otpVerificationResponse = userBusiness.VerifyOTPLoginAsync(existingUser.UserId, context.Request.Password).GetAwaiter().GetResult();
                    if (otpVerificationResponse.Success && otpVerificationResponse.Result == ResourceInformation.GetResValue("OTP_Valid"))
                    {
                        context.Request.Username = existingUser.UserName;
                        context.Request.LoginHint = existingUser.UserName;
                        context.Request.ClientSecret = clientData.ClientSecret;
                    }
                    else
                    {
                        returnResult.Success = false;
                        context.Reject(
                            error: OpenIdConnectConstants.Errors.InvalidRequest,
                            description: otpVerificationResponse.Result);
                        return Task.FromResult<object>(returnResult);
                    }
                }

                Guid userIdFromCookie = GetUserIdFromCookie(context);

                if (userIdFromCookie != null && Guid.Empty != userIdFromCookie)
                {
                    var userResult = userBusiness.GetUserNameById(userIdFromCookie).GetAwaiter().GetResult();
                    if (userResult != null && userResult.Success && userResult.Data != null)
                    {
                        context.Request.Username = userResult.Data.ToString();
                        context.Request.LoginHint = userResult.Data.ToString();
                        context.Request.ClientSecret = clientData.ClientSecret;
                    }
                }

                var userDetails = userBusiness.GetUserByUserName(context.Request.Username).GetAwaiter().GetResult();
                if (userDetails == null)
                {
                    context.Request.ClientSecret = null;
                    context.Validate();
                    return Task.FromResult<object>(null);
                }
                //if (context.Request.IsAuthorizationCodeFlow() && !string.IsNullOrEmpty(context.Request.Password))
                if (context.Request.IsAuthorizationCodeFlow())
                {
                    //returnResult = ValidatePassword(context, userDetails, returnResult).GetAwaiter().GetResult();
                    var ipAddress = context.HttpContext.Connection.RemoteIpAddress;
                    context.Request.ClientSecret = clientData.ClientSecret;

                    returnResult = PasswordGrantTypeValidation(context, returnResult, userDetails, clientData, ipAddress).GetAwaiter().GetResult();

                    //Returning success with invalid authorization code
                    if (!returnResult.Success)
                    {
                        if (returnResult.Result == ResourceInformation.GetResValue("OrganizationNotExists") || returnResult.Result == ResourceInformation.GetResValue("NoUserApplicationMappingFound"))
                        {
                            context.Request.ClientSecret = null;
                            context.Validate();
                            return Task.FromResult<object>(null);
                        }
                        else
                        {
                            context.Reject(
                            error: OpenIdConnectConstants.Errors.InvalidRequest,
                            description: returnResult.Result);
                            return Task.FromResult<object>(returnResult);
                        }
                    }

                    context.Request.LoginHint = context.Request.Username;
                }
                if (userDetails.RolesArray.Contains(Enum.GetName(typeof(UserRoles), UserRoles.SuperAdmin)))
                {
                    context.Request.ClientSecret = clientData.ClientSecret;
                    context.Validate();
                    return Task.FromResult<object>(null);
                }
                if (userDetails.ClientTypeId != Convert.ToString(clientData.ClientTypeId))
                {
                    returnResult.Success = false;
                    context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidRequest,
                    description: ResourceInformation.GetResValue("InvalidAuthRelation"));
                    return Task.FromResult<object>(returnResult);

                }
                context.Request.LoginHint = userDetails.UserId.ToString();
                context.Request.ClientSecret = clientData.ClientSecret;

                context.Validate();
            }
            catch (Exception ex)
            {
                _logger.Error("CustomOAuthProvider", "ValidateAuthorizationRequest", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ExceptionLogger.LogException(ex);
            }
            return Task.FromResult<object>(null);

        }

        public override Task HandleAuthorizationRequest(HandleAuthorizationRequestContext context)
        {
            var customPasswordHashForApplication = _serviceProvider.GetRequiredService<ICustomPasswordHash>();
            var identity = new ClaimsIdentity(OpenIdConnectServerDefaults.AuthenticationScheme);
            identity.AddClaim(OpenIdConnectConstants.Claims.Subject, context.Request.Username, OpenIdConnectConstants.Destinations.IdentityToken);
            var tckt = new AuthenticationTicket(new ClaimsPrincipal(identity), new AuthenticationProperties(new Dictionary<string, string>
                    {
                      {
                         KeyConstant.ClientId, context.Request.ClientId
                      },
                      {
                         KeyConstant.UserName, context.Request.Username
                      },
                       {
                        KeyConstant.sts, context.Request.ClientSecret
                      }
                    }), OpenIdConnectServerDefaults.AuthenticationScheme);

            context.Validate(tckt);
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// To validate token request
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateTokenRequest(ValidateTokenRequestContext context)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                string clientId = string.Empty;
                string clientSecret = string.Empty;

                if (context.Request.GrantType == KeyConstant.AuthCode)
                {
                    var auth_code = _customPasswordHash.Decrypt(context.Request.Code.ToString(), _applicationSettings.Value.Eck).ToString();
                    var jwtHandler = new JwtSecurityTokenHandler();
                    var tokenDetails = jwtHandler.ReadToken(auth_code) as JwtSecurityToken;
                    if (tokenDetails.ValidTo < DateTime.UtcNow)
                    {
                        returnResult.Success = false;
                        context.Reject(
                        error: OpenIdConnectConstants.Errors.AccessDenied,
                        description: ResourceInformation.GetResValue("AuthCodeExpired"));
                        return Task.FromResult<object>(returnResult);
                    }
                    var cli = tokenDetails.Payload.Single(c => c.Key == KeyConstant.ClientId).Value.ToString();
                    var sts = tokenDetails.Payload.Single(c => c.Key == KeyConstant.sts).Value.ToString();
                    if (context.Request.ClientId != cli)
                    {
                        returnResult.Success = false;
                        context.Reject(
                        error: OpenIdConnectConstants.Errors.AccessDenied,
                        description: ResourceInformation.GetResValue("InvalidClient"));
                        return Task.FromResult<object>(returnResult);
                    }
                    var uname = tokenDetails.Payload.Single(c => c.Key == KeyConstant.UserName).Value.ToString();

                    var clientBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IClientBusiness>();
                    var clientData = clientBusiness.GetById(context.Request.ClientId).Result;
                    if (context.Request.RedirectUri != clientData.RedirectURL && context.Request.RedirectUri != clientData.DebugURL)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("InvalidRedirectURL");
                        context.Reject(
                        error: OpenIdConnectConstants.Errors.AccessDenied,
                        description: ResourceInformation.GetResValue("InvalidRedirectURL"));
                        return Task.FromResult<object>(returnResult);
                    }

                    if (String.IsNullOrEmpty(context.Request.Password))
                        context.Request.LoginHint = uname;
                    if (cli != context.ClientId)
                    {
                        returnResult.Success = false;
                        context.Reject(
                        error: OpenIdConnectConstants.Errors.AccessDenied,
                        description: ResourceInformation.GetResValue("InvalidAuthRelation"));
                        return Task.FromResult<object>(returnResult);
                    }
                    clientId = cli;
                    clientSecret = sts;
                    context.Request.Username = uname;
                    context.Request.ClientId = cli;
                    context.Request.ClientSecret = sts;
                }
                else
                {
                    clientId = context.ClientId;
                    clientSecret = context.ClientSecret;
                }

                var result = TokenRequestValidation(context, returnResult, clientSecret).GetAwaiter().GetResult();
                if (!result.Success)
                {
                    return Task.FromResult<object>(returnResult);
                }

                context.Validate();
            }
            catch (Exception ex)
            {
                _logger.Error("CustomOAuthProvider", "ValidateTokenRequest", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ExceptionLogger.LogException(ex);
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// To handle token request based on grant type
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task HandleTokenRequest(HandleTokenRequestContext context)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var clientBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IClientBusiness>();
                var clientData = await clientBusiness.GetById(context.Request.ClientId);

                if (clientData == null)
                {
                    returnResult.Success = false;
                }
                else
                {
                    if (clientData.TokenValidationPeriod != 0)
                        context.Options.AccessTokenLifetime = TimeSpan.FromMinutes(Convert.ToInt32(clientData.TokenValidationPeriod));
                    returnResult.Success = true;
                }
                var identity = new ClaimsIdentity(KeyConstant.JWT);
                var enableIPWhitelisting = _applicationSettings.Value.EnableIPWhitelisting;
                var ipAddress = context.HttpContext.Connection.RemoteIpAddress;

                if (context.Request.IsPasswordGrantType() || context.Request.GrantType == KeyConstant.AuthCode)
                {
                    await PasswordGrantType(context, returnResult, clientData, identity, enableIPWhitelisting, ipAddress);
                }
                else if (context.Request.IsClientCredentialsGrantType())
                {
                    await ClientCredentialsGrantTypeAsync(context, returnResult, clientData, identity, enableIPWhitelisting, ipAddress);
                }
                else if (context.Request.IsRefreshTokenGrantType())
                {
                    //Code added to verify cookie user and subid provided in ticket principal
                    if (clientData.Flow == KeyConstant.AuthCode)
                    {
                        await RefreshTokenGrantType(context, returnResult, clientData);
                        //Guid userIdFromCookie = GetUserIdFromCookie(context);

                        //if (userIdFromCookie != null && Guid.Empty != userIdFromCookie)
                        //{
                        //    if (context.Ticket.Principal.Claims.FirstOrDefault(c => c.Type == KeyConstant.Subid).Value.ToLower() == userIdFromCookie.ToString().ToLower())
                        //    {
                        //        await RefreshTokenGrantType(context, returnResult, clientData);
                        //    }
                        //    else
                        //    {
                        //        returnResult.Success = false;
                        //        context.Reject(
                        //        error: OpenIdConnectConstants.Errors.InvalidRequest,
                        //        description: ResourceInformation.GetResValue("UnauthorizedUser"));
                        //    }
                        //}
                        //else
                        //{
                        //    returnResult.Success = false;
                        //    context.Reject(
                        //    error: OpenIdConnectConstants.Errors.InvalidRequest,
                        //    description: ResourceInformation.GetResValue("UnauthorizedUser"));
                        //}
                    }
                    else
                    {
                        await RefreshTokenGrantType(context, returnResult, clientData);
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.Error("CustomOAuthProvider", "HandleTokenRequestTokenRequest", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ExceptionLogger.LogException(ex);
            }
        }

        //public override Task HandleLogoutRequest(HandleLogoutRequestContext context) {
        //    ReturnResult returnResult = new ReturnResult();
        //    return Task.FromResult<object>(null);
        //}

        /// <summary>
        /// This Method handles token request for password grant type
        /// </summary>
        /// <param name="context"></param>
        /// <param name="returnResult"></param>
        /// <param name="clientData"></param>
        /// <param name="identity"></param>
        /// <param name="enableIPWhitelisting"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public async Task<ReturnResult> PasswordGrantType(BaseValidatingTicketContext context, ReturnResult returnResult, OauthClient clientData,
                                       ClaimsIdentity identity, bool enableIPWhitelisting, System.Net.IPAddress ipAddress)
        {
            try
            {
                var customPasswordHashForApplication = _serviceProvider.GetRequiredService<ICustomPasswordHash>();
                if (returnResult.Success == false)
                {
                    context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: ResourceInformation.GetResValue("InvalidClient"));
                    return returnResult;
                }
                CustomPasswordHash customPasswordHash = new CustomPasswordHash(_applicationSettings);

                if ((string.IsNullOrEmpty(context.Request.Username) || string.IsNullOrEmpty(context.Request.Password)))
                {
                    if (string.IsNullOrEmpty(context.Request.LoginHint))
                    {
                        returnResult.Success = false;
                        context.Reject(
                                error: OpenIdConnectConstants.Errors.InvalidRequest,
                                description: ResourceInformation.GetResValue("InvalidGrant"));
                        return returnResult;
                    }
                }
                var userBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IUserBusiness>();
                var userDetails = await userBusiness.GetUserByUserName(context.Request.Username);

                returnResult = await PasswordGrantTypeValidation(context, returnResult, userDetails, clientData, ipAddress);

                if (!returnResult.Success)
                    return returnResult;

                if (userDetails != null)
                {
                    //For user with Multi-orgAccess
                    if (userDetails.MultipleOrgAccess)
                    {
                        identity.AddClaim(KeyConstant.IsMultiOrg, "true", OpenIdConnectConstants.Destinations.AccessToken);
                    }
                    else
                    {
                        identity.AddClaim(KeyConstant.IsMultiOrg, "false", OpenIdConnectConstants.Destinations.AccessToken);
                    }

                    identity.AddClaim(OpenIdConnectConstants.Claims.Subject, userDetails.UserName, OpenIdConnectConstants.Destinations.AccessToken);
                    identity.AddClaim(KeyConstant.TwoFactorEnabled, Convert.ToString(userDetails.TwoFactorEnabled).ToLower(), OpenIdConnectConstants.Destinations.AccessToken);

                    identity.AddClaim(KeyConstant.EmailId, userDetails.EmailId, OpenIdConnectConstants.Destinations.AccessToken);
                }
                //var allowedScopes = clientData.AllowedScopes.Split(' ');
                var Scope = clientData.AllowedScopes.Split(' ');
                //var contextScopes = Scope.Where(s => !string.IsNullOrEmpty(s)).ToList();

                //Encrypting ClientID
                var customPasswordHashForClient = _serviceProvider.GetRequiredService<ICustomPasswordHash>();
                var encryptedClientId = customPasswordHashForClient.Encrypt(context.Request.ClientId, _encryptionKey);
                identity.AddClaim(OpenIdConnectConstants.Claims.ClientId, encryptedClientId, OpenIdConnectConstants.Destinations.AccessToken);

                if (clientData.ClientExpireOn != null)
                {
                    identity.AddClaim(KeyConstant.ClientExpireOn, clientData.ClientExpireOn.ToString(), OpenIdConnectConstants.Destinations.AccessToken);
                }

                if (clientData.ClientTypeId == (int)ClientTypes.DeviceClient)
                {
                    var deviceBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IDeviceBusiness>();
                    var deviceUser = (Device)deviceBusiness.GetUserByDeviceId(context.Request.Username).GetAwaiter().GetResult().Data;

                    var organizationBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IOrganizationBusiness>();
                    var organization = await organizationBusiness.GetOrganizationByOrgId(deviceUser.OrgId);

                    var applicationUserMappingBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IApplicationUserMappingBusiness>();
                    var deviceAppMapping = await applicationUserMappingBusiness.GetClientIdMappingWithApplicationId(context.Request.ClientId.ToString(), context.Request.ClientSecret.ToString(), organization.OrgName);

                    identity.AddClaim(OpenIdConnectConstants.Claims.Subject, deviceUser.DeviceId, OpenIdConnectConstants.Destinations.AccessToken);

                    if (deviceUser != null && (clientData.ClientTypeId == deviceUser.ClientTypeId || deviceUser.ClientTypeId == (int)ClientTypes.SecurityApiClient))
                    {
                        identity.AddClaim(KeyConstant.OrgId, organization.OrgName, OpenIdConnectConstants.Destinations.AccessToken);

                        var applicationBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IApplicationBusiness>();
                        var application = await applicationBusiness.GetAppById(deviceUser.AppId);
                        //Encrypting Application
                        //var customPasswordHashForApplication = _serviceProvider.GetRequiredService<ICustomPasswordHash>();
                        var encryptedApplication = customPasswordHashForApplication.Encrypt(application.AppName, _encryptionKey);
                        identity.AddClaim(KeyConstant.AppName, encryptedApplication, OpenIdConnectConstants.Destinations.AccessToken);

                        identity.AddClaim(KeyConstant.Client_Type, Enum.GetName(typeof(ClientTypes), deviceUser.ClientTypeId), OpenIdConnectConstants.Destinations.AccessToken);
                    }
                }
                else
                {
                    if (userDetails.ClientTypeIdArray.Contains(clientData.ClientTypeId) || userDetails.ClientTypeIdArray.Contains((int)ClientTypes.SecurityApiClient) || userDetails.RolesArray.Contains(Enum.GetName(typeof(UserRoles), UserRoles.SuperAdmin)))
                    {
                        if (userDetails.ProviderName == ApplicationLevelConstants.IMSAuthProvider)
                        {
                            if (string.IsNullOrEmpty(context.Request.LoginHint))
                            {
                                returnResult = await ValidatePassword(context, userDetails, returnResult);
                                if (!returnResult.Success)
                                    return returnResult;
                            }
                        }
                        var userBusinessForResetLock = _httpContext.HttpContext.RequestServices.GetRequiredService<IUserBusiness>();
                        await userBusinessForResetLock.ResetLock(context.Request.Username.ToString(), (int)LockType.LoginLock);

                        identity.AddClaim(KeyConstant.Subid, userDetails.UserId.ToString(), OpenIdConnectConstants.Destinations.AccessToken);
                        if (userDetails.AppUserId != null)
                            identity.AddClaim(KeyConstant.AppUserId, Convert.ToString(userDetails.AppUserId), OpenIdConnectConstants.Destinations.AccessToken);

                        if (userDetails.AppOrgId != null)
                            identity.AddClaim(KeyConstant.AppOrgId, Convert.ToString(userDetails.AppOrgId), OpenIdConnectConstants.Destinations.AccessToken);

                        if (userDetails.ProviderName == ApplicationLevelConstants.IMSAuthProvider)
                        {
                            identity.AddClaim(KeyConstant.Passwordexpireat, userDetails.PasswordExpiration.ToString(), OpenIdConnectConstants.Destinations.AccessToken);

                            var remainingDays = (userDetails.PasswordExpiration - DateTime.Now).Days;
                            identity.AddClaim(KeyConstant.RemainingDays, remainingDays.ToString(), OpenIdConnectConstants.Destinations.AccessToken);
                        }

                        foreach (var userRole in userDetails.RolesArray)
                        {
                            identity.AddClaim(OpenIdConnectConstants.Claims.Role, userRole, OpenIdConnectConstants.Destinations.AccessToken);
                        }

                        if (clientData.ClientTypeId == (int)ClientTypes.ServiceClient && userDetails.ClientTypeIdArray.Contains((int)ClientTypes.ServiceClient))
                        {
                            var orgId = context.HttpContext.Request.Headers[KeyConstant.OrgId];
                            var organizationBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IOrganizationBusiness>();
                            if (!string.IsNullOrEmpty(orgId))
                            {
                                var organization = await organizationBusiness.GetOrganizationIdByOrgName(orgId);
                                identity.AddClaim(KeyConstant.OrgId, orgId, OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.AccessToken);
                            }
                            else
                            {
                                var organization = await organizationBusiness.GetOrganizationIdByOrgName(userDetails.OrgNameArray[0]);
                                identity.AddClaim(KeyConstant.OrgId, userDetails.OrgNameArray[0], OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.AccessToken);
                            }
                        }
                        else
                        {
                            var organizationBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IOrganizationBusiness>();
                            var organization = await organizationBusiness.GetOrganizationIdByOrgName(userDetails.OrgNameArray[0]);
                            identity.AddClaim(KeyConstant.OrgId, userDetails.OrgNameArray[0], OpenIdConnectConstants.Destinations.AccessToken);
                            identity.AddClaim(KeyConstant.TenantId, Convert.ToString(organization.TenantDB), OpenIdConnectConstants.Destinations.AccessToken);
                        }

                        if (userDetails.AppNameArray != null)
                        {
                            foreach (string appName in userDetails.AppNameArray)
                            {
                                //Encrypting Application
                                var encryptedApplication = customPasswordHashForApplication.Encrypt(appName, _encryptionKey);
                                identity.AddClaim(KeyConstant.AppName, encryptedApplication, OpenIdConnectConstants.Destinations.AccessToken);
                            }
                        }

                        foreach (string c in userDetails.ClientTypeArray)
                        {
                            identity.AddClaim(KeyConstant.Client_Type, c, OpenIdConnectConstants.Destinations.AccessToken);
                        }
                    }
                    else
                    {
                        returnResult.Success = false;
                        context.Reject(
                             error: OpenIdConnectConstants.Errors.InvalidRequest,
                             description: ResourceInformation.GetResValue("ProvideValidUserNamePwd"));
                        return returnResult;
                    }
                }

                foreach (var scope in Scope)
                {
                    identity.AddClaim(OpenIdConnectConstants.Claims.Scope, scope, OpenIdConnectConstants.Destinations.AccessToken);
                }

                bool isFreshLogin = true; ;
                Guid userIdFromCookie = GetUserIdFromCookie(context);

                if (userIdFromCookie != null && Guid.Empty != userIdFromCookie)
                    isFreshLogin = false;

                var props = new AuthenticationProperties(new Dictionary<string, string>
                    {
                      {
                         KeyConstant.Audience, context.Request.ClientId ?? string.Empty
                      },
                      {
                        KeyConstant.AudienceKey, clientData.ClientSecret
                      },
                      {
                        KeyConstant.IsRefreshTokenFlow, bool.FalseString
                      },
                      {
                        KeyConstant.IsFreshLogin, isFreshLogin.ToString()
                      },
                    });

                if (userDetails != null)
                {
                    await userBusiness.FirstTimeLogin(userDetails);
                }

                var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), props, KeyConstant.JWT);
                ticket.SetScopes(OpenIdConnectConstants.Scopes.OfflineAccess);

                context.Validate(ticket);
                if (userDetails != null)
                {
                    var gid = Guid.NewGuid();
                    var tempOptions = _cookieOptions;
                    tempOptions.Expires = DateTimeOffset.Now.AddMinutes(clientData.TokenValidationPeriod * 3);
                    var cookieLoad = customPasswordHashForApplication.Encrypt(userDetails.UserId + "$&$/" + gid, _encryptionKey);
                    context.HttpContext.Response.Cookies.Append("session-id", cookieLoad, tempOptions);
                }
                returnResult.Success = true;
                returnResult.Result = ResourceInformation.GetResValue("Validated");
            }
            catch (Exception ex)
            {
                _logger.Error("CustomOAuthProvider", "PasswordGrantType", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ExceptionLogger.LogException(ex);
            }

            return null;
        }
        /// <summary>
        /// This Method handles token request for client credentials grant type
        /// </summary>
        /// <param name="context"></param>
        /// <param name="returnResult"></param>
        /// <param name="clientData"></param>
        /// <param name="identity"></param>
        /// <param name="enableIPWhitelisting"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public async Task<object> ClientCredentialsGrantTypeAsync(HandleTokenRequestContext context, ReturnResult returnResult, OauthClient clientData,
                                                 ClaimsIdentity identity, bool enableIPWhitelisting, System.Net.IPAddress ipAddress)
        {
            try
            {
                if (returnResult.Success == false)
                {
                    context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: ResourceInformation.GetResValue("InvalidClient"));
                    return returnResult;
                }
                var allowedScopes = clientData.AllowedScopes.Split(' ');
                var Scope = context.Request.Scope.Split(' ');
                var contextScopes = Scope.Where(s => !string.IsNullOrEmpty(s)).ToList();
                if (allowedScopes.Length != contextScopes.Count ||
                    allowedScopes.Any(scope => !contextScopes.Contains(scope)))
                {
                    returnResult.Success = false;
                    context.Reject(
                         error: OpenIdConnectConstants.Errors.InvalidScope,
                         description: ResourceInformation.GetResValue("InvalidScope"));
                    return returnResult;
                }

                //Encrypting ClientID
                var customPasswordHashForClient = _serviceProvider.GetRequiredService<ICustomPasswordHash>();
                var encryptedClientId = customPasswordHashForClient.Encrypt(context.Request.ClientId, _encryptionKey);
                identity.AddClaim(OpenIdConnectConstants.Claims.ClientId, encryptedClientId, OpenIdConnectConstants.Destinations.AccessToken);

                if (clientData.ClientExpireOn != null)
                {
                    identity.AddClaim(KeyConstant.ClientExpireOn, clientData.ClientExpireOn.ToString(), OpenIdConnectConstants.Destinations.AccessToken);
                }

                identity.AddClaim(KeyConstant.Client_Type, Enum.GetName(typeof(ClientTypes), clientData.ClientTypeId), OpenIdConnectConstants.Destinations.AccessToken);
                var applicationBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IApplicationBusiness>();
                var appDetails = await applicationBusiness.GetAppById(clientData.AppId);
                if (appDetails == null)
                {
                    returnResult.Success = false;
                    context.Reject(
                         error: OpenIdConnectConstants.Errors.InvalidScope,
                         description: ResourceInformation.GetResValue("ApplicationDetailsNotFound"));
                    return returnResult;
                }

                var appName = appDetails.AppName;
                //Encrypting Application
                var customPasswordHashForApplication = _serviceProvider.GetRequiredService<ICustomPasswordHash>();
                var encryptedApplication = customPasswordHashForApplication.Encrypt(appName, _encryptionKey);
                identity.AddClaim(KeyConstant.AppName, encryptedApplication, OpenIdConnectConstants.Destinations.AccessToken);

                identity.AddClaim(OpenIdConnectConstants.Claims.Subject, KeyConstant.Client, OpenIdConnectConstants.Destinations.AccessToken);

                var orgId = context.HttpContext.Request.Headers[KeyConstant.OrgId].ToString();

                if (!string.IsNullOrEmpty(orgId))
                {
                    var organizationBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IOrganizationBusiness>();
                    var organizationDetails = await organizationBusiness.GetOrganizationIdByOrgName(orgId);
                    if (organizationDetails == null)
                    {
                        returnResult.Success = false;
                        context.Reject(
                             error: OpenIdConnectConstants.Errors.InvalidScope,
                             description: ResourceInformation.GetResValue("OrganizationNotExists"));
                        return returnResult;
                    }
                    int orgIdfromOrgName = organizationDetails.OrgId;

                    if (enableIPWhitelisting && clientData.ClientTypeId == (int)ClientTypes.ServiceClient)
                    {
                        //check if the Service client's IP address is authorized.
                        IIPTableBusiness iPTableBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IIPTableBusiness>();
                        var isIpAllowed = await iPTableBusiness.IsIPAuthorized(ipAddress, orgIdfromOrgName, clientData.AppId);
                        if (isIpAllowed.Success == false)
                        {
                            returnResult.Success = false;
                            context.Reject(
                            error: OpenIdConnectConstants.Errors.InvalidRequest,
                            description: ResourceInformation.GetResValue("UnauthorizedAccessException") + " Your IP:" + ipAddress.ToString());
                            return returnResult;
                        }
                    }
                    var applicationUserMappingBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IApplicationUserMappingBusiness>();
                    var userAppMapping = await applicationUserMappingBusiness.GetClientIdMappingWithApplicationId(context.Request.ClientId.ToString(), context.Request.ClientSecret.ToString(), orgId);
                    if (userAppMapping == null)
                    {
                        returnResult.Success = false;
                        context.Reject(
                             error: OpenIdConnectConstants.Errors.InvalidRequest,
                             description: ResourceInformation.GetResValue("InvalidData"));
                        return returnResult;
                    }
                }

                if (clientData.ClientTypeId == (int)ClientTypes.KioskClient)
                {
                    if (string.IsNullOrEmpty(orgId))
                    {
                        returnResult.Success = false;
                        context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidClient,
                        description: ResourceInformation.GetResValue("InvalidData"));
                        return returnResult;
                    }

                    var tokenData = new TokenData() { Role = new[] { UserRoles.SuperAdmin.ToString() }, UserClientTypes = new[] { Enum.GetName(typeof(ClientTypes), 1) }, OrgId = _applicationSettings.Value.MstOrg };
                    var organizationBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IOrganizationBusiness>();
                    var org = (await organizationBusiness.GetByOrgName(tokenData, orgId)).Data;
                    if (org == null)
                    {
                        returnResult.Success = false;
                        context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidRequest,
                        description: ResourceInformation.GetResValue("OrganizationNotExists"));
                        return returnResult;
                    }
                }

                if (!string.IsNullOrEmpty(orgId))
                {
                    identity.AddClaim(KeyConstant.OrgId, orgId, OpenIdConnectConstants.Destinations.AccessToken);
                }

                foreach (var scope in Scope)
                {
                    identity.AddClaim(OpenIdConnectConstants.Claims.Scope, scope, OpenIdConnectConstants.Destinations.AccessToken);
                }
                var props = new AuthenticationProperties(new Dictionary<string, string>
                    {
                        {
                             KeyConstant.Audience, context.Request.ClientId ?? string.Empty
                        },
                        {
                            KeyConstant.AudienceKey, clientData.ClientSecret
                        },
                        {
                            KeyConstant.IsRefreshTokenFlow,bool.FalseString
            }
                    });

                var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), props, KeyConstant.JWT);

                ticket.SetScopes(OpenIdConnectConstants.Scopes.OfflineAccess);

                context.Validate(ticket);

                returnResult.Success = true;
                returnResult.Result = ResourceInformation.GetResValue("Validated");
            }
            catch (Exception ex)
            {
                _logger.Error("CustomOAuthProvider", "ClientCredentialsGrantType", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ExceptionLogger.LogException(ex);
            }
            return null;
        }
        /// <summary>
        /// This Method handles token request for refresh token grant type
        /// </summary>
        /// <param name="context"></param>
        /// <param name="returnResult"></param>
        /// <returns></returns>
        public Task RefreshTokenGrantType(HandleTokenRequestContext context, ReturnResult returnResult, OauthClient clientData)
        {
            try
            {
                var originalClient = context.Ticket.Properties.Items[KeyConstant.Audience];
                context.Ticket.Properties.Items[KeyConstant.IsRefreshTokenFlow] = bool.TrueString;
                var currentClient = context.Request.ClientId;

                if (originalClient != currentClient)
                {
                    returnResult.Success = false;
                    context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: ResourceInformation.GetResValue("invalid_clientId"));
                    return Task.FromResult<object>(returnResult);
                }
                var newPrincipal = new ClaimsPrincipal(context.Ticket.Principal);

                var newTicket = new AuthenticationTicket(newPrincipal, context.Ticket.Properties, KeyConstant.JWT);
                newTicket.SetScopes(OpenIdConnectConstants.Scopes.OfflineAccess);

                if (clientData.Flow == KeyConstant.AuthCode)
                {
                    //context.HttpContext.Response.Cookies.Delete("session-id", _cookieOptions);
                    var customPasswordHashForApplication = _serviceProvider.GetRequiredService<ICustomPasswordHash>();
                    var gid = Guid.NewGuid();
                    Guid userIdFromCookie = GetUserIdFromCookie(context);
                    var tempOptions = _cookieOptions;
                    tempOptions.Expires = DateTimeOffset.Now.AddMinutes(clientData.TokenValidationPeriod * 3);
                    var cookieLoad = customPasswordHashForApplication.Encrypt(userIdFromCookie.ToString() + "$&$/" + gid.ToString(), _encryptionKey);
                    context.HttpContext.Response.Cookies.Append("session-id", cookieLoad, tempOptions);
                }

                context.Validate(newTicket);
            }
            catch (Exception ex)
            {
                _logger.Error("CustomOAuthProvider", "RefreshTokenGrantType", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ExceptionLogger.LogException(ex);
            }

            return Task.FromResult<object>(null);
        }
        /// <summary>
        /// This Method is to check that if the Auth Provider is Azure Ad then check whether the given user is valid Azure Ad user or not
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns>return true if valid Azure Ad account other wise false</returns>
        public Task<bool> IsValidAzureAdAccount(string userName, string password)
        {
            HttpClient client = new HttpClient();
            //Token token = new Token();
            var values = new Dictionary<string, string>
                            {
                                { "resource", _applicationSettings.Value.AzurAdSettings.ResourceId },
                                { "client_id", _applicationSettings.Value.AzurAdSettings.ClientId },
                                { "grant_type", ApplicationLevelConstants.AzureAdGrant_Type },
                                { "username", userName },
                                { "password", password },
                                { "scope", ApplicationLevelConstants.AzureAdScope },
                            };
            var content = new FormUrlEncodedContent(values);
            var data = client.PostAsync(_applicationSettings.Value.AzurAdSettings.TokenUrl, content).GetAwaiter().GetResult();
            if (data.IsSuccessStatusCode)
                return Task.FromResult(true);
            else
                return Task.FromResult(false);
        }
        public override Task HandleLogoutRequest(HandleLogoutRequestContext context)
        {
            //var userId = GetUserIdFromCookie(context);
            //var authorizatonToken = context.HttpContext.Request.Headers;
            //if (authorizatonToken.ContainsKey("Authorization") && !string.IsNullOrEmpty(authorizatonToken["Authorization"]))
            //{
            //    string tokenUserId = string.Empty;
            //    string accessToken = authorizatonToken["Authorization"].ToString().Replace("Bearer ", "");
            //    var jwtHandler = new JwtSecurityTokenHandler();
            //    var tokenDetails = jwtHandler.ReadToken(accessToken) as JwtSecurityToken;
            //    if (tokenDetails.Payload.ContainsKey(KeyConstant.Subid))
            //    {
            //        tokenUserId = tokenDetails.Payload.Single(c => c.Key == KeyConstant.Subid).Value.ToString();
            //        if (tokenUserId.ToLower() == userId.ToString().ToLower())
            //            context.HttpContext.Response.Cookies.Delete("session-id", _cookieOptions);
            //    }
            //}

            context.HttpContext.Response.Cookies.Delete("session-id", _cookieOptions);
            return base.HandleLogoutRequest(context);
        }
        /// <summary>
        /// To serialize and generate refresh token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task SerializeRefreshToken(SerializeRefreshTokenContext context)
        {
            ReturnResult returnResult = new ReturnResult();
            DateTime expiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_applicationSettings.Value.RefreshTokenExpireTimeSpanInMinutes));
            try
            {
                var refreshToken = new RefreshToken();
                var clientBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IClientBusiness>();
                var clientDataForRefreshToken = await clientBusiness.GetById(context.Request.ClientId);
                if (clientDataForRefreshToken != null)
                {
                    if (clientDataForRefreshToken.TokenValidationPeriod != 0 && clientDataForRefreshToken.Flow.Equals(KeyConstant.AuthCode, StringComparison.OrdinalIgnoreCase))
                    {
                        var refreshTokenExpiry = TimeSpan.FromMinutes(Convert.ToInt32(clientDataForRefreshToken.TokenValidationPeriod * 3));
                        expiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToInt32(refreshTokenExpiry.TotalMinutes));
                    }
                    refreshToken.ClientId = clientDataForRefreshToken.ClientId;
                }
                // copy all properties and set the desired lifetime of refresh token  
                var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Items)
                {
                    IssuedUtc = context.Ticket.Properties.IssuedUtc,
                    ExpiresUtc = expiresUtc
                };

                var ticket = new AuthenticationTicket(context.Ticket.Principal, refreshTokenProperties, context.Scheme.Name);

                var ticketSerializer = new TicketSerializer();
                var byteArray = ticketSerializer.Serialize(ticket);

                var orgName = context.Ticket.Principal.Claims.FirstOrDefault(c => c.Type == KeyConstant.OrgId);

                if (orgName != null)
                {
                    //var clientDetails =await _clientBusiness.GetById(context.Request.ClientId.ToString());
                    var organizationBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IOrganizationBusiness>();
                    var organization = await organizationBusiness.GetOrganizationIdByOrgName(orgName.Value.ToString());
                    if (organization != null)
                    {
                        if (context.Ticket.Principal.Claims.FirstOrDefault(c => c.Type == KeyConstant.Subid) == null)
                        {
                            refreshToken.RefreshTokenId = Guid.NewGuid();
                            refreshToken.RefreshAuthenticationTicket = byteArray;
                            refreshToken.TokenExpirationDateTime = expiresUtc;
                            refreshToken.OrgId = organization.OrgId;
                            refreshToken.AppId = clientDataForRefreshToken.AppId;
                        }
                        else
                        {
                            var userId = context.Ticket.Principal.Claims.FirstOrDefault(c => c.Type == KeyConstant.Subid).Value;
                            refreshToken.RefreshTokenId = Guid.NewGuid();
                            refreshToken.RefreshAuthenticationTicket = byteArray;
                            refreshToken.TokenExpirationDateTime = expiresUtc;
                            refreshToken.OrgId = organization.OrgId;
                            refreshToken.UserId = new Guid(userId);
                            refreshToken.AppId = clientDataForRefreshToken.AppId;
                        }
                    }
                    else
                    {
                        refreshToken.RefreshTokenId = Guid.NewGuid();
                        refreshToken.RefreshAuthenticationTicket = byteArray;
                        refreshToken.TokenExpirationDateTime = expiresUtc;
                    }
                }
                else
                {
                    refreshToken.RefreshTokenId = Guid.NewGuid();
                    refreshToken.RefreshAuthenticationTicket = byteArray;
                    refreshToken.TokenExpirationDateTime = expiresUtc;
                }
                IRefreshTokenBusiness refreshTokenBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IRefreshTokenBusiness>();
                await refreshTokenBusiness.Save(refreshToken);
                context.RefreshToken = refreshToken.RefreshTokenId.ToString();

                context.HandleSerialization();
            }
            catch (Exception ex)
            {
                _logger.Error("CustomOAuthProvider", "SerializeRefreshToken", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ExceptionLogger.LogException(ex);
            }
        }
        /// <summary>
        /// To deserialize refresh token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task DeserializeRefreshToken(DeserializeRefreshTokenContext context)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var refreshTokenBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IRefreshTokenBusiness>();
                var refreshToken = (RefreshToken)refreshTokenBusiness.Get(Guid.Parse(context.RefreshToken.ToString())).GetAwaiter().GetResult().Data;
                if (refreshToken != null)
                {
                    var ticketSerializer = new TicketSerializer();
                    var ticket = ticketSerializer.Deserialize(refreshToken.RefreshAuthenticationTicket);
                    var clientBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IClientBusiness>();
                    var clientDetails = await clientBusiness.GetById(context.Request.ClientId);
                    var client = clientDetails.IsActive.Value ? clientDetails : null;
                    if (client != null)
                    {
                        if (client.DeleteRefreshToken.Value)
                        {
                            //IRefreshTokenBusiness refreshTokenBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IRefreshTokenBusiness>();
                            await refreshTokenBusiness.Delete(refreshToken.RefreshTokenId);
                        }
                    }
                    context.Ticket = ticket;

                    context.HandleDeserialization();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("CustomOAuthProvider", "DeserializeRefreshToken", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ExceptionLogger.LogException(ex);
            }
        }

        public override async Task DeserializeAuthorizationCode(DeserializeAuthorizationCodeContext context)
        {
            if (context.AuthorizationCode != null)
            {
                var identity = new ClaimsIdentity(OpenIdConnectServerDefaults.AuthenticationScheme);
                identity.AddClaim(OpenIdConnectConstants.Claims.Subject, context.Request.Username, OpenIdConnectConstants.Destinations.IdentityToken);

                var authProp = new AuthenticationProperties(new Dictionary<string, string>
                    {
                      {
                         KeyConstant.ClientId, context.Request.ClientId
                      },
                      {
                         KeyConstant.UserName, context.Request.Username
                      },
                       {
                        KeyConstant.sts, context.Request.ClientSecret
                      },
                    {
                        OpenIdConnectConstants.Properties.Presenters, "[\"" + context.Request.ClientId + "\"]"
                    },
                    });
                authProp.ExpiresUtc = DateTime.UtcNow.AddMinutes(_applicationSettings.Value.CodeExpire);
                var tckt = new AuthenticationTicket(new ClaimsPrincipal(identity), authProp, OpenIdConnectServerDefaults.AuthenticationScheme);
                context.Ticket = tckt;
                context.HandleDeserialization();
            }
        }

        public async Task<ReturnResult> ValidatePassword(BaseValidatingContext context, UserDetails userDetails, ReturnResult returnResult)
        {
            string password = context.Request.Password;
            CustomPasswordHash customPasswordHash = new CustomPasswordHash(_applicationSettings);
            CustomPasswordCheck customPasswordCheck = new CustomPasswordCheck(_applicationSettings, _logger);
            bool pwdMatch;
            pwdMatch = customPasswordHash.ScryptHashStringVerify(userDetails.PasswordHash, password.Trim());

            if (!pwdMatch)
            {
                _logger.Error("CustomOAuthProvider", "Password Check first", userDetails.PasswordHash + ":" + password.Trim(), string.Empty);
                //   using (CustomPasswordCheck customPasswordCheck = new CustomPasswordCheck(_applicationSettings, _logger))
                //  {
                if (!customPasswordCheck.CustomPwdCheck(userDetails.PasswordHash, password.Trim()))
                {
                    _logger.Error("CustomOAuthProvider", "Password check second", userDetails.PasswordHash + ":" + password.Trim(), string.Empty);
                    var userBusinessForLockAccount = _httpContext.HttpContext.RequestServices.GetRequiredService<IUserBusiness>();
                    returnResult = await userBusinessForLockAccount.LockAccount(context.Request.Username, (int)LockType.LoginLock);
                    returnResult.Success = false;
                    string errorMessage = ResourceInformation.GetResValue("ProvideValidUserNamePwd") + ", " + returnResult.Result;
                    returnResult.Result = errorMessage;
                    context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidRequest,
                    description: errorMessage);
                    customPasswordCheck.Dispose();
                    return returnResult;
                }
                customPasswordCheck.Dispose();
                //  }

            }
            else
            {
                customPasswordHash.Dispose();
            }
            //}
            returnResult.Success = true;
            var userBusinessForResetLock = _httpContext.HttpContext.RequestServices.GetRequiredService<IUserBusiness>();
            await userBusinessForResetLock.ResetLock(context.Request.Username.ToString(), (int)LockType.LoginLock);
            return returnResult;

            //customPasswordHash = null;
        }

        public async Task<ReturnResult> TokenRequestValidation(BaseValidatingContext context, ReturnResult returnResult, string clientSecret)
        {
            string clientId = context.Request.ClientId;
            if (clientId == null)
            {
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("InvalidClient");
                context.Reject(
                error: OpenIdConnectConstants.Errors.InvalidClient,
                description: ResourceInformation.GetResValue("InvalidClient"));
                return returnResult;
            }

            var grantType = context.Request.GrantType;
            if (string.IsNullOrEmpty(grantType))
            {
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("unsupported_grant_type");
                context.Reject(
                error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                description: ResourceInformation.GetResValue("unsupported_grant_type"));
                return returnResult;
            }

            //var clientBusiness = new ClientBusiness(_clientRepository, _userRepository, _customPasswordHash, _logger);
            var clientBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IClientBusiness>();
            var clientData = await clientBusiness.GetById(clientId);
            var doesRefreshTokenReq = context.Request.GetParameters().Any(p => p.Key == KeyConstant.GrantType && p.Value[0] == KeyConstant.RefreshToken);
            if (clientData == null || (!doesRefreshTokenReq && clientData.ClientSecret != clientSecret))
            {
                if (clientData != null && grantType != KeyConstant.RefreshToken)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidClient");
                    context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: ResourceInformation.GetResValue("InvalidClient"));
                    return returnResult;
                }

            }

            if (KeyConstant.AuthCode != clientData.Flow && grantType != KeyConstant.RefreshToken)
            {
                var allowedScopes = clientData.AllowedScopes.Split(' ');
                var Scope = context.Request.Scope.Split(' ');
                var contextScopes = Scope.Where(s => !string.IsNullOrEmpty(s)).ToList();
                if (allowedScopes.Length != contextScopes.Count ||
                    allowedScopes.Any(scope => !contextScopes.Contains(scope)))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidScope");
                    context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidScope,
                    description: ResourceInformation.GetResValue("InvalidScope"));
                    return returnResult;
                }
            }

            //Added request url validation for token generation
            if (grantType == KeyConstant.RefreshToken || grantType == KeyConstant.AuthCode)
            {
                //Check client application's URL for generating token

                if (clientData.Flow == KeyConstant.AuthCode)
                {
                    string referer = new Uri(Convert.ToString(context.HttpContext.Request.Headers["Origin"])).AbsoluteUri;
                    //if (context.Request.ClientSecret != clientData.ClientSecret)
                    //{
                    //    returnResult.Success = false;
                    //    context.Reject(
                    //    error: OpenIdConnectConstants.Errors.AccessDenied,
                    //    description: ResourceInformation.GetResValue("InvalidClient"));
                    //    return returnResult;
                    //}
                    if (referer != clientData.RedirectURL && referer != clientData.DebugURL)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("InvalidRequestURL");
                        context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidRequest,
                        description: ResourceInformation.GetResValue("InvalidRequestURL"));
                        return returnResult;
                    }
                }

            }
            if (clientData.Flow != grantType && grantType != KeyConstant.RefreshToken)
            {
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("InvalidGrant");
                context.Reject(
                error: OpenIdConnectConstants.Errors.InvalidClient,
                description: ResourceInformation.GetResValue("InvalidGrant"));
                return returnResult;
            }

            if (clientData.ClientExpireOn != null && clientData.ClientExpireOn < DateTime.Now)
            {
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("ClientExpired");
                context.Reject(
                error: OpenIdConnectConstants.Errors.InvalidClient,
                description: ResourceInformation.GetResValue("ClientExpired"));
                return returnResult;
            }

            if (clientData.Flow == KeyConstant.AuthCode && context.Request.IsRefreshTokenGrantType())
            {
                //if (context.Request.ClientSecret != clientData.ClientSecret)
                //{
                //    returnResult.Success = false;
                //    context.Reject(
                //    error: OpenIdConnectConstants.Errors.AccessDenied,
                //    description: ResourceInformation.GetResValue("InvalidClient"));
                //    return returnResult;
                //}

                //if (string.IsNullOrEmpty(context.HttpContext.Request.Cookies["session-id"]))
                //{
                //    returnResult.Success = false;
                //    returnResult.Result = ResourceInformation.GetResValue("UnauthorizedUser");
                //    context.Reject(
                //    error: OpenIdConnectConstants.Errors.InvalidRequest,
                //    description: ResourceInformation.GetResValue("UnauthorizedUser"));
                //    return returnResult;
                //}
            }

            //var tempOptions = _cookieOptions;
            //tempOptions.Expires = DateTimeOffset.Now.AddMinutes(clientData.TokenValidationPeriod + 60);
            //if (context.HttpContext.Request.Cookies.Count() > 0 && context.HttpContext.Request.Cookies["session-id"] != null)
            //{
            //    context.HttpContext.Response.Cookies.Append("session-id", context.HttpContext.Request.Cookies["session-id"], tempOptions);
            //}
            returnResult.Success = true;
            return returnResult;
        }

        public async Task<ReturnResult> PasswordGrantTypeValidation(BaseValidatingContext context, ReturnResult returnResult, UserDetails userDetails, OauthClient clientData, IPAddress ipAddress)
        {
            var enableIPWhitelisting = _applicationSettings.Value.EnableIPWhitelisting;
            CustomPasswordHash customPasswordHash = new CustomPasswordHash(_applicationSettings);
            var customPasswordHashForApplication = _serviceProvider.GetRequiredService<ICustomPasswordHash>();

            if (userDetails != null)
            {

                if (userDetails.ProviderName == ApplicationLevelConstants.AzureAdAuthProvider)
                {
                    var isValidAzureAd = await IsValidAzureAdAccount(context.Request.Username, context.Request.Password);
                    if (!isValidAzureAd)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("UnAuthorisedAccess");
                        context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidRequest,
                        description: ResourceInformation.GetResValue("UnAuthorisedAccess"));
                        return returnResult;
                    }
                }

                if (userDetails.ProviderName == ApplicationLevelConstants.IMSAuthProvider)
                {
                    //check if user's password has expired.
                    if (userDetails.PasswordExpiration.Date < DateTime.Now.Date)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("UserPasswordExpired");
                        context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidRequest,
                        description: ResourceInformation.GetResValue("UserPasswordExpired"));
                        return returnResult;
                    }
                }
                if (userDetails.IsAccLock)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("AccountLocked");
                    context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidRequest,
                    description: ResourceInformation.GetResValue("AccountLocked"));
                    return returnResult;
                }

                if (!userDetails.RolesArray.Contains(Enum.GetName(typeof(UserRoles), UserRoles.SuperAdmin)))
                {
                    //var applicationUserMappingBusiness = _serviceProvider.GetRequiredService<IApplicationUserMappingBusiness>();
                    //var userAppMapping = await applicationUserMappingBusiness.GetUserMappingWithApplicationId(userDetails.UserId.ToString(), context.Request.ClientId.ToString(), context.Request.ClientSecret.ToString());
                    //if (userAppMapping == null)
                    //{
                    //    returnResult.Success = false;
                    //    returnResult.Result = ResourceInformation.GetResValue("NoUserApplicationMappingFound");
                    //    context.Reject(
                    //    error: OpenIdConnectConstants.Errors.InvalidRequest,
                    //    description: ResourceInformation.GetResValue("NoUserApplicationMappingFound"));
                    //    return returnResult;

                    //}
                }

            }

            if (clientData.ClientTypeId == (int)ClientTypes.DeviceClient)
            {
                var deviceBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IDeviceBusiness>();
                var deviceUser = (Device)deviceBusiness.GetUserByDeviceId(context.Request.Username).GetAwaiter().GetResult().Data;

                if (deviceUser == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DeviceNotExist");
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidRequest,
                        description: ResourceInformation.GetResValue("DeviceNotExist"));
                    return returnResult;
                }

                if (enableIPWhitelisting)
                { //check if the device client's IP address is authorized.
                    IIPTableBusiness iPTableBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IIPTableBusiness>();

                    var isIpAllowed = await iPTableBusiness.IsIPAuthorized(ipAddress, deviceUser.OrgId, deviceUser.AppId);
                    if (isIpAllowed.Success == false)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("UnauthorizedAccessException");
                        context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidRequest,
                        description: ResourceInformation.GetResValue("UnauthorizedAccessException") + " Your IP:" + ipAddress.ToString());
                        return returnResult;
                    }
                }
                var organizationBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IOrganizationBusiness>();
                var organization = await organizationBusiness.GetOrganizationByOrgId(deviceUser.OrgId);
                if (organization == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("OrganizationNotExists");
                    context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidRequest,
                    description: ResourceInformation.GetResValue("OrganizationNotExists"));
                    return returnResult;
                }

                var applicationUserMappingBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IApplicationUserMappingBusiness>();
                var deviceAppMapping = await applicationUserMappingBusiness.GetClientIdMappingWithApplicationId(context.Request.ClientId.ToString(), context.Request.ClientSecret.ToString(), organization.OrgName);
                if (deviceAppMapping == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidGrant");
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidRequest,
                        description: ResourceInformation.GetResValue("InvalidGrant"));
                    return returnResult;
                }

                if (deviceUser != null && (clientData.ClientTypeId == deviceUser.ClientTypeId || deviceUser.ClientTypeId == (int)ClientTypes.SecurityApiClient))
                {
                    if (deviceUser.PrimaryKey != customPasswordHash.Encrypt(context.Request.Password, deviceUser.Subject.ToString()))
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("IncorrectDeviceIdKey");
                        context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidRequest,
                        description: ResourceInformation.GetResValue("IncorrectDeviceIdKey"));
                        return returnResult;
                    }

                    var applicationBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IApplicationBusiness>();
                    var application = await applicationBusiness.GetAppById(deviceUser.AppId);
                    //Encrypting Application
                    //var customPasswordHashForApplication = _serviceProvider.GetRequiredService<ICustomPasswordHash>();
                    var encryptedApplication = customPasswordHashForApplication.Encrypt(application.AppName, _encryptionKey);

                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidData");
                    context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidRequest,
                    description: ResourceInformation.GetResValue("InvalidData"));
                    return returnResult;
                }
            }
            else if (userDetails == null)
            {
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("ProvideValidUserNamePwd");
                context.Reject(
                      error: OpenIdConnectConstants.Errors.InvalidRequest,
                      description: ResourceInformation.GetResValue("ProvideValidUserNamePwd"));
                return returnResult;
            }

            else
            {
                //var user = await _userBusiness.GetUserByUserName(context.Request.Username);

                //var organization = await _organizationBusiness.GetOrganizationByOrgId(userDetails.OrgId);

                if (userDetails.ClientTypeIdArray.Contains(clientData.ClientTypeId) || userDetails.ClientTypeIdArray.Contains((int)ClientTypes.SecurityApiClient) || userDetails.RolesArray.Contains(Enum.GetName(typeof(UserRoles), UserRoles.SuperAdmin)))
                {
                    if (userDetails.ProviderName == ApplicationLevelConstants.IMSAuthProvider)
                    {
                        if (string.IsNullOrEmpty(context.Request.LoginHint))
                        {
                            returnResult = await ValidatePassword(context, userDetails, returnResult);
                            if (!returnResult.Success)
                                return returnResult;
                        }
                    }

                    if (clientData.ClientTypeId == (int)ClientTypes.ServiceClient && userDetails.ClientTypeIdArray.Contains((int)ClientTypes.ServiceClient))
                    {
                        #region IP Authorization for Service client with UserPassword Flow

                        if (enableIPWhitelisting)
                        {
                            //check if the Service client with GrantType as Password's IP address is authorized.
                            IIPTableBusiness iPTableBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IIPTableBusiness>();
                            var isIpAllowed = await iPTableBusiness.IPAuthorizedForServiceClient(ipAddress, userDetails.OrgId.Value, userDetails.AppId);
                            if (isIpAllowed.Success == false)
                            {
                                returnResult.Success = false;
                                returnResult.Result = ResourceInformation.GetResValue("UnauthorizedAccessException") + " Your IP:" + ipAddress.ToString();
                                context.Reject(
                                error: OpenIdConnectConstants.Errors.InvalidRequest,
                                description: ResourceInformation.GetResValue("UnauthorizedAccessException") + " Your IP:" + ipAddress.ToString());
                                return returnResult;
                            }
                        }
                        #endregion

                        var orgId = context.HttpContext.Request.Headers[KeyConstant.OrgId];
                        var organizationBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IOrganizationBusiness>();
                        if (!string.IsNullOrEmpty(orgId))
                        {
                            var organization = await organizationBusiness.GetOrganizationIdByOrgName(orgId);
                            if (organization == null)
                            {
                                returnResult.Success = false;
                                returnResult.Result = ResourceInformation.GetResValue("OrganizationNotExists");
                                context.Reject(
                                error: OpenIdConnectConstants.Errors.InvalidRequest,
                                description: ResourceInformation.GetResValue("OrganizationNotExists"));
                                return returnResult;
                            }

                            //identity.AddClaim(KeyConstant.OrgId, orgId, OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.AccessToken);
                        }
                        else
                        {
                            var organization = await organizationBusiness.GetOrganizationIdByOrgName(userDetails.OrgNameArray[0]);
                            if (organization == null)
                            {
                                returnResult.Success = false;
                                returnResult.Result = ResourceInformation.GetResValue("OrganizationNotExists");
                                context.Reject(
                                error: OpenIdConnectConstants.Errors.InvalidRequest,
                                description: ResourceInformation.GetResValue("OrganizationNotExists"));
                                return returnResult;
                            }
                            //identity.AddClaim(KeyConstant.OrgId, userDetails.OrgNameArray[0], OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.AccessToken);
                        }
                    }
                    else
                    {
                        var organizationBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IOrganizationBusiness>();
                        var organization = await organizationBusiness.GetOrganizationIdByOrgName(userDetails.OrgNameArray[0]);
                        if (organization == null)
                        {
                            if (string.IsNullOrEmpty(context.Request.LoginHint))
                            {
                                returnResult.Success = false;
                                returnResult.Result = ResourceInformation.GetResValue("OrganizationNotExists");
                                context.Reject(
                                error: OpenIdConnectConstants.Errors.InvalidRequest,
                                description: ResourceInformation.GetResValue("OrganizationNotExists"));
                                return returnResult;
                            }
                            else
                            {
                                returnResult.Success = false;
                                returnResult.Result = ResourceInformation.GetResValue("OrganizationNotExists");
                                return returnResult;
                            }
                        }
                        //identity.AddClaim(KeyConstant.OrgId, userDetails.OrgNameArray[0], OpenIdConnectConstants.Destinations.AccessToken);
                    }
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidUserNamePwd");
                    context.Reject(
                         error: OpenIdConnectConstants.Errors.InvalidRequest,
                         description: ResourceInformation.GetResValue("ProvideValidUserNamePwd"));
                    return returnResult;
                }
            }
            returnResult.Success = true;
            return returnResult;
        }
        #endregion

        private Guid GetUserIdFromCookie(BaseValidatingContext context)
        {
            if (context.HttpContext.Request.Cookies.Count() > 0 && !string.IsNullOrEmpty(context.HttpContext.Request.Cookies["session-id"]))
            {
                string cookieLoad = _customPasswordHash.Decrypt(context.HttpContext.Request.Cookies["session-id"], _applicationSettings.Value.Eck).ToString();
                var userId = cookieLoad.Split("$&$/")[0];

                Guid id;
                if (Guid.TryParse(userId, out id))
                {
                    return id;
                }
            }
            return Guid.Empty;
        }

    }
}


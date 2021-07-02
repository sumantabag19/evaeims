using EVA.EIMS.Business;
using EVA.EIMS.Common;
using EVA.EIMS.Common.Constants;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Helper;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Extensions;
using EVA.EIMS.Helper.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using EVA.EIMS.Entity;
using System.Threading.Tasks;
using EVA.EIMS.Contract.Business;
using System.Collections.Generic;
using EVA.EIMS.Entity.ComplexEntities;
using EVA.EIMS.Logging;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace AuthorizationServer.Api.Formats
{
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        #region Private variables
        private readonly string _issuer;
        private readonly IOptions<ApplicationSettings> _applicationSettings;
        private readonly ILogger _logger;
        private readonly ICustomPasswordHash _customPasswordHash;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContext;
        #endregion

        #region Public Constructor
        public string Issuer { get { return _issuer; } }
        public CustomJwtFormat(string issuer, ServiceProvider provider)
        {
            _issuer = issuer;
            _httpContext = provider.GetRequiredService<IHttpContextAccessor>();
            _applicationSettings = provider.GetRequiredService<IOptions<ApplicationSettings>>();
            _customPasswordHash = provider.GetRequiredService<ICustomPasswordHash>();
            _logger = provider.GetRequiredService<ILogger>();
            _hostingEnvironment = provider.GetRequiredService<IHostingEnvironment>();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method is used to generate JWT  AuthToken and AccessToken
        /// </summary>
        /// <param name="data">data</param>
        /// <returns></returns>
        public string Protect(AuthenticationTicket data)
        {
            try
            {
                // if authentication ticket properties does contains audience key then we consider this as auth flow other wise it is access token flow
                if (!data.Properties.Items.ContainsKey(KeyConstant.Audience))
                {
                    var identity = new ClaimsIdentity(KeyConstant.JWT);
                    var handler = new JwtSecurityTokenHandler();
                    string jwt;
                    var client_id = data.Properties.Items.FirstOrDefault(c => c.Key == KeyConstant.ClientId).Value;
                    var username = data.Properties.Items.FirstOrDefault(c => c.Key == KeyConstant.UserName).Value;
                    var client_sts = data.Properties.Items.FirstOrDefault(c => c.Key == KeyConstant.sts).Value;
                    var encryptclient_id = client_id.ToString();
                    var encryptusername = username.ToString();
                    if (client_sts == null)
                    {

                        jwt = KeyConstant.InvalidUserCode;
                        return jwt;
                    }
                    var encryptsts = client_sts.ToString();
                    identity.AddClaim(KeyConstant.ClientId, encryptclient_id, OpenIdConnectConstants.Destinations.IdentityToken);
                    identity.AddClaim(KeyConstant.UserName, encryptusername, OpenIdConnectConstants.Destinations.IdentityToken);
                    identity.AddClaim(KeyConstant.sts, encryptsts, OpenIdConnectConstants.Destinations.IdentityToken);
                    var token = new JwtSecurityToken(null, null, identity.Claims, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(_applicationSettings.Value.CodeExpire));
                    jwt = _customPasswordHash.Encrypt(handler.WriteToken(token), _applicationSettings.Value.Eck).ToString();
                    return jwt;
                }
                else
                {
                    DateTime? clientExpireOn = null;
                    string claimClient;
                    string encryptAppName;
                    if (data == null)
                        throw new ArgumentNullException(nameof(data));

                    var audienceId = data.Properties.Items.ContainsKey(KeyConstant.Audience)
                        ? data.Properties.Items[KeyConstant.Audience]
                        : null;

                    if (string.IsNullOrWhiteSpace(audienceId))
                        throw new InvalidOperationException("AuthenticationTicket.Properties does not include audience");

                    var IsRefreshTokenflow = data.Properties.Items[KeyConstant.IsRefreshTokenFlow];

                    var audienceKey = data.Properties.Items[KeyConstant.AudienceKey];

                    var keyByteArray = new SymmetricSecurityKey(WebEncoders.Base64UrlDecode(audienceKey));

                    //var signingKey = new SigningCredentials(keyByteArray, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

                    //Sign token using private key
                    string privateKeyPath = _hostingEnvironment.ContentRootPath + Path.DirectorySeparatorChar + _applicationSettings.Value.PrivateKeyPath;
                    SigningCredentials signingKey;
                    //using (RSA privateRsa = RSAHelper.PrivateKeyFromPemFile(_applicationSettings.Value.PrivateKeyPath))
                    RSA privateRsa = RSAHelper.PrivateKeyFromPemFile(privateKeyPath);
                    var privateKey = new RsaSecurityKey(privateRsa);
                    signingKey = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);

                    var issued = data.Properties.IssuedUtc;
                    var expires = data.Properties.ExpiresUtc;

                    if (issued == null || expires == null)
                        throw new ArgumentNullException($"data.Properties.IssuedUtc & data.Properties.ExpiresUtc");
                    var claimClientType = data.Principal.Claims.FirstOrDefault(c => c.Type == KeyConstant.Client_Type);


                    var claimOrg = data.Principal.Claims.FirstOrDefault(c => c.Type == KeyConstant.OrgId);
                    var handler = new JwtSecurityTokenHandler();
                    string jwt;
                    if (claimClientType != null && claimOrg == null &&
                        claimClientType.Value.ToString() == Enum.GetName(typeof(ClientTypes), ClientTypes.HadoopClient))
                    {
                        var orgIdentity = new ClaimsIdentity(KeyConstant.JWT);

                        var encryptClientId = data.Principal.Claims.FirstOrDefault(c => c.Type == KeyConstant.ClientId);

                        if (encryptClientId != null)
                        {
                            claimClient = _customPasswordHash.Decrypt(encryptClientId.Value.ToString(), _applicationSettings.Value.Eck).ToString();
                            var clientBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IClientBusiness>();
                            var clientDetails = clientBusiness.GetById(claimClient).GetAwaiter().GetResult();

                            if (clientDetails != null && clientDetails.ClientExpireOn != null)
                            {
                                clientExpireOn = clientDetails.ClientExpireOn.Value;
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException(ResourceInformation.GetResValue("ClientNotFound"));
                        }
                        var organizationBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IOrganizationBusiness>();
                        List<Organization> orgList = organizationBusiness.GetAllActiveOrganizationByClientId(claimClient).GetAwaiter().GetResult();

                        if (orgList.Count() == 0)
                        {
                            throw new InvalidOperationException(ResourceInformation.GetResValue("ClientNoAccessToAnyOrg"));
                        }
                        var applicationBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IApplicationBusiness>();
                        var appName = applicationBusiness.GetAppNameFromClientId(claimClient).GetAwaiter().GetResult();

                        if (appName != null)
                        {
                            encryptAppName = _customPasswordHash.Encrypt(appName.ToString(), _applicationSettings.Value.Eck).ToString();
                        }
                        else
                            throw new InvalidOperationException(ResourceInformation.GetResValue("ApplicationDetailsNotFound"));

                        foreach (var org in orgList)
                        {
                            if (!org.OrgName.Equals(_applicationSettings.Value.MstOrg))
                            {
                                var identity = new ClaimsIdentity(KeyConstant.JWT);
                                identity.AddClaim(KeyConstant.OrgId, org.OrgName, OpenIdConnectConstants.Destinations.AccessToken);
                                identity.AddClaim(OpenIdConnectConstants.Claims.ClientId, encryptClientId.Value.ToString(), OpenIdConnectConstants.Destinations.AccessToken);
                                identity.AddClaim(KeyConstant.TenantId, Convert.ToString(org.TenantDB), OpenIdConnectConstants.Destinations.AccessToken);

                                if (clientExpireOn != null)
                                {
                                    identity.AddClaim(KeyConstant.ClientExpireOn, clientExpireOn.ToString(), OpenIdConnectConstants.Destinations.AccessToken);
                                }

                                foreach (var scope in data.Principal.Claims.Where(c => c.Type == OpenIdConnectConstants.Claims.Scope))
                                {
                                    identity.AddClaim(OpenIdConnectConstants.Claims.Scope, scope.Value, OpenIdConnectConstants.Destinations.AccessToken);
                                }

                                identity.AddClaim(KeyConstant.AppName, encryptAppName, OpenIdConnectConstants.Destinations.AccessToken);

                                identity.AddClaim(KeyConstant.Client_Type, claimClientType.Value.ToString(), OpenIdConnectConstants.Destinations.AccessToken);

                                var orgToken = new JwtSecurityToken(_issuer, audienceId, identity.Claims, issued.Value.UtcDateTime,
                                    expires.Value.UtcDateTime, signingKey);

                                orgIdentity.AddClaim(org.OrgName, handler.WriteToken(orgToken), OpenIdConnectConstants.Destinations.AccessToken);
                            }
                        }
                        orgIdentity.AddClaim(KeyConstant.AppName, encryptAppName, OpenIdConnectConstants.Destinations.AccessToken);

                        orgIdentity.AddClaim(OpenIdConnectConstants.Claims.ClientId, encryptClientId.Value.ToString(), OpenIdConnectConstants.Destinations.AccessToken);

                        orgIdentity.AddClaim(KeyConstant.Client_Type, claimClientType.Value.ToString(), OpenIdConnectConstants.Destinations.AccessToken);

                        if (clientExpireOn != null)
                        {
                            orgIdentity.AddClaim(KeyConstant.ClientExpireOn, clientExpireOn.ToString(), OpenIdConnectConstants.Destinations.AccessToken);
                        }

                        var multiOrgToken = new JwtSecurityToken(_issuer, audienceId, orgIdentity.Claims, issued.Value.UtcDateTime,
                        expires.Value.UtcDateTime, signingKey);

                        jwt = handler.WriteToken(multiOrgToken);
                    }
                    else if (claimClientType != null && claimOrg != null &&
                             claimClientType.ToString() == Enum.GetName(typeof(ClientTypes), ClientTypes.KioskClient))
                    {
                        var token = new JwtSecurityToken(_issuer, audienceId, data.Principal.Claims, issued.Value.UtcDateTime,
                            expires.Value.UtcDateTime, signingKey);
                        jwt = handler.WriteToken(token);
                    }
                    else
                    {
                        var token = new JwtSecurityToken(_issuer, audienceId, data.Principal.Claims, issued.Value.UtcDateTime,
                        expires.Value.UtcDateTime, signingKey);
                        jwt = handler.WriteToken(token);
                        var claimClientTypeCheck = data.Principal.Claims.Where(c => c.Type == KeyConstant.Client_Type);
                        foreach (var itemClaimClient in claimClientTypeCheck)
                        {
                            if (itemClaimClient.Value.ToString() == Enum.GetName(typeof(ClientTypes), ClientTypes.UiWebClient))
                            {
                                var claimSubType = data.Principal.Claims.FirstOrDefault(c => c.Type == KeyConstant.Sub);
                                //if sub exist
                                if (claimSubType.Value.ToString() != null)
                                {
                                    var isFreshLogin = data.Properties.Items[KeyConstant.IsFreshLogin];
                                    if (isFreshLogin.ToLower() == "true")
                                    {
                                        //UserDetails existingUser = _userBusiness.GetUserByUserName(claimClientType.Value.ToString()).Result;
                                        var userBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IUserBusiness>();
                                        var finalResult = userBusiness.VerifyAccountLoginAsync(claimSubType.Value.ToString(), 1, jwt.ToString(), Convert.ToBoolean(IsRefreshTokenflow)).GetAwaiter().GetResult();
                                    }
                                }
                            }
                        }

                    }

                    return jwt;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("CustomJwtFormat", "Protect", ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        public string Protect(AuthenticationTicket data, string purpose)
        {
            throw new NotImplementedException();
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }

        public AuthenticationTicket Unprotect(string protectedText, string purpose)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}

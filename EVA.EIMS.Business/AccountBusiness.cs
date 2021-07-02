using EVA.EIMS.Common;
using EVA.EIMS.Common.Constants;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity.ViewModel;
using EVA.EIMS.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
    public class AccountBusiness : IAccountBusiness
    {
        private readonly string _encryptionKey;
        private readonly IApplicationBusiness _applicationBusiness;
        private readonly IOrganizationTenantMapping _organizationTenantMapping;
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<ApplicationSettings> _applicationSettings;

        public AccountBusiness(IServiceProvider provider, IApplicationBusiness applicationBusiness, IOrganizationTenantMapping organizationTenantMapping)
        {
            _applicationSettings = provider.GetRequiredService<IOptions<ApplicationSettings>>();
            _encryptionKey = _applicationSettings.Value.Eck;
            _serviceProvider = provider;
            _applicationBusiness = applicationBusiness;
            _organizationTenantMapping = organizationTenantMapping;
        }

        /// <summary>
        /// Generate EIMS token from AD token
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="adAccessToken"></param>
        /// <param name="idToken"></param>
        /// <returns></returns>
        public async Task<ReturnResult> GetEIMSTokenFromADToken(string clientId, string adAccessToken, string idToken)
        {
            ReturnResult returnResult = new ReturnResult();
            var link = KeyConstant.GraphURL;
            var uri = new Uri(string.Format(link, string.Empty));
            HttpClient _client = new HttpClient();
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", adAccessToken);

            var postResponseTask = _client.GetAsync(uri);
            var postResponse = await postResponseTask;
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            if ((int)postResponse.StatusCode == Convert.ToInt32(HttpStatusCode.Unauthorized))
            {
                returnResult.Success = false;
                returnResult.Result = HttpStatusCode.Unauthorized.ToString();
                return returnResult;
            }
            var stream = idToken;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = handler.ReadToken(stream) as JwtSecurityToken;

            string uid = string.Empty;
            if (tokenS.Claims.FirstOrDefault(claim => claim.Type == KeyConstant.EmailId) != null)
                uid = tokenS.Claims.FirstOrDefault(claim => claim.Type == KeyConstant.EmailId).Value;
            else if (tokenS.Claims.FirstOrDefault(claim => claim.Type == KeyConstant.PreferredUserName) != null)
                uid = tokenS.Claims.FirstOrDefault(claim => claim.Type == KeyConstant.PreferredUserName).Value;

            var tid = tokenS.Claims.First(claim => claim.Type == KeyConstant.TenantId).Value;
            var oid = tokenS.Claims.First(claim => claim.Type == KeyConstant.ObjectId).Value;

            var cid = tokenS.Claims.First(claim => claim.Type == KeyConstant.Aud).Value;
            if (cid != clientId)
            {
                returnResult.Success = false;
                returnResult.Result = HttpStatusCode.BadRequest.ToString();
                return returnResult;
            }
            var organizationTenant = await _organizationTenantMapping.GetOrgIDbyTenantId(tid);
            var appBusiness = _serviceProvider.GetRequiredService<IApplicationBusiness>();
            var clientData = await appBusiness.GetClientIdByAzureAppId(cid);

            //Encrypting ClientID
            var customPasswordHashForClient = _serviceProvider.GetRequiredService<ICustomPasswordHash>();
            var encryptedClientId = customPasswordHashForClient.Encrypt(clientData.ClientId, _encryptionKey);

            var claimsidentity = new ClaimsIdentity(KeyConstant.JWT);
            claimsidentity.AddClaim(new Claim(KeyConstant.EmailId, uid));
            claimsidentity.AddClaim(new Claim(KeyConstant.OrgId, organizationTenant.FirstOrDefault()?.OrgName));
            claimsidentity.AddClaim(new Claim(KeyConstant.Sub, uid));
            claimsidentity.AddClaim(new Claim(KeyConstant.Subid, oid));
            claimsidentity.AddClaim(new Claim(KeyConstant.Aud, clientData.ClientId));
            claimsidentity.AddClaim(new Claim(KeyConstant.ADUser, bool.TrueString.ToLower()));
            claimsidentity.AddClaim(new Claim(KeyConstant.IsMultiOrg, bool.FalseString.ToLower()));
            claimsidentity.AddClaim(new Claim(KeyConstant.Role, UserRoles.SiteUser.ToString()));
            claimsidentity.AddClaim(new Claim(ClaimTypes.Role, UserRoles.SiteUser.ToString()));
            claimsidentity.AddClaim(new Claim(KeyConstant.Client_Type, "UiWebClient"));
            claimsidentity.AddClaim(new Claim(KeyConstant.Scope, "read"));
            claimsidentity.AddClaim(new Claim(KeyConstant.Scope, "write"));
            claimsidentity.AddClaim(new Claim(KeyConstant.Scope, "uiapi_all"));
            claimsidentity.AddClaim(new Claim(KeyConstant.TwoFactorEnabled, bool.FalseString.ToLower()));

            claimsidentity.AddClaim(new Claim(KeyConstant.ClientId, encryptedClientId));
            claimsidentity.AddClaim(new Claim(KeyConstant.Issuer, _applicationSettings.Value.IMSEndPoint.ToString()));

            var keyByteArray = new SymmetricSecurityKey(WebEncoders.Base64UrlDecode(clientData.ClientSecret));
            var signingKey = new SigningCredentials(keyByteArray, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);
            var token1 = new JwtSecurityToken(null, null, claimsidentity.Claims, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(clientData.TokenValidationPeriod), signingKey);
            var accessToken = handler.WriteToken(token1);

            returnResult.Success = true;
            returnResult.Data = new
            {
                access_token = accessToken,
                scope = "offline_access",
                token_type = "Bearer",
                expires_in = Convert.ToInt32(TimeSpan.FromMinutes(clientData.TokenValidationPeriod).TotalSeconds)
            };
            return returnResult;
        }

        /// <summary>
        /// Generate exchange token using support user token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="org"></param>
        /// <returns></returns>
        public async Task<ReturnResult> GetTokenFromSupportToken(AccessTokenModel accessToken, string org)
        {
            ReturnResult returnResult = new ReturnResult();
            Guid userId = Guid.Empty;
            var jwtHandler = new JwtSecurityTokenHandler();

            var tokenDetails = jwtHandler.ReadToken(accessToken.token) as JwtSecurityToken;
            var isSupportUser = tokenDetails.Claims.FirstOrDefault(c => c.Type == KeyConstant.Role).Value.ToString().Equals(UserRoles.Support.ToString(), StringComparison.OrdinalIgnoreCase);
            var isSiteAdminUser = tokenDetails.Claims.FirstOrDefault(c => c.Type == KeyConstant.Role).Value.ToString().Equals(UserRoles.SiteAdmin.ToString(), StringComparison.OrdinalIgnoreCase);
            var isMasterOrgMapped = tokenDetails.Claims.FirstOrDefault(c => c.Type == KeyConstant.OrgId).Value.ToString().Equals(_applicationSettings.Value.MstOrg.ToString(), StringComparison.OrdinalIgnoreCase);

            if (tokenDetails.Payload.ContainsKey(KeyConstant.Subid))
            {
                Guid.TryParse(tokenDetails.Payload.Single(c => c.Key == KeyConstant.Subid).Value.ToString(), out userId);
            }

            if (!(isMasterOrgMapped && (isSupportUser || isSiteAdminUser)))
            {
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("UnauthorizedTokenExchange");
                return returnResult;
            }

            var stream = accessToken.token;
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(stream) as JwtSecurityToken;

            var clientId = tokenS.Claims.First(claim => claim.Type == KeyConstant.Aud).Value;
            var clientBusiness = _serviceProvider.GetRequiredService<IClientBusiness>();
            var clientData = await clientBusiness.GetById(clientId);

            var validationParams = new TokenValidationParameters()
            {
                ValidAudience = clientData.ClientId,
                ValidIssuer = tokenDetails.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(WebEncoders.Base64UrlDecode(clientData.ClientSecret))
            };

            try
            {
                jwtHandler.ValidateToken(accessToken.token, validationParams, out SecurityToken tokenValues);
            }
            catch
            {
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("InvalidToken");
                return returnResult;
            }

            var userBusiness = _serviceProvider.GetRequiredService<IUserBusiness>();
            var validateTokenExchangeResult = await userBusiness.VerifyTokenExchange(userId, org);

            if (!validateTokenExchangeResult.Success)
            {
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("NoPermissionOrMismatchOrganization");
                return returnResult;
            }

            //Encrypting ClientID
            var customPasswordHashForClient = _serviceProvider.GetRequiredService<ICustomPasswordHash>();
            var encryptedClientId = customPasswordHashForClient.Encrypt(clientId, _encryptionKey);

            var claimsidentity = new ClaimsIdentity(KeyConstant.JWT);
            claimsidentity.AddClaim(new Claim(KeyConstant.EmailId, tokenS.Claims.First(claim => claim.Type == KeyConstant.EmailId).Value));
            claimsidentity.AddClaim(new Claim(KeyConstant.OrgId, org));
            claimsidentity.AddClaim(new Claim(KeyConstant.Sub, tokenS.Claims.First(claim => claim.Type == KeyConstant.Sub).Value));
            claimsidentity.AddClaim(new Claim(KeyConstant.Subid, tokenS.Claims.First(claim => claim.Type == KeyConstant.Subid).Value));
            claimsidentity.AddClaim(new Claim(KeyConstant.Aud, clientId));
            claimsidentity.AddClaim(new Claim(KeyConstant.IsMultiOrg, bool.TrueString.ToLower()));
            claimsidentity.AddClaim(new Claim(KeyConstant.Role, UserRoles.Support.ToString()));
            claimsidentity.AddClaim(new Claim(KeyConstant.Client_Type, "UiWebClient"));
            claimsidentity.AddClaim(new Claim(KeyConstant.Scope, "read"));
            claimsidentity.AddClaim(new Claim(KeyConstant.Scope, "write"));
            claimsidentity.AddClaim(new Claim(KeyConstant.Scope, "uiapi_all"));
            claimsidentity.AddClaim(new Claim(KeyConstant.TwoFactorEnabled, tokenS.Claims.First(claim => claim.Type == KeyConstant.TwoFactorEnabled).Value));

            claimsidentity.AddClaim(new Claim(KeyConstant.ClientId, encryptedClientId));
            claimsidentity.AddClaim(new Claim(KeyConstant.Issuer, _applicationSettings.Value.IMSEndPoint.ToString()));

            var keyByteArray = new SymmetricSecurityKey(WebEncoders.Base64UrlDecode(clientData.ClientSecret));
            var signingKey = new SigningCredentials(keyByteArray, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);
            var token1 = new JwtSecurityToken(null, null, claimsidentity.Claims, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(clientData.TokenValidationPeriod), signingKey);
            var newToken = handler.WriteToken(token1);

            returnResult.Success = true;
            returnResult.Data = new
            {
                access_token = newToken,
                scope = "offline_access",
                token_type = "Bearer",
                expires_in = Convert.ToInt32(TimeSpan.FromMinutes(clientData.TokenValidationPeriod).TotalSeconds)
            };
            return returnResult;
        }
    }
}

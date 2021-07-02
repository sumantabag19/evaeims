using EVA.EIMS.Common;
using EVA.EIMS.Common.Constants;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using EVA.EIMS.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace EVA.EIMS.Security.API.Filters
{
    /// <summary>
    /// This is a global authorization filter to validate Token details
    /// </summary>
    public class AuthorizationTokenFilter : IAuthorizationFilter, IDisposable
    {
        #region Private Variables
        private string _exceptionMessage = string.Empty;
        private bool _disposed;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger _logger;
        #endregion

        public AuthorizationTokenFilter(IHttpClientFactory clientFactory, ILogger logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        #region Public Method

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (Authorize(context))
                return;

            // ILogging _logger = context.HttpContext.RequestServices.GetService<ILogging>();
            // _logger.Log(LogType.INFO, "AuthorizationTokenFilter", "Authorize", "Authorize Method return false", "");

            //throw new UnauthorizedAccessException($"Unauthorized access. {_exceptionMessage}");
        }
        #endregion

        #region Private Method
        /// <summary>
        /// method to use validate access token
        /// </summary>
        /// <param name="context"></param>
        /// <returns>returns true or false</returns>
        private bool Authorize(AuthorizationFilterContext context)
        {
            _exceptionMessage = string.Empty;


            ICustomPasswordHash customPasswordHash = context.HttpContext.RequestServices.GetRequiredService<ICustomPasswordHash>();
            IOptions<ApplicationSettings> applicationSettings = context.HttpContext.RequestServices.GetRequiredService<IOptions<ApplicationSettings>>();
            var encryptionKey = applicationSettings.Value.Eck;

            try
            {
                string[] AnonymousActions = new string[] { "gethealth", "healthz" };
                if (AnonymousActions.Contains(Convert.ToString(context.RouteData.Values["Action"])?.ToLower()))
                    return true;

                //For unathorised access, check if the api has [AllowAnonymous] attribute
                if (context.Filters.Any(item => item is IAllowAnonymousFilter))
                    return true;


                var jwtHandler = new JwtSecurityTokenHandler();
                //var jwtInput = context.HttpContext.Request.Headers.SingleOrDefault(x => x.Key == "Authorization");
                // var jwtInput = ((Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.FrameRequestHeaders)context.HttpContext.Request.Headers).HeaderAuthorization.ToString().Replace("Bearer ", "");
                string jwtInput = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split("Bearer ").Last();
                if (string.IsNullOrEmpty(jwtInput) || !jwtHandler.CanReadToken(jwtInput)) throw new UnauthorizedAccessException(MessageConstants.InvalidAccessToken);

                //Verify if the user is using a previously generated token without logging out
                IIMSLogOutBusiness iMSLogOutBusiness = context.HttpContext.RequestServices.GetService<IIMSLogOutBusiness>();
                var result = iMSLogOutBusiness.IsUserLoggedOut(jwtInput).GetAwaiter().GetResult();
                if (result.Success)
                {
                    throw new UnauthorizedAccessException(MessageConstants.InvalidAccessToken);
                }

                var tokenDetails = jwtHandler.ReadToken(jwtInput) as JwtSecurityToken;
                if (tokenDetails == null) throw new UnauthorizedAccessException(MessageConstants.InvalidAccessToken);

                //Decrypting ClientId
                var encryptedClientId = tokenDetails.Payload.Single(c => c.Key == KeyConstant.ClientId).Value.ToString();
                var clientId = customPasswordHash.Decrypt(encryptedClientId, encryptionKey);

                if (!string.IsNullOrEmpty(clientId))
                {
                    context.HttpContext.Items.Add(KeyConstant.ClientId, clientId);
                }

                IClientBusiness clientBusiness = (IClientBusiness)context.HttpContext.RequestServices.GetService(typeof(IClientBusiness));
                IUserBusiness userBusiness = (IUserBusiness)context.HttpContext.RequestServices.GetService(typeof(IUserBusiness));

                var client = clientBusiness.GetById(clientId).GetAwaiter().GetResult();

                if (client == null)
                {
                    _exceptionMessage = $"Invalid_client {clientId} ";
                    Exception ex = new Exception(_exceptionMessage);
                    throw new UnauthorizedAccessException(MessageConstants.InvalidClient, ex);
                }

                //var validationParams =
                //    new TokenValidationParameters()
                //    {
                //        ValidAudience = clientId,
                //        ValidIssuer = tokenDetails.Issuer,
                //        IssuerSigningKey = new SymmetricSecurityKey(WebEncoders.Base64UrlDecode(client.ClientSecret))
                //    };

                //jwtHandler.ValidateToken(jwtInput, validationParams, out SecurityToken tokenValues);

                ValidateToken(context, jwtInput);
                if (tokenDetails.Payload.ContainsKey(KeyConstant.OrgId))
                {
                    var organization = tokenDetails.Payload.Single(c => c.Key == KeyConstant.OrgId).Value.ToString();
                    context.HttpContext.Items.Add(KeyConstant.OrgId, organization);
                }

                if (tokenDetails.Payload.ContainsKey("sub"))
                {
                    var username = tokenDetails.Payload.Single(c => c.Key == "sub").Value.ToString();
                    var user = userBusiness.GetUserByUserNameInternally(username).GetAwaiter().GetResult();

                    if (user != null && user.IsAccLock)
                    {
                        _exceptionMessage = $"Account_Locked {user.UserName}";
                        Exception ex = new Exception(_exceptionMessage);
                        throw new UnauthorizedAccessException(MessageConstants.AccountLocked, ex);
                    }

                    context.HttpContext.Items.Add(KeyConstant.UserName, tokenDetails.Payload.Single(c => c.Key == "sub").Value.ToString());
                }


                if (tokenDetails.Payload.Any(c => c.Key == KeyConstant.Client_Type))
                {
                    List<string> client_type = new List<string>();
                    if (tokenDetails.Payload[KeyConstant.Client_Type].GetType().ToString().Equals("System.String"))
                    {
                        client_type.Add((string)tokenDetails.Payload[KeyConstant.Client_Type]);
                    }
                    else
                    {
                        foreach (var item in ((Newtonsoft.Json.Linq.JArray)tokenDetails.Payload[KeyConstant.Client_Type]).Children())
                        {
                            client_type.Add(item.ToString());
                        }

                    }
                    context.HttpContext.Items.Add(KeyConstant.Client_Type, client_type);
                }

                if (tokenDetails.Payload.Any(c => c.Key == KeyConstant.Role))
                {
                    List<string> role = new List<string>();
                    if (tokenDetails.Payload[KeyConstant.Role].GetType().ToString().Equals("System.String"))
                    {
                        role.Add((string)tokenDetails.Payload[KeyConstant.Role]);
                    }
                    else
                    {
                        foreach (var item in ((Newtonsoft.Json.Linq.JArray)tokenDetails.Payload[KeyConstant.Role]).Children())
                        {
                            role.Add(item.ToString());
                        }

                    }
                    context.HttpContext.Items.Add(KeyConstant.Role, role);
                }
                return client.ClientTypeId == (int)ClientTypes.SecurityApiClient ||
                       client.ClientTypeId == (int)ClientTypes.UiWebClient
                || client.ClientTypeId == (int)ClientTypes.ServiceClient || client.ClientTypeId == (int)ClientTypes.DeviceClient;
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case MessageConstants.UnauthorizedAccess:
                        throw new UnauthorizedAccessException(MessageConstants.UnauthorizedAccess);
                    case MessageConstants.InvalidAccessToken:
                        throw new UnauthorizedAccessException(MessageConstants.InvalidAccessToken);
                    case MessageConstants.AccountLocked:
                        throw new UnauthorizedAccessException(MessageConstants.AccountLocked, ex);
                    case MessageConstants.InvalidClient:
                        throw new UnauthorizedAccessException(MessageConstants.InvalidClient, ex);
                    default:
                        throw new UnauthorizedAccessException(MessageConstants.InternalServerError, ex);
                }
            }
        }

        private void ValidateToken(AuthorizationFilterContext context, string token)
        {
            IOptions<ApplicationSettings> applicationSettings = context.HttpContext.RequestServices.GetRequiredService<IOptions<ApplicationSettings>>();
            var iMSEndPoint = applicationSettings.Value.IMSEndPoint;
            string content = string.Empty;

            var claimClient = _clientFactory.CreateClient();
            var uri = new Uri(iMSEndPoint + KeyConstant.JWKS);
            var responseData = claimClient.GetAsync(uri).Result;
            var jwkSet = JsonConvert.DeserializeObject<JWKSET>(responseData.Content.ReadAsStringAsync().Result);

            if (jwkSet.keys.Count() > 0)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                RsaSecurityKey signingKey = new RsaSecurityKey(new RSAParameters()
                {
                    Modulus = FromBase64Url(jwkSet.keys.FirstOrDefault().Modulus),
                    Exponent = FromBase64Url(jwkSet.keys.FirstOrDefault().Exponent)
                });

                SecurityToken validatedToken;
                var tokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = iMSEndPoint,
                    IssuerSigningKey = signingKey,
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };

                if (!string.IsNullOrWhiteSpace(token))
                {
                    if (token.Trim() == "Bearer")
                        throw new UnauthorizedAccessException("InvalidAccessToken");
                }
                else
                    throw new UnauthorizedAccessException("InvalidAccessToken");

                try
                {
                    tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
                    var readToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                    content = readToken.Payload.SerializeToJson();

                    if (content.Contains(KeyConstant.ClientId))
                    {
                        IMSClaimDetails response = JsonConvert.DeserializeObject<IMSClaimDetails>(content);
                        context.HttpContext.Items.Add(KeyConstant.ImsClaim, response);
                        context.RouteData.Values.Add("UserName", response.sub);
                        context.RouteData.Values.Add("UserId", response.subid);
                    }
                    else
                    {
                        throw new UnauthorizedAccessException(MessageConstants.InvalidAccessToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("AuthorizaionTokenFilter", "OnAuthorization", ex.Message.ToString(), ex.StackTrace);
                    throw new UnauthorizedAccessException(MessageConstants.UnauthorizedAccess);
                }
            }
            else
            {
                throw new UnauthorizedAccessException(MessageConstants.UnauthorizedAccess);
            }
        }

        private byte[] FromBase64Url(string base64Url)
        {
            string padded = base64Url.Length % 4 == 0 ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
            string base64 = padded.Replace("_", "/").Replace("-", "+");
            return Convert.FromBase64String(base64);
        }

        #endregion

        #region Dispose
        /// <summary>
        /// Method to dispose by parameter.
        /// </summary>
        /// <param name="disposing"></param>
        /// 
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {

            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}

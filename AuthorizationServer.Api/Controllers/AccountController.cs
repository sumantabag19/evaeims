using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity.ViewModel;
using EVA.EIMS.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthorizationServer.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        // GET: api/Account
        private readonly string _encryptionKey;
        private readonly ICustomPasswordHash _customPasswordHash;
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<ApplicationSettings> _applicationSettings;
        private readonly IHttpContextAccessor _httpContext;
        private readonly CookieOptions _cookieOptions;
        public class TokenHolder
        {
            public string idToken { get; set; }
            public string adAccessToken { get; set; }
        }
        public AccountController(IServiceProvider provider, ICustomPasswordHash customPasswordHash, IHttpContextAccessor httpContext)
        {

            _serviceProvider = provider;
            _applicationSettings = provider.GetRequiredService<IOptions<ApplicationSettings>>();
            _customPasswordHash = customPasswordHash;
            _encryptionKey = _applicationSettings.Value.Eck;
            _httpContext = httpContext;
            _cookieOptions = new CookieOptions
            {
                Path = "/;SameSite = None",
                HttpOnly = true,
                Secure = true,
                //Expires = DateTimeOffset.Now.AddHours(6),
                IsEssential = true,
                SameSite = SameSiteMode.None,
            };
        }

        /// <summary>
        /// Logout session
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/account/logout")]
        public async Task<IActionResult> Get()
        {
            var authorizatonToken = Request.Headers;
            if (!authorizatonToken.ContainsKey("Authorization") || string.IsNullOrEmpty(authorizatonToken["Authorization"]))
            {
                return BadRequest(HttpStatusCode.Unauthorized);
            }
            var iMSLogOutBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IIMSLogOutBusiness>();
            var result = await iMSLogOutBusiness.SaveLogOutUserToken(authorizatonToken["Authorization"]);
            if (result.Success)
            {
                return Ok(result.Result);
            }
            else
            {
                return BadRequest(result.Result);
            }



        }

        /// <summary>
        /// Verify session id cookie
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "CheckSession")]
        [Route("/account/checksession")]
        public IActionResult verifyUser()
        {
            var tempOptions = _cookieOptions;
            tempOptions.Expires = DateTimeOffset.Now.AddMinutes(10);
            Response.Cookies.Append("consent-cookie", "true", tempOptions);

            if (Request.Cookies.Count > 0 && !string.IsNullOrEmpty(HttpContext.Request.Cookies["session-id"]))
            {
                string cookieLoad = _customPasswordHash.Decrypt(HttpContext.Request.Cookies["session-id"], _applicationSettings.Value.Eck).ToString();
                string userId = cookieLoad.Split("$&$/")[0];
                Guid id;
                if (Guid.TryParse(userId, out id))
                {
                    var authorizatonToken = Request.Headers;
                    if (authorizatonToken.ContainsKey("Authorization") && !string.IsNullOrEmpty(authorizatonToken["Authorization"]))
                    {
                        string tokenUserId = string.Empty;
                        string accessToken = authorizatonToken["Authorization"].ToString().Replace("Bearer ", "");
                        var jwtHandler = new JwtSecurityTokenHandler();
                        var tokenDetails = jwtHandler.ReadToken(accessToken) as JwtSecurityToken;
                        if (tokenDetails.Payload.ContainsKey(KeyConstant.Subid))
                        {
                            tokenUserId = tokenDetails.Payload.Single(c => c.Key == KeyConstant.Subid).Value.ToString();
                            if (tokenUserId.ToLower() != userId.ToLower())
                                return Ok(new { isExist = false });
                        }
                    }

                    //var userBusiness = _serviceProvider.GetRequiredService<IUserBusiness>();
                    //var userResult = userBusiness.GetUserNameById(id).GetAwaiter().GetResult();

                    //if (userResult != null && userResult.Success && userResult.Data != null)
                    //{
                    return Ok(new { isExist = true });
                    //}

                }

            }
            Response.Cookies.Delete("session-id", _cookieOptions);
            return Ok(new { isExist = false });
        }

        /// <summary>
        /// Fetch azure app id from EIMS client id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet(Name = "GetID")]
        [Route("/account/getclientid")]
        public async Task<IActionResult> GetClientID(string id)
        {
            var clientBusiness = _serviceProvider.GetRequiredService<IClientBusiness>();
            var clientData = await clientBusiness.GetById(id);
            if (clientData == null)
            {
                return NotFound();
            }

            var applicationBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IApplicationBusiness>();
            var app = await applicationBusiness.GetAppById(clientData.AppId);
            return Ok(new { clientID = app.AzureAppId });
        }

        /// <summary>
        /// Generate EIMS token from AD token
        /// </summary>
        /// <param name="tokenHolder"></param>
        /// <returns></returns>
        [HttpPost(Name = "Gettoken")]
        [Route("/account/connect/adtoken")]
        public async Task<IActionResult> GetToken([FromBody] TokenHolder tokenHolder)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            Request.Headers.TryGetValue("clientid", out var cli);
            if (string.IsNullOrEmpty(cli))
            {
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                return (IActionResult)response;

            }

            var accountBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IAccountBusiness>();
            var result = await accountBusiness.GetEIMSTokenFromADToken(cli, tokenHolder.adAccessToken, tokenHolder.idToken);

            if (!result.Success)
            {
                if (result.Result == HttpStatusCode.Unauthorized.ToString())
                    return (IActionResult)new HttpResponseMessage(HttpStatusCode.Unauthorized);
                else
                    return (IActionResult)new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Generate exchange token using support user token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        [HttpPost(Name = "GetExchangeToken")]
        [Route("/account/connect/exchangetoken")]
        public async Task<IActionResult> ExchangeToken([FromBody] AccessTokenModel accessToken)
        {
            var headers = Request.Headers;
            string org = string.Empty;

            if (headers.ContainsKey(ValidationConstants.OrganizationName) && headers[ValidationConstants.OrganizationName].First() != String.Empty)
                org = headers[ValidationConstants.OrganizationName].ToString();
            else
                return new ContentResult { Content = ResourceInformation.GetResValue("ProvideOrganizationName"), StatusCode = StatusCodes.Status401Unauthorized };

            var jwtHandler = new JwtSecurityTokenHandler();
            if (accessToken == null || !jwtHandler.CanReadToken(accessToken.token))
            {
                return new ContentResult { Content = ResourceInformation.GetResValue("ProvideAccessToken"), StatusCode = StatusCodes.Status401Unauthorized };
            }

            var accountBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IAccountBusiness>();
            var result = await accountBusiness.GetTokenFromSupportToken(accessToken, org);
            if (!result.Success)
            {
                return new ContentResult { Content = result.Result, StatusCode = StatusCodes.Status401Unauthorized };
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Verfiy third party cookies
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "CheckCookie")]
        [Route("/account/checkcookie")]
        public IActionResult CheckCookie()
        {
            if (Request.Cookies.Count > 0 && !string.IsNullOrEmpty(HttpContext.Request.Cookies["consent-cookie"]))
                return Ok(new { isExist = true });
            else
                return Ok(new { isExist = false });
        }

        /// <summary>
        /// Generate EIMS token from AD token
        /// </summary>
        /// <param name="tokenHolder"></param>
        /// <returns></returns>
        [HttpPost(Name = "Gettoken")]
        [Route("/account/VerifyMobileLogin")]
        public async Task<IActionResult> VerifyMobileLogin([FromBody] VerifyMobile mobile)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);

            if (ModelState.IsValid)
            {
                var userBusiness = _httpContext.HttpContext.RequestServices.GetRequiredService<IUserBusiness>();
                var result = await userBusiness.VerifyMobileLoginAsync(mobile.Mobile);
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }
    }
}

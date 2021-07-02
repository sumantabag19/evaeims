using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using AspNet.Security.OpenIdConnect.Primitives;
using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using EVA.EIMS.Logging;


namespace AuthorizationServer.Api.Controllers
{
    [Route("api/claims/[action]")]
    public class ClaimController : Controller
    {
        private readonly ILogger _logger;
        private readonly IClientBusiness _clientBusiness;
        public ClaimController(ILogger logger, IClientBusiness clientBusiness)
        {
            _logger = logger;
            _clientBusiness = clientBusiness;
        }
        /// <summary>
        /// This method used by auth flow as redirect URL
        /// </summary>
        /// <param name="request"></param>
        /// <returns>it returns auth code</returns>
        [HttpGet]
        [ActionName("Redirect")]
        public IActionResult Redirect(OpenIdConnectRequest request)
        {
            //var code = new AuthCodeModel { Code = request.Code };            
            //var json = Json(code);
            //return Ok(json.Value);
            return Ok(request.Code);
        }
        /// <summary>
        /// For testing
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Get")]
        public IActionResult Get()
        {
            return Ok("Success");
        }

        /// <summary>
        /// This method is used to get the claims details by passing valid access token
        /// </summary>
        /// <param name="model">token string </param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("")]
        public IActionResult Post([FromBody]TokenModel model)
        {
            try
            {
                if (model.access_token == null)
                {
                    return BadRequest(ResourceInformation.GetResValue("ProvideValidToken"));
                }

                var handler = new JwtSecurityTokenHandler();
                
                var tokenS = handler.ReadToken(model.access_token) as JwtSecurityToken;

                if (tokenS != null)
                {   //Check for the expiry of token
                    if (tokenS.ValidTo <= DateTime.UtcNow)
                    {
                        return BadRequest(ResourceInformation.GetResValue("TokenExpired"));
                    }

                    if (tokenS.Payload.Any(c => c.Key == KeyConstant.ClientExpireOn))
                    {
                        var clientExpireOn = DateTime.Parse(tokenS.Payload.Single(c => c.Key == KeyConstant.ClientExpireOn).Value.ToString());

                        if (clientExpireOn != null && clientExpireOn < DateTime.Now)
                        {
                            return BadRequest(ResourceInformation.GetResValue("ClientExpired"));
                        }
                    }
                    return Ok(tokenS.Payload);
                }
                else
                    return BadRequest(ResourceInformation.GetResValue("ProvideValidToken"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
		/// This method is used to get the claims details for device only by passing valid access token
		/// </summary>
		/// <param name="model">token string </param>
		/// <returns></returns>
		[HttpPost]
        [ActionName("DeviceClaims")]
        public IActionResult PostDeviceClaims([FromBody]TokenModel model)
        {
            try
            {
                if (model.access_token == null)
                {
                    return BadRequest(ResourceInformation.GetResValue("ProvideValidToken"));
                }

                var handler = new JwtSecurityTokenHandler();

                var tokenS = handler.ReadToken(model.access_token) as JwtSecurityToken;

                if (tokenS != null)
                {   //Check for the expiry of token
                    if (tokenS.ValidTo <= DateTime.UtcNow)
                    {
                        return BadRequest(ResourceInformation.GetResValue("TokenExpired"));
                    }

                    if (tokenS.Payload.Any(c => c.Key == KeyConstant.ClientExpireOn))
                    {
                        var clientExpireOn = DateTime.Parse(tokenS.Payload.Single(c => c.Key == KeyConstant.ClientExpireOn).Value.ToString());

                        if (clientExpireOn != null && clientExpireOn < DateTime.Now)
                        {
                            return BadRequest(ResourceInformation.GetResValue("ClientExpired"));
                        }
                    }
                    return Ok(tokenS.Payload);
                }
                else
                    return BadRequest(ResourceInformation.GetResValue("ProvideValidToken"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class TokenModel
    {
        public string access_token { get; set; }
    }
}

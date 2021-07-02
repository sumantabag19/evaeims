using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AuthorizationServer.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/OAuth/[action]")]
    public class OAuthController : Controller
    {
        private readonly IOptions<ApplicationSettings> _applicationSettings;
        public OAuthController(IOptions<ApplicationSettings> applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }

        [HttpGet]
        [ActionName("auth")]
        public IActionResult OAuthRedirect(OpenIdConnectRequest request)
        {
            //string referer = new Uri(HttpContext.Request.Headers["Origin"].ToString()).AbsoluteUri;
            //if (_applicationSettings.Value.SSOReleaseUrl != referer && _applicationSettings.Value.SSOLocalUrl != referer)
            //    return BadRequest();

            return Ok(request.Code);
        }
    }
}
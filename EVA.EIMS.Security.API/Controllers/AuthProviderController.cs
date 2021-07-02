using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVA.EIMS.Security.API.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/AuthProvider")]
    public class AuthProviderController : Controller
    {
        #region Private Variables
        private IAuthProviderBusiness _authProviderBusiness;
        #endregion
        public AuthProviderController(IAuthProviderBusiness authProviderBusiness)
        {
            _authProviderBusiness = authProviderBusiness;
        }
        /// <summary>
        /// This Method returns all the AuthProvider Details
        /// </summary>
        /// <returns>List of AuthProvider</returns>
        // GET: api/AuthProvider
        [HttpGet]
        [Route("GetAuthProvider")]
        public async Task<IActionResult> Get()
        {
            var result = await _authProviderBusiness.Get();
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }
        /// <summary>
        /// This Method returns the Authprovider details for given provider name 
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns>return single provider details</returns>
        // GET: api/AuthProvider/5
        [HttpGet]
        [Route("GetAuthProvider/{providerName}")]
        public async Task<IActionResult> Get(string providerName)
        {
            var result = await _authProviderBusiness.GetByName(providerName);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }
        /// <summary>
        /// This Method Add new AuthProvider details
        /// </summary>
        /// <param name="authProviderMaster"></param>
        /// <returns>Success or failure message</returns>
        // POST: api/AuthProvider
        [HttpPost]
        [Route("SaveAuthProvider")]
        public async Task<IActionResult> Post([FromBody]AuthProviderMaster authProviderMaster)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            ModelState.Remove("ProviderID");
            if (ModelState.IsValid)
            {
                var result = await _authProviderBusiness.Save(tokenData.UserName, authProviderMaster);

                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }
        /// <summary>
        /// This method update the given AuthProvider details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authProviderMaster"></param>
        /// <returns>returns success or failure</returns>
        // PUT: api/AuthProvider/5
        [HttpPut]
        [Route("UpdateAuthProvider")]
        public async Task<IActionResult> Put([FromBody]AuthProviderMaster authProviderMaster)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            if (ModelState.IsValid)
            {
                var result = await _authProviderBusiness.Update(tokenData.UserName, authProviderMaster);

                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }
        /// <summary>
        /// This method delete the given AuthProvider details
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns>return success or failure</returns>
        // DELETE: api/ApiWithActions/5
        [HttpDelete]
        [Route("DeleteAuthProvider/{providerId}")]
        public async Task<IActionResult> Delete(int providerId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _authProviderBusiness.Delete(tokenData.UserName, providerId);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }
    }
}

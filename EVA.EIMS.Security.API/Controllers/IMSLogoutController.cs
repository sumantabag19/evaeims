using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVA.EIMS.Security.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]/[action]")]
    public class IMSLogOutController : Controller, IDisposable
    {

        #region Private Variables
        private readonly IIMSLogOutBusiness _iMSLogOutBusiness;
        #endregion

        #region Constructor
        public IMSLogOutController(IIMSLogOutBusiness iMSLogOutBusiness)
        {
            _iMSLogOutBusiness = iMSLogOutBusiness;
        }
        #endregion

        /// <summary>
        /// This method is used to get multiple logged out token.
        /// </summary>
        /// <returns>returns multiple logged out tokens</returns>
        [HttpGet]
        [ActionName("GetLoggedOutUserToken")]
        public async Task<IActionResult> GetLoggedOutUserToken()
        {

            var result = await _iMSLogOutBusiness.GetLoggedOutUserToken();
            if (result.Success)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Result);
            }
        }

        /// <summary>
        /// This method is used to save the user log out token
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SaveLogOutUserToken")]
        public async Task<IActionResult> Post()
        {
            var authorizatonToken = Request.Headers;
            if (!authorizatonToken.ContainsKey("Authorization") || string.IsNullOrEmpty(authorizatonToken["Authorization"]))
            {
                return BadRequest(HttpStatusCode.Unauthorized);
            }
            var result = await _iMSLogOutBusiness.SaveLogOutUserToken(authorizatonToken["Authorization"]);
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
        /// This method deletes the logged out token in the database at the scheduled time
        /// </summary>
        /// <param name="tokenDetails">tokenDetails</param>
        /// <returns>HTTP response message</returns>
        [HttpDelete]
        [ActionName("DeleteLoggedOutToken")]
        public async Task DeleteLoggedOutToken()
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            await _iMSLogOutBusiness.DeleteLoggedOutToken(tokenData);

        }


        #region Dispose

        /// <summary>
        /// Method to dispose by parameter.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _iMSLogOutBusiness.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

    }


}
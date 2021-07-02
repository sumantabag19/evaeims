using EVA.EIMS.Common;
using EVA.EIMS.Common.Constants;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Security.API.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/Password/[action]")]
    public class PasswordManagementController : Controller, IDisposable
    {
        #region Private Variables
        private readonly IUserBusiness _userBusiness;
        #endregion

        #region Constructor
        public PasswordManagementController(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }
        #endregion

        #region Public API Methods
        /// <summary>
        /// This method is used to update the password in case of forgot password.
        /// </summary>
        /// <param name="userCredentials">userCredentials object</param>
        /// <returns>Returns success or failure message</returns>


        [AllowAnonymous]
        [HttpPut]
        [ActionName("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UserCredentials updateUserCredentials)
        {
            ModelState.Remove("UserName");
            ModelState.Remove("OldPassword");
            ModelState.Remove("EmailId");
            if (ModelState.IsValid)
            {
                var result = await _userBusiness.UpdatePassword(updateUserCredentials);
                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// This method is used to change the password
        /// </summary>
        /// <param name="userCredentials">userCredentials object</param>
        /// <returns>Returns success or failure message</returns>
        [HttpPut]
        [ActionName("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] UserCredentials userCredentials)
        {
            ModelState.Remove("EmailId");
            ModelState.Remove("UserId");
            if (ModelState.IsValid)
            {
                var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
                if (tokenData != null)
                {
                    var result = await _userBusiness.ChangePassword(tokenData.UserName, userCredentials);
                    if (result.Success)
                        return Ok(result.Result);
                    else
                        return BadRequest(result.Result);
                }
                else
                    return BadRequest(ResourceInformation.GetResValue("UnauthorizedAccessException"));
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// This method is used to reset any user's password by Superadmin.
        /// </summary>
        /// <param name="userCredentials">userCredentials object</param>
        /// <returns>Returns success or failure message</returns>

        [HttpPut]
        [ActionName("ResetAnyUserPassword")]
        public async Task<IActionResult> ResetAnyUserPassword([FromBody] UserCredentials userCredentials)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("OldPassword");
            ModelState.Remove("EmailId");
            if (ModelState.IsValid)
            {
                var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
                var result = await _userBusiness.ResetAnyUserPassword(tokenData.UserName, userCredentials);
                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
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
                _userBusiness.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
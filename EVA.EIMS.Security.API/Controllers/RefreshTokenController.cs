using EVA.EIMS.Common;
using EVA.EIMS.Common.Constants;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Helper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Security.API.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class RefreshTokenController : Controller
    {
        #region Private Variables
        private IRefreshTokenBusiness _refreshTokenBusiness;
        #endregion

        #region Constructor
        //Resolve Injected RefreshTokenBusiness object in constructor
        public RefreshTokenController(IRefreshTokenBusiness refreshTokenBusiness)
        {
            _refreshTokenBusiness = refreshTokenBusiness;
        }
        #endregion

        #region Public API Methods
        ///// <summary>
        ///// This method is used get the refreshToken details by id
        ///// </summary>
        ///// <param name="refreshTokenId">refreshTokenId</param>
        ///// <returns>returns single refreshToken details</returns>
        //[HttpGet]
        //[ActionName("getrefreshtokenbyid")]
        //public RefreshToken Get(Guid refreshTokenId)
        //{
        //    return _refreshTokenBusiness.Get(refreshTokenId);
        //}

        ///// <summary>
        ///// This method is used to save the refreshToken details
        ///// </summary>
        ///// <param name="refreshToken">refreshToken object</param>
        ///// <returns>returns response  message</returns>
        //[HttpPost]
        //[ActionName("saverefreshtoken")]
        //public IActionResult Post(RefreshToken refreshToken)
        //{
        //    var result = _refreshTokenBusiness.Save(refreshToken);

        //    if (result.Success)
        //        return Ok(result.Result);
        //    else
        //        return BadRequest(result.Result);
        //}

        ///// <summary>
        ///// This method is used to delete the refreshToken details
        ///// </summary>
        ///// <param name="refreshTokenId">refreshTokenId</param>
        ///// <returns>returns response  message</returns>
        //[HttpDelete]
        //[ActionName("deleterefreshtoken")]
        //public IActionResult Delete(Guid refreshTokenId)
        //{
        //    var result = _refreshTokenBusiness.Delete(refreshTokenId);

        //    if (result.Success)
        //        return Ok(result.Result);
        //    else
        //        return BadRequest(result.Result);
        //}

        /// <summary>
        /// This method is used to fetch RefreshToken count by clientid
        /// </summary>
        /// <returns>returns response  message</returns>
        [HttpGet]
        [ActionName("GetClientwiseTokenCount")]
        public async Task<IActionResult> GetClientwiseTokenCount()
        {
            var result = await _refreshTokenBusiness.GetClientwiseTokenCount();
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);

        }


        /// <summary>
        /// This API is accessed by the EIMS Scheduler to send token generation request count notification
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SendTokenRequestCountNotification")]
        public async Task Post()
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            await _refreshTokenBusiness.SendTokenRequestCountEmail(tokenData);
        }

        /// <summary>
        /// This method is used to clear RefreshToken data
        /// </summary>
        /// <returns>returns response  message</returns>
        [HttpDelete]
        [ActionName("DeleteSheduledRefreshToken")]
        public async Task<IActionResult> Delete()
        {

            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _refreshTokenBusiness.DeleteExpiredRefreshTokens();
            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);

        }

        #endregion

        #region Dispose
        /// <summary>
        /// Method to dispose by parameter.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _refreshTokenBusiness.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
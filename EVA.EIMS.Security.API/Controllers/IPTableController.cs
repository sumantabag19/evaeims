using System;
using System.Threading.Tasks;
using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using Microsoft.AspNetCore.Mvc;

namespace EVA.EIMS.Security.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]/[action]")]
    public class IPTableController : Controller, IDisposable
    {
        #region Private Variables
        private IIPTableBusiness _iPTableBusiness;
        #endregion

        #region Constructor
        public IPTableController(IIPTableBusiness iPTableBusiness)
        {
            _iPTableBusiness = iPTableBusiness;
        }
        #endregion

        /// <summary>
        /// This method is used to get the multiple IP details
        /// </summary>
        /// <returns>returns  multiple IP details</returns>

        [HttpGet]
        [ActionName("GetIpTableDetails")]
        public async Task<IActionResult> Get()
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _iPTableBusiness.Get();
            if (result != null)
                return Ok(result.Data);
            else
                return BadRequest(ResourceInformation.GetResValue("NotExists"));

        }

        /// <summary>
        /// This method is used get the IP details by id
        /// </summary>
        /// <param name="ipAddressId">ipAddressId</param>
        /// <returns>returns single IP details</returns>
        [HttpGet]
        [ActionName("GetIpTableDetailsById")]
        public async Task<IActionResult> GetById([FromQuery] int ipAddressId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _iPTableBusiness.GetById(ipAddressId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to save the IP details
        /// </summary>
        /// <param name="iPTable">iPTable</param>
        /// <returns>returns response  message</returns>
        [HttpPost]
        [ActionName("SaveIpDetails")]
        public async Task<IActionResult> Post([FromBody] IPTable iPTable)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            ModelState.Remove("IPAddressId");
            if (ModelState.IsValid)
            {
                var result = await _iPTableBusiness.Save(tokenData.UserName, iPTable);

                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);

        }

        /// <summary>
        /// This method is used to update the IP details
        /// </summary>
        /// <param name="iPTable">iPTable</param>
        /// <returns>returns response message</returns>
        [HttpPut]
        [ActionName("UpdateIPDetails")]
        public async Task<IActionResult> Put([FromBody] IPTable iPTable)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            if (ModelState.IsValid)
            {
                var result = await _iPTableBusiness.Update(tokenData.UserName, iPTable);

                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);

        }

        /// <summary>
        /// This method is used to delete the IP details
        /// </summary>
        /// <param name="ipAddressId">ipAddressId</param>
        /// <returns>returns response  message</returns>
        [HttpDelete]
        [ActionName("DeleteIpDetails")]
        public async Task<IActionResult> Delete([FromQuery] int ipAddressId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _iPTableBusiness.Delete(tokenData.UserName, ipAddressId);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

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
                _iPTableBusiness.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
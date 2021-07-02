using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Security.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]/[action]")]
    public class ClientTypeModuleAccessController : Controller, IDisposable
    {
        #region Private Variables
        private IClientTypeModuleAccessBusiness _clientTypeModuleAccessBusiness;
        #endregion

        #region Constructor
        public ClientTypeModuleAccessController(IClientTypeModuleAccessBusiness clientTypeModuleAccessBusiness)
        {
            _clientTypeModuleAccessBusiness = clientTypeModuleAccessBusiness;

        }
        #endregion

        #region Public API Methods
        #region ClientTypeModuleAccess API Methods
        /// <summary>
        /// This method is used to get the multiple ClientTypeModuleAccess details.
        /// </summary>
        /// <returns>returns multiple ClientTypeModuleAccess details</returns>
        /// 
        [HttpGet]
        [ActionName("GetClientTypeModuleAccess")]
        public async Task<IActionResult> Get()
        {
            var result = await _clientTypeModuleAccessBusiness.Get();

            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used get ClientTypeModuleAccess details by id.
        /// </summary>
        /// <param name="accessId">accessId</param>
        /// <returns>returns single ClientTypeModuleAccess details</returns>
        [HttpGet]
        [ActionName("GetClientTypeModuleAccessById")]
        public async Task<IActionResult> GetbyId([FromQuery] int accessId)
        {

            var result = await _clientTypeModuleAccessBusiness.GetById(accessId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used get the ClientTypeModuleAccess details by id.
        /// </summary>
        /// <param name="clientTypeId">clientTypeId</param>
        /// <returns>returns assigned and unassigned ClientTypeModuleAccess details</returns>
        [HttpGet]
        [ActionName("GetClientTypeModuleAccessByClientTypeId")]
        public async Task<IActionResult> GetbyClientTypeId([FromQuery] int clientTypeId)
        {
            var result = await _clientTypeModuleAccessBusiness.GetByClientTypeId(clientTypeId);

            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to save one ClientTypeModuleAccess Entry.
        /// </summary>
        /// <param name="clientTypeModuleAccessModel">clientTypeModuleAccessModel object</param>
        /// <returns>returns response message</returns>
        [HttpPost]
        [ActionName("SaveClientTypeModuleAccess")]
        public async Task<IActionResult> Post([FromBody] ClientTypeModuleAccessModel clientTypeModuleAccessModel)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _clientTypeModuleAccessBusiness.Save(tokenData.UserName, clientTypeModuleAccessModel);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to save multiple the ClientTypeModuleAccess details.
        /// </summary>
        /// <param name="clientTypeModuleAccessModelList">clientTypeModuleAccessModelList</param>
        /// <returns>returns response message</returns>
        [HttpPost]
        [ActionName("SaveMultipleClientTypeModuleAccess")]
        public async Task<IActionResult> PostRange([FromBody] List<ClientTypeModuleAccessModel> clientTypeModuleAccessModelList)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _clientTypeModuleAccessBusiness.SaveRange(tokenData.UserName, clientTypeModuleAccessModelList);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to update single ClientTypeModuleAccess entry.
        /// </summary>
        /// <param name="clientTypeAccessId">clientTypeAccessId</param>
        /// <param name="clientTypeModuleAccess">clientTypeModuleAccess object</param>
        /// <returns>returns response message</returns>
        [HttpPut]
        [ActionName("UpdateClientTypeModuleAccess")]
        public async Task<IActionResult> Put([FromQuery] int clientTypeAccessId, [FromBody] ClientTypeModuleAccessModel clientTypeModuleAccess)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _clientTypeModuleAccessBusiness.Update(tokenData.UserName, clientTypeAccessId, clientTypeModuleAccess);
            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to update multiple the ClientTypeModuleAccess details.
        /// </summary>
        /// <param name="clientTypeId">clientTypeId</param>
        /// <param name="clientTypeModuleAccessModelList">clientTypeModuleAccessModelList object</param>
        /// <returns>returns response message</returns>
        [HttpPut]
        [ActionName("UpdateMultipleClientTypeModuleAccess")]
        public async Task<IActionResult> PutRange([FromQuery] int clientTypeId, [FromBody] IEnumerable<ClientTypeModuleAccessModel> clientTypeModuleAccessModelList)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _clientTypeModuleAccessBusiness.UpdateRange(tokenData.UserName, clientTypeId, clientTypeModuleAccessModelList);
            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to soft delete single ClientTypeModuleAccess details.
        /// </summary>
        /// <param name="clientTypeAccessId">clientTypeAccessId</param>
        /// <returns>returns response  message</returns>
        [HttpDelete]
        [ActionName("DeleteClientTypeModuleAccess")]
        public async Task<IActionResult> Delete([FromQuery]int clientTypeAccessId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _clientTypeModuleAccessBusiness.Delete(tokenData.UserName, clientTypeAccessId);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to soft delete multiple ClientTypeModuleAccess details.
        /// </summary>
        /// <param name="clientTypeId">clientTypeId</param>
        /// <returns>returns response  message</returns>
        [HttpDelete]
        [ActionName("DeleteClientTypeModuleAccessByClientType")]
        public async Task<IActionResult> DeleteRange([FromQuery]int clientTypeId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _clientTypeModuleAccessBusiness.DeleteByClientType(tokenData.UserName, clientTypeId);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        #endregion

        #region ClientTypeAccessException API Methods
        /// <summary>
        /// This method is used to get all ClientTypeAccessExceptions details.
        /// </summary>
        /// <returns>returns response message</returns>
        [HttpGet]
        [ActionName("GetClientTypeAccessException")]
        public async Task<IActionResult> GetClientTypeAccessException()
        {
            var result = await _clientTypeModuleAccessBusiness.GetClientTypeAccessException();

            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to save one ClientTypeModuleAccess Entry.
        /// </summary>
        /// <param name="accessExceptionModel">accessExceptionModel object</param>
        /// <returns>returns response message</returns>
        [HttpPost]
        [ActionName("SaveClientTypeAccessException")]
        public async Task<IActionResult> PostClientTypeAccessException([FromBody] AccessExceptionModel accessExceptionModel)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _clientTypeModuleAccessBusiness.SaveClientTypeAccessException(tokenData.UserName, accessExceptionModel);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to ClientTypeAccessException details.
        /// </summary>
        /// <param name="accessExceptionId">accessExceptionId</param>
        /// <returns>returns response  message</returns>
        [HttpDelete]
        [ActionName("DeleteClientTypeAccessException")]
        public async Task<IActionResult> DeleteClientTypeAccessException([FromQuery]int accessExceptionId)
        {
            var result = await _clientTypeModuleAccessBusiness.DeleteClientTypeAccessException(accessExceptionId);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }
        #endregion
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
                _clientTypeModuleAccessBusiness.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
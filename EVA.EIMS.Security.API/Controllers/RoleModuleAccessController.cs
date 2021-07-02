using EVA.EIMS.Common;
using EVA.EIMS.Common.Constants;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity.ViewModel;
using EVA.EIMS.Helper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Security.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]/[action]")]
    public class RoleModuleAccessController : Controller, IDisposable
    {
        #region Private Variables
        private IRoleModuleAccessBusiness _roleModuleAccessBusiness;
        #endregion

        #region Constructor
        public RoleModuleAccessController(IRoleModuleAccessBusiness RoleModuleAccessBusiness)
        {
             _roleModuleAccessBusiness = RoleModuleAccessBusiness;
            
        }
        #endregion

        #region Public API Methods
        #region RoleModuleAccess API Methods
        /// <summary>
        /// This method is used to get the multiple RoleModuleAccess details.
        /// </summary>
        /// <returns>returns response message</returns>
        [HttpGet]
        [ActionName("GetRoleModuleAccess")]
        public async Task<IActionResult> Get()
        {
            var result = await _roleModuleAccessBusiness.Get();

            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used get RoleModuleAccess details by id.
        /// </summary>
        /// <param name="accessId">accessId</param>
        /// <returns>returns response message</returns>
        [HttpGet]
        [ActionName("GetRoleModuleAccessById")]
        public async Task<IActionResult> GetbyId([FromQuery] int accessId)
        {
            var result = await _roleModuleAccessBusiness.GetById(accessId);

            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used get the RoleModuleAccess details by id.
        /// </summary>
        /// <param name="roleId">roleId</param>
        /// <returns>returns response message</returns>
        [HttpGet]
        [ActionName("GetRoleModuleAccessByRoleId")]
        public async Task<IActionResult> GetbyRoleId([FromQuery] int roleId)
        {
            var result = await _roleModuleAccessBusiness.GetByRoleId(roleId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }
   
        /// <summary>
        /// This method is used to save one RoleModuleAccess Entry.
        /// </summary>
        /// <param name="roleModuleAccessModel">roleModuleAccessModel object</param>
        /// <returns>returns response message</returns>
        [HttpPost]
        [ActionName("SaveRoleModuleAccess")]
        public async Task<IActionResult> Post([FromBody] RoleModuleAccessModel roleModuleAccessModel)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _roleModuleAccessBusiness.Save(tokenData.UserName, roleModuleAccessModel);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to save multiple the RoleModuleAccess details.
        /// </summary>
        /// <param name="RoleModuleAccess">RoleModuleAccess object</param>
        /// <returns>returns response message</returns>
        [HttpPost]
        [ActionName("SaveMultipleRoleModuleAccess")]
        public async Task<IActionResult> PostRange([FromBody] List<RoleModuleAccessModel> roleModuleAccessModelList)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _roleModuleAccessBusiness.SaveRange(tokenData.UserName, roleModuleAccessModelList);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to update single RoleModuleAccess entry.
        /// </summary>
        /// <param name="roleId">RoleModuleAccessId</param>
        /// <param name="RoleModuleAccess">RoleModuleAccess object</param>
        /// <returns>returns response message</returns>
        [HttpPut]
        [ActionName("UpdateRoleModuleAccess")]
        public async Task<IActionResult> Put([FromQuery] int roleAccessId, [FromBody] RoleModuleAccessModel roleModuleAccess)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _roleModuleAccessBusiness.Update(tokenData.UserName, roleAccessId, roleModuleAccess);
            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to update multiple the RoleModuleAccess details.
        /// </summary>
        /// <param name="roleId">roleId</param>
        /// <param name="roleModuleAccessModelList">roleModuleAccessModelList object</param>
        /// <returns>returns response message</returns>
        [HttpPut]
        [ActionName("UpdateMultipleRoleModuleAccess")]
        public async Task<IActionResult> PutRange([FromQuery] int roleId, [FromBody] IEnumerable<RoleModuleAccessModel> roleModuleAccessModelList)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _roleModuleAccessBusiness.UpdateRange(tokenData.UserName, roleId, roleModuleAccessModelList);
            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to soft delete single RoleModuleAccess details.
        /// </summary>
        /// <param name="roleAccessId">roleAccessId</param>
        /// <returns>returns response  message</returns>
        [HttpDelete]
        [ActionName("DeleteRoleModuleAccess")]
        public async Task<IActionResult> Delete([FromQuery]int roleAccessId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _roleModuleAccessBusiness.Delete(tokenData.UserName, roleAccessId);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to soft delete multiple RoleModuleAccess details.
        /// </summary>
        /// <param name="roleId">roleId</param>
        /// <returns>returns response  message</returns>
        [HttpDelete]
        [ActionName("DeleteRoleModuleAccessByRole")]
        public async Task<IActionResult> DeleteRange([FromQuery]int roleId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _roleModuleAccessBusiness.DeleteByRole(tokenData.UserName, roleId);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }
        #endregion

        #region RoleAccessException API Methods
        /// <summary>
        /// This method is used to get all RoleAccessExceptions details.
        /// </summary>
        /// <returns>returns response message</returns>
        [HttpGet]
        [ActionName("GetRoleAccessException")]
        public async Task<IActionResult> GetRoleAccessException()
        {
            var result = await _roleModuleAccessBusiness.GetRoleAccessException();

            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to save one RoleModuleAccess Entry.
        /// </summary>
        /// <param name="accessExceptionModel">accessExceptionModel object</param>
        /// <returns>returns response message</returns>
        [HttpPost]
        [ActionName("SaveRoleAccessException")]
        public async Task<IActionResult> PostRoleAccessException([FromBody] AccessExceptionModel accessExceptionModel)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _roleModuleAccessBusiness.SaveRoleAccessException(tokenData.UserName, accessExceptionModel);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to RoleAccessException details.
        /// </summary>
        /// <param name="accessExceptionId">accessExceptionId</param>
        /// <returns>returns response  message</returns>
        [HttpDelete]
        [ActionName("DeleteRoleAccessException")]
        public async Task<IActionResult> DeleteRoleAccessException([FromQuery]int accessExceptionId)
        {
            var result = await _roleModuleAccessBusiness.DeleteRoleAccessException(accessExceptionId);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }
        #endregion

        #region Action API Method
        /// <summary>
        /// This method is used to get the all Actions details.
        /// </summary>
        /// <returns>returns response message</returns>
        [HttpGet]
        [ActionName("GetAllActions")]
        public async Task<IActionResult> GetAllActions()
        {
            var result = await _roleModuleAccessBusiness.GetAllActions();

            if (result.Success)
                return Ok(result.Data);
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
                _roleModuleAccessBusiness.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
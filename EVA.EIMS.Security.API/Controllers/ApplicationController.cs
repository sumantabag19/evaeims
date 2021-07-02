using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EVA.EIMS.Security.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]/[action]")]
    public class ApplicationController : Controller, IDisposable
    {
        #region Private Variables
        private IApplicationBusiness _applicationBusiness;
        private IApplicationRoleMappingBusiness _applicationRoleMappingBusiness;
        #endregion

        #region Constructor
        public ApplicationController(IApplicationBusiness applicationBusiness, IApplicationRoleMappingBusiness applicationRoleMappingBusiness)
        {
            _applicationBusiness = applicationBusiness;
            _applicationRoleMappingBusiness = applicationRoleMappingBusiness;
        }
        #endregion

        #region Public API Methods

        #region Application API Methods
        /// <summary>
        /// This method is used to get the multiple application details.
        /// </summary>
        /// <returns>returns multiple application details</returns>

        [HttpGet]
        [ActionName("GetApplication")]
        public async Task<IActionResult> Get()
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _applicationBusiness.Get(tokenData);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used get the application details by id.
        /// </summary>
        /// <param name="appId">applicationId</param>
        /// <returns>returns single application details</returns>
        [HttpGet]
        [ActionName("GetApplicationById")]
        public async Task<IActionResult> GetById([FromQuery] int appId)
        {
            var result = await _applicationBusiness.GetById(appId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used get the application details by id.
        /// </summary>
        /// <param name="appId">applicationId</param>
        /// <returns>returns single application details</returns>
        [HttpGet]
        [ActionName("GetApplicationByName")]
        public async Task<IActionResult> GetByAppName([FromQuery]string appName)
        {
            var result = await _applicationBusiness.GetByApplicationName(appName);
            if (result != 0)
                return Ok(result);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// This method is used to save the application details.
        /// </summary>
        /// <param name="application">application object</param>
        /// <returns>returns response message</returns>
        [HttpPost]
        [ActionName("SaveApplication")]
        public async Task<IActionResult> Post([FromBody] Application application)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            ModelState.Remove("AppId");
            if (ModelState.IsValid)
            {
                var result = await _applicationBusiness.Save(tokenData.UserName, application);
                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            else { return BadRequest(ModelState); }
        }

        /// <summary>
        /// This method is used to update the application details.
        /// </summary>
        /// <param name="appId">applicationId</param>
        /// <param name="application">application object</param>
        /// <returns>returns response message</returns>
        [HttpPut]
        [ActionName("UpdateApplication")]
        public async Task<IActionResult> Put([FromBody] Application application)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            if (ModelState.IsValid)
            {
                var result = await _applicationBusiness.Update(tokenData.UserName, application.AppId, application);
                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            else { return BadRequest(ModelState); }
        }

        /// <summary>
        /// This method is used to delete the application details.
        /// </summary>
        /// <param name="appId">applicationId</param>
        /// <returns>returns response  message</returns>
        [HttpDelete]
        [ActionName("DeleteApplication")]
        public async Task<IActionResult> Delete([FromQuery]int appId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _applicationBusiness.Delete(tokenData.UserName, appId);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }
        /// <summary>
        /// this method returns all the application associated with given user 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>application list</returns>
        [HttpGet]
        [ActionName("GetAllApplicationByUserId")]
        public async Task<IActionResult> GetAllApplicationByUserId([FromQuery] Guid userId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _applicationBusiness.GetAllApplicationByUserId(userId, tokenData);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }
        #endregion

        #region RoleApplicationMapping API Methods
        /// <summary>
        /// Get all application roles
        /// </summary>
        /// <param name="appRoleId"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetAllApplicationRoles")]
        public async Task<IActionResult> GetAllApplicationRoles([FromQuery]int appRoleId)
        {
            var result = await _applicationRoleMappingBusiness.GetAllApplicationRoles(appRoleId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// Get role assigned to particular applicaion
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetApplicationRoles")]
        public async Task<IActionResult> GetApplicationRoles([FromQuery]int appId)
        {
            var result = await _applicationRoleMappingBusiness.GetApplicationRoles(appId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// Get application role by application role id
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetApplicationRolesById")]
        public async Task<IActionResult> GetApplicationRolesById([FromQuery]int applicationRoleId)
        {
            var result = await _applicationRoleMappingBusiness.GetApplicationRolesById(applicationRoleId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// Get all roles which are not assigned to particular application
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetAllRolesByApplicationName")]
        public async Task<IActionResult> GetAllRolesByApplicationName([FromQuery]string applicationName)
        {
            var result = await _applicationRoleMappingBusiness.GetAllRolesByApplicationName(applicationName);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }
        /// <summary>
        /// This method is used to save the application details.
        /// </summary>
        /// <param name="application">application object</param>
        /// <returns>returns response message</returns>
        [HttpPost]
        [ActionName("SaveApplicationRoles")]
        public async Task<IActionResult> SaveApplicationRoles([FromBody] ApplicationRoleMapping application)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _applicationRoleMappingBusiness.SaveApplicationRoles(tokenData.UserName, application);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to save the application details.
        /// </summary>
        /// <param name="application">application object</param>
        /// <returns>returns response message</returns>
        [HttpPost]
        [ActionName("SaveApplicationNameRoleNames")]
        public async Task<IActionResult> SaveApplicationNameRoleNames([FromBody]ApplicationRoleViewModel applicationRole)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _applicationRoleMappingBusiness.SaveApplicationRolesMapping(tokenData.UserName, applicationRole);

            if (result.Success)
                 return Ok(result.Result);
            else
                 return BadRequest(result.Result);
        }

        /// <summary>
        /// Method to delete application role mapping
        /// </summary>
        /// <param name="applicationRoleId"></param>
        /// <returns></returns>
        [HttpDelete]
        [ActionName("DeleteApplicationRole")]
        public async Task<IActionResult> DeleteApplicationRole([FromQuery] int applicationRoleId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _applicationRoleMappingBusiness.DeleteApplicationRoles(tokenData.UserName.ToString(), applicationRoleId);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// Update application role
        /// </summary>
        /// <param name="applicationRoleMapping"></param>
        /// <returns></returns>
        [HttpPut]
        [ActionName("UpdateApplicationRole")]
        public async Task<IActionResult> UpdateApplicationRole([FromBody]ApplicationRoleModel applicationRoleMapping)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _applicationRoleMappingBusiness.UpdateApplicationRoles(tokenData.UserName.ToString(),applicationRoleMapping);

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
                _applicationBusiness.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}

using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ViewModel;
using EVA.EIMS.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace EVA.EIMS.Security.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]/[action]")]
    public class OrganizationController : Controller, IDisposable
    {
        #region Instance Variables

        private IOrganizationBusiness _organizationBusiness;
        private IOrganizationApplicationmappingBusiness _organizationApplicationmappingBusiness;

        #endregion

        #region Constructor
        public OrganizationController(IOrganizationBusiness organizationBusiness, IOrganizationApplicationmappingBusiness organizationApplicationmappingBusiness)
        {
            _organizationBusiness = organizationBusiness;
            _organizationApplicationmappingBusiness = organizationApplicationmappingBusiness;
        }
        #endregion

        #region Public API Methods

        #region Organization
        /// <summary>
        /// This method is used to get the multiple organization details    
        /// </summary>
        /// <returns>returns multiple organization details</returns>
        [HttpGet]
        [ActionName("GetOrganization")]
        public async Task<IActionResult> Get()
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _organizationBusiness.Get(tokenData);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);

        }

        /// <summary>
        /// This method is used to get the organization details by id
        /// </summary>
        /// <param name="orgId">orgId</param>
        /// <returns>returns single organization details</returns>
        [HttpGet]
        [ActionName("GetOrganizationById")]
        public async Task<IActionResult> GetOrg([FromQuery] string orgId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _organizationBusiness.GetByOrgName(tokenData, orgId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }


        /// <summary>
        /// This method is used to save the organization details
        /// </summary>
        /// <param name="org">organization object</param>
        /// <returns>returns response message</returns>
        [HttpPost]
        [ActionName("SaveOrganization")]
        public async Task<IActionResult> Post([FromBody] Organization org)
        {

            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            ModelState.Remove("OrgId");
            if (ModelState.IsValid)
            {
                var result = await _organizationBusiness.Save(tokenData.UserName, org);
                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);

        }

        /// <summary>
        /// This method is used to update the organization details
        /// </summary>
        /// <param name="orgId">orgId</param>
        /// <param name="org">organization object</param>
        /// <returns>returns response message</returns>
        [HttpPut]
        [ActionName("UpdateOrganization")]
        public async Task<IActionResult> Put([FromBody] Organization org)
        {

            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _organizationBusiness.Update(tokenData.UserName, org.OrgId, org);
            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to delete the organization details
        /// </summary>
        /// <param name="orgId">orgId</param>
        /// <returns>returns response message</returns>
        [HttpDelete]
        [ActionName("DeleteOrganization")]
        public async Task<IActionResult> Delete([FromQuery] string orgId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _organizationBusiness.Delete(tokenData.UserName, orgId);
            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);

        }

        /// <summary>
        /// This method is used to delete all users & devices by organization Id
        /// </summary>
        /// <param name="orgId">orgId</param>
        /// <returns>returns response message</returns>
        [HttpDelete]
        [ActionName("DeleteAllUsersByOrgId")]
        public async Task<IActionResult> DeleteAllUsersAndDevicesByOrgId([FromQuery] string orgId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _organizationBusiness.DeleteAllUsersAndDevicesById(tokenData.UserName, orgId);
            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);

        }

        #endregion

        #region Organization Application Mapping
        /// <summary>
        /// to save data
        /// </summary>
        /// <param name="organizationApplicationmapping"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SaveOrganizationApplicationMapping")]
        public async Task<IActionResult> SaveOrganizationApplicationMapping([FromBody] OrganizationApplicationmapping organizationApplicationmapping)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            ModelState.Remove("OrganizationApplicationId");
            if (ModelState.IsValid)
            {
                var result = await _organizationApplicationmappingBusiness.SaveOrgAppMapping(tokenData.UserName, organizationApplicationmapping);
                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// to save data
        /// </summary>
        /// <param name="organizationApplicationmapping"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SaveOrganizationApplication")]
        public async Task<IActionResult> SaveOrganizationApplication([FromBody] OrganizationApplicationViewModel organizationApplicationmapping)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            ModelState.Remove("OrganizationApplicationId");
            if (ModelState.IsValid)
            {
                var result = await _organizationApplicationmappingBusiness.SaveOrganizatonApplicationMapping(tokenData.UserName, organizationApplicationmapping);
                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// to update orgid, appid and active status
        /// </summary>
        /// <param name="mappingId"></param>
        /// <param name="organizationApplicationmapping"></param>
        /// <returns></returns>
        [HttpPut]
        [ActionName("UpdateOrganizationApplicationMapping")]
        public async Task<IActionResult> UpdateOrganizationApplicationMapping([FromBody] OrganizationApplicationmapping organizationApplicationmapping)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            if (ModelState.IsValid)
            {
                var result = await _organizationApplicationmappingBusiness.UpdateOrgAppMapping(tokenData, organizationApplicationmapping.OrganizationApplicationId, organizationApplicationmapping);
                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);

        }

        /// <summary>
        /// to delete applicationid ad corrsponding organization id
        /// </summary>
        /// <param name="mappingId"></param>
        /// <returns></returns>
        [HttpDelete]
        [ActionName("DeleteOrganizationApplicationMapping")]
        public async Task<IActionResult> DeleteOrganizationApplicationMapping([FromQuery] int mappingId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _organizationApplicationmappingBusiness.Delete(tokenData.UserName.ToString(), mappingId);
            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }



        /// <summary>
        /// to get all applicationid corresponding to organizationid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetAllOrgAppMapping")]
        public async Task<IActionResult> GetAllOrgAppMapping()
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _organizationApplicationmappingBusiness.Get(tokenData);

            if (result != null)
                return Ok(result);
            else
                return BadRequest(ResourceInformation.GetResValue("NotExists"));
        }


        /// <summary>
        /// to get all applicationid corresponding to organizationid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetAllOrgNameAppNameMapping")]
        public async Task<IActionResult> GetAllOrgNameAppNameMapping([FromQuery] int orgAppMappingId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _organizationApplicationmappingBusiness.GetOrgNameAppName(tokenData, orgAppMappingId);

            if (result != null)
                return Ok(result.Data);
            else
                return BadRequest(ResourceInformation.GetResValue("NotExists"));
        }

        /// <summary>
        /// to get all applicationid corresponding to organizationid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetAllExceptionAppNameByOrgName")]
        public async Task<IActionResult> GetAllExceptionAppNameByOrgNname([FromQuery] string orgName)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _organizationApplicationmappingBusiness.GetAllExceptionAppNameByOrgId(tokenData, orgName);

            if (result != null)
                return Ok(result.Data);
            else
                return BadRequest(ResourceInformation.GetResValue("NotExists"));
        }

        /// <summary>
        /// to get all applicationid corresnponding organizationid
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetAllAppByOrgId")]
        public async Task<IActionResult> GetAllAppByOrgId([FromQuery] int orgId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _organizationApplicationmappingBusiness.GetById(orgId);
            if (result != null)
                return Ok(result);
            else
                return BadRequest(ResourceInformation.GetResValue("NotExists"));
        }
        /// <summary>
        /// to get all organization and application details
        /// </summary>
        /// <returns>list of organization application details</returns>
        [HttpGet]
        [ActionName("GetAllOrgAppDetails")]
        public async Task<IActionResult> GetAllOrgAppDetails()
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _organizationBusiness.GetOrganizatioApplicationDetails(tokenData);

            if (result != null)
                return Ok(result.Data);
            else
                return BadRequest(ResourceInformation.GetResValue("NotExists"));
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
                _organizationBusiness.Dispose();
                _organizationApplicationmappingBusiness.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}


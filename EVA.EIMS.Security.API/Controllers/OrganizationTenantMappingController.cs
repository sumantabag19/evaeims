using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVA.EIMS.Security.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class OrganizationTenantMappingController : Controller
    {
        #region Private Variables

        private IOrganizationTenantMapping _organizationTenantMapping;

        #endregion


        #region Constructor
        public OrganizationTenantMappingController(IOrganizationTenantMapping organizationTenantMapping)
        {
            _organizationTenantMapping = organizationTenantMapping;
        }
        #endregion

        // Get: api/OrganizationTenantMapping/GetAllOrgTenantMapping
        [HttpGet]
        [ActionName("GetAllOrgTenantMapping")]
        public async Task<IActionResult> Get()
        {
            var result = await _organizationTenantMapping.GetAllOrgTenantMapping();
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // Get: api/OrganizationTenantMapping/GetbyTenantId?tenantId=
        [HttpGet]
        [ActionName("GetbyTenantId")]
        public async Task<IActionResult> Get(string tenantId)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _organizationTenantMapping.GetOrgIDbyTenantId(tenantId));
            }
            return BadRequest(ModelState);
        }

        // Get: api/OrganizationTenantMapping/GetbyTenantId?tenantId=
        [HttpGet]
        [ActionName("GetOrgAndAppDetails")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(string tenantId, string azureAppId)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _organizationTenantMapping.GetOrgAppDetails(tenantId, azureAppId));
            }
            return BadRequest(ModelState);
        }


        // POST: api/OrganizationTenantMapping/CreateOrganizationTenantMaping?userId=
        [HttpPost]
        [ActionName("CreateOrganizationTenantMaping")]
        public async Task<IActionResult> Post([FromBody]OrganizationTenantMappingDomainModel mappingModel, Guid userId)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _organizationTenantMapping.CreateOrganizationTenantMapping(mappingModel, userId));
            }
            return BadRequest(ModelState);
        }

        // PUT: api/OrganizationTenantMapping/UpdateTenantOrgMapping?userId=
        [HttpPut]
        [ActionName("UpdateTenantOrgMapping")]
        public async Task<IActionResult> Put([FromBody]OrganizationTenantMappingDomainModel mappingModel, Guid userId)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _organizationTenantMapping.UpdateTenantOrgMapping(mappingModel,userId));
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/OrganizationTenantMapping/DeleteOrgTenantMapping?userId=
        [HttpDelete]
        [ActionName("DeleteOrgTenantMapping")]
        public async Task<IActionResult> Delete([FromBody]OrganizationTenantMappingDomainModel mappingModel, Guid userId)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _organizationTenantMapping.DeleteOrgTenantMapping(mappingModel, userId));
            }
            return BadRequest(ModelState);
        }
    }
}

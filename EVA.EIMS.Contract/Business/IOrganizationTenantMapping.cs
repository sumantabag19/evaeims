using System;
using System.Collections.Generic;
using System.Text;
using EVA.EIMS.Entity.ViewModel;
using System.Threading.Tasks;
using EVA.EIMS.Common;

namespace EVA.EIMS.Contract.Business
{
    public interface IOrganizationTenantMapping: IDisposable
    {
        Task<List<OrganizationTenantMappingModel>> GetOrgIDbyTenantId(string tenantId);
        Task<IEnumerable<OrganizationTenantMappingDomainModel>> GetAllOrgTenantMapping();
        Task<ReturnResult> CreateOrganizationTenantMapping(OrganizationTenantMappingDomainModel organizationTenantMappingModel, Guid guid);
        Task<ReturnResult> UpdateTenantOrgMapping(OrganizationTenantMappingDomainModel organizationTenantMappingModel,Guid guid);
        Task<ReturnResult> DeleteOrgTenantMapping(OrganizationTenantMappingDomainModel organizationTenantMappingModel, Guid guid);

        Task<List<OrganizationApplicationDetailModel>> GetOrgAppDetails(string tenantId,string azureAppId);
    }
}

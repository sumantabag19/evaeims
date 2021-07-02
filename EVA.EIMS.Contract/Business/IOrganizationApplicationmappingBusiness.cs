using EVA.EIMS.Common;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ComplexEntities;
using EVA.EIMS.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IOrganizationApplicationmappingBusiness: IDisposable
    {
        Task<IEnumerable<OrganizationApplicationmapping>> Get(TokenData tokenData);
        Task<OrganizationApplicationMappingDetails> GetById(int orgId);
        Task<ReturnResult> SaveOrgAppMapping(string UserName,OrganizationApplicationmapping organizationApplicationmapping);
        Task<ReturnResult> UpdateOrgAppMapping(TokenData tokenData, int mappingId, OrganizationApplicationmapping organizationApplicationmapping);
        Task<ReturnResult> Delete(string userName,int mappingId);
        Task<ReturnResult> GetOrgNameAppName(TokenData tokenData, int orgAppMappingId);
        Task<ReturnResult> SaveOrganizatonApplicationMapping(string userName, OrganizationApplicationViewModel orgApp);
        Task<ReturnResult> GetAllExceptionAppNameByOrgId(TokenData tokenData, string orgName);
    }
}

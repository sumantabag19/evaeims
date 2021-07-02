using EVA.EIMS.Common;
using EVA.EIMS.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IOrganizationBusiness : IDisposable
    {
        Task<ReturnResult> Get(TokenData tokenData);
        Task<ReturnResult> GetByOrgName(TokenData tokenData, string orgName);
        Task<ReturnResult> Save(string userName, Organization org);
        Task<ReturnResult> Update(string userName, int orgId, Organization org);
        Task<ReturnResult> Delete(string userName, string orgName);
        Task<ReturnResult> DeleteAllUsersAndDevicesById(string userName, string orgName);
        Task<Organization> GetOrganizationIdByOrgName(string OrgName);
        Task<Organization> GetOrganizationByOrgId(int? OrgId);
        Task<ReturnResult> GetOrganizatioApplicationDetails(TokenData tokenData);
        Task<List<Organization>> GetAllActiveOrganizationByClientId(string clientId);
        Task<List<Organization>> GetAllOrganizationByClientId(string clientId);
    }
}

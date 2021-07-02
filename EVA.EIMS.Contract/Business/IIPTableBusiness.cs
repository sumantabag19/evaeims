using EVA.EIMS.Common;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ComplexEntities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IIPTableBusiness : IDisposable
    {
        Task <ReturnResult> Get();
        Task <ReturnResult> GetById(int iPAddressId);
        Task<ReturnResult> Save(string userName, IPTable iPTable);
        Task<ReturnResult> Update(string userName, IPTable iPTable);
        Task<ReturnResult> Delete(string userName, int iPAddressId);
        Task<IPTable> GetByOrgIdAndAppId(int orgId, int appId);
        Task<IEnumerable<IPAddressRange>> GetByOrgIdAndAppIdArray(int orgId, string appId);
        Task<ReturnResult> IsIPAuthorized(IPAddress ipAddress, int orgId, int appId);
        Task<ReturnResult> IPAuthorizedForServiceClient(IPAddress ipAddress, int orgId, string appId);
    }

}

using EVA.EIMS.Entity.ComplexEntities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IApplicationUserMappingBusiness:IDisposable
    {
        Task<ApplicationUserDetails> GetUserMappingWithApplicationId(string UserName, string AppId, string ClientId);
        Task<ApplicationUserDetails> GetClientIdMappingWithApplicationId(string ClientId, string ClientSecret, string OrgName);
        Task<bool> ValidateUserApplicationAccess(Guid usedId, int appId);

    }
}

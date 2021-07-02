using EVA.EIMS.Common;
using EVA.EIMS.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IAuthProviderBusiness:IDisposable
    {
        Task<ReturnResult> Get();
		Task<ReturnResult> GetByName(string providerName);
		Task<ReturnResult> Save(string userName, AuthProviderMaster authProvider );
		Task<ReturnResult> Update(string userName, AuthProviderMaster authProviderMaster);
		Task<ReturnResult> Delete(string userName, int providerId);
        Task<AuthProviderMaster> GetById(int providerId);
    }
}

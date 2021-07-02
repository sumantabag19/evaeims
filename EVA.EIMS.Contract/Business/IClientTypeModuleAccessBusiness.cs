using EVA.EIMS.Common;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IClientTypeModuleAccessBusiness : IDisposable
    {
        #region ClientTypeModuleAccess Methods
        Task<ReturnResult> Get();
        Task<ReturnResult> GetById(int clientTypeAccessId);
        Task<ReturnResult> GetByClientTypeId(int clientTypeId);
        Task<ReturnResult> Save(string userName, ClientTypeModuleAccessModel clientTypeModuleAccess);
        Task<ReturnResult> Update(string userName, int clientTypeAccessId, ClientTypeModuleAccessModel clientTypeModuleAccessModel);
        Task<ReturnResult> Delete(string userName, int clientTypeAccessId);
        Task<ReturnResult> SaveRange(string userName, IEnumerable<ClientTypeModuleAccessModel> clientTypeModuleAccessmodelList);
        Task<ReturnResult> UpdateRange(string userName, int clientTypeAccessId, IEnumerable<ClientTypeModuleAccessModel> clientTypeModuleAccessModelList);
        Task<ReturnResult> DeleteByClientType(string userName, int clientTypeAccessId);
        #endregion

        #region ClientTypeAccessException Methods
        Task<ReturnResult> GetClientTypeAccessException();
        Task<ReturnResult> SaveClientTypeAccessException(string userName, AccessExceptionModel accessExceptionModel);
        Task<ReturnResult> DeleteClientTypeAccessException(int accessExceptionId);
        #endregion
    }
}

using EVA.EIMS.Common;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IRoleModuleAccessBusiness : IDisposable
    {
        #region RoleModuleAccess Methods
        Task<ReturnResult> Get();
        Task<ReturnResult> GetById(int roleAccessId);
        Task<ReturnResult> GetByRoleId(int roleId);
        Task<ReturnResult> Save(string userName, RoleModuleAccessModel roleModuleAccess);
        Task<ReturnResult> Update(string userName, int roleAccessId, RoleModuleAccessModel roleModuleAccessModel);
        Task<ReturnResult> Delete(string userName, int roleAccessId);
        Task<ReturnResult> SaveRange(string userName, IEnumerable<RoleModuleAccessModel> roleModuleAccessmodelList);
        Task<ReturnResult> UpdateRange(string userName, int roleId, IEnumerable<RoleModuleAccessModel> roleModuleAccessModelList);
		Task<ReturnResult> DeleteByRole(string userName, int roleId);
		#endregion

		#region RoleAccessException Methods
		Task<ReturnResult> GetRoleAccessException();
		Task<ReturnResult> SaveRoleAccessException(string userName, AccessExceptionModel accessExceptionModel);
		Task<ReturnResult> DeleteRoleAccessException(int accessExceptionId);
        #endregion
        #region Action Methods
        Task<ReturnResult> GetAllActions();
        #endregion
    }
}

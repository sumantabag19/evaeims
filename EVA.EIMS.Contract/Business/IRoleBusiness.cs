using EVA.EIMS.Common;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IRoleBusiness : IDisposable
    {
        Task<ReturnResult> GetRole();

		Task<ReturnResult> GetRole(int roleId);

		Task<ReturnResult> SaveRole(string userName, Role role);

		Task<ReturnResult> UpdateRole(string userName, int roleId, Role role);

		Task<ReturnResult> DeleteRole(string userName, int roleId);
    Task<ReturnResult> GetRoleByRoleName(string roleName);

    }
}

using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Repository
{
    public interface IRoleModuleAccessRepository : IBaseRepository<RoleModuleAccess>, IDisposable
    {
		Task<bool> AddRange(IEnumerable<RoleModuleAccess> roleModuleAccessList);
		Task<bool> UpdateRange(IEnumerable<RoleModuleAccess> roleModuleAccessList);
		Task<bool> DeleteRange(IEnumerable<RoleModuleAccess> roleModuleAccessList);
        IUnitOfWork UnitOfWork { get; }
    }
}

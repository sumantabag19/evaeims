using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Repository
{
    public interface IClientTypeModuleAccessRepository : IBaseRepository<ClientTypeModuleAccess>, IDisposable
    {
		Task<bool> AddRange(IEnumerable<ClientTypeModuleAccess> clientTypeModuleAccessList);
		Task<bool> UpdateRange(IEnumerable<ClientTypeModuleAccess> clientTypeModuleAccessList);
		Task<bool> DeleteRange(IEnumerable<ClientTypeModuleAccess> clientTypeModuleAccessList);
        IUnitOfWork UnitOfWork { get; }
    }
}

using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Repository
{
    public interface IApplicationRoleMappingRepository : IBaseRepository<ApplicationRoleMapping>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
        Task AddRange(IEnumerable<ApplicationRoleMapping> appRole);

    }
}

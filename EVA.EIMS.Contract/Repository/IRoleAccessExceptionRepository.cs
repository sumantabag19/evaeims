using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;

namespace EVA.EIMS.Contract.Repository
{
    public interface IRoleAccessExceptionRepository : IBaseRepository<RoleAccessException>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

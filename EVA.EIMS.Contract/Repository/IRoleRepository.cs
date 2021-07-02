using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;

namespace EVA.EIMS.Contract.Repository
{
    public interface IRoleRepository : IBaseRepository<Role>,IDisposable
    {
         IUnitOfWork UnitOfWork { get; }
    }
}

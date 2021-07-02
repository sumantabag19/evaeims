using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;

namespace EVA.EIMS.Contract.Repository
{
    public interface IModuleRepository : IBaseRepository<Module>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }

    }
}


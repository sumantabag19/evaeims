using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;

namespace EVA.EIMS.Contract.Repository
{
    public interface IActionsRepository : IBaseRepository<Actions>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}


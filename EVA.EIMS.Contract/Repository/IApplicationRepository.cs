using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;

namespace EVA.EIMS.Contract.Repository
{
    public interface IApplicationRepository : IBaseRepository<Application>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

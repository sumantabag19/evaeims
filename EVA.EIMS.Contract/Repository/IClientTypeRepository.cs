using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;

namespace EVA.EIMS.Contract.Repository
{
    public interface IClientTypeRepository : IBaseRepository<ClientType>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
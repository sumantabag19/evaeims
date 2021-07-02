using System;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;

namespace EVA.EIMS.Contract.Repository
{
    public interface IIPTableRepository:IBaseRepository<IPTable>,IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

using System;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;

namespace EVA.EIMS.Contract.Repository
{
    public interface IAuditUserLoginRepository:IBaseRepository<AuditUserLogin>,IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

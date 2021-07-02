using System;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;

namespace EVA.EIMS.Contract.Repository
{
    public interface IAuditDbChangeRepository: IBaseRepository<AuditDbChange>,IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace EVA.EIMS.Contract.Repository
{
    public interface IAccessTypeRepository : IBaseRepository<AccessType>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

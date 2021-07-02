using EVA.EIMS.Contract.Infrastructure;
using System;
using EVA.EIMS.Entity;

namespace EVA.EIMS.Contract.Repository
{
    public interface ISecurityQuestionRepository : IBaseRepository<SecurityQuestion>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}


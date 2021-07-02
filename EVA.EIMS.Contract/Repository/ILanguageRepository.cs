using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;

namespace EVA.EIMS.Contract.Repository
{
    public interface ILanguageRepository : IBaseRepository<Language>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

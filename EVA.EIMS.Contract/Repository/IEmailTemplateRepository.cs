using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;

namespace EVA.EIMS.Contract.Repository
{
    public interface IEmailTemplateRepository : IBaseRepository<EmailTemplate>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

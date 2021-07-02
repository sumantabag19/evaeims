using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;

namespace EVA.EIMS.Contract.Repository
{
    public interface IClientRepository : IBaseRepository<OauthClient>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

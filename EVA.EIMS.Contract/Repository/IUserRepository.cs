using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Repository
{
    public interface IUserRepository:IBaseRepository<User>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }

        Task UpdateRange(IEnumerable<User> users);
    }
}

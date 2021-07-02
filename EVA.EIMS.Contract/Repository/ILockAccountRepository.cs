using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Repository
{
    public interface ILockAccountRepository:IBaseRepository<LockAccount>,IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
        Task RemoveRange(IEnumerable<LockAccount> users);
        Task<int> ExcecuteQueryAsync(string query);
    }
}

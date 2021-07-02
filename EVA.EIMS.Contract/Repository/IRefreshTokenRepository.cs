using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Repository
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
        int ExcecuteQuery(string query);
        Task<int> ExecuteQueryAsync(string query);
    }
}

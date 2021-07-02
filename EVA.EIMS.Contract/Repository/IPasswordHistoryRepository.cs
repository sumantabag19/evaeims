using System;
using System.Threading.Tasks;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
namespace EVA.EIMS.Contract.Repository
{
    public interface IPasswordHistoryRepository:IBaseRepository<PasswordHistory>,IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
        Task<int> ExcecuteQueryAsync(string query);


    }
}

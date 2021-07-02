using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Repository
{
    public interface IIMSLogOutRepository : IBaseRepository<IMSLogOutToken>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
        Task<int> ExecuteQueryAsync(string query);
    }
}

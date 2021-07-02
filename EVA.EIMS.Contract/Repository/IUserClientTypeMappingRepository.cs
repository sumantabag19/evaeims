using System;
using EVA.EIMS.Entity;
using EVA.EIMS.Contract.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Repository
{
    public interface IUserClientTypeMappingRepository : IBaseRepository<UserClientTypeMapping>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
        Task DeleteRange(IEnumerable<UserClientTypeMapping> entity);
        Task AddRange(List<UserClientTypeMapping> userClientTypeMapping);
    }
}

using System;
using EVA.EIMS.Entity;
using EVA.EIMS.Contract.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Repository
{
    public interface IApplicationUserMappingRepository:IBaseRepository<ApplicationUserMapping>,IDisposable
    {
        Task AddRange(IEnumerable<ApplicationUserMapping> userAns);
        Task DeleteRange(IEnumerable<ApplicationUserMapping> users);
        IUnitOfWork UnitOfWork { get; }
    }
}

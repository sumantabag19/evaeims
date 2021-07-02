using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
namespace EVA.EIMS.Contract.Repository
{
    public interface IUserRoleMappingRepository:IBaseRepository<UserRoleMapping>,IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
        Task DeleteRange(IEnumerable<UserRoleMapping> entity);
        Task AddRange(List<UserRoleMapping> userRoleMapping);
    }
}

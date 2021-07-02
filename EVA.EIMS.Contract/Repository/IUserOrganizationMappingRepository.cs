using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Repository
{
    public interface IUserOrganizationMappingRepository : IBaseRepository<UserOrganizationMapping>, IDisposable
    {
		Task<bool> AddRange(IEnumerable<UserOrganizationMapping> userOrganizationMappingList);
		Task<bool> DeleteRange(IEnumerable<UserOrganizationMapping> userOrganizationMappingList);
        IUnitOfWork UnitOfWork { get; }
    }
}

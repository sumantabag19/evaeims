using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ComplexEntities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Repository
{
    public interface IOrganizationApplicationmappingRepository: IBaseRepository<OrganizationApplicationmapping>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
        Task AddRange(IEnumerable<OrganizationApplicationmapping> entity);
    }
}

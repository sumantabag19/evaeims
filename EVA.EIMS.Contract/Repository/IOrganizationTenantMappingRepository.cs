using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ViewModel;

using System;
using System.Collections.Generic;
using System.Text;

namespace EVA.EIMS.Contract.Repository
{
    public interface IOrganizationTenantMappingRepository : IBaseRepository<OrganizationTenantMappingModel>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

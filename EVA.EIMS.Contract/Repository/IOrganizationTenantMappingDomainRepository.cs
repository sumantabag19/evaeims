using System;
using System.Collections.Generic;
using System.Text;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity.ViewModel;

namespace EVA.EIMS.Contract.Repository
{
    public interface IOrganizationTenantMappingDomainRepository: IBaseRepository<OrganizationTenantMappingDomainModel>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

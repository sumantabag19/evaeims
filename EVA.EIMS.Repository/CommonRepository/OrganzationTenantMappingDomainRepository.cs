using System;
using System.Collections.Generic;
using System.Text;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity.ViewModel;
using EVA.EIMS.Helper;
using EVA.EIMS.Logging;

namespace EVA.EIMS.Repository.CommonRepository
{
    public class OrganzationTenantMappingDomainRepository: BaseRepository<OrganizationTenantMappingDomainModel>, IOrganizationTenantMappingDomainRepository
    {
        protected new readonly IUnitOfWork _uow;
        private readonly ILogger _logger;
        private bool _disposed;

        public IUnitOfWork UnitOfWork { get { return _uow; } }

        public OrganzationTenantMappingDomainRepository(IUnitOfWork uow, ILogger logger) : base(uow, logger)
        {
            _logger = logger;
            _uow = uow;
            _disposed = false;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _uow.Dispose();
                base.Dispose();
            }

            _disposed = true;
        }
    }
}

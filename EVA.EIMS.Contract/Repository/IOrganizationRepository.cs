using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;



namespace EVA.EIMS.Contract.Repository
{
    public interface IOrganizationRepository : IBaseRepository<Organization>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

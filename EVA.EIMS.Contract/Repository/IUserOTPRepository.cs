using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;

namespace EVA.EIMS.Contract.Repository
{
    public interface IUserOTPRepository : IBaseRepository<UserOTP>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}

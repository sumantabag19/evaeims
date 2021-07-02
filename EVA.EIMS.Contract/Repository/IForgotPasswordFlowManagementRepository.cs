using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Repository
{
    public interface IForgotPasswordFlowManagementRepository : IBaseRepository<ForgotPasswordFlowManagement>, IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
        Task AddOrUpdate(ForgotPasswordFlowManagement forgotPasswordFlowManagement);
    }
}




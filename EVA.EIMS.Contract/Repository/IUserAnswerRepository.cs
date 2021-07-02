using EVA.EIMS.Common;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Repository
{
    public interface IUserAnswerRepository : IBaseRepository<UserAnswer>, IDisposable
    {
        Task AddRange(IEnumerable<UserAnswer> userAns);
        Task DeleteRange(IEnumerable<UserAnswer> users);
        IUnitOfWork UnitOfWork { get; }
    }
}

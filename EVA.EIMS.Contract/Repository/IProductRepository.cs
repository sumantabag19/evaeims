using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Repository
{
    /// <summary>
    /// Product Repository Interface
    /// </summary>
    public interface IProductRepository : IBaseRepository<Products>, IExecuteEntity<Products>, IExcecuteQuery, ISelectService<Products>, IDisposable
    {

        IUnitOfWork UnitOfWork { get; }

        Task<List<Products>> GetAllProducts();

        Task<List<Products>> ExecuteProcedure(string procName, IEnumerable<Parameters> param);

    }
}

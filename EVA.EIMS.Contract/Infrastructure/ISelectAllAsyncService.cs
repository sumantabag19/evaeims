using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Infrastructure
{
    /// <summary>
    /// Implement this interface to get data using linq expression
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ISelectAllAsyncService<TEntity> where TEntity : class
    {
        /// <summary>
        /// Asynchronous method to get all records.
        /// </summary>
        /// <param name="predicate">linq expression</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> SelectAllAsync();
    }
}

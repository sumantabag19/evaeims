using System.Collections.Generic;

namespace EVA.EIMS.Contract.Infrastructure
{
    /// <summary>
    /// Implement this interface to get data using linq expression
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ISelectAllService<TEntity> where TEntity : class
    {        
        /// <summary>
        /// Get all records
        /// </summary>
        /// <param name="predicate">linq expression</param>
        /// <returns></returns>
        IEnumerable<TEntity> SelectAll();
    }
}

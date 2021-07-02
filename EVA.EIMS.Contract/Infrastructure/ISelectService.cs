using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EVA.EIMS.Contract.Infrastructure
{
    /// <summary>
    /// Implement this interface to get data using linq expression
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ISelectService<TEntity> where TEntity : class
    {
        /// <summary>
        /// Get filterd data as per linq expression
        /// </summary>
        /// <param name="predicate">linq expression</param>
        /// <returns></returns>
        IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> predicate);
    }
}

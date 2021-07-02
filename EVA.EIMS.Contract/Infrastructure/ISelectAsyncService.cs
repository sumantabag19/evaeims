using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Infrastructure
{
    /// <summary>
    /// Implement this interface to get data using linq expression
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ISelectAsyncService<TEntity> where TEntity : class
    {
        /// <summary>
        /// Get filterd data as per linq expression
        /// </summary>
        /// <param name="predicate">linq expression</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> SelectAsync(Expression<Func<TEntity, bool>> predicate);


		/// <summary>
		/// Get filterd first or defaultdata as per linq expression
		/// </summary>
		/// <param name="predicate">linq expression</param>
		/// <returns></returns>
		Task<TEntity> SelectFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
	}
}

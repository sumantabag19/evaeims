using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Infrastructure
{
    /// <summary>
    /// This interface need to be implemented by repositories which wants to add new record in entity asynchronously
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IAddAsyncService<TEntity> where TEntity : class
    {
        /// <summary>
        /// Asynchronous to add new record in to entity.
        /// </summary>
        /// <param name="entity">Database/DBContext entity</param>
        /// <returns></returns>
        Task<EntityEntry<TEntity>> AddAsync(TEntity entity);
    }
}

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EVA.EIMS.Contract.Infrastructure
{
    /// <summary>
    /// This interface need to be implemented by repositories which wants to add new record in entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IAddService<TEntity> where TEntity : class
    {
        /// <summary>
        /// Synchronous method to add new record in to entity.
        /// </summary>
        /// <param name="entity">Database/DBContext entity</param>
        /// <returns></returns>
        EntityEntry<TEntity> Add(TEntity entity);
    }
}

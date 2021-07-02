using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EVA.EIMS.Contract.Infrastructure
{
    /// <summary>
    /// This interface need to be implemented by repositories 
    /// which wants to modify record details in entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IUpdateService<TEntity> where TEntity : class
    {
        /// <summary>
        /// Synchronously update record in to entity
        /// </summary>
        /// <param name="entity">Database/DBContext entity</param>
        /// <returns></returns>
        EntityEntry<TEntity> Update(TEntity entity);
    }
}

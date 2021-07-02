using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EVA.EIMS.Contract.Infrastructure
{
    /// <summary>
    /// This interface need to be implemented by repositories 
    /// which wants to delete single record form entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IDeleteService<TEntity> where TEntity : class
    {
        /// <summary>
        /// Delete record in to entity
        /// </summary>
        /// <param name="entity">Database/DBContext entity</param>
        /// <returns></returns>
        EntityEntry<TEntity> Delete(TEntity entity);
    }
}

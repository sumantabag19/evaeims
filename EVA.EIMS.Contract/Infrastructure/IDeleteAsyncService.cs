using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Infrastructure
{
    /// <summary>
    /// This interface need to be implemented by repositories 
    /// which wants to delete single record form entity asynchronously
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IDeleteAsyncService<TEntity> where TEntity : class
    {
        /// <summary>
        /// Delete record in to entity
        /// </summary>
        /// <param name="entity">Database/DBContext entity</param>
        /// <returns></returns>
        Task<EntityEntry<TEntity>> DeleteAsync(TEntity entity);
    }
}

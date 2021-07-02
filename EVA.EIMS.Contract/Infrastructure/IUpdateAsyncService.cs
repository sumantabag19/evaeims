using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Infrastructure
{
    /// <summary>
    /// This interface need to be implemented by repositories 
    /// which wants to modify record details in entity asynchronously
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IUpdateAsyncService<TEntity> where TEntity : class
    {
        /// <summary>
        /// Asynchronously update record in to entity
        /// </summary>
        /// <param name="entity">Database/DBContext entity</param>
        /// <returns></returns>
        Task<EntityEntry<TEntity>> UpdateAsync(TEntity entity);
    }
}

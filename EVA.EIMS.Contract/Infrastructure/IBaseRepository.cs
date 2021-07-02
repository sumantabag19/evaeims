namespace EVA.EIMS.Contract.Infrastructure
{
    /// <summary>
    /// This interface provide collection of all base operation interfaces
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IBaseRepository<TEntity> : IAddService<TEntity>, IUpdateService<TEntity>, IDeleteService<TEntity>, ISelectService<TEntity>, ISelectAllService<TEntity>, IAddAsyncService<TEntity>, IUpdateAsyncService<TEntity>, IDeleteAsyncService<TEntity>, ISelectAsyncService<TEntity>, ISelectAllAsyncService<TEntity> where TEntity : class
    {
    }
}

using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Helper;
using EVA.EIMS.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EVA.EIMS.Repository
{
    /// <summary>
    /// Base Repository class is a base class which provides basic Add, Update, Delete functionality to other repositories 
    /// Each CRUD operation shold to maintain traice audit and for that User name details are important
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>, IDisposable where TEntity : class
    {
        #region Private Variable
        protected readonly IUnitOfWork _uow;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public BaseRepository(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Add new record in entity provided by repository
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual EntityEntry<TEntity> Add(TEntity entity)
        {
            return _uow.DbContext.Set<TEntity>().Add(entity);
        }

        /// <summary>
        /// Add new record in entity provided by repository
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<EntityEntry<TEntity>> AddAsync(TEntity entity)
        {
           return await _uow.DbContext.Set<TEntity>().AddAsync(entity);
        }

        /// <summary>
        /// Synchronous method to modify single record from entity provided by repository
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual EntityEntry<TEntity> Update(TEntity entity)
        {
            return _uow.DbContext.Set<TEntity>().Update(entity);
        }

        /// <summary>
        /// Modify single record from entity provided by repository
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<EntityEntry<TEntity>> UpdateAsync(TEntity entity)
        {
            return await Task.Run(() => _uow.DbContext.Set<TEntity>().Update(entity));
        }

        /// <summary>
        /// Delete single record from entity provided by repository
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual EntityEntry<TEntity> Delete(TEntity entity)
        {
            return _uow.DbContext.Set<TEntity>().Remove(entity);
        }

        /// <summary>
        /// Delete single record from entity provided by repository
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<EntityEntry<TEntity>> DeleteAsync(TEntity entity)
        {
            return await Task.Run(() => _uow.DbContext.Set<TEntity>().Remove(entity));
        }

        /// <summary>
        /// This method gets list of TEntity based on provided expression.
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>List of TEntity.</returns>
        public virtual IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> predicate)
        {
            return _uow.DbContext.Set<TEntity>().AsNoTracking().Where(predicate).ToList();
        }

        /// <summary>
        /// This asynchronous method gets list of TEntity based on provided expression.
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>List of OauthClients.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _uow.DbContext.Set<TEntity>().AsNoTracking().Where(predicate).ToListAsync();
        }

        /// <summary>
        /// This method gets list of TEntity objects.
        /// </summary>
        /// <returns>List of TEntity.</returns>
        public virtual IEnumerable<TEntity> SelectAll()
        {
            return _uow.DbContext.Set<TEntity>().AsNoTracking().ToList();
        }

        /// <summary>
        /// This asynchronous method gets list of TEntity objects.
        /// </summary>
        /// <returns>List of TEntity.</returns>
        public virtual async Task<IEnumerable<TEntity>> SelectAllAsync()
        {
            return await _uow.DbContext.Set<TEntity>().AsNoTracking().ToListAsync();
        }

		/// <summary>
		/// Get filterd first or defaultdata as per linq expression
		/// </summary>
		/// <param name="predicate">linq expression</param>
		/// <returns></returns>
		public virtual async Task<TEntity> SelectFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
		{
			return await _uow.DbContext.Set<TEntity>().AsNoTracking().Where(predicate).FirstOrDefaultAsync();
		}
        #endregion

        #region Dispose
        /// <summary>
        /// Method to dispose.
        /// </summary>
        public void Dispose()
        {
            _uow.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

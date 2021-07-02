using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Repository
{
    /// <summary>
    /// ClientRepository provides basic CRUD operations inherited from BaseRepository.
    /// </summary>
    public class UserAnswerRepository : BaseRepository<UserAnswer>, IUserAnswerRepository
    {
        #region Private Variables
        protected new readonly IUnitOfWork _uow;
        private readonly ILogger _logger;

        private bool _disposed;
        #endregion

        #region Public Properties
        public IUnitOfWork UnitOfWork { get { return _uow; } }
        #endregion

        #region Constructor
        public UserAnswerRepository(IUnitOfWork uow, ILogger logger) : base(uow, logger)
        {
            _logger = logger;
            _uow = uow;
            _disposed = false;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Add new record in entity provided by repository
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task AddRange(IEnumerable<UserAnswer> entity)
        {
            await _uow.DbContext.Set<UserAnswer>().AddRangeAsync(entity);
        }

        /// <summary>
        /// Delete Multiple record from entity provided by repository
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task DeleteRange(IEnumerable<UserAnswer> entity)
        {
			await Task.Run(() => _uow.DbContext.Set<UserAnswer>().RemoveRange(entity));
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Method to dispose by parameter.
        /// </summary>
        /// <param name="disposing"></param>
        /// 
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _uow.Dispose();
                base.Dispose();
            }

            _disposed = true;
        }

        /// <summary>
        /// Method to dispose.
        /// </summary>
        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

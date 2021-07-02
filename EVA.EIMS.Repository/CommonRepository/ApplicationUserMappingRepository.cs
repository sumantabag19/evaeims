using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using EVA.EIMS.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Repository.CommonRepository
{
    public class ApplicationUserMappingRepository:BaseRepository<ApplicationUserMapping>, IApplicationUserMappingRepository
    {
        #region Private Variables
        protected new readonly IUnitOfWork _uow;
        private bool _disposed;
        private readonly ILogger _logger;
        #endregion

        #region Public Properties
        public IUnitOfWork UnitOfWork { get { return _uow; } }
        #endregion

        #region Constructor
        public ApplicationUserMappingRepository(IUnitOfWork uow, ILogger logger) : base(uow, logger)
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
        public async Task AddRange(IEnumerable<ApplicationUserMapping> entity)
        {
            await _uow.DbContext.Set<ApplicationUserMapping>().AddRangeAsync(entity);
        }

        /// <summary>
        /// Delete Multiple record from entity provided by repository
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task DeleteRange(IEnumerable<ApplicationUserMapping> entity)
        {
            await Task.Run(() => _uow.DbContext.Set<ApplicationUserMapping>().RemoveRange(entity));
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

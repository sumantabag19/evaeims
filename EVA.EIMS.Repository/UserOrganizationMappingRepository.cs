using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Repository.CommonRepository
{
    /// <summary>
    /// UserOrganizationMappingRepository provides basic CRUD operations inherited from BaseRepository.
    /// </summary>
    public class UserOrganizationMappingRepository : BaseRepository<UserOrganizationMapping>, IUserOrganizationMappingRepository
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
        public UserOrganizationMappingRepository(IUnitOfWork uow, ILogger logger) : base(uow, logger)
        {
            _logger = logger;
            _uow = uow;
            _disposed = false;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Add multiple new RoleModuleAccess record 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>bool</returns>
        public async Task<bool> AddRange(IEnumerable<UserOrganizationMapping> entity)
        {
            await _uow.DbContext.Set<UserOrganizationMapping>().AddRangeAsync(entity); 
            return true;
        }

        /// <summary>
        /// Delete Multiple RoleModuleAccess record 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>bool</returns>
        public async Task<bool> DeleteRange(IEnumerable<UserOrganizationMapping> entity)
        {
			await Task.Run(() => _uow.DbContext.Set<UserOrganizationMapping>().RemoveRange(entity));
            return true;
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

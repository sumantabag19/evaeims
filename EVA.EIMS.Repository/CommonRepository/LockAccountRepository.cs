using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using EVA.EIMS.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Repository.CommonRepository
{
    public class LockAccountRepository:BaseRepository<LockAccount>, ILockAccountRepository
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
        public LockAccountRepository(IUnitOfWork uow, ILogger logger) : base(uow, logger)
        {
            _logger = logger;
            _uow = uow;
            _disposed = false;
        }
		#endregion

		#region Public Methods
		/// <summary>
		/// Method to Remove Lock details.
		/// </summary>
		/// <param name="users"></param>
		public async Task RemoveRange(IEnumerable<LockAccount> users)
        {
			await Task.Run(() => UnitOfWork.DbContext.Set<LockAccount>().RemoveRange(users));
        }
       

        /// <summary>
        /// Method to execute sql query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<int> ExcecuteQueryAsync(string query)
        {
            return await Task.Run(() => _uow.DbContext.Database.ExecuteSqlCommand(query));
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

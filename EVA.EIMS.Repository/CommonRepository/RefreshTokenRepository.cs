using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using EVA.EIMS.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace EVA.EIMS.Repository
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
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
		public RefreshTokenRepository(IUnitOfWork uow, ILogger logger) : base(uow, logger)
		{
			_logger = logger;
			_uow = uow;
			_disposed = false;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Execute sql command on database which does not return table data (Insert, Update, Delete) 
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public int ExcecuteQuery(string query)
        {
            return _uow.DbContext.Database.ExecuteSqlCommand(query);
        }

		/// <summary>
		/// Execute sql command on database with query
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public async Task<int> ExecuteQueryAsync(string query)
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

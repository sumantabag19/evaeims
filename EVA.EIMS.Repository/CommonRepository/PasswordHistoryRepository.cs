using System;
using System.Threading.Tasks;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EVA.EIMS.Repository
{
    public class PasswordHistoryRepository : BaseRepository<PasswordHistory>, IPasswordHistoryRepository
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
        public PasswordHistoryRepository(IUnitOfWork uow, ILogger logger) : base(uow, logger)
        {
            _logger = logger;
            _uow = uow;
            _disposed = false;
        }
        #endregion
        #region Public Method
        /// <summary>
        /// Execute sql command on database which does not return table data (Insert, Update, Delete) 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<int> ExcecuteQueryAsync(string query)
        {
            return await _uow.DbContext.Database.ExecuteSqlCommandAsync(query);
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

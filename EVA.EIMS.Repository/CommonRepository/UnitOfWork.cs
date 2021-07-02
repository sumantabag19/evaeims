using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace EVA.EIMS.Repository
{
    /// <summary>
    /// UnitOfWork class take cares of context and commit database changes new
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        #region Private Variables
        private readonly DbContext _context;
        private bool _disposed;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor to initialize DBContext.
        /// </summary>
        /// <param name="DbContext"></param>
        public UnitOfWork(DbContext context, ILogger logger)
        {
            _context = context;
            //_context.ChangeTracker.AutoDetectChangesEnabled = false;
            _disposed = false;
            _logger = logger;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Save changes synchronously
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
        /// <summary>
        /// Save changes asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync()
        {

            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Get the DBContext. 
        /// </summary>
        public DbContext DbContext
        {
            get { return _context; }
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Method to dispose by parameter.
        /// </summary>
        /// <param name="disposing"></param>
        /// 
        protected void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {

                if (_context != null)
                    _context.Dispose();

            }

            _disposed = true;
        }

        /// <summary>
        /// Method to dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

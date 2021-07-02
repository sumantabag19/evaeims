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

    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        #region Public Properties
       
        public IUnitOfWork UnitOfWork { get { return _uow; } }
        #endregion

        #region Private Variables
        protected new readonly IUnitOfWork _uow;
        private bool _disposed;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public UserRepository(IUnitOfWork uow, ILogger logger) : base(uow, logger)
        {
            _logger = logger;
            _uow = uow;
            _disposed = false;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Method to update multiple Users.
        /// </summary>
        /// <param name="disposing"></param>
        public async Task UpdateRange(IEnumerable<User> users)
        {
			await Task.Run(() => UnitOfWork.DbContext.Set<User>().UpdateRange(users));
        }

        #endregion

        #region Dispose
        /// <summary>
        /// Method to dispose by parameter.
        /// </summary>
        /// <param name="disposing"></param>
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

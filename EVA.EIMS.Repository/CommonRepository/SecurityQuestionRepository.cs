using System;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using EVA.EIMS.Logging;
using Microsoft.AspNetCore.Http;

namespace EVA.EIMS.Repository
{
    public class SecurityQuestionRepository : BaseRepository<SecurityQuestion>, ISecurityQuestionRepository
    {
        #region Public Property
        public IUnitOfWork UnitOfWork { get { return _uow; } }
        #endregion

        #region Private Variables
        private bool _disposed;
        protected new readonly IUnitOfWork _uow;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public SecurityQuestionRepository (IUnitOfWork uow, ILogger logger) : base(uow, logger)
        {
            _logger = logger;
            _uow = uow;
            _disposed = false;
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

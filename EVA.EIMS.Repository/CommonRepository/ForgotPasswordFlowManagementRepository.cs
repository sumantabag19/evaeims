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
    public class ForgotPasswordFlowManagementRepository : BaseRepository<ForgotPasswordFlowManagement>, IForgotPasswordFlowManagementRepository
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
        public ForgotPasswordFlowManagementRepository(IUnitOfWork uow, ILogger logger) : base(uow, logger)
        {
            _logger = logger;
            _uow = uow;
            _disposed = false;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds ForgotPasswordFlowManagement record if not exists or Update
        /// <param name="forgotPasswordFlowManagement">forgotPasswordFlowManagement</param>
        /// </summary>
        /// <returns>int</returns>
        public async Task AddOrUpdate(ForgotPasswordFlowManagement forgotPasswordFlowManagement)
        {
            ForgotPasswordFlowManagement existingFlowData = await UnitOfWork.DbContext.Set<ForgotPasswordFlowManagement>().FindAsync(forgotPasswordFlowManagement.UserId);

            if (existingFlowData != null)
            {
                existingFlowData.VerifiedEmail = forgotPasswordFlowManagement.VerifiedEmail;
                existingFlowData.VerifiedEmailOn = forgotPasswordFlowManagement.VerifiedEmailOn;
                existingFlowData.VerifiedOTP = forgotPasswordFlowManagement.VerifiedOTP;
                existingFlowData.VerifiedOTPOn = forgotPasswordFlowManagement.VerifiedOTPOn;
                existingFlowData.VerifiedSecurityQuestions = forgotPasswordFlowManagement.VerifiedSecurityQuestions;
                existingFlowData.VerifiedSecurityQuestionsOn = forgotPasswordFlowManagement.VerifiedSecurityQuestionsOn;

				await Task.Run(() => UnitOfWork.DbContext.Set<ForgotPasswordFlowManagement>().Update(existingFlowData));
            }
            else
            {
               await  UnitOfWork.DbContext.Set<ForgotPasswordFlowManagement>().AddAsync(forgotPasswordFlowManagement);
            }
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

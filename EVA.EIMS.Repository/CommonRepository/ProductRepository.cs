using EVA.EIMS.Common;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Repository
{
    /// <summary>
    /// This is product repository, It should take care of operations related to Producs Entity only
    /// For Add, Update and Delate operation use BaseRepository
    /// </summary>
    public class ProductRepository : BaseRepository<Products>, IProductRepository
    {
        #region Private Variables        
        protected new readonly IUnitOfWork _uow;
        private IExecuterStoreProc<Products> sqlProcExecutor;
        private readonly ILogger _logger;
        private bool _disposed;
        #endregion

        #region Constructor
        public ProductRepository(IUnitOfWork uow, IExecuterStoreProc<Products> _sqlProcExecutor, ILogger logger) : base(uow, logger)
        {
            _logger = logger;
            _uow = uow;
            _disposed = false;
            sqlProcExecutor = _sqlProcExecutor;
        }
        #endregion

        #region Public Properties
        public IUnitOfWork UnitOfWork { get { return _uow; } }
        #endregion

        #region Public Methods
        ///// <summary>
        ///// Get filterd data as per linq expression
        ///// </summary>
        ///// <param name="predicate">linq expression</param>
        ///// <returns></returns>
        //public async Task<IEnumerable<Products>> SelectAsync(Expression<Func<Products, bool>> predicate)
        //{
        //        return await _uow.DbContext.Set<Products>().Where(predicate).ToListAsync();
        //}

        #region Public Methods
        /// <summary>
        /// Get filterd data as per linq expression
        /// </summary>
        /// <returns></returns>
        public async Task<List<Products>> GetAllProducts()
        {
            return await _uow.DbContext.Set<Products>().ToListAsync();
        }

        /// <summary>
        /// Get data using stored procedure
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <param name="sqlParam"></param>
        /// <returns></returns>
        //public async Task<List<Products>> ExecuteSqlProcedure<T>(string procName, params T[] sqlParam) where T : class
        //{
        //        return await _uow.DbContext.Set<Products>().FromSql(procName, sqlParam).ToListAsync();
        //}

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

        public Task<List<Products>> ExecuteProcedure<T>(string procName, IEnumerable<Parameters> sqlParam) where T : class
        {
            return sqlProcExecutor.ExecuteProcedureAsync(procName, sqlParam);
        }

        public Task<List<Products>> ExecuteAsync(string procName)
        {
            throw new NotImplementedException();
        }

        public Task<List<Products>> ExecuteEntityAsync<T>(string procName, params T[] sqlParam) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<List<Products>> ExecuteProcedure(string procName, IEnumerable<Parameters> param)
        {
            throw new NotImplementedException();
        }

        public Task<List<Products>> ExecuteEntityAsync(string procName, IEnumerable<Parameters> param = null)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

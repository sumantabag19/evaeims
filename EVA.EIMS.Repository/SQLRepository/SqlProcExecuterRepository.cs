using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Repository
{
    /// <summary>
    /// Use SqlProcExecuterRepository class to execute stored procedures which return complex types
    /// </summary>
    /// <typeparam name="QueryEntity">This will be a complex type provided by business</typeparam>
    public class SqlProcExecuterRepository<QueryEntity> : IExecuterStoreProc<QueryEntity> where QueryEntity : class
    {
        #region Private Variables
        protected readonly IUnitOfWork _uow;
        private bool _disposed;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public SqlProcExecuterRepository(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _disposed = false;
            _logger = logger;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Execute stored procedure on complex data type to get record
        /// </summary>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<List<QueryEntity>> ExecuteProcedureAsync(string procName, IEnumerable<Parameters> param)
        {
            if (param != null)
            {
                List<SqlParameter> sqlParam = GetParameter(ref procName, param);
                return await _uow.DbContext.Set<QueryEntity>().AsNoTracking().FromSql(procName.ToString(), sqlParam.ToArray()).ToListAsync();
            }
            return await _uow.DbContext.Set<QueryEntity>().AsNoTracking().FromSql(procName).ToListAsync();

        }

        /// <summary>
        /// Execute stored procedure on complex data type to get record
        /// </summary>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<QueryEntity> ExecuteProcedure(string procName, IEnumerable<Parameters> param = null)
        {
            if (param != null)
            {
                List<SqlParameter> sqlParam = GetParameter(ref procName, param);
                return _uow.DbContext.Set<QueryEntity>().AsNoTracking().FromSql(procName.ToString(), sqlParam.ToArray()).ToList();
            }
            return _uow.DbContext.Set<QueryEntity>().AsNoTracking().FromSql(procName).ToList();
        }

        /// <summary>
        /// Execute stored procedure on complex data type to get record without adding complex object in DBContext class
        /// </summary>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<List<QueryEntity>> ExecuteProcAsync(string procName, IEnumerable<Parameters> param = null)
        {
            string procedureName = procName;

            if (param != null)
            {
                List<SqlParameter> sqlParam = GetParameter(ref procName, param);
                //return await Task.Run(() =>
                //{
                using (var command = _uow.DbContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = procedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(sqlParam.ToArray());
                    try
                    {
                        //_uow.DbContext.Database.OpenConnection();
                        if (command.Connection.State != ConnectionState.Open)
                        {
                            await command.Connection.OpenAsync();
                        }
                        using (var result = await command.ExecuteReaderAsync())
                        {
                            List<QueryEntity> list = new List<QueryEntity>();
                            QueryEntity obj = default(QueryEntity);
                            while (await result.ReadAsync())
                            {
                                obj = Activator.CreateInstance<QueryEntity>();
                                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                                {
                                    if (!object.Equals(result[prop.Name], DBNull.Value))
                                    {
                                        prop.SetValue(obj, result[prop.Name], null);
                                    }
                                }
                                list.Add(obj);
                            }
                            return list;
                        }
                    }
                    finally
                    {
                        //_uow.DbContext.Database.CloseConnection();
                        command.Connection.Close();
                    }
                }
                //}
                //);
            }
            return await _uow.DbContext.Set<QueryEntity>().AsNoTracking().FromSql(procName).ToListAsync();
        }


        /// <summary>
        /// Execute procedure using reader on databse to get record
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<QueryEntity>> ExecuteAsync(string query)
        {
            // return await Task.Run(() =>
            //    {
            using (var command = _uow.DbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                _uow.DbContext.Database.OpenConnection();
                try
                {
                    //_uow.DbContext.Database.OpenConnection();
                    if (command.Connection.State != ConnectionState.Open)
                    {
                        await command.Connection.OpenAsync();
                    }
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        List<QueryEntity> list = new List<QueryEntity>();
                        QueryEntity obj = default(QueryEntity);
                        while (await result.ReadAsync())
                        {
                            obj = Activator.CreateInstance<QueryEntity>();
                            foreach (PropertyInfo prop in obj.GetType().GetProperties())
                            {
                                if (!object.Equals(result[prop.Name], DBNull.Value))
                                {
                                    prop.SetValue(obj, result[prop.Name], null);
                                }
                            }
                            list.Add(obj);
                        }
                        return list;
                    }
                }
                finally
                {
                    command.Connection.Close();
                }

            }

            //}
            //);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// To get Sql Specific parameters
        /// </summary>
        /// <param name="procName">procName</param>
        /// <param name="param">param</param>
        /// <returns>Returns List of type SqlParameter </returns>
        private List<SqlParameter> GetParameter(ref string procName, IEnumerable<Parameters> param)
        {
            StringBuilder procedureName = new StringBuilder(procName);
            List<SqlParameter> sqlParam = new List<SqlParameter>();
            if (param != null)
            {
                foreach (Parameters p in param)
                {
                    sqlParam.Add(new SqlParameter() { ParameterName = p.ParamKey, Value = p.Value });
                    procedureName.Append(" @" + p.ParamKey + ",");
                }
            }

            procName = procedureName.ToString();
            procName = procName.Remove(procName.Length - 1);
            return sqlParam;
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

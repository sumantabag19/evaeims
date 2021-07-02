using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
    public class MySqlProcExecuterRepository<QueryEntity> : IExecuterStoreProc<QueryEntity> where QueryEntity : class
    {
        #region Private Variables
        protected readonly IUnitOfWork _uow;
        private readonly ILogger _logger;
        private bool _disposed;
        #endregion

        #region Constructor
        public MySqlProcExecuterRepository(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _disposed = false;
        }
        #endregion

        #region Public Methods



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
                List<MySqlParameter> mysqlParam = GetParameters(ref procName, param);

                return _uow.DbContext.Set<QueryEntity>().FromSql(procName, mysqlParam.ToArray()).ToList();
            }
            return _uow.DbContext.Set<QueryEntity>().FromSql(procName).ToList();
        }


        /// <summary>
        /// Execute stored procedure async on complex data type to get record
        /// </summary>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<List<QueryEntity>> ExecuteProcedureAsync(string procName, IEnumerable<Parameters> param = null)
        {
            if (param != null)
            {
                List<MySqlParameter> mysqlParam = GetParameters(ref procName, param);
                return await _uow.DbContext.Set<QueryEntity>().FromSql(procName, mysqlParam).ToListAsync();
            }
            return await _uow.DbContext.Set<QueryEntity>().FromSql(procName).ToListAsync();
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
                List<MySqlParameter> mysqlParam = GetParameters(ref procName, param);
                return await Task.Run(() =>
                {
                    using (var command = _uow.DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = procedureName;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(mysqlParam.ToArray());
                        _uow.DbContext.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {
                            List<QueryEntity> list = new List<QueryEntity>();
                            QueryEntity obj = default(QueryEntity);
                            while (result.Read())
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
                }
            );
            }
            return await _uow.DbContext.Set<QueryEntity>().FromSql(procName).ToListAsync();
        }

        /// <summary>
        /// Execute procedure using reader on databse to get record
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<QueryEntity>> ExecuteAsync(string query)
        {
            return await Task.Run(() =>
            {
                using (var command = _uow.DbContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    _uow.DbContext.Database.OpenConnection();

                    using (var result = command.ExecuteReader())
                    {
                        List<QueryEntity> list = new List<QueryEntity>();
                        QueryEntity obj = default(QueryEntity);
                        while (result.Read())
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
            }
             );
        }
        #endregion

        #region Private Methods
        private static List<MySqlParameter> GetParameters(ref string procName, IEnumerable<Parameters> param)
        {
            StringBuilder procedureName = new StringBuilder(procName);
            List<MySqlParameter> mysqlParam = new List<MySqlParameter>();
            procedureName.Append("(");
            foreach (Parameters p in param)
            {
                mysqlParam.Add(new MySqlParameter() { ParameterName = p.ParamKey, Value = p.Value });
                procedureName.Append(" @" + p.ParamKey + ",");
            }


            procName = "CALL " + procedureName;
            procName = procName.Remove(procName.Length - 1);
            procName = procName + ")";


            return mysqlParam;
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

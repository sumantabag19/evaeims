using EVA.EIMS.Common;
using EVA.EIMS.Helper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace EVA.EIMS.Contract.Repository
{
    /// <summary>
    /// Use this interface to execute stored procedures for complex data type
    /// </summary>
    /// <typeparam name="QueryEntity"></typeparam>
    public interface IExecuterSqlProc<ComplexEntity> : IDisposable where ComplexEntity : class
    {
        /// <summary>
        /// Execute stored procedures for complex data type
        /// </summary>
        /// <param name="query">Procedure Name</param>
        /// <param name="sqlParam"></param>
        /// <returns></returns>
        Task<List<ComplexEntity>> ExecuteProcedureAsync<T>(string procName, List<Parameters> sqlParam) where T : class;

        /// <summary>
        /// Execute procedure using reader on databse to get record
        /// </summary>
        /// <param name="procName">Procedure Name</param>
        /// <returns></returns>
        Task<List<ComplexEntity>> ExecuteAsync(string procName);
    }
}

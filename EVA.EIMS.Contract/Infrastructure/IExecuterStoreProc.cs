using EVA.EIMS.Common;
using EVA.EIMS.Helper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace EVA.EIMS.Contract.Infrastructure
{
  /// <summary>
  /// Use this interface to execute stored procedures for complex data type
  /// </summary>
  /// <typeparam name="QueryEntity"></typeparam>
  public interface IExecuterStoreProc<ComplexEntity> : IDisposable where ComplexEntity : class
  {
    /// <summary>
    /// Execute stored procedures for complex data type
    /// </summary>
    /// <param name="procName">Procedure Name</param>
    /// <param name="Param"></param>
    /// <returns></returns>
    List<ComplexEntity> ExecuteProcedure(string procName, IEnumerable<Parameters> param = null);

    /// <summary>
    /// Execute stored procedures Async for complex data type
    /// </summary>
    /// <param name="procName">Procedure Name</param>
    /// <param name="Param"></param>
    /// <returns></returns>
    Task<List<ComplexEntity>> ExecuteProcedureAsync(string procName, IEnumerable<Parameters> param = null);


    /// <summary>
    /// Execute stored procedure on complex data type to get record without adding complex object in DBContext class
    /// </summary>
    /// <param name="query"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    Task<List<ComplexEntity>> ExecuteProcAsync(string procName, IEnumerable<Parameters> param = null);

    /// <summary>
    /// Execute procedure using reader on databse to get record
    /// </summary>
    /// <param name="procName">Procedure Name</param>
    /// <returns></returns>
    Task<List<ComplexEntity>> ExecuteAsync(string procName);
  }
}

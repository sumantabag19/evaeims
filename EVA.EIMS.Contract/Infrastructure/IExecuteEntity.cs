using EVA.EIMS.Helper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Infrastructure
{
    /// <summary>
    /// Implement this interface while executing procedure directly on database entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IExecuteEntity<TEntity> 
    {
        /// <summary>
        /// Execute procedure directly on database entity
        /// </summary>
        /// <param name="query">procedure name</param>
        /// <param name="param"> Should be parameters</param>
        /// <returns></returns>
        Task<List<TEntity>> ExecuteEntityAsync(string procName, IEnumerable<Parameters> param = null);
    }
}

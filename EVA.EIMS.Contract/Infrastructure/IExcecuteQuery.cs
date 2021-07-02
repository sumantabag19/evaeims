using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Infrastructure
{
    /// <summary>
    /// Implement this interface while executing Inser or Update procedure/query on database which return number of records affected
    /// </summary>
    public interface IExcecuteQuery
    {
        /// <summary>
        /// Executing Insert or Update procedure/query on database which return number of records affected
        /// </summary>
        /// <param name="query">procedure name</param>
        /// <returns></returns>
        Task<int> ExcecuteQueryAsync(string query);
    }
}

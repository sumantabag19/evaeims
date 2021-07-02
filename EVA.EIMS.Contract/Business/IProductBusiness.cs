using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ComplexEntities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IProductBusiness : IDisposable
    {
        /// <summary>
        /// Get Product details using stored procedure
        /// </summary>
        /// <returns></returns>
        Task<List<Products>> Get();

        /// <summary>
        /// Get Product details using linq expression
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Products>> GetByExpression();

        /// <summary>
        /// Get Product details using parameterized stored procedure
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<Products>> GetById(int id);

        /// <summary>
        /// Get complex type(procedure result which does not mach with any entity) data using parameterized stored procedure 
        /// </summary>
        /// <returns></returns>
        Task<List<proc_GetAllProductsSales>> GetByExecuteProcedureAsync(HttpContext ctx);

        /// <summary>
        /// Get complex type(procedure result which does not mach with any entity) data using parameterized stored procedure 
        /// </summary>
        /// <returns></returns>
        Task<List<proc_GetAllProductsSales>> GetByExecuteSQLAsync(HttpContext context);

        /// <summary>
        /// Execute stored procedure which 
        /// </summary>
        /// <returns></returns>
        Task<int> ExcecuteQueryAsync();

        /// <summary>
        /// Add new record in to entity
        /// </summary>
        /// <param name="product">Database/DBContext entity</param>
        /// <returns></returns>
        Task<int> Add(Products product);

        /// <summary>
        /// Update record in to entity
        /// </summary>
        /// <param name="product">Database/DBContext entity</param>
        /// <returns></returns>
        Task<int> Modify(Products product);

        /// <summary>
        /// Delete record from entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> Delete(int id);
    }
}

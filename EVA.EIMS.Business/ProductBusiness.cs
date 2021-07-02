using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ComplexEntities;
using EVA.EIMS.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
    /// <summary>
    /// Implement all business logic in this class
    /// Notes:
    /// 1.Use try catch if you are writing any business logic. Log each exception and then throw exception for controller
    ///  Controller will provide proper response to API consumer
    /// 2.Use Express Mapper for entity mapping 
    /// </summary>
    public class ProductBusiness : IProductBusiness
    {
        #region Private Variable
        private readonly IProductRepository _productRepository;
        private readonly string _userName = string.Empty;
        private bool _disposed;
        #endregion

        #region Constructor

        public ProductBusiness(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _disposed = false;
        }
        #endregion

        //public IProductRepository GetProductRepository { get { return _productRepository; } }

        #region Public Methods
        /// <summary>
        /// Get Product details using stored procedure
        /// </summary>
        /// <returns></returns>
        public async Task<List<Products>> Get()
        {
            //return await GetProductRepository.ExecuteEntityAsync(ProcedureConstants.procGetAllProducts, new object[] { });

            //return await GetProductRepository.GetAllProducts();


            List<Parameters> queryParameters = new List<Parameters>() {
                new Parameters(){ ParamKey="pid", Value="1"}
                //new Parameters(){ ParameterName="pname", Value="w"}
            };

            return await _productRepository.ExecuteProcedure(ProcedureConstants.procGetAllProductsByProductId, queryParameters);

        }

        /// <summary>
        /// Get Product details using linq expression
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Products>> GetByExpression()
        {
            return await _productRepository.SelectAsync(s => s.productId == 1);
        }

        /// <summary>
        /// Get Product details using parameterized stored procedure
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<Products>> GetById(int id)
        {
            List<Parameters> sqlParams = new List<Parameters>() {
                new Parameters { ParamKey="productId", Value=id}
            };//{ new SqlParameter("@productId", id) };
            return await _productRepository.ExecuteEntityAsync(ProcedureConstants.procGetAllProductsByProductId + " @productId", sqlParams);
        }

        /// <summary>
        /// Get complex type(procedure result which does not mach with any entity) data using parameterized stored procedure 
        /// <param name="context"> Http context object to resolve dependancy</param>
        /// </summary>
        /// <returns></returns>
        public async Task<List<proc_GetAllProductsSales>> GetByExecuteProcedureAsync(HttpContext context)
        {
            //For Complex response type use SqlProcExecuterRepository class
            IExecuterStoreProc<proc_GetAllProductsSales> sqlProcExecuterRepository = context.RequestServices.GetService<IExecuterStoreProc<proc_GetAllProductsSales>>();

            List<Parameters> param = new List<Parameters>() {
                new Parameters(){ ParamKey="productId", Value="1"}
            };

            return await sqlProcExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllProductsSales + " @productId", param);
        }

        /// <summary>
        /// Get complex type(procedure result which does not mach with any entity) data using parameterized stored procedure 
        /// </summary>
        /// <returns></returns>
        public async Task<List<proc_GetAllProductsSales>> GetByExecuteSQLAsync(HttpContext context)
        {
            IExecuterStoreProc<proc_GetAllProductsSales> sqlProcExecuterRepository = context.RequestServices.GetService<IExecuterStoreProc<proc_GetAllProductsSales>>();
            //SqlProcExecuterRepository<proc_GetAllProductsSales> sqlProcExecuterRepository = new SqlProcExecuterRepository<proc_GetAllProductsSales>(GetProductRepository.UnitOfWork);
            return await sqlProcExecuterRepository.ExecuteAsync(ProcedureConstants.procGetAllProductsSales + " 1");
        }

        /// <summary>
        /// Execute stored procedure which 
        /// </summary>
        /// <returns></returns>
        public async Task<int> ExcecuteQueryAsync()
        {
            return await _productRepository.ExcecuteQueryAsync(ProcedureConstants.procInsertProduct);
        }

        /// <summary>
        /// Add new record in to entity
        /// </summary>
        /// <param name="product">Database/DBContext entity</param>
        /// <returns></returns>
        public async Task<int> Add(Products product)
        {
            await _productRepository.AddAsync(product);
            return await _productRepository.UnitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Update record in to entity
        /// </summary>
        /// <param name="product">Database/DBContext entity</param>
        /// <returns></returns>
        public async Task<int> Modify(Products product)
        {
            await _productRepository.UpdateAsync(product);
            return await _productRepository.UnitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Delete record from entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> Delete(int id)
        {
            Products prd = new Products { productId = id };
            await _productRepository.DeleteAsync(prd);
            return await _productRepository.UnitOfWork.SaveChangesAsync();
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
                _productRepository.Dispose();
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

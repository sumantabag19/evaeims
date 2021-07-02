using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EVA.EIMS.Security.API.Controllers
{
    /// <summary>
    /// Each controler should have correct Version and Route information
    /// Do not handel exception explicitly
    /// </summary>
    
    [ApiVersion("1.0")]
    [Route("api/[controller]/[action]")]
    public class ProductController : Controller, IDisposable
    {
        #region Private Variables
        //private IUnitOfWork _uow;
        private IProductBusiness _productBusiness;
        //private readonly ICustomPasswordHash _customPasswordHash;
        #endregion

        #region Constructor
        //Create UOW and business class objects in constructor
        //public ProductsController(IProducsBusiness producsBusiness, ICustomPasswordHash customPasswordHash)
        public ProductController(IProductBusiness producsBusiness)
        {
            _productBusiness = producsBusiness;
           // _customPasswordHash = customPasswordHash;
        }

        #endregion

        #region Public Async API Methods
        // GET api/values
        [HttpGet]
        [ActionName("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            //get details from authentication filter

            var details = HttpContext.Items[KeyConstant.ImsClaim];
            return Ok(await _productBusiness.Get());
        }

        // GET api/values
        [HttpGet]
        [ActionName("GetByExpression")]
        public async Task<IActionResult> GetByExpression()
        {
            return Ok(await _productBusiness.GetByExpression());
        }

        // GET api/values/GetById/5
        [HttpGet("{id}")]
        [ActionName("GetByProductId")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _productBusiness.GetById(id));
        }

        // GET api/values/GetByProcedure
        [HttpGet]
        [ActionName("GetByProcedure")]
        public async Task<IActionResult> GetByExecuteProcedureAsync()
        {
            //Pass HttpContext to resolve internal dependancy
            return Ok(await _productBusiness.GetByExecuteProcedureAsync(HttpContext));
        }

        // GET api/values/GetByProcedure
        [HttpGet]
        [ActionName("GetByExecuteSQLAsync")]
        public async Task<IActionResult> GetByExecuteSQLAsync()
        {
            //Pass HttpContext to resolve internal dependancy
            return Ok(await _productBusiness.GetByExecuteSQLAsync(HttpContext));
        }

        // GET api/values/GetByProcedure
        [HttpGet]
        [ActionName("ExcecuteQueryAsync")]
        public async Task<IActionResult> ExcecuteQueryAsync()
        {
            return Ok(await _productBusiness.ExcecuteQueryAsync());
        }

        // POST api/values
        [HttpPost]
        [ActionName("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody]Products product)
        {
            //_productBusiness = new ProducsBusiness();
            return Ok(await _productBusiness.Add(product) + MessageConstants.AddedSuccessfully);
        }

        // PUT api/values/5
        [HttpPut]
        [ActionName("ModifyProduct")]
        public async Task<IActionResult> Put([FromBody]Products product)
        {
            // _productBusiness = new ProducsBusiness();
            return Ok(await _productBusiness.Modify(product) + MessageConstants.ModifiedSuccessfully);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [ActionName("DeleteProduct")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _productBusiness.Delete(id) + MessageConstants.DeletedSuccessfully);
        }

        #region Commented Example
        // POST api/values
        //[HttpPost]
        //[ActionName("AddAndDeleteProduct")]
        //public async Task<IActionResult> AddAndDeleteProduct([FromBody]Products value)
        //{
        //    ProductRepository productRepository = new ProductRepository(_uow);

        //    Products prd = new Products { productId = 63 };
        //    await productRepository.DeleteAsync(prd);
        //    await productRepository.AddAsync(value);
        //    await _uow.SaveChangesAsync();
        //    return StatusCode((int)HttpStatusCode.OK);
        //}
        #endregion
        #endregion

        #region Dispose

        /// <summary>
        /// Method to dispose by parameter.
        /// </summary>
        /// <param name="disposing"></param>
        /// 
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _productBusiness.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
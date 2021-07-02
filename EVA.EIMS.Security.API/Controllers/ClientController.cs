using EVA.EIMS.Common;
using EVA.EIMS.Common.Constants;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ViewModel;
using EVA.EIMS.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EVA.EIMS.Security.API.Controllers
{
    /// <summary>
    /// Each controler should have correct Version and Route information
    /// Do not handel exception explicitly
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]/[action]")]
    public class ClientController : Controller, IDisposable
    {
        #region Private Variables
        private IClientBusiness _clientBusiness;
        #endregion

        #region Constructor
        public ClientController(IClientBusiness clientBusiness)
        {
            _clientBusiness = clientBusiness;
        }
        #endregion

        #region Public API Methods
        /// <summary>
        /// This method is used to get the multiple  client details
        /// </summary>
        /// <returns>returns multiple client details</returns>
        /// 
        [HttpGet]
        [ActionName("GetClient")]
        public async Task<IActionResult> Get()
        {
            var result = await _clientBusiness.Get();
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to get the multiple  client details
        /// </summary>
        /// <returns>returns multiple client details</returns>
        /// 
        [HttpGet]
        [ActionName("GetClientDetails")]
        public async Task<IActionResult> GetClientDetails([FromQuery] int oauthClientId)
        {
            var result = await _clientBusiness.GetClient(oauthClientId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used get the client details by id
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <returns>returns single client details</returns>
        [HttpGet]
        [ActionName("GetClientById")]
        public async Task<IActionResult> Get([FromQuery] string clientId)
        {
            var result = await _clientBusiness.GetById(clientId);
            if (result != null)
                return Ok(result);
            else
                return BadRequest(ResourceInformation.GetResValue("NotExists"));

        }

        /// <summary>
        /// This method is used get the client details by id
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <returns>returns single client details</returns>
        [HttpGet]
        [ActionName("GetClientByIdForInActiveClient")]
        public async Task<IActionResult> GetForInActiveClient([FromQuery] string clientId)
        {
            var result = await _clientBusiness.GetByIdForInActiveClient(clientId);
            if (result != null)
                return Ok(result);
            else
                return BadRequest(ResourceInformation.GetResValue("NotExists"));

        }

        [HttpGet]
        [ActionName("GetAzureAppIdByClientId")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAzureAppIdByClientId(string clientId)
        {
            var result = await _clientBusiness.GetAzureAppIdByClientId(clientId);
            if (result != null)
                return Ok(result);
            else
                return BadRequest(ResourceInformation.GetResValue("NotExists"));

        }

        /// <summary>
        /// This method is used to save the client details
        /// </summary>
        /// <param name="client">client object</param>
        /// <returns>returns response  message</returns>
        [HttpPost]
        [ActionName("SaveClient")]
        public async Task<IActionResult> Post([FromBody]OauthClient client)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            ModelState.Remove("OauthClientId");
            if (ModelState.IsValid)
            {
                var result = await _clientBusiness.Save(tokenData.UserName, client);

                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// This method is used to update the client details
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <param name="client">client object</param>
        /// <returns>returns response message</returns>
        [HttpPut]
        [ActionName("UpdateClient")]
        public async Task<IActionResult> Put([FromBody]OauthClient client)
        {

            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            if (ModelState.IsValid)
            {
                var result = await _clientBusiness.Update(tokenData.UserName, client.ClientId, client);

                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// This method is used to delete the client details
        /// </summary>
        /// <param name="clientId">client</param>
        /// <returns>returns response  message</returns>
        [HttpDelete]
        [ActionName("DeleteClient")]
        public async Task<IActionResult> Delete([FromQuery] string clientId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _clientBusiness.Delete(tokenData.UserName, clientId);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }


        /// <summary>
        /// To create client dynamically by superadmin and service client 
        /// </summary>
        /// <param name="dynamicClient"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CreateDynamicClient")]
        public async Task<IActionResult> CreateDynamicClient([FromBody]OauthClient dynamicClient)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _clientBusiness.DynamicClientCreation(tokenData, dynamicClient);

            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        ///  update client secret of expired client and set client expiraton time
        /// </summary>
        /// <param name="updateClientModel"></param>
        /// <returns></returns>
        [HttpPut]
        [ActionName("UpdateClientSecret")]
        public async Task<IActionResult> UpdateClientSecret([FromBody]OAuthClientViewModel updateClientModel)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _clientBusiness.UpdateClientSecret(tokenData, updateClientModel);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }
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
                _clientBusiness.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}

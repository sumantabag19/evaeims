using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EVA.EIMS.Security.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]/[action]")]
    public class ClientTypeController : Controller, IDisposable
    {
        #region Private Variables
        private IClientTypeBusiness _clientTypeBusiness;
        #endregion

    #region Constructor
    public ClientTypeController(IClientTypeBusiness clientTypeBusiness)
    {
      _clientTypeBusiness = clientTypeBusiness;
    }
    #endregion

    #region Public API Methods
    /// <summary>
    /// This method is used to get the multiple client type details.
    /// </summary>
    /// <returns>returns multiple client type details</returns>
    [HttpGet]
    [ActionName("GetClientType")]
    public async Task<IActionResult> Get()
    {
      var result = await _clientTypeBusiness.Get();
      if (result.Success)
        return Ok(result.Data);
      else
        return BadRequest(result.Result);
    }

    /// <summary>
    /// This method is used get the client type details by id.
    /// </summary>
    /// <param name="clientTypeId">client type id</param>
    /// <returns>returns single client type details</returns>
    [HttpGet]
    [ActionName("GetClientTypeById")]
    public async Task<IActionResult> Get([FromQuery]int clientTypeId)
    {
      var result = await _clientTypeBusiness.GetById(clientTypeId);
      if (result.Success)
        return Ok(result.Data);
      else
        return BadRequest(result.Result);
    }

    /// <summary>
    /// This method is used get the clienttype details by clienttype name.
    /// </summary>
    /// <param name="appId">applicationId</param>
    /// <returns>returns single application details</returns>
    [HttpGet]
    [ActionName("GetClientTypeByName")]
    public async Task<IActionResult> GetByClientTypeName([FromQuery]string clientTypeName)
    {
      var result = await _clientTypeBusiness.GetByClientTypeName(clientTypeName);
      if (result != 0)
        return Ok(result);
      else
        return BadRequest(result);
    }

    /// <summary>
    /// This method is used to save the client type details
    /// </summary>
    /// <param name="clientType">ClientType object</param>
    /// <returns>returns response message</returns>
    [HttpPost]
    [ActionName("SaveClientType")]
    public async Task<IActionResult> Post([FromBody]ClientType clientType)
    {
      var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
      ModelState.Remove("ClientTypeId");
      if (ModelState.IsValid)
      {
        var result = await _clientTypeBusiness.Save(tokenData.UserName, clientType);

        if (result.Success)
          return Ok(result.Result);
        else
          return BadRequest(result.Result);
      }
      return BadRequest(ModelState);
    }

    /// <summary>
    /// This method is used to update the client type details.
    /// </summary>
    /// <param name="clientTypeId">client type id</param>
    /// <param name="clientType">ClientType object</param>
    /// <returns>returns response message</returns>
    [HttpPut]
    [ActionName("UpdateClientType")]
    public async Task<IActionResult> Put([FromBody]ClientType clientType)
    {
      var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
      if (ModelState.IsValid)
      {
        var result = await _clientTypeBusiness.Update(tokenData.UserName, clientType.ClientTypeId, clientType);

        if (result.Success)
          return Ok(result.Result);
        else
          return BadRequest(result.Result);
      }
      return BadRequest(ModelState);
    }

    /// <summary>
    /// This method is used to delete the client type details.
    /// </summary>
    /// <param name="clientTypeId">client type id</param>
    /// <returns>returns response message</returns>
    [HttpDelete]
    [ActionName("DeleteClientType")]
    public async Task<IActionResult> Delete([FromQuery]int clientTypeId)
    {
      var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

      var result = await _clientTypeBusiness.Delete(tokenData.UserName, clientTypeId);

      if (result.Success)
        return Ok(result.Result);
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
        _clientTypeBusiness.Dispose();
      }
      base.Dispose(disposing);
    }
    #endregion
  }
}

using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Security.API.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class RoleController : Controller
    {
        #region Private Variables
        private IRoleBusiness _roleBusiness;
        #endregion

    #region Constructor
    public RoleController(IRoleBusiness roleBusiness)
    {
      _roleBusiness = roleBusiness;
    }
    #endregion

    #region Public Async API Methods

    /// <summary>
    /// This method is used to get the multiple  Role details
    /// </summary>
    /// <returns>returns multiple Role details</returns>
    /// 
    [HttpGet]
    [ActionName("GetRole")]
    public async Task<IActionResult> Get()
    {
      var result = await _roleBusiness.GetRole();
      if (result.Success)
        return Ok(result.Data);
      else
        return BadRequest(result.Result);
    }

    /// <summary>
    /// This method is used get the role details by id
    /// </summary>
    /// <param name="roleId">roleId</param>
    /// <returns>returns single role details</returns>
    [HttpGet]
    [ActionName("GetRoleById")]
    public async Task<IActionResult> Get([FromQuery] int roleId)
    {
      var result = await _roleBusiness.GetRole(roleId);
      if (result.Success)
        return Ok(result.Data);
      else
        return BadRequest(result.Result);
    }

    /// <summary>
    /// This method is used get the role details by role name
    /// </summary>
    /// <param name="roleName">roleName</param>
    /// <returns>returns single role details</returns>
    [HttpGet]
    [ActionName("GetRoleByRoleName")]
    public async Task<IActionResult> GetByRoleName([FromQuery] string roleName)
    {
      var result = await _roleBusiness.GetRoleByRoleName(roleName);
      if (result.Success)
        return Ok(result.Data);
      else
        return BadRequest(result.Result);
    }

    /// <summary>
    /// This method is used to save the role details
    /// </summary>
    /// <param name="roleObj">roleObj</param>
    /// <returns>returns response  message</returns>
    [HttpPost]
    [ActionName("SaveRole")]
    public async Task<IActionResult> Post([FromBody] Role roleObj)
    {
      var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
      ModelState.Remove("RoleId");
      if (ModelState.IsValid)
      {
        var result = await _roleBusiness.SaveRole(tokenData.UserName, roleObj);

        if (result.Success)
          return Ok(result.Result);
        else
          return BadRequest(result.Result);
      }
      return BadRequest(ModelState);

    }

    /// <summary>
    /// This method is used to update the role details
    /// </summary>
    /// <param name="roleId">roleId</param>
    /// <param name="roleObj">roleObj</param>
    /// <returns>returns response message</returns>
    [HttpPut]
    [ActionName("UpdateRole")]
    public async Task<IActionResult> Put([FromBody] Role roleObj)
    {
      var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
      if (ModelState.IsValid)
      {
        var result = await _roleBusiness.UpdateRole(tokenData.UserName, roleObj.RoleId, roleObj);

        if (result.Success)
          return Ok(result.Result);
        else
          return BadRequest(result.Result);
      }
      return BadRequest(ModelState);
    }

    /// <summary>
    /// This method is used to delete the role details
    /// </summary>
    /// <param name="roleId">role</param>
    /// <returns>returns response  message</returns>
    [HttpDelete]
    [ActionName("DeleteRole")]
    public async Task<IActionResult> Delete([FromQuery] int roleId)
    {
      var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

      var result = await _roleBusiness.DeleteRole(tokenData.UserName, roleId);

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
        _roleBusiness.Dispose();
      }
      base.Dispose(disposing);
    }
    #endregion
  }
}

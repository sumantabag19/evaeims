using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ViewModel;
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
    public class DeviceController : Controller, IDisposable
    {
        #region Private Variables
        private IDeviceBusiness _deviceBusiness;
        #endregion

        #region Constructor
        public DeviceController(IDeviceBusiness deviceBusiness)
        {
            _deviceBusiness = deviceBusiness;
        }
        #endregion

        #region Public API Methods
        /// <summary>
        /// This method is used to get the multiple device details by organization id
        /// </summary>
        /// <returns>Returns device details of particular organization</returns>
        [HttpGet]
        [ActionName("")]
        public async Task<IActionResult> Get([FromQuery]string deviceId = null)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _deviceBusiness.GetDevice(tokenData, deviceId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        //public IActionResult Post(Device device)
        //{
        //    var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
        //    if (tokenData != null && (tokenData.ClientTypeId != (int)ClientTypeEnum.SecurityApiClient && !tokenData.Role.Contains(UserRoles.SuperAdmin.ToString())))
        //    {
        //        //return Request.CreateResponse(HttpStatusCode.Unauthorized, "Unauthorized user.");
        //        return Unauthorized();
        //    }
        //    var result = _deviceBusiness.SaveDevice(tokenData, device);

        //    if (result.Success)
        //        return Ok(result.Result);
        //    else
        //        return BadRequest(result.Result);
        //}

        /// <summary>
        /// This method is used to save the device details
        /// </summary>
        /// <param name="device">device object</param>
        /// <returns>returns response message</returns>
        [HttpPost]
        [ActionName("")]
        public async Task<IActionResult> Post([FromBody] DeviceModel device)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            if (ModelState.IsValid)
            {
                var result = await _deviceBusiness.SaveDevice(tokenData, device);

                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// This method is used to update the device details
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="device"></param>
        /// <returns>returns response message</returns>
        [HttpPut]
        [ActionName("")]
        public async Task<IActionResult> Put([FromBody] DeviceModel device)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            ModelState.Remove("GatewayDeviceId");
            ModelState.Remove("AppId");
            if (ModelState.IsValid)
            {
                var result = await _deviceBusiness.UpdateDevice(tokenData, device.DeviceId, device);

                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// This method is used to delete the device details
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns>returns response message</returns>
        [HttpDelete]
        [ActionName("")]
        public async Task<IActionResult> Delete([FromQuery]string deviceId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _deviceBusiness.DeleteDevice(tokenData, deviceId);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// API to get device details on the basis of serial key.
        /// Device get bloked (IsUsed=true) once requested data
        /// </summary>
        /// <param name="SerialKey"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("DeviceBySerialKey")]
        public async Task<IActionResult> GetDeviceBySerialKey([FromQuery]Guid SerialKey)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _deviceBusiness.GetDeviceBySerialKey(tokenData, SerialKey, tokenData.OrgId);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// API to get device details on the basis of organisation.
        /// </summary>
        /// <param name="orgId">Oraganisation Id</param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("DeviceByOrg")]
        public async Task<IActionResult> GetDeviceByOrg([FromQuery]string OrgId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _deviceBusiness.GetDeviceByOrg(tokenData, OrgId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        ///// <summary>
        ///// API to update device IsUsed status on the basis of serial key and orgId
        ///// </summary>
        ///// <param name="device"></param>
        ///// <returns></returns>

        [HttpPost]
        [ActionName("UpdateDeviceUsedStatus")]
        public async Task<IActionResult> UpdateDeviceUsedStatus([FromBody]DeviceModel device)
        {

            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            if (ModelState.IsValid)
            {
                var result = await _deviceBusiness.UpdateDeviceUsedStatus(tokenData, device.SerialKey, device.IsUsed);
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
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
                _deviceBusiness.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
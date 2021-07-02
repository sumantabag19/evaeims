using EVA.EIMS.Common;
using EVA.EIMS.Common.Constants;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ViewModel;
using EVA.EIMS.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Security.API.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class UserController : Controller
    {
        #region Private Variables
        private IUserBusiness _userBusiness;
        #endregion

        #region Constructor
        public UserController(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }
        #endregion

        #region Public API Methods

        #region User
        /// <summary>
        /// This method is used to get the multiple  user details
        /// </summary>
        /// <returns>returns multiple user details</returns>
        /// 
        [HttpGet]
        [ActionName("GetUser")]
        public async Task<IActionResult> Get()
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _userBusiness.GetUser(tokenData);

            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to get the multiple  user details
        /// </summary>
        /// <returns>returns multiple user details</returns>
        /// 
        [HttpGet]
        [ActionName("GetUserforUI")]
        public async Task<IActionResult> GetUser()
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _userBusiness.GetUserForUI(tokenData);

            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used get the user details by id
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>returns single user details</returns>
        [HttpGet]
        [ActionName("GetUserById")]
        public async Task<IActionResult> Get([FromQuery] Guid userId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _userBusiness.GetUser(tokenData, userId);

            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used get the user details by id
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>returns single user details</returns>
        [HttpGet]
        [ActionName("GetUserByUserName")]
        public async Task<IActionResult> Get([FromQuery] string username)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            if (tokenData != null && (!tokenData.UserClientTypes.Contains(ClientTypes.SecurityApiClient.ToString()) && !tokenData.Role.Contains(UserRoles.SuperAdmin.ToString())))
            {
                return BadRequest(ResourceInformation.GetResValue("UnauthorizedAccessException"));
            }
            var result = await _userBusiness.GetUserByUserName(username);
            if (result != null)
                return Ok(result);
            else
                return BadRequest(ResourceInformation.GetResValue("NotExists"));
        }
        /// <summary>
        /// method to return user details as per given user name to user other than superadmin
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetSpecificUserByName")]
        public async Task<IActionResult> GetSpecificUserByName([FromQuery] string username)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _userBusiness.GetUser(tokenData, username);
            if (result != null)
                return Ok(result);
            else
                return BadRequest(ResourceInformation.GetResValue("NotExists"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userOTP"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [ActionName("GetTwoFactorByUserName")]
        public async Task<IActionResult> GetTwoFactorByUserName([FromQuery]string userName)
        {
            var result = await _userBusiness.GetTwoFactorByUserName(userName);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);

        }

        /// <summary>
        /// This method is used to save the user
        /// </summary>
        /// <param name="user">user object</param>
        /// <returns>returns response  message</returns>       
        [HttpPost]
        [ActionName("SaveUser")]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            ModelState.Remove("UserId");
            ModelState.Remove("OrgId");
            ModelState.Remove("FamilyName");
            if (ModelState.IsValid)
            {
                var result = await _userBusiness.SaveUser(tokenData, user);

                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// This method is used to update the user details
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="user">user object</param>
        /// <returns>returns response message</returns>
        [HttpPut]
        [ActionName("UpdateUser")]
        //Changed string to Guid for userId
        public async Task<IActionResult> Put([FromBody] User user)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            ModelState.Remove("FamilyName");
            ModelState.Remove("OrgId");
            ModelState.Remove("PlainTextPassword");
            if (ModelState.IsValid)
            {
                var result = await _userBusiness.UpdateUser(tokenData, user.UserId, user);

                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// This method is used to delete the user details
        /// </summary>
        /// <param name="userId">user</param>
        /// <returns>returns response  message</returns>
        //Changed string to string for userId
        [HttpDelete]
        [ActionName("DeleteUser")]
        public async Task<IActionResult> Delete([FromQuery] Guid userId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _userBusiness.DeleteUser(tokenData, userId);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to get the user details by user id
        /// </summary>
        /// <param name="orgId">orgId</param>
        /// <returns>returns user details by Organization</returns>
        [HttpGet]
        [ActionName("GetUsersByOrg")]
        public async Task<IActionResult> GetByOrgName([FromQuery] string orgId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            if (tokenData != null && (!tokenData.UserClientTypes.Contains(ClientTypes.SecurityApiClient.ToString()) && !tokenData.Role.Contains(UserRoles.SuperAdmin.ToString())))
            {
                return BadRequest(ResourceInformation.GetResValue("UnauthorizedAccessException"));
            }
            var result = await _userBusiness.GetUsersByOrganization(tokenData, orgId);

            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }
        #endregion

        #region User Application Mapping

        /// <summary>
        /// This method is used to map a user with an application
        /// </summary>
        /// /// <param name="userId">userId</param>
        /// <param name="appId">tokenData object</param>
        /// <returns>returns response message</returns>
        [HttpPost]
        [ActionName("AddOrUpdateUserApplication")]

        public async Task<IActionResult> SaveUserApplication([FromBody] ApplicationUserMappingModel applicationUserMappingModel)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            if (ModelState.IsValid)
            {
                var result = await _userBusiness.AddOrUpdateUserApplication(tokenData, applicationUserMappingModel);

                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// This method is used to update the mapping of a user with an application
        /// </summary>
        /// /// <param name="applicationUserMappingModel">ApplicationUserMappingModel object</param>
        /// <returns>returns response message</returns>
        //[HttpPut]
        //[ActionName("UpdateUserApplication")]

        //public async Task<IActionResult> UpdateUserApplication([FromBody] ApplicationUserMappingModel applicationUserMappingModel)
        //{
        //    var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

        //    var result = await _userBusiness.UpdateUserApplication(tokenData.UserName, applicationUserMappingModel);

        //    if (result.Success)
        //        return Ok(result.Result);
        //    else
        //        return BadRequest(result.Result);
        //}

        /// <summary>
        /// This method is used to delete the mapping of a user with an application
        /// </summary>
        /// <param name="mappingId">Mapping Id</param>
        /// <returns>returns response message</returns>
        [HttpDelete]
        [ActionName("DeleteUserApplication")]
        public async Task<IActionResult> DeleteUserApplication([FromQuery] int mappingId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _userBusiness.DeleteUserApplication(tokenData, mappingId);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        #endregion

        #region Forgot Password
        /// <summary>
        /// Verify account on the basis of user name and email
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userEmail"></param>
        /// <returns>userId</returns>

        [AllowAnonymous]
        [HttpPost]
        [ActionName("VerifyAccount")]
        public async Task<IActionResult> VerifyAccount([FromBody] UserCredentials userCredentials)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("OldPassword");
            ModelState.Remove("NewPassword");
            if (ModelState.IsValid)
            {
                var result = await _userBusiness.VerifyAccount(userCredentials.UserName, userCredentials.EmailId);
                if (result.Success)
                    return Ok(result.Data);
                else
                {
                    var failedResponse = new VerifyUserModel()
                    {
                        UserId = Guid.Parse(KeyConstant.InvalidForgotPasswordInput),
                        TwoFactorEnable = false
                    };
                    return Ok(failedResponse);
                }
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Verify OTP provided by user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="otp"></param>
        /// <returns></returns>

        [AllowAnonymous]
        [HttpPost]
        [ActionName("VerifyOTP")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTP userOTP)
        {
            if (ModelState.IsValid)
            {
                var result = await _userBusiness.VerifyOTP(userOTP.UserId, userOTP.OTPString);
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);

        }

        #endregion

        #region Locked Users
        /// <summary>
        /// Method to get all locked user based on specific role
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetAllLockedUser")]
        public async Task<IActionResult> GetAllLockedUser()
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            if (tokenData != null && (tokenData.Role.Contains(UserRoles.SiteUser.ToString())))
            {
                return BadRequest(ResourceInformation.GetResValue("UnauthorizedAccessException"));
            }
            var result = await _userBusiness.GetAllLockedUser(tokenData);

            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);

        }

        /// <summary>
        /// Method to unlock locked users under specific user role, siteadmin or superadmin
        /// </summary>
        /// <param name="unlockUsers"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UnlockUsers")]
        public async Task<IActionResult> UnlockUsers([FromBody]List<UnlockUsers> unlockUsers = null)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            if (unlockUsers == null)
            {
                return NotFound(ResourceInformation.GetResValue("NoRecordsFound"));
            }
            if (ModelState.IsValid)
            {
                var result = await _userBusiness.UnlockUsers(tokenData, unlockUsers);
                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);

        }

        /// <summary>
        /// Unlock locked users without changing password
        /// </summary>
        /// <param name="unlockUserByUserNames"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UnlockUserWithOutPwd")]
        public async Task<IActionResult> UnlockUserWithOutPwd([FromBody]List<UnlockUserByUserName> unlockUserByUserNames)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            if (unlockUserByUserNames == null)
            {
                return NotFound(ResourceInformation.GetResValue("NoRecordsFound"));
            }

            if (ModelState.IsValid)
            {
                var result = await _userBusiness.UnlockUserByUserName(tokenData, unlockUserByUserNames);
                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }
        #endregion

        /// <summary>
        /// This API is accessed by the AutoEmailerTask Scheduler to send password expiry notification
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SendPasswordExpNotification")]
        public async Task Post()
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            await _userBusiness.SendEmailPwdExpNotify(tokenData);

        }

        #region UserOrganizationMapping API Methods
        /// <summary>
        /// This method is used to get the multiple UserOrganizationMapping details.
        /// </summary>
        /// <returns>returns response message</returns>
        [HttpGet]
        [ActionName("GetAllUserOrgMapping")]
        public async Task<IActionResult> GetAllUserOrgMapping()
        {
            var result = await _userBusiness.GetAllUserOrgMapping();

            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used get the UserOrganizationMapping details by id.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>returns response message</returns>
        [HttpGet]
        [ActionName("GetUserOrgMappingByUserId")]
        public async Task<IActionResult> GetbyRoleId([FromQuery] Guid userId)
        {
            var result = await _userBusiness.GetUserOrgMappingByUserId(userId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }



        /// <summary>
        /// This method is used get the OrganizationApplicationMapping details by username.
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [ActionName("GetOrgAppMappingByUserName")]
        public async Task<IActionResult> GetOrgAppMappingByUserName()
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _userBusiness.GetOrgAppMappingByUserName(tokenData.UserName);
            if (result != null)
                return Ok(result);
            else
                return BadRequest((ResourceInformation.GetResValue("DataLoadFailure")));
        }

        /// <summary>
        /// This method is used to save one UserOrganizationMapping Entry.
        /// </summary>
        /// <param name="UserOrganizationMappingModel">UserOrganizationMappingModel object</param>
        /// <returns>returns response message</returns>
        [HttpPost]
        [ActionName("SaveUserOrgMapping")]
        public async Task<IActionResult> SaveUserOrgMapping([FromBody] UserOrganizationMappingModel userOrganizationMappingModel)
        {
            ModelState.Remove("UserIdArray");
            ModelState.Remove("OrgNameArray");
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            if (ModelState.IsValid)
            {
                var result = await _userBusiness.SaveUserOrgMapping(tokenData.UserName, userOrganizationMappingModel);

                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// This method is used to update single UserOrganizationMapping entry.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="userOrganizationMapping">userOrganizationMapping object</param>
        /// <returns>returns response message</returns>
        [HttpPut]
        [ActionName("UpdateUserOrgMapping")]
        public async Task<IActionResult> UpdateUserOrgMapping([FromBody] UserOrganizationMappingModel userOrganizationMappingModel)
        {
            ModelState.Remove("UserIdArray");
            ModelState.Remove("OrgNameArray");
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            if (ModelState.IsValid)
            {
                var result = await _userBusiness.UpdateUserOrgMapping(tokenData.UserName, userOrganizationMappingModel);
                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// This method is used to soft delete single UserOrganizationMapping details.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>returns response  message</returns>
        [HttpDelete]
        [ActionName("DeleteUserOrgMappingByUserId")]
        public async Task<IActionResult> DeleteUserOrgMappingByUserId([FromQuery]Guid userId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _userBusiness.DeleteUserOrgMappingByUserId(userId);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to save multiple User Organization mappings Entry.
        /// </summary>
        /// <param name="UserOrganizationMappingModel">UserOrganizationMappingModel object</param>
        /// <returns>returns response message</returns>
        [HttpPost]
        [ActionName("SaveMultiUserOrgMapping")]
        public async Task<IActionResult> SaveMultiUserOrgMapping([FromBody] UserOrganizationMappingModel userOrganizationMappingModel)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            ModelState.Remove("UserId");
            ModelState.Remove("OrgIdArray");
            if (ModelState.IsValid)
            {
                var result = await _userBusiness.SaveMultiUserOrgMapping(tokenData.UserName, userOrganizationMappingModel);

                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }

        #endregion


        #region User Login OTP
        /// <summary>
        /// Verify OTP provided by user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="otp"></param>
        /// <returns></returns>

        [AllowAnonymous]
        [HttpPost]
        [ActionName("VerifyOTPLogin")]
        public async Task<IActionResult> VerifyOTPLoginAsync([FromBody] VerifyOTP userOTP)
        {
            if (ModelState.IsValid)
            {
                var result = await _userBusiness.VerifyOTPLoginAsync(userOTP.UserId, userOTP.OTPString);
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);

        }

        #endregion


        [AllowAnonymous]
        [HttpPost]
        [ActionName("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyUserModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userBusiness.VerifyEmailAsync(user.UserId);
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
                _userBusiness.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}


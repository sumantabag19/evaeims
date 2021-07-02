using EVA.EIMS.Common;
using EVA.EIMS.Common.Constants;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ComplexEntities;
using EVA.EIMS.Entity.ViewModel;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
    public class UserBusiness : IUserBusiness
    {
        #region Private Variable
        private readonly IUserRepository _userRepository;
        private readonly ICustomPasswordHash _customPasswordHash;
        private readonly int _passwordExpirationDay;
        private bool _disposed;
        private readonly IOptions<ApplicationSettings> _applicationSettings;
        private readonly string _mstOrg;
        private readonly string _mstClientType;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        //private readonly IIMSLogOutRepository _iMSLogOutRepository;
        private readonly IUserRoleMappingRepository _userRoleMappingRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IApplicationRepository _applicationRepository;
        #endregion

        #region Constructor
        public UserBusiness(IServiceProvider serviceProvider, IUserRepository userRepository, IOptions<ApplicationSettings> applicationSettings, ICustomPasswordHash customPasswordHash, ILogger logger,
                            IIMSLogOutRepository iMSLogOutRepository, IUserRoleMappingRepository userRoleMappingRepository, IRoleRepository roleRepository, IOrganizationRepository organizationRepository,
                            IEmailTemplateRepository emailTemplateRepository, IApplicationRepository applicationRepository)
        {
            _serviceProvider = serviceProvider;
            _userRepository = userRepository;
            _customPasswordHash = customPasswordHash;
            _disposed = false;
            _applicationSettings = applicationSettings;
            _mstOrg = _applicationSettings.Value.MstOrg;
            _mstClientType = _applicationSettings.Value.MstClientTypeId;
            _customPasswordHash = customPasswordHash;
            _passwordExpirationDay = _applicationSettings.Value.PasswordExpirationDays;
            _logger = logger;
            //_iMSLogOutRepository = iMSLogOutRepository;
            _userRoleMappingRepository = userRoleMappingRepository;
            _roleRepository = roleRepository;
            _organizationRepository = organizationRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _applicationRepository = applicationRepository;
        }
        #endregion

        #region Public Methods

        #region User
        /// <summary>
        /// Get All User details using stored procedure
        /// <param name="tokenData">tokenData</param>
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnResult> GetUser(TokenData tokenData)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IExecuterStoreProc<UserModel> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<UserModel>>();

                List<Parameters> param = new List<Parameters>() {
                    new Parameters("p_Roles", tokenData.Role.FirstOrDefault()),
                    new Parameters("p_OrgId", tokenData.OrgId),
                    new Parameters("p_ClientType", String.Join(",", tokenData.UserClientTypes)),
                    new Parameters("p_UserName", tokenData.UserName) };

                var userDetails = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllUsers.ToString(), param);
                if (userDetails != null && userDetails.Count > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = userDetails;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "GetUser", ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Get specific user details by user id
        /// </summary>
        /// <param name="tokenData">tokenData</param>
        /// <param name="id">id</param>
        /// <returns>returns single user details</returns>
        public async Task<ReturnResult> GetUser(TokenData tokenData, Guid id)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IExecuterStoreProc<UserModel> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<UserModel>>();

                List<Parameters> param = new List<Parameters>() {
                    new Parameters("p_Roles", tokenData.Role.FirstOrDefault()),
                    new Parameters("p_OrgId", tokenData.OrgId),
                    new Parameters("p_ClientType", String.Join(",", tokenData.UserClientTypes)),
                    new Parameters("p_UserName", tokenData.UserName),
                new Parameters("p_UseId", id.ToString()) };

                var userData = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetUserById.ToString(), param);
                if (userData != null && userData.Count > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = userData.FirstOrDefault();
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "GetUser", ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        /// <summary>
        /// get specific user details as per login user role
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<ReturnResult> GetUser(TokenData tokenData, string username)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IExecuterStoreProc<UserModel> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<UserModel>>();

                List<Parameters> param = new List<Parameters>() {
                    new Parameters("p_Roles", tokenData.Role.FirstOrDefault()),
                    new Parameters("p_TokenUserName", tokenData.UserName),
                    new Parameters("p_UserName", username) };

                var userData = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetUserByName.ToString(), param);
                if (userData != null && userData.Count > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = userData.FirstOrDefault();
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "GetUser", ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Get All User details using stored procedure
        /// <param name="tokenData">tokenData</param>
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnResult> GetUserForUI(TokenData tokenData)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IExecuterStoreProc<UserModelForUI> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<UserModelForUI>>();

                List<Parameters> param = new List<Parameters>() {
                    new Parameters("p_Roles", tokenData.Role.FirstOrDefault()),
                    new Parameters("p_OrgId", tokenData.OrgId),
                    new Parameters("p_ClientType", String.Join(",", tokenData.UserClientTypes)),
                    new Parameters("p_UserName", tokenData.UserName) };

                var userDetails = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllUsersForUI.ToString(), param);
                if (userDetails != null && userDetails.Count > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = userDetails;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "GetUserForUI", ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Method to get TwoFactorEnabled of any user 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<ReturnResult> GetTwoFactorByUserName(string userName)
        {
            ReturnResult returnResult = new ReturnResult();
            UserTwoFactorEnable userTwoFactorEnable = new UserTwoFactorEnable();
            try
            {
                var user = await _userRepository.SelectFirstOrDefaultAsync(x => x.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && x.IsActive.Value);
                if (user == null)
                {
                    userTwoFactorEnable.userId = Guid.NewGuid();
                    userTwoFactorEnable.twoFactorEnabled = false;
                    returnResult.Success = true;
                    returnResult.Data = userTwoFactorEnable;
                    return returnResult;
                }
                userTwoFactorEnable.userId = user.UserId;
                userTwoFactorEnable.twoFactorEnabled = user.TwoFactorEnabled;
                returnResult.Success = true;
                returnResult.Data = userTwoFactorEnable;
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "GetTwoFactorByUserName", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to user details by UserName
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>returns a single user </returns>
        public async Task<UserDetails> GetUserByUserName(string name)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IExecuterStoreProc<UserDetails> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<UserDetails>>();

                List<Parameters> param = new List<Parameters>() {
                    new Parameters("p_name", name) };

                var lstUser = (await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllUserDetailsByName.ToString(), param)).FirstOrDefault();

                if (lstUser != null)
                {
                    return lstUser;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return null;
                }


            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "GetUserByUserName", ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// This method is used to user details by UserName
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>returns a single user </returns>
        public async Task<UserDetails> GetUserByMobile(string mobile)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IExecuterStoreProc<UserDetails> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<UserDetails>>();

                List<Parameters> param = new List<Parameters>() {
                    new Parameters("p_Mobile", mobile) };

                var lstUser = (await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllUserDetailsByMobile.ToString(), param)).FirstOrDefault();

                if (lstUser != null)
                {
                    return lstUser;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return null;
                }


            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "GetUserByMobile", ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Get user details by Organization
        /// </summary>
        /// <param name="tokenData">tokenData</param>
        /// <param name="orgId">orgId</param>
        /// <returns>returns List of user details</returns>
        public async Task<ReturnResult> GetUsersByOrganization(TokenData tokenData, string orgId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IExecuterStoreProc<UserModel> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<UserModel>>();

                List<Parameters> param = new List<Parameters>() {
                    new Parameters("p_OrgId", orgId )};

                var userData = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllUserByOrgId.ToString(), param);

                if (userData != null)
                {
                    returnResult.Success = true;
                    returnResult.Data = userData.ToList();
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "GetUsersByOrganization", ex.Message, ex.StackTrace);
                returnResult.Result = ExceptionLogger.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// To Add New User
        /// </summary>
        /// <param name="tokenData">tokenData</param>
        /// <param name="user">user</param>
        /// <returns>returns a responce message</returns>
        public async Task<ReturnResult> SaveUser(TokenData tokenData, User user)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (user == null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideUserDetails");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(user.UserId.ToString()) || user.UserId == null)
                    user.UserId = Guid.NewGuid();

                user.Subject = Guid.NewGuid();

                if (string.IsNullOrEmpty(user.UserName))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideUserName");
                    return returnResult;
                }
                if (user.OrgId == null && string.IsNullOrEmpty(user.OrgName))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideOrganizationName");
                    return returnResult;
                }
                if (string.IsNullOrEmpty(user.EmailId))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideEmailId");
                    return returnResult;
                }
                if (string.IsNullOrEmpty(user.ProviderName))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideAuthProviderName");
                    return returnResult;
                }
                if (user.ProviderName != ApplicationLevelConstants.IMSAuthProvider)
                {
                    foreach (var role in user.Roles)
                    {
                        if (role.ToUpper() == UserRoles.SuperAdmin.ToString().ToUpper())
                        {
                            returnResult.Success = false;
                            returnResult.Result = ResourceInformation.GetResValue("NotValidForSuperAdminRole");
                            return returnResult;
                        }
                    }
                }
                // Added Authentication ProviderId to user entity from ProviderName
                IAuthProviderRepository authProviderRepository = _serviceProvider.GetRequiredService<IAuthProviderRepository>();
                var authProvider = (await authProviderRepository.SelectFirstOrDefaultAsync(ap => ap.ProviderName.Equals(user.ProviderName, StringComparison.OrdinalIgnoreCase) && ap.IsActive.Value));
                if (authProvider == null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValideAuthProvider");
                    return returnResult;
                }
                user.ProviderId = authProvider.ProviderID;
                if (user.ProviderName == ApplicationLevelConstants.IMSAuthProvider)
                {
                    if (string.IsNullOrEmpty(user.PlainTextPassword))
                    {
                        returnResult.Result = ResourceInformation.GetResValue("ProvidePlainTextPassword");
                        return returnResult;
                    }
                    #region Saving User Data

                    if (!_customPasswordHash.ValidatePassword(user.PlainTextPassword, out string errorMsg))
                    {
                        returnResult.Result = errorMsg;
                        return returnResult;
                    }
                    user.PasswordHash = _customPasswordHash.ScryptHash(user.PlainTextPassword);

                    if (user.TwoFactorEnabled == null)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("ProvideTwoFactorEnableStatus");
                        return returnResult;
                    }
                }
                else
                {
                    user.PasswordHash = string.Empty;
                }
                IOrganizationRepository organizationRepository = _serviceProvider.GetRequiredService<IOrganizationRepository>();
                IRoleRepository roleRepository = _serviceProvider.GetRequiredService<IRoleRepository>();
                //IOrganizationApplicationmappingRepository organizationApplicationMappingRepo = _serviceProvider.GetRequiredService<IOrganizationApplicationmappingRepository>();
                var tempOrg = await organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgId == user.OrgId || o.OrgName == user.OrgName);
                user.OrgId = tempOrg.OrgId;
                if (user.Roles.FirstOrDefault() == Enum.GetName(typeof(UserRoles), UserRoles.SuperAdmin))
                {
                    if (tempOrg.OrgName != _mstOrg)
                        returnResult.Result = ResourceInformation.GetResValue("MasterOrganizationUser");
                    return returnResult;
                }
                //var tempApp = await organizationApplicationMappingRepo.SelectAsync(x=>x.OrgId==user.OrgId);
                if (tokenData.OrgId.Equals(_mstOrg))
                {
                    var exstingUserId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(tokenData.UserName))).UserId;
                    foreach (string role in tokenData.Role)
                    {
                        //Siteadmin belonging to master organization should create only support users
                        if (tokenData.Role.FirstOrDefault().ToUpper() == UserRoles.SiteAdmin.ToString().ToUpper())
                        {
                            user.Roles = new[] { UserRoles.Support.ToString() };
                        }
                        // Checks Wether user has limited organization access
                        else if ((await roleRepository.SelectAsync(r => r.RoleName.Equals(role) && r.MultipleOrgAccess == true)).Any())
                        {
                            IUserOrganizationMappingRepository userOrganizationMappingRepository = _serviceProvider.GetRequiredService<IUserOrganizationMappingRepository>();
                            if (!(await userOrganizationMappingRepository.SelectAsync(uom => uom.UserId.Equals(exstingUserId) && uom.OrgId.Equals(user.OrgId))).Any())
                            {
                                returnResult.Result = ResourceInformation.GetResValue("NoPermission");
                                return returnResult;
                            }
                        }
                    }
                }
                else
                {
                    if (!tokenData.OrgId.Equals(tempOrg.OrgName) && tokenData.UserName.ToUpper() != KeyConstant.Client.ToUpper())
                    {
                        returnResult.Result = ResourceInformation.GetResValue("MismatchOrganization");
                        return returnResult;
                    }
                }
                var userExist = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(user.UserName.Trim()));
                var mobileExist = await _userRepository.SelectFirstOrDefaultAsync(u => u.PhoneNumber.Equals(user.PhoneNumber.Trim()));
                if ((userExist != null && !userExist.IsActive.Value) || (mobileExist != null && !mobileExist.IsActive.Value))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ExistsAndInActive");
                    //returnResult.Data = userExist.UserId;
                    return returnResult;
                }

                if (userExist != null && userExist.IsActive.Value)
                {
                    returnResult.Result =
                        $"{ResourceInformation.GetResValue("User")} {user.UserName} {ResourceInformation.GetResValue("AlreadyExist")}";
                    //returnResult.Data = userExist.UserId;
                    return returnResult;
                }
                if (mobileExist != null && mobileExist.IsActive.Value)
                {
                    returnResult.Result =
                        $"{ResourceInformation.GetResValue("Mobile")} {user.PhoneNumber} {ResourceInformation.GetResValue("AlreadyExist")}";
                    //returnResult.Data = userExist.UserId;
                    return returnResult;
                }
                if (!(await organizationRepository.SelectAsync(o => o.OrgId.Equals(user.OrgId) && o.IsActive.Value)).Any())
                {
                    returnResult.Result =
                        $"{ResourceInformation.GetResValue("Organization")} {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                if (user.IsActive == null)
                {
                    user.IsActive = true;
                }

                var creator = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(tokenData.UserName));
                Guid userId = Guid.Empty;
                if (creator == null) userId = user.UserId;
                else userId = creator.UserId;

                user.CreatedBy = userId;
                user.ModifiedBy = user.CreatedBy;
                user.CreatedOn = DateTime.Now;
                user.ModifiedOn = DateTime.Now;
                user.IsFirstTimeLogin = true;
                if (user.ProviderName == ApplicationLevelConstants.IMSAuthProvider)
                {
                    user.TwoFactorEnabled = user.TwoFactorEnabled.Value;
                    user.LastPasswordChangeOn = DateTime.Now;
                    user.PasswordExpiration = DateTime.Now.AddDays(_passwordExpirationDay);
                    user.RequiredSecurityQuestion = true;
                }
                else
                {
                    user.TwoFactorEnabled = false;
                    user.LastPasswordChangeOn = DateTime.MaxValue;
                    user.PasswordExpiration = DateTime.MaxValue;
                    user.RequiredSecurityQuestion = false;
                }
                await _userRepository.AddAsync(user);

                #endregion

                #region Saving UserClientType Mapping Data

                if (!tokenData.UserClientTypes.Contains(Enum.GetName(typeof(ClientTypes), Int32.Parse(_mstClientType))) &&
                    tokenData.UserClientTypes.Contains(((int)ClientTypes.UiWebClient).ToString()))
                {
                    user.ClientType = tokenData.UserClientTypes;
                }

                if (user.ClientType.Contains(Enum.GetName(typeof(ClientTypes), Int32.Parse(_mstClientType))) && !tokenData.UserClientTypes.Contains(Enum.GetName(typeof(ClientTypes), Int32.Parse(_mstClientType))))
                {
                    returnResult.Result =
                        $"{ResourceInformation.GetResValue("NoPermission")}  {ResourceInformation.GetResValue("ClientType")}";
                    return returnResult;
                }
                else if (user.ClientType == null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideClientType");
                    return returnResult;
                }

                IClientTypeRepository clientTypeRepository = _serviceProvider.GetRequiredService<IClientTypeRepository>();

                var existingClientTypeList = (await clientTypeRepository.SelectAsync(c => user.ClientType.Contains(c.ClientTypeName) && c.IsActive.Value)).ToList();

                if (existingClientTypeList == null || existingClientTypeList.Count() != user.ClientType.ToList().Count())
                {
                    returnResult.Result =
                        $"{ResourceInformation.GetResValue("ClientType")}  {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                IUserClientTypeMappingRepository userClientTypeMappingRepository = _serviceProvider.GetRequiredService<IUserClientTypeMappingRepository>();

                List<UserClientTypeMapping> updateUserClientTypes = new List<UserClientTypeMapping>();

                foreach (ClientType tempClientType in existingClientTypeList)
                {
                    updateUserClientTypes.Add(new UserClientTypeMapping
                    {
                        UserId = user.UserId,
                        ClientTypeId = tempClientType.ClientTypeId,
                        IsActive = true
                    });
                }
                await userClientTypeMappingRepository.AddRange(updateUserClientTypes);

                #endregion

                #region Saving UserRole Mapping Data

                if (user.Roles == null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideRoleDetails");
                    return returnResult;
                }

                if (user.Roles.Contains(UserRoles.SuperAdmin.ToString()) && !tokenData.Role.Contains(UserRoles.SuperAdmin.ToString()))
                {
                    returnResult.Result =
                        $"{ResourceInformation.GetResValue("NoPermission")}  {ResourceInformation.GetResValue("Role")}";
                    return returnResult;
                }
                //SiteAdmin can not Add a user of role SiteAdmin
                //if (user.Roles.Contains(UserRoles.SiteAdmin.ToString()) && tokenData.Role.Contains(UserRoles.SiteAdmin.ToString()))
                //{
                //    returnResult.Result =
                //        $"{ResourceInformation.GetResValue("NoPermission")}  {ResourceInformation.GetResValue("Role")}";
                //    return returnResult;
                //}

                if ((await roleRepository.SelectAsync(r => user.Roles.Contains(r.RoleName) && !r.IsActive.Value)).Any())
                {
                    returnResult.Result =
                        $"{ResourceInformation.GetResValue("Role")}  {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
                var rolesList = (await roleRepository.SelectAsync(r => user.Roles.Contains(r.RoleName))).ToList();

                IUserRoleMappingRepository userRoleMappingRepository = _serviceProvider.GetRequiredService<IUserRoleMappingRepository>();
                //Guid userId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(tokenData.UserName))).UserId;

                List<UserRoleMapping> updateUserRoles = new List<UserRoleMapping>();

                foreach (Role tempRole in rolesList)
                {
                    // Only SuperAdmin and siteadmin with master organization can add multipleOrgAccess Roles
                    if (tempRole.MultipleOrgAccess == true
                        && (!tokenData.Role.Contains(UserRoles.SuperAdmin.ToString())
                            && !(tokenData.Role.Contains(UserRoles.SiteAdmin.ToString()) && tokenData.OrgId.Equals(_mstOrg, StringComparison.OrdinalIgnoreCase))))
                    {
                        returnResult.Result =
                            $"{ResourceInformation.GetResValue("NoPermission")}  {ResourceInformation.GetResValue("Role")}";
                        return returnResult;
                    }
                    // Only User with master organization can have MultipleOrgAccess role
                    if (tempRole.MultipleOrgAccess == true && !tempOrg.OrgName.ToUpper().Equals(tokenData.OrgId.ToUpper()))
                    {
                        returnResult.Result =
                            $"{ResourceInformation.GetResValue("CanNotAssign")}{ResourceInformation.GetResValue("Role")}";
                        return await Task.FromResult(returnResult);
                    }
                    updateUserRoles.Add(new UserRoleMapping
                    {
                        UserId = user.UserId,
                        RoleId = tempRole.RoleId,
                        IsActive = true,
                        CreatedBy = userId,
                        ModifiedBy = userId,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now
                    });
                }
                await userRoleMappingRepository.AddRange(updateUserRoles);

                #endregion

                await _userRepository.UnitOfWork.SaveChangesAsync();

                //Send user registration email
                var requiredEmailTemplateInfo = await _emailTemplateRepository.SelectFirstOrDefaultAsync(e => e.EmailTemplateName == EmailConstants.UserRegistrationTemplate);
                var appInfo = await _applicationRepository.SelectFirstOrDefaultAsync(e => e.AppId == requiredEmailTemplateInfo.AppId);

                SendUserRegistrationNotification(user, requiredEmailTemplateInfo, appInfo.AppUrl);

                returnResult.Success = true;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedSuccess")}";
                returnResult.Data = user.UserId;
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "SaveUser", ex.Message, ex.StackTrace);
                returnResult.Result =
                    $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// To Update Existing User
        /// </summary>
        /// <param name="tokenData">tokenData</param>
        /// <param name="id">id</param>
        /// <param name="user">user</param>
        /// <returns>returns a responce message</returns>
        public async Task<ReturnResult> UpdateUser(TokenData tokenData, Guid id, User user)
        {
            IPasswordHistoryBusiness passwordHistoryBusiness = _serviceProvider.GetRequiredService<IPasswordHistoryBusiness>();
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (user == null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideUserDetails");
                    return returnResult;
                }
                if (string.IsNullOrEmpty(id.ToString()))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideUserId");
                    return returnResult;
                }
                if (user.OrgId == 0 && string.IsNullOrEmpty(user.OrgName))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideOrganizationName");
                    return returnResult;
                }
                if (user.UserId != id)
                {
                    returnResult.Result = ResourceInformation.GetResValue("MismatchUserId");
                    return returnResult;
                }

                if (user.IsActive == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("IsActiveRequired");
                    return returnResult;
                }

                IExecuterStoreProc<User> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<User>>();

                List<Parameters> param = new List<Parameters>() {
                    new Parameters("p_Roles", tokenData.Role.FirstOrDefault()),
                    new Parameters("p_OrgId", tokenData.OrgId),
                    new Parameters("p_ClientType", String.Join(",", tokenData.UserClientTypes)),
                    new Parameters("p_UserName", tokenData.UserName),
                    new Parameters("p_UseId", id.ToString()) };

                User updateUser = (await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllExistUserById.ToString(), param)).FirstOrDefault();

                if (updateUser == null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("UnauthorizedUser");
                    return returnResult;
                }

                // Assigning existing role in case of Update
                if (user.Roles.Length == 0)
                {
                    var userResponse = await GetUser(tokenData, id);
                    if (userResponse.Success)
                    {
                        UserModel userData = JsonConvert.DeserializeObject<UserModel>(JsonConvert.SerializeObject(userResponse.Data));
                        user.Roles = userData.RolesArray;
                    }
                }
                IOrganizationRepository organizationRepository = _serviceProvider.GetRequiredService<IOrganizationRepository>();
                IRoleRepository roleRepository = _serviceProvider.GetRequiredService<IRoleRepository>();
                var tempOrg = await organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgId == user.OrgId || o.OrgName == user.OrgName);
                user.OrgId = tempOrg.OrgId;
                if (user.Roles.FirstOrDefault() == Enum.GetName(typeof(UserRoles), UserRoles.SuperAdmin))
                {
                    if (tempOrg.OrgName != _mstOrg)
                        returnResult.Result = ResourceInformation.GetResValue("MasterOrganizationUser");
                    return returnResult;
                }
                if (tokenData.OrgId.Equals(_mstOrg))
                {
                    var exstingUserId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(tokenData.UserName))).UserId;
                    foreach (string tokenRole in tokenData.Role)
                    {
                        // Checks Wether user has limited organization access
                        if ((await roleRepository.SelectAsync(r => r.RoleName.Equals(tokenRole) && r.MultipleOrgAccess == true)).Any())
                        {
                            IUserOrganizationMappingRepository userOrganizationMappingRepository = _serviceProvider.GetRequiredService<IUserOrganizationMappingRepository>();

                            if (!(await userOrganizationMappingRepository.SelectAsync(uom => uom.UserId.Equals(exstingUserId) && uom.OrgId.Equals(user.OrgId))).Any())
                            {
                                returnResult.Result = ResourceInformation.GetResValue("NoPermission");
                                return returnResult;
                            }
                        }
                    }
                }
                else
                {
                    if (!tokenData.OrgId.Equals(tempOrg.OrgName) && tokenData.UserName.ToUpper() != KeyConstant.Client.ToUpper())
                    {
                        returnResult.Result = ResourceInformation.GetResValue("MismatchOrganization");
                        return returnResult;
                    }
                }
                if (user.UserName != updateUser.UserName)
                {
                    returnResult.Result = ResourceInformation.GetResValue("MismatchUserName");
                    return returnResult;
                }

                /// Check if account is already locked
                if (updateUser.IsAccLock)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("AccountLocked");
                    return returnResult;
                }

                IUserRoleMappingRepository userRoleMappingRepository = _serviceProvider.GetRequiredService<IUserRoleMappingRepository>();

                //Check to see SiteAdmin cannot update role of SiteAdmin
                if (tokenData.Role.Contains(UserRoles.SiteAdmin.ToString()))
                {
                    var roleDetailsOfSiteAdmin = await roleRepository.SelectFirstOrDefaultAsync(r => r.RoleName.Equals(UserRoles.SiteAdmin.ToString()));
                    var roleOfUserToBeUpdated = await userRoleMappingRepository.SelectFirstOrDefaultAsync(ur => ur.UserId.Equals(user.UserId));
                    //Check to see if the user to be updated is also a SiteAdmin
                    //if (roleOfUserToBeUpdated.RoleId == roleDetailsOfSiteAdmin.RoleId)
                    //{
                    //    returnResult.Result =
                    //         $"{ResourceInformation.GetResValue("NoPermission")}  {ResourceInformation.GetResValue("Role")}";
                    //    return returnResult;
                    //}
                }

                if (user.TwoFactorEnabled != null)
                {
                    updateUser.TwoFactorEnabled = user.TwoFactorEnabled;
                }

                Guid userId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(tokenData.UserName) && u.IsActive.Value)).UserId;


                if (!String.IsNullOrEmpty(user.EmailId))
                    updateUser.EmailId = user.EmailId;

                if (!String.IsNullOrEmpty(user.PhoneNumber))
                    updateUser.PhoneNumber = user.PhoneNumber;

                if (!String.IsNullOrEmpty(user.Name))
                    updateUser.Name = user.Name;

                if (!String.IsNullOrEmpty(user.Name))
                    updateUser.Name = user.Name;

                if (!String.IsNullOrEmpty(user.FamilyName))
                    updateUser.FamilyName = user.FamilyName;

                updateUser.ModifiedOn = DateTime.Now;
                updateUser.IsActive = user.IsActive;
                updateUser.IsFirstTimeLogin = user.IsFirstTimeLogin;
                updateUser.ModifiedBy = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(tokenData.UserName))).UserId;

                //  User's client type can not be updated as per old IMS
                #region Commented  Updating UserClientType Mapping Data

                //if (!tokenData.UserClientTypes.Contains(Enum.GetName(typeof(ClientTypes), Int32.Parse(_mstClientType))))
                //{
                //    if ((tokenData.UserClientTypes.ToList().Except(updateuser.ClientTypeArray.ToList())).Count() > 0)
                //    {
                //        returnResult.Result = $"{ResourceInformation.GetResValue("NoPermissionToUpdateClientType")}";
                //        return returnResult;
                //    }
                //}

                //if ((user.ClientType.ToList().Except(updateuser.ClientTypeArray.ToList())).Count() > 0)
                //{
                //    returnResult.Result = $"{ResourceInformation.GetResValue("UnauthorizedClient")}";
                //    return returnResult;
                //}

                //else if (user.ClientType == null)
                //{
                //    returnResult.Result = ResourceInformation.GetResValue("ProvideClientType");
                //    return returnResult;
                //}

                //IClientTypeRepository clientTypeRepository = _serviceProvider.GetRequiredService<IClientTypeRepository>();

                //var existingClientTypeList = clientTypeRepository.SelectAsync(c => user.ClientType.Contains(c.ClientTypeName) && c.IsActive.Value).ToList();

                //if (existingClientTypeList == null || existingClientTypeList.Count() != user.ClientType.ToList().Count())
                //{
                //    returnResult.Result =
                //        $"{ResourceInformation.GetResValue("ClientType")}  {ResourceInformation.GetResValue("NotExists")}";
                //    return returnResult;
                //}

                //IUserClientTypeMappingRepository userClientTypeMappingRepository = _serviceProvider.GetRequiredService<IUserClientTypeMappingRepository>();

                //var deleteUserClientTypeList = userClientTypeMappingRepository.SelectAsync(ucm => ucm.UserId.Equals(updateuser.UserId) && ucm.IsActive.Value).ToList();

                //List<UserClientTypeMapping> updateUserClientTypes = new List<UserClientTypeMapping>();

                //foreach (ClientType tempClientType in existingClientTypeList)
                //{
                //    updateUserClientTypes.Add(new UserClientTypeMapping
                //    {
                //        UserId = user.UserId,
                //        ClientTypeId = tempClientType.ClientTypeId,
                //        IsActive = true
                //    });
                //}

                //if (deleteUserClientTypeList != null && deleteUserClientTypeList.Count > 0 && updateUserClientTypes != null && updateUserClientTypes.Count > 0)
                //{
                //    userClientTypeMappingRepository.DeleteRange(deleteUserClientTypeList);
                //    userClientTypeMappingRepository.AddRange(updateUserClientTypes);
                //}

                #endregion

                #region Updating UserRole Mapping Data

                List<Role> roleList = (await roleRepository.SelectAsync(r => user.Roles.Contains(r.RoleName) && r.IsActive.Value)).ToList();

                List<UserRoleMapping> updateUserRoles = new List<UserRoleMapping>();
                foreach (Role tempRole in roleList)
                {
                    if (tempRole.MultipleOrgAccess == true && !tokenData.Role.Contains(UserRoles.SuperAdmin.ToString()))
                    {
                        returnResult.Result =
                            $"{ResourceInformation.GetResValue("NoPermission")}  {ResourceInformation.GetResValue("Role")}";
                        return returnResult;
                    }
                    // Only User with master organization can have MultipleOrgAccess role
                    if (tempRole.MultipleOrgAccess == true && !tempOrg.OrgName.ToUpper().Equals(tokenData.OrgId.ToUpper()))
                    {
                        returnResult.Result =
                            $"{ResourceInformation.GetResValue("CanNotAssign")} {ResourceInformation.GetResValue("Role")}";
                        return await Task.FromResult(returnResult);
                    }

                    updateUserRoles.Add(new UserRoleMapping
                    {
                        UserId = user.UserId,
                        RoleId = tempRole.RoleId,
                        IsActive = true,
                        CreatedBy = userId,
                        CreatedOn = DateTime.Now,
                        ModifiedBy = userId,
                        ModifiedOn = DateTime.Now
                    });
                }

                if (user.Roles == null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideRoleDetails");
                    return returnResult;
                }

                var deleteUserRoleList =
                       (await userRoleMappingRepository.SelectAsync(urm => urm.UserId.Equals(updateUser.UserId) && urm.IsActive)).ToList();

                var role = user.Roles.Select(r => r).FirstOrDefault();

                if (role == UserRoles.SuperAdmin.ToString() && tokenData.Role.Contains(UserRoles.SiteAdmin.ToString()))
                {
                    returnResult.Result =
                        $"{ResourceInformation.GetResValue("NoPermission")}'{role}' {ResourceInformation.GetResValue("Role")}";
                    return returnResult;
                }



                if ((await roleRepository.SelectAsync(r => r.RoleId.Equals(role) && !r.IsActive.Value)).Any())
                {
                    returnResult.Result =
                        $"{ResourceInformation.GetResValue("Role")} '{role}' {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
                var roleDetails = await roleRepository.SelectFirstOrDefaultAsync(r => r.RoleName.Equals(role) && r.IsActive.Value);
                if (roleDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("RoleDetailsNotFound");
                    return returnResult;
                }
                var roleId = roleDetails.RoleId;

                await userRoleMappingRepository.DeleteRange(deleteUserRoleList);
                await userRoleMappingRepository.AddRange(updateUserRoles);
                #endregion

                await _userRepository.UpdateAsync(updateUser);

                await _userRepository.UnitOfWork.SaveChangesAsync();
                returnResult.Success = true;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateSuccess")}";

                return returnResult;
            }
            catch (Exception ex)
            {
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                returnResult.Success = false;
                return returnResult;
            }
        }

        /// <summary>
        /// Delete User by UserId
        /// </summary>
        /// <param name="tokenData">tokenData</param>
        /// <param name="id">id</param>
        /// <returns>returns response  message</returns>
        public async Task<ReturnResult> DeleteUser(TokenData tokenData, Guid id)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (string.IsNullOrEmpty(id.ToString()))
                    returnResult.Result = ResourceInformation.GetResValue("ProvideUserId");

                IExecuterStoreProc<User> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<User>>();

                List<Parameters> param = new List<Parameters>() {
                    new Parameters("p_Roles", tokenData.Role.FirstOrDefault()),
                    new Parameters("p_OrgId", tokenData.OrgId),
                    new Parameters("p_ClientType", String.Join(",", tokenData.UserClientTypes)),
                    new Parameters("p_UserName", tokenData.UserName),
                    new Parameters("p_UseId", id.ToString()) };

                User deleteUser = (await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllExistUserById.ToString(), param)).FirstOrDefault();

                if (deleteUser == null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("UnauthorizedUser");
                    return returnResult;
                }
                if (!deleteUser.IsActive.Value)
                {
                    returnResult.Result = ResourceInformation.GetResValue("DeletedUser");
                    return returnResult;
                }
                IRoleRepository roleRepository = _serviceProvider.GetRequiredService<IRoleRepository>();

                if (tokenData.OrgId == _mstOrg)
                {
                    foreach (string role in tokenData.Role)
                    {
                        // Checks Whether user has limited organization access
                        if ((await roleRepository.SelectAsync(r => r.RoleName.Equals(role) && r.MultipleOrgAccess == true)).Any())
                        {
                            IUserOrganizationMappingRepository userOrganizationMappingRepository = _serviceProvider.GetRequiredService<IUserOrganizationMappingRepository>();
                            var exstingUserId = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(tokenData.UserName))).UserId;

                            if (!(await userOrganizationMappingRepository.SelectAsync(uom => uom.UserId.Equals(exstingUserId) && uom.OrgId.Equals(deleteUser.OrgId))).Any())
                            {
                                returnResult.Result = ResourceInformation.GetResValue("NoPermission");
                                return returnResult;
                            }
                        }
                    }
                }
                else
                {
                    IOrganizationRepository organizationRepository = _serviceProvider.GetRequiredService<IOrganizationRepository>();

                    var tempOrg = await organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgId == deleteUser.OrgId);
                    if (!tokenData.OrgId.Equals(tempOrg.OrgName) && tokenData.UserName.ToUpper() != KeyConstant.Client.ToUpper())
                    {
                        returnResult.Result = ResourceInformation.GetResValue("MismatchOrganization");
                        return returnResult;
                    }

                }

                deleteUser.ModifiedBy = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(tokenData.UserName) && u.IsActive.Value)).UserId;
                deleteUser.ModifiedOn = DateTime.Now;
                deleteUser.IsActive = false;

                //Delete respective UserRoleMapping.
                IUserRoleMappingRepository userRoleMappingRepository = _serviceProvider.GetRequiredService<IUserRoleMappingRepository>();
                var existingUserRoleMapping = (await userRoleMappingRepository.SelectAsync(urm => urm.UserId.Equals(deleteUser.UserId) && urm.IsActive)).ToList();
                existingUserRoleMapping.ForEach(urm => urm.IsActive = false);
                await userRoleMappingRepository.DeleteRange(existingUserRoleMapping);

                //Delete respective UserClientMapping
                IUserClientTypeMappingRepository userClientTypeMappingRepository = _serviceProvider.GetRequiredService<IUserClientTypeMappingRepository>();
                var existingUserClienyTypeMapping = (await userClientTypeMappingRepository.SelectAsync(ucm => ucm.UserId.Equals(deleteUser.UserId) && ucm.IsActive.Value)).ToList();
                existingUserClienyTypeMapping.ForEach(ucm => ucm.IsActive = false);
                await userClientTypeMappingRepository.DeleteRange(existingUserClienyTypeMapping);

                await _userRepository.UpdateAsync(deleteUser);
                await _userRepository.UnitOfWork.SaveChangesAsync();

                returnResult.Success = true;
                returnResult.Result = ResourceInformation.GetResValue("DataDeleteSuccess");

                return returnResult;


            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "DeleteUser", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// Get User name by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ReturnResult> GetUserNameById(Guid userId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var existingUser = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserId.Equals(userId) && u.IsActive.Value);

                if (existingUser != null)
                {
                    returnResult.Success = true;
                    returnResult.Data = existingUser.UserName;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "GetUserNameById", ex.Message, ex.StackTrace);
                throw ex;
            }
        }
        #endregion

        #region User Application Mapping

        /// <summary>
        /// This method is used to map a user with an application
        /// </summary>
        /// <param name="tokenData">tokenData object</param>
        /// /// <param name="applicationUserMappingModel">applicationUserMappingModel</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> AddOrUpdateUserApplication(TokenData tokenData, ApplicationUserMappingModel applicationUserMappingModel)
        {
            IApplicationUserMappingRepository _applicationUserMappingRepository = _serviceProvider.GetRequiredService<IApplicationUserMappingRepository>();
            IOrganizationRepository organizationRepository = _serviceProvider.GetRequiredService<IOrganizationRepository>();
            IRoleRepository roleRepository = _serviceProvider.GetRequiredService<IRoleRepository>();
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (applicationUserMappingModel == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidDetails");
                    return returnResult;
                }
                foreach (var appid in applicationUserMappingModel.AppId.Distinct())
                {
                    IExecuterStoreProc<RoleModel> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<RoleModel>>();
                    List<Parameters> param = new List<Parameters>() {
                            new Parameters("p_AppId", appid) };
                    var listOfApplicationRoles = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetApplicationRoles.ToString(), param);

                    IExecuterStoreProc<UserModel> procExecuterUserRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<UserModel>>();
                    List<Parameters> userParam = new List<Parameters>() {
                    new Parameters("p_Roles", tokenData.Role.FirstOrDefault()),
                    new Parameters("p_OrgId", tokenData.OrgId),
                    new Parameters("p_ClientType", String.Join(",", tokenData.UserClientTypes)),
                    new Parameters("p_UserName", tokenData.UserName),
                    new Parameters("p_UseId", applicationUserMappingModel.UserId) };
                    var userData = await procExecuterUserRepository.ExecuteProcedureAsync(ProcedureConstants.procGetUserById.ToString(), userParam);
                    var userRole = userData.Select(x => x.Roles).ToList();
                    if (userData.FirstOrDefault().UserName == tokenData.UserName)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("UnauthorisedUser");
                        return returnResult;
                    }
                    foreach (var role in userRole)
                    {
                        if (listOfApplicationRoles == null || !listOfApplicationRoles.Select(x => x.RoleName).Contains(role))
                        {
                            returnResult.Success = false;
                            returnResult.Result = ResourceInformation.GetResValue("AppRoleNotPresent");
                            return returnResult;
                        }
                    }
                }
                var userDetailsFromToken = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(tokenData.UserName) && u.IsActive.Value && (!u.IsAccLock)));
                Guid userIdFromToken = userDetailsFromToken.UserId;

                var userDetailsToBeAdded = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserId.Equals(applicationUserMappingModel.UserId) && u.IsActive.Value && (!u.IsAccLock)));

                // check for multi-org access
                if (tokenData.OrgId.Equals(_mstOrg))
                {
                    var tokenApplicationMapping = await _applicationUserMappingRepository.SelectAsync(aum => aum.UserId.Equals(userIdFromToken) && aum.IsActive);
                    foreach (string tokenRole in tokenData.Role)
                    {
                        // Checks whether user has limited organization access
                        if ((await roleRepository.SelectAsync(r => r.RoleName.Equals(tokenRole) && r.MultipleOrgAccess == true)).Any())
                        {
                            IUserOrganizationMappingRepository userOrganizationMappingRepository = _serviceProvider.GetRequiredService<IUserOrganizationMappingRepository>();

                            if (!(await userOrganizationMappingRepository.SelectAsync(uom => uom.UserId.Equals(userIdFromToken) && uom.OrgId.Equals(userDetailsToBeAdded.OrgId))).Any())
                            {
                                returnResult.Result = ResourceInformation.GetResValue("NoPermission");
                                return returnResult;
                            }

                            if (tokenApplicationMapping == null && tokenApplicationMapping.Count() == 0)
                            {
                                returnResult.Success = false;
                                returnResult.Result = ResourceInformation.GetResValue("UnauthorisedUser");
                                return returnResult;
                            }

                            var tokenApplicationsAccess = tokenApplicationMapping.Select(s => s.AppId).ToList();

                            var applicationsToAdd = applicationUserMappingModel.AppId.Distinct().ToList();

                            var applicationAccess = applicationsToAdd.Except(tokenApplicationsAccess).ToList();

                            if (applicationAccess != null && applicationAccess.Count() > 0)
                            {
                                returnResult.Success = false;
                                returnResult.Result = ResourceInformation.GetResValue("UnauthorisedUser");
                                return returnResult;
                            }
                        }
                    }
                }

                else
                {
                    var siteAdminMapping = await _applicationUserMappingRepository.SelectAsync(aum => aum.UserId.Equals(userIdFromToken) && aum.IsActive);
                    if (siteAdminMapping == null && siteAdminMapping.Count() == 0)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("UnauthorisedUser");
                        return returnResult;
                    }

                    var siteAdminApplications = siteAdminMapping.Select(s => s.AppId).ToList();

                    var applicationsToAdd = applicationUserMappingModel.AppId.Distinct().ToList();

                    var applicationAccess = applicationsToAdd.Except(siteAdminApplications).ToList();

                    if (applicationAccess != null && applicationAccess.Count() > 0)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("UnauthorisedUser");
                        return returnResult;
                    }

                    var userRole = (await _userRoleMappingRepository.SelectAsync(x => x.UserId == applicationUserMappingModel.UserId)).Select(x => x.RoleId).ToList();
                    var siteadminrole = (await _roleRepository.SelectFirstOrDefaultAsync(x => x.RoleName.Equals(Enum.GetName(typeof(UserRoles), UserRoles.SiteAdmin))));
                    var superadminrole = (await _roleRepository.SelectFirstOrDefaultAsync(x => x.RoleName.Equals(Enum.GetName(typeof(UserRoles), UserRoles.SuperAdmin))));

                    if (userRole.Contains(superadminrole.RoleId))
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("UnauthorisedUser");
                        return returnResult;
                    }
                }

                if (applicationUserMappingModel.IsActive == null)
                {
                    applicationUserMappingModel.IsActive = true;
                }

                IApplicationUserMappingBusiness _applicationUserMappingBusiness = _serviceProvider.GetRequiredService<IApplicationUserMappingBusiness>();

                foreach (var appId in applicationUserMappingModel.AppId.Distinct())
                {
                    var userAccess = await _applicationUserMappingBusiness.ValidateUserApplicationAccess(applicationUserMappingModel.UserId, appId);
                    if (!userAccess)
                    {
                        returnResult.Result = ResourceInformation.GetResValue("UserApplicationInaccessible");
                        returnResult.Success = false;
                        return returnResult;
                    }
                }


                List<ApplicationUserMapping> userApplicationMapping = new List<ApplicationUserMapping>();
                foreach (int appId in applicationUserMappingModel.AppId.Distinct())
                {
                    userApplicationMapping.Add(new ApplicationUserMapping
                    {
                        UserId = applicationUserMappingModel.UserId,
                        AppId = appId,
                        CreatedBy = userIdFromToken,
                        ModifiedBy = userIdFromToken,
                        IsActive = applicationUserMappingModel.IsActive.Value,

                    });
                }

                var existingUserApplicationMapping = await _applicationUserMappingRepository.SelectAsync(aum => aum.UserId == applicationUserMappingModel.UserId);
                if (existingUserApplicationMapping != null && existingUserApplicationMapping.Count() > 0)
                {
                    await _applicationUserMappingRepository.DeleteRange(existingUserApplicationMapping);
                    await _applicationUserMappingRepository.AddRange(userApplicationMapping);
                    await _applicationUserMappingRepository.UnitOfWork.SaveChangesAsync();
                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("DataSavedSuccess");
                    return returnResult;
                }

                else
                {
                    await _applicationUserMappingRepository.AddRange(userApplicationMapping);
                    await _applicationUserMappingRepository.UnitOfWork.SaveChangesAsync();
                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("DataSavedSuccess");
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "AddOrUpdateUserApplication", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result =
                    $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        ///// <summary>
        ///// This method is used to update the mapping of a user with an application
        ///// </summary>
        ///// <param name="userName">userName</param>
        ///// /// <param name="applicationUserMappingModel">ApplicationUserMappingModel object</param>
        ///// <returns>returns response message</returns>
        //public async Task<ReturnResult> UpdateUserApplication(string userName, ApplicationUserMappingModel applicationUserMappingModel)
        //{
        //    ReturnResult returnResult = new ReturnResult();
        //    try
        //    {
        //        if (applicationUserMappingModel == null)
        //        {
        //            returnResult.Result = ResourceInformation.GetResValue("ProvideMappingDetails");
        //            return returnResult;
        //        }

        //        if (applicationUserMappingModel.IsActive == null)
        //        {
        //            returnResult.Success = false;
        //            returnResult.Result = ResourceInformation.GetResValue("IsActiveRequired");
        //            return returnResult;
        //        }
        //        IApplicationUserMappingBusiness _applicationUserMappingBusiness = _serviceProvider.GetRequiredService<IApplicationUserMappingBusiness>();

        //        var userAccess = await _applicationUserMappingBusiness.ValidateUserApplicationAccess(applicationUserMappingModel.UserId, applicationUserMappingModel.AppId);

        //        if (!userAccess)
        //        {
        //            returnResult.Result = ResourceInformation.GetResValue("UserApplicationInaccessible");
        //            returnResult.Success = false;
        //            return returnResult;
        //        }

        //        IApplicationUserMappingRepository _applicationUserMappingRepository = _serviceProvider.GetRequiredService<IApplicationUserMappingRepository>();
        //        var updatedApplicationUserMapping = await _applicationUserMappingRepository.SelectFirstOrDefaultAsync(x => x.MappingId.Equals(applicationUserMappingModel.MappingId) && x.IsActive);

        //        if (updatedApplicationUserMapping == null)
        //        {
        //            returnResult.Result =
        //                $"{ResourceInformation.GetResValue("UserApplicationMapping")} {ResourceInformation.GetResValue("NotExists")}";
        //            return returnResult;
        //        }

        //        var userDetails = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
        //        if (userDetails == null)
        //        {
        //            returnResult.Success = false;
        //            returnResult.Result = ResourceInformation.GetResValue("UserDetailsNotFound");
        //            return returnResult;
        //        }
        //        Guid userIdFromToken = userDetails.UserId;
        //        updatedApplicationUserMapping.UserId = applicationUserMappingModel.UserId;
        //        updatedApplicationUserMapping.AppId = applicationUserMappingModel.AppId;
        //        updatedApplicationUserMapping.IsActive = applicationUserMappingModel.IsActive.Value;
        //        updatedApplicationUserMapping.ModifiedBy = userIdFromToken;

        //        var result = await _applicationUserMappingRepository.UpdateAsync(updatedApplicationUserMapping);

        //        if (result.State.Equals(EntityState.Modified))
        //        {
        //            await _applicationUserMappingRepository.UnitOfWork.SaveChangesAsync();

        //            returnResult.Success = true;
        //            returnResult.Result = ResourceInformation.GetResValue("DataUpdateSuccess");
        //        }
        //        else
        //        {
        //            returnResult.Success = false;
        //            returnResult.Result = ResourceInformation.GetResValue("DataUpdateFailure");
        //        }

        //        return returnResult;
        //    }

        //    catch (Exception ex)
        //    {
        //        _logger.Log(LogType.ERROR, "UserBusiness", "UpdateUserApplication", ex.Message, ex.StackTrace);
        //        returnResult.Success = false;
        //        returnResult.Result =
        //            $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
        //        return returnResult;
        //    }

        //}

        /// <summary>
        /// Used to delete the mapping of a user with an application
        /// </summary>
        /// <param name="userName">userName</param>
        ///<param name="mappingId">Mapping Id</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> DeleteUserApplication(TokenData tokenData, int mappingId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IApplicationUserMappingRepository _applicationUserMappingRepository = _serviceProvider.GetRequiredService<IApplicationUserMappingRepository>();
                IRoleRepository roleRepository = _serviceProvider.GetRequiredService<IRoleRepository>();
                var deleteUserApplication = await _applicationUserMappingRepository.SelectFirstOrDefaultAsync(u => u.MappingId.Equals(mappingId) && u.IsActive);
                if (deleteUserApplication == null)
                {
                    returnResult.Success = false;
                    returnResult.Result =
                        $"{ResourceInformation.GetResValue("UserApplicationMapping")} {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
                var userDetailsFromToken = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(tokenData.UserName) && u.IsActive.Value && (!u.IsAccLock)));
                Guid userIdFromToken = userDetailsFromToken.UserId;
                var organizationFromToken = userDetailsFromToken.OrgId;

                var userAsscociatedWithMapping = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserId == deleteUserApplication.UserId && u.IsActive.Value);
                Guid userIdToBeDeleted = userAsscociatedWithMapping.UserId;
                var userToBeDeletedOrganization = userAsscociatedWithMapping.OrgId;

                // check for multi-org access
                if (tokenData.OrgId.Equals(_mstOrg))
                {
                    foreach (string tokenRole in tokenData.Role)
                    {
                        // Checks whether user has limited organization access
                        if ((await roleRepository.SelectAsync(r => r.RoleName.Equals(tokenRole) && r.MultipleOrgAccess == true)).Any())
                        {
                            IUserOrganizationMappingRepository userOrganizationMappingRepository = _serviceProvider.GetRequiredService<IUserOrganizationMappingRepository>();

                            if (!(await userOrganizationMappingRepository.SelectAsync(uom => uom.UserId.Equals(userIdFromToken) && uom.OrgId.Equals(userToBeDeletedOrganization))).Any())
                            {
                                returnResult.Success = false;
                                returnResult.Result = ResourceInformation.GetResValue("NoPermission");
                                return returnResult;
                            }

                            var tokenApplicationMapping = await _applicationUserMappingRepository.SelectAsync(aum => aum.UserId.Equals(userIdFromToken) && aum.IsActive);
                            if (tokenApplicationMapping == null && tokenApplicationMapping.Count() == 0)
                            {
                                returnResult.Success = false;
                                returnResult.Result = ResourceInformation.GetResValue("UnauthorisedUser");
                                return returnResult;
                            }

                            var tokenApplicationsAccess = tokenApplicationMapping.Select(s => s.AppId).ToList();

                            if (!tokenApplicationsAccess.Contains(deleteUserApplication.AppId))
                            {
                                returnResult.Success = false;
                                returnResult.Result = ResourceInformation.GetResValue("UnauthorisedUser");
                                return returnResult;
                            }
                        }
                    }
                }

                else
                {
                    var siteAdminMapping = await _applicationUserMappingRepository.SelectAsync(aum => aum.UserId.Equals(userIdFromToken) && aum.IsActive);
                    if (siteAdminMapping == null && siteAdminMapping.Count() == 0)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("UnauthorisedUser");
                        return returnResult;
                    }

                    var organizationOfSiteAdmin = userDetailsFromToken.OrgId;

                    var siteAdminApplications = siteAdminMapping.Select(a => a.AppId);

                    if (!siteAdminApplications.Contains(deleteUserApplication.AppId) || organizationOfSiteAdmin != userToBeDeletedOrganization)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("UnauthorisedUser");
                        return returnResult;
                    }
                }

                deleteUserApplication.IsActive = false;
                deleteUserApplication.ModifiedBy = userIdFromToken;

                var result = await _applicationUserMappingRepository.UpdateAsync(deleteUserApplication);
                if (result.State.Equals(EntityState.Modified))
                {
                    await _applicationUserMappingRepository.UnitOfWork.SaveChangesAsync();
                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("DataDeleteSuccess");
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DataDeleteFailure");
                }

                return returnResult;

            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "DeleteUserApplication", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result =
                    $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        #endregion

        #region Forgot Password.
        /// <summary>
        /// Verify account on the basis of user name and email
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userEmail"></param>
        /// <returns>userId</returns>
        public async Task<ReturnResult> VerifyAccount(string userName, string userEmail)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IForgotPasswordFlowManagementRepository forgotPasswordFlowManagementRepository = _serviceProvider.GetRequiredService<IForgotPasswordFlowManagementRepository>();
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userEmail))
                {
                    UserDetails existingUser = await GetUserByUserName(userName);
                    if (existingUser == null)
                    {
                        returnResult.Success = false;
                        returnResult.Result = $"{ResourceInformation.GetResValue("User")} {ResourceInformation.GetResValue("NotExists")}";
                        return returnResult;
                    }
                    if (existingUser.ProviderName != ApplicationLevelConstants.IMSAuthProvider)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("NotIMSAccount");
                        return returnResult;
                    }
                    if (existingUser == null)
                    {
                        returnResult.Success = false;
                        returnResult.Result =
                            $"{ResourceInformation.GetResValue("VerifyAccount")} {ResourceInformation.GetResValue("NotExists")}";
                        return returnResult;
                    }

                    if (existingUser.IsAccLock)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("AccountLocked");
                        return returnResult;
                    }

                    if (!existingUser.EmailId.Equals(userEmail))
                    {
                        returnResult = await LockAccount(existingUser.UserName.ToString(), (int)LockType.VerificationLock);
                        returnResult.Success = false;
                        returnResult.Result =
                            $"{ResourceInformation.GetResValue("VerifyAccount")} {ResourceInformation.GetResValue("EmailNotExits")}" + " " + returnResult.Result;
                        return returnResult;
                    }
                    await ResetLock(existingUser.UserName.ToString(), (int)LockType.VerificationLock);

                    //If the organization/user has enabled multi - factor authentication then generate OTP and send it over Email
                    if (existingUser.TwoFactorEnabled)
                    {
                        IUserOTPRepository _userOTPRepository = _serviceProvider.GetRequiredService<IUserOTPRepository>();
                        int otpLength = _applicationSettings.Value.OTPLength;
                        string UniqueKey = CommonMethods.RandomOTP(otpLength);
                        var result = await _userOTPRepository.AddAsync(new UserOTP
                        {
                            OTPHash = UniqueKey,
                            UserId = existingUser.UserId,
                            OTPCreationDatetime = DateTime.Now,
                            OTPExpirationDatetime = DateTime.Now.AddMinutes(_applicationSettings.Value.ForgotPasswordSession),
                            OTPTypeId = 2,
                        });

                        #region Email Sending

                        IEmailSender _mailSender = _serviceProvider.GetRequiredService<IEmailSender>();

                        //EmailSender sendEmail = new EmailSender();
                        string emailBody = _mailSender.ComposeOTPMailBody(userName, UniqueKey);
                        string emailSubject = ResourceInformation.GetResValue("PasswordRecovery");

                        var emailFlag = await _mailSender.SendEmail(userEmail, ApplicationLevelConstants.FromEmailID, emailSubject, emailBody);

                        if (emailFlag.Success)
                        {
                            returnResult.Result = existingUser.UserId.ToString();

                            if (result.State.Equals(EntityState.Added))
                            {
                                await forgotPasswordFlowManagementRepository.AddOrUpdate(new ForgotPasswordFlowManagement()
                                {
                                    UserId = existingUser.UserId,
                                    VerifiedEmail = true,
                                    VerifiedEmailOn = DateTime.Now,
                                    VerifiedOTP = false,
                                    VerifiedOTPOn = null,
                                    VerifiedSecurityQuestions = false,
                                    VerifiedSecurityQuestionsOn = null
                                });
                                await _userOTPRepository.UnitOfWork.SaveChangesAsync();
                                returnResult.Success = true;
                            }
                        }
                        else
                        {
                            returnResult.Result = ResourceInformation.GetResValue("EmailNotSent");
                        }
                        returnResult.Data = new VerifyUserModel()
                        {
                            UserId = existingUser.UserId,
                            TwoFactorEnable = existingUser.TwoFactorEnabled
                        };
                        #endregion
                    }
                    else
                    {

                        returnResult.Data = new VerifyUserModel()
                        {
                            UserId = existingUser.UserId,
                            TwoFactorEnable = existingUser.TwoFactorEnabled
                        };

                        await forgotPasswordFlowManagementRepository.AddOrUpdate(new ForgotPasswordFlowManagement()
                        {
                            UserId = existingUser.UserId,
                            VerifiedEmail = true,
                            VerifiedEmailOn = DateTime.Now,
                            VerifiedOTP = false,
                            VerifiedOTPOn = null,
                            VerifiedSecurityQuestions = false,
                            VerifiedSecurityQuestionsOn = null
                        });
                        await forgotPasswordFlowManagementRepository.UnitOfWork.SaveChangesAsync();
                        returnResult.Success = true;
                    }
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidUserNameAndEmail");
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "VerifyAccount", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("NotVerified")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// Verify token against provided user id in database
        /// If token is expired or invalid return responce accordingly
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="otp"></param>
        /// <returns></returns>
        public async Task<ReturnResult> VerifyOTP(Guid userId, string otp)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if ((userId != Guid.Empty) && (!string.IsNullOrEmpty(otp)))
                {
                    IUserOTPRepository _userOTPRepository = _serviceProvider.GetRequiredService<IUserOTPRepository>();
                    var user = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserId.Equals(userId) && u.IsActive.Value);

                    if (user == null)
                    {
                        returnResult.Success = false;
                        returnResult.Result =
                            $"{ResourceInformation.GetResValue("VerifyOTP")} {ResourceInformation.GetResValue("NotExists")}";
                        return returnResult;
                    }
                    /// Check if account is already locked
                    if (user.IsAccLock)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("AccountLocked");
                        return returnResult;
                    }

                    var otpVerification = (await _userOTPRepository.SelectAsync(o => o.UserId.Equals(userId))).OrderByDescending(o => o.OTPCreationDatetime).FirstOrDefault();

                    if (otpVerification == null)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("OTPNotExists");
                    }
                    //Check if otp is same as last otp generated for the same user
                    if (otpVerification.OTPHash != otp)
                    {

                        returnResult = await LockAccount(user.UserName.ToString(), (int)LockType.OTPLock);
                        returnResult.Result = ResourceInformation.GetResValue("OTP_Invalid") + " " + returnResult.Result;
                        return returnResult;
                    }

                    //Check if otp expired expire
                    if (otpVerification.OTPExpirationDatetime < DateTime.Now)
                    {
                        returnResult = await LockAccount(user.UserName.ToString(), (int)LockType.OTPLock);
                        returnResult.Result = ResourceInformation.GetResValue("OTP_Expired") + " " + returnResult.Result;
                        return returnResult;
                    }

                    //Add otp valid status to data base 

                    //Create response 
                    await ResetLock(user.UserName.ToString(), (int)LockType.OTPLock);


                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("OTP_Valid");
                    IForgotPasswordFlowManagementRepository forgotPasswordFlowManagementRepository = _serviceProvider.GetRequiredService<IForgotPasswordFlowManagementRepository>();

                    ForgotPasswordFlowManagement flowManagement = await forgotPasswordFlowManagementRepository.SelectFirstOrDefaultAsync(fpm => fpm.UserId.Equals(userId));

                    if (flowManagement != null)
                    {
                        if (flowManagement.VerifiedEmailOn.AddMinutes(_applicationSettings.Value.ForgotPasswordSession) > DateTime.Now)
                        {
                            flowManagement.VerifiedOTP = true;
                            flowManagement.VerifiedOTPOn = DateTime.Now;
                            await _userOTPRepository.DeleteAsync(otpVerification);
                            await _userOTPRepository.UnitOfWork.SaveChangesAsync();
                            await forgotPasswordFlowManagementRepository.UpdateAsync(flowManagement);
                            await forgotPasswordFlowManagementRepository.UnitOfWork.SaveChangesAsync();
                        }
                        else
                        {
                            returnResult.Success = false;
                            returnResult.Result = ResourceInformation.GetResValue("EmailVerificationTimeOut");
                        }
                    }
                    else
                    {
                        returnResult.Success = false;
                        returnResult.Result = $"{ResourceInformation.GetResValue("AccountVerificationRequired")}";
                    }
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidInput");
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "VerifyOTP", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to update password in case of Forgot Password
        /// If token is expired or invalid return responce accordingly
        /// </summary>
        /// <param name="userCredentials">userCredentials object</param>
        /// <returns>Returns success or failure message</returns>
        public async Task<ReturnResult> UpdatePassword(UserCredentials userCredentials)
        {
            ReturnResult flowresult = await VerifyForgotPasswordFlow(userCredentials.UserId);
            if (flowresult.Success == false)
            {
                return flowresult;
            }

            IPasswordHistoryBusiness passwordHistoryBusiness = _serviceProvider.GetRequiredService<IPasswordHistoryBusiness>();
            ILockAccountRepository lockAccountRepository = _serviceProvider.GetRequiredService<ILockAccountRepository>();
            IAuthProviderRepository authProviderRepository = _serviceProvider.GetRequiredService<IAuthProviderRepository>();
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (userCredentials == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideAllDetails");
                    return returnResult;
                }

                if (userCredentials.UserId == null || userCredentials.UserId == Guid.Empty)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideUserId");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(userCredentials.NewPassword))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideNewPwd");
                    return returnResult;
                }

                var userDetails = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserId == userCredentials.UserId && u.IsActive.Value);

                var providerDetails = await authProviderRepository.SelectFirstOrDefaultAsync(p => p.ProviderID == userDetails.ProviderId);

                var providerName = providerDetails.ProviderName;
                if (providerName != ApplicationLevelConstants.IMSAuthProvider)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("UserNotFound");
                    return returnResult;
                }

                if (userDetails.IsAccLock)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("AccountLocked");
                    return returnResult;
                }

                if (!_customPasswordHash.ValidatePassword(userCredentials.NewPassword, out string errorMsg))
                {
                    returnResult.Success = false;
                    returnResult.Result = errorMsg;
                    return returnResult;
                }

                var resultReturn = await passwordHistoryBusiness.ManagePassword(userDetails.UserId.ToString(), userCredentials.NewPassword.ToString(), userCredentials.UserId.ToString());

                if (resultReturn.Success == false)
                {
                    return await Task.FromResult(resultReturn);
                }

                userDetails.PasswordHash = _customPasswordHash.ScryptHash(userCredentials.NewPassword);
                userDetails.LastPasswordChangeOn = DateTime.Now;
                userDetails.PasswordExpiration = DateTime.Now.AddDays(_passwordExpirationDay);

                var result = await _userRepository.UpdateAsync(userDetails);
                if (result.State.Equals(EntityState.Modified))
                {
                    await _userRepository.UnitOfWork.SaveChangesAsync();

                    var deleteRecord = await lockAccountRepository.SelectAsync(x => x.UserId == userDetails.UserId);
                    if (deleteRecord != null && deleteRecord.Any())
                    {
                        await lockAccountRepository.RemoveRange(deleteRecord);
                        await lockAccountRepository.UnitOfWork.SaveChangesAsync();
                    }
                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("PasswordUpdated");
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("PasswordUpdateFailure");
                }
                IForgotPasswordFlowManagementRepository forgotPasswordFlowManagementRepository = _serviceProvider.GetRequiredService<IForgotPasswordFlowManagementRepository>();
                ForgotPasswordFlowManagement managedFlow = await forgotPasswordFlowManagementRepository.SelectFirstOrDefaultAsync(fp => fp.UserId.Equals(userCredentials.UserId));
                await forgotPasswordFlowManagementRepository.DeleteAsync(managedFlow);
                await forgotPasswordFlowManagementRepository.UnitOfWork.SaveChangesAsync();

                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "UpdatePassword", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataUpdateFailure");
                return returnResult;
            }
        }

        #endregion

        #region Change Password
        /// <summary>
        /// This method is used to change password.
        /// </summary>
        /// <param name="userCredentials">userCredentials object</param>
        /// <param name="userName">userName from Token</param>
        /// <returns>Returns success or failure message</returns>
        public async Task<ReturnResult> ChangePassword(string userName, UserCredentials userCredentials)
        {

            IForgotPasswordFlowManagementRepository forgotPasswordFlowManagementRepository = _serviceProvider.GetRequiredService<IForgotPasswordFlowManagementRepository>();
            IPasswordHistoryBusiness passwordHistoryBusiness = _serviceProvider.GetRequiredService<IPasswordHistoryBusiness>();
            ILockAccountRepository lockAccountRepository = _serviceProvider.GetRequiredService<ILockAccountRepository>();
            IAuthProviderRepository authProviderRepository = _serviceProvider.GetRequiredService<IAuthProviderRepository>();

            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (userCredentials == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideAllDetails");
                    return returnResult;
                }

                var userDetails = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);

                if (userDetails.UserName != userCredentials.UserName)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("TokenUserMismatch");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(userCredentials.NewPassword))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideNewPwd");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(userCredentials.OldPassword))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideOldPwd");
                    return returnResult;
                }

                var providerDetails = await authProviderRepository.SelectFirstOrDefaultAsync(p => p.ProviderID == userDetails.ProviderId);
                var providerName = providerDetails.ProviderName;

                if (providerName != ApplicationLevelConstants.IMSAuthProvider)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("UserNotFound");
                    return returnResult;
                }

                if (userDetails.IsAccLock)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("AccountLocked");
                    return returnResult;
                }

                var matchOldPassword = _customPasswordHash.ScryptHashStringVerify(userDetails.PasswordHash, userCredentials.OldPassword);
                if (!matchOldPassword)
                {
                    await LockAccount(userName, (int)LockType.ResetPasswordLock);
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("OldPasswordMismatch");
                    return returnResult;
                }

                if (!_customPasswordHash.ValidatePassword(userCredentials.NewPassword, out string errorMsg))
                {
                    returnResult.Success = false;
                    returnResult.Result = errorMsg;
                    return returnResult;
                }

                var resultReturn = await passwordHistoryBusiness.ManagePassword(userDetails.UserId.ToString(), userCredentials.NewPassword.ToString(), userDetails.UserId.ToString());

                if (resultReturn.Success == false)
                {
                    return await Task.FromResult(resultReturn);
                }

                userDetails.PasswordHash = _customPasswordHash.ScryptHash(userCredentials.NewPassword);
                userDetails.LastPasswordChangeOn = DateTime.Now;
                userDetails.PasswordExpiration = DateTime.Now.AddDays(_passwordExpirationDay);

                var result = await _userRepository.UpdateAsync(userDetails);
                if (result.State.Equals(EntityState.Modified))
                {
                    await _userRepository.UnitOfWork.SaveChangesAsync();

                    var deleteRecord = await lockAccountRepository.SelectAsync(x => x.UserId == userDetails.UserId);
                    if (deleteRecord != null && deleteRecord.Any())
                    {
                        await lockAccountRepository.RemoveRange(deleteRecord);
                        await lockAccountRepository.UnitOfWork.SaveChangesAsync();
                    }

                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("PasswordUpdated");
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("PasswordUpdateFailure");
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "ChangePassword", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataUpdateFailure");
                return returnResult;
            }
        }

        #endregion

        #region Send Password Expiry Mail Notification.
        /// <summary>
        /// Used to get the email and username of all users to whom email must be sent for Password Expiration
        /// </summary>
        /// <returns>true or false</returns>
        public async Task SendEmailPwdExpNotify(TokenData tokenData)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (tokenData != null)
                {
                    var emailSubject = ResourceInformation.GetResValue("PasswordExpireEmail");
                    string emailBody = "";
                    var emailFlag = new ReturnResult();

                    var emails = new List<EmailAddress>();
                    IEmailSender _mailSender = _serviceProvider.GetRequiredService<IEmailSender>();
                    IExecuterStoreProc<SendEmailPwdExpNotify> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<SendEmailPwdExpNotify>>();

                    var getEmailPwdNotifyDetails = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetUsersForPwdExpNotify.ToString(), null);
                    if (getEmailPwdNotifyDetails != null && getEmailPwdNotifyDetails.Count() != 0)
                    {
                        foreach (var item in getEmailPwdNotifyDetails)
                        {
                            emailSubject = item.EmailSubject;
                            emailBody = item.EmailBody;
                            //emailBody = _mailSender.ComposePwdExpMailBodyCustomize(item.UserName,item.Name,item.PasswordExpiration,_applicationSettings.Value.EnvURL);
                            emailFlag = await _mailSender.SendEmail(item.EmailId, ApplicationLevelConstants.FromEmailID, emailSubject, emailBody);
                            if (!emailFlag.Success)
                                _logger.Info("UserBusiness", "SendEmailPwdExpNotify", "Email not sent to" + emailFlag.Data, "Mail failed at" + DateTime.Now);
                        }
                    }
                    else
                    {
                        _logger.Info("UserBusiness", "SendEmailPwdExpNotify", "No emails to send", "No emails sent" + DateTime.Now);
                    }
                }
                else
                {
                    _logger.Info("UserBusiness", "SendMail", "Invalid Token", "Mail failed at" + DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "SendEmailPwdExpNotify", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region Account Lock and Unlock
        /// <summary>
        /// To lock account for wrong verification, login, password reset, otp,security question answer
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<ReturnResult> LockAccount(string userName, int lockType)
        {
            ILockAccountRepository lockAccountRepository = _serviceProvider.GetRequiredService<ILockAccountRepository>();
            LockAccount userAccount = new LockAccount();
            ReturnResult returnResult = new ReturnResult();

            try
            {
                var lockUser = (await _userRepository.SelectAsync(x => x.UserName.Equals(userName) && x.IsActive.Value)).FirstOrDefault();

                var lockAccount = (await lockAccountRepository.SelectAsync(x => x.UserId == lockUser.UserId && x.LockTypeId == lockType)).FirstOrDefault();

                if (lockAccount != null && lockUser != null)
                {
                    if (lockAccount.FailedLockDate != null && lockAccount.FailedLockDate.Date == DateTime.UtcNow.Date)
                        lockAccount.FailedLockCount = lockAccount.FailedLockCount + 1;
                    else
                    {
                        lockAccount.FailedLockCount = 1;
                        lockAccount.FailedLockDate = DateTime.UtcNow;
                    }

                    if (lockAccount.FailedLockCount >= _applicationSettings.Value.NumberOfAttempts)
                    {
                        lockUser.IsAccLock = true;
                        lockUser.LockAccountDate = DateTime.Now;
                        lockUser.LockTypeID = lockType;
                        //_logger.Log(LogType.INFO, "UserBusiness", "LockAccount", "Update Lock Account", string.Empty);
                        var resultReturn = await _userRepository.UpdateAsync(lockUser);
                        if (resultReturn.State.Equals(EntityState.Modified))
                        {
                            await _userRepository.UnitOfWork.SaveChangesAsync();

                            var lockAccountDetails = await lockAccountRepository.SelectAsync(x => x.UserId == lockUser.UserId);

                            await lockAccountRepository.RemoveRange(lockAccountDetails);

                            await lockAccountRepository.UnitOfWork.SaveChangesAsync();
                            //_logger.Log(LogType.INFO, "UserBusiness", "LockAccount", "Complete Update Lock Account", string.Empty);

                            returnResult.Success = true;
                            returnResult.Result = ResourceInformation.GetResValue("AccountLocked");
                        }
                    }
                    else
                    {
                        lockAccount.LockTypeId = lockType;

                        var result = await lockAccountRepository.UpdateAsync(lockAccount);
                        if (result.State.Equals(EntityState.Modified))
                        {
                            await lockAccountRepository.UnitOfWork.SaveChangesAsync();
                            returnResult.Result = "Attempt completed : " + lockAccount.FailedLockCount;
                            returnResult.Success = true;
                        }
                    }
                }
                else
                {
                    userAccount.UserId = lockUser.UserId;
                    userAccount.FailedLockCount = 1;
                    userAccount.FailedLockDate = DateTime.UtcNow;
                    userAccount.LockTypeId = lockType;

                    var result = await lockAccountRepository.AddAsync(userAccount);
                    if (result.State.Equals(EntityState.Added))
                    {
                        returnResult.Result = "Attempt completed : 1";
                        returnResult.Success = true;
                        await lockAccountRepository.UnitOfWork.SaveChangesAsync();
                    }
                }

                return returnResult;

            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "LockAccount", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                return returnResult;
            }
        }

        /// <summary>
        /// To reset specific lock if user gives valid credentials
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="lockType"></param>
        /// <returns></returns>
        public async Task<ReturnResult> ResetLock(string userName, int lockType)
        {
            ILockAccountRepository lockAccountRepository = _serviceProvider.GetRequiredService<ILockAccountRepository>();

            ReturnResult returnResult = new ReturnResult();
            try
            {

                int success = await lockAccountRepository.ExcecuteQueryAsync(ProcedureConstants.procResetLockOfUser.ToString() + " '" + userName + "'," + lockType);

                if (success != 0)
                {
                    await lockAccountRepository.UnitOfWork.SaveChangesAsync();
                    returnResult.Success = true;
                }

                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "ResetLock", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                return returnResult;
            }

        }
        /// <summary>
        /// Method to get all locked user under the user based on his role
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public async Task<ReturnResult> GetAllLockedUser(TokenData tokenData)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {

                var userDetails = await GetUserByUserName(tokenData.UserName.ToString());

                if (userDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("NotExists");
                    return returnResult;
                }
                if (userDetails.IsAccLock)
                {
                    returnResult.Success = false;
                    returnResult.Result = (ResourceInformation.GetResValue("AccountLocked"));
                    return returnResult;
                }
                IExecuterStoreProc<LockedUsers> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<LockedUsers>>();


                List<Parameters> param = new List<Parameters>() {
                    new Parameters("p_Roles", tokenData.Role.FirstOrDefault()),
                    new Parameters("p_OrgId", tokenData.OrgId),
                    new Parameters("p_ClientType", userDetails.ClientType),
                    new Parameters("p_UserName", tokenData.UserName)
                };

                var lockedUserData = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllLockedUsers.ToString(), param);

                if (lockedUserData != null)
                {
                    returnResult.Success = true;
                    returnResult.Data = lockedUserData;
                    return returnResult;
                }
                else
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "GetAllLockedUser", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// To unlock multiple users or single user
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="unlockUsers"></param>
        /// <returns></returns>
        public async Task<ReturnResult> UnlockUsers(TokenData tokenData, List<UnlockUsers> unlockUsers)
        {
            ILockAccountRepository lockAccountRepository = _serviceProvider.GetRequiredService<ILockAccountRepository>();

            ReturnResult returnResult = new ReturnResult();
            List<User> user = new List<User>();

            try
            {

                var userDetails = await GetUserByUserName(tokenData.UserName.ToString());
                if (userDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("UnauthorisedUser");
                    return returnResult;
                }
                if (tokenData.Role.Contains(UserRoles.SiteAdmin.ToString()))
                {
                    foreach (var users in unlockUsers)
                    {
                        var unlockUser = await GetUserByUserName(users.userName);

                        var unlockUserDetails = await _userRepository.SelectFirstOrDefaultAsync(x => x.UserName.Equals(users.userName.ToString()) && x.IsActive.Value);

                        if (unlockUserDetails == null)
                        {
                            returnResult.Success = false;
                            returnResult.Result = users.userName.ToString() + " " + ResourceInformation.GetResValue("NotExists");
                            continue;
                        }

                        //if (string.IsNullOrEmpty(users.password))
                        //{
                        //    returnResult.Success = false;
                        //    returnResult.Result = ResourceInformation.GetResValue("PasswordRequired");
                        //    return returnResult;
                        //}

                        //if (!_customPasswordHash.ValidatePassword(users.password, out string errorMsg))
                        //{
                        //    returnResult.Result = errorMsg;
                        //    return returnResult;
                        //}

                        if (unlockUser.OrgId != userDetails.OrgId)
                        {
                            returnResult.Success = false;
                            returnResult.Result = ResourceInformation.GetResValue("OutSideOfOrganization");
                            return returnResult;
                        }
                        if (!unlockUser.AppIdArray.Intersect(userDetails.AppIdArray).Any())
                        {
                            returnResult.Success = false;
                            returnResult.Result = ResourceInformation.GetResValue("DifferentAppid");
                            return returnResult;
                        }
                        if (unlockUser.Roles.Equals(UserRoles.SiteAdmin.ToString()) || unlockUser.Roles.Equals(UserRoles.SuperAdmin.ToString()))
                        {
                            returnResult.Success = false;
                            returnResult.Result = ResourceInformation.GetResValue("UnauthorisedUser");
                            return returnResult;
                        }

                        if (users.IsAccLock == false)
                        {
                            unlockUserDetails.IsAccLock = false;
                            unlockUserDetails.ModifiedBy = userDetails.UserId;
                            unlockUserDetails.ModifiedOn = DateTime.Now;
                            unlockUserDetails.LockAccountDate = null;
                            unlockUserDetails.LockTypeID = 0;
                            unlockUserDetails.UnlockAccountDate = DateTime.UtcNow;
                            //unlockUserDetails.PasswordHash = _customPasswordHash.ScryptHash(users.password);
                            user.Add(unlockUserDetails);
                        }
                    }

                    if (user != null)
                    {
                        await _userRepository.UpdateRange(user);
                        await _userRepository.UnitOfWork.SaveChangesAsync();
                        returnResult.Success = true;
                        returnResult.Result = ResourceInformation.GetResValue("DataUpdateSuccess");
                    }
                    else
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("DataUpdateFailure");
                    }
                }

                if (tokenData.Role.Contains(UserRoles.SuperAdmin.ToString()))
                {
                    foreach (var users in unlockUsers)
                    {
                        var unlockUserDetails = await _userRepository.SelectFirstOrDefaultAsync(x => x.UserName.Equals(users.userName.ToString()) && x.IsActive.Value);

                        if (unlockUserDetails == null)
                        {
                            returnResult.Success = false;
                            returnResult.Result = users.userName.ToString() + " " + ResourceInformation.GetResValue("NotExists");
                            continue;
                        }

                        //if (string.IsNullOrEmpty(users.password))
                        //{
                        //    returnResult.Success = false;
                        //    returnResult.Result = ResourceInformation.GetResValue("PasswordRequired");
                        //    return returnResult;
                        //}

                        //if (!_customPasswordHash.ValidatePassword(users.password, out string errorMsg))
                        //{
                        //    returnResult.Result = errorMsg;
                        //    return returnResult;
                        //}

                        if (users.IsAccLock == false)
                        {
                            unlockUserDetails.IsAccLock = false;
                            unlockUserDetails.ModifiedBy = userDetails.UserId;
                            unlockUserDetails.ModifiedOn = DateTime.Now;
                            unlockUserDetails.LockAccountDate = null;
                            unlockUserDetails.LockTypeID = 0;
                            unlockUserDetails.UnlockAccountDate = DateTime.UtcNow;
                            // unlockUserDetails.PasswordHash = _customPasswordHash.ScryptHash(users.password);
                            user.Add(unlockUserDetails);
                        }
                    }

                    if (user != null)
                    {
                        await _userRepository.UpdateRange(user);
                        await _userRepository.UnitOfWork.SaveChangesAsync();
                        returnResult.Success = true;
                        returnResult.Result = ResourceInformation.GetResValue("DataUpdateSuccess");
                    }
                    else
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("DataUpdateFailure");
                    }

                }
                return returnResult;
            }
            catch (Exception ex)
            {
                returnResult.Success = false;
                _logger.Error("UserBusiness", "UnlockUsers", ex.Message, ex.StackTrace);
                return returnResult;
            }
        }

        /// <summary>
        /// To unlock user wihout changing password
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="unlockUserByUserNames"></param>
        /// <returns></returns>
        public async Task<ReturnResult> UnlockUserByUserName(TokenData tokenData, List<UnlockUserByUserName> unlockUserByUserNames)
        {
            ReturnResult returnResult = new ReturnResult();
            List<User> user = new List<User>();
            try
            {
                var userDetails = await GetUserByUserName(tokenData.UserName.ToString());
                if (userDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("UnauthorisedUser");
                    return returnResult;
                }
                if (tokenData.Role.Contains(UserRoles.SiteAdmin.ToString()))
                {
                    foreach (var users in unlockUserByUserNames)
                    {
                        var unlockUser = await GetUserByUserName(users.userName);

                        var unlockUserDetails = await _userRepository.SelectFirstOrDefaultAsync(x => x.UserName.Equals(users.userName.ToString()) && x.IsActive.Value);

                        if (unlockUserDetails == null)
                        {
                            returnResult.Success = false;
                            returnResult.Result = users.userName.ToString() + " " + ResourceInformation.GetResValue("NotExists");
                            continue;
                        }

                        if (unlockUser.OrgId != userDetails.OrgId)
                        {
                            returnResult.Success = false;
                            returnResult.Result = ResourceInformation.GetResValue("OutSideOfOrganization");
                            return returnResult;
                        }
                        if (!unlockUser.AppIdArray.Intersect(userDetails.AppIdArray).Any())
                        {
                            returnResult.Success = false;
                            returnResult.Result = ResourceInformation.GetResValue("DifferentAppid");
                            return returnResult;
                        }
                        if (unlockUser.Roles.Equals(UserRoles.SiteAdmin.ToString()) || unlockUser.Roles.Equals(UserRoles.SuperAdmin.ToString()))
                        {
                            returnResult.Success = false;
                            returnResult.Result = ResourceInformation.GetResValue("UnauthorisedUser");
                            return returnResult;
                        }

                        if (users.isAccLock == false)
                        {
                            unlockUserDetails.IsAccLock = false;
                            unlockUserDetails.ModifiedBy = userDetails.UserId;
                            unlockUserDetails.ModifiedOn = DateTime.Now;
                            unlockUserDetails.LockAccountDate = null;
                            unlockUserDetails.LockTypeID = 0;
                            unlockUserDetails.UnlockAccountDate = DateTime.UtcNow;
                            user.Add(unlockUserDetails);
                        }
                    }

                    if (user != null)
                    {
                        await _userRepository.UpdateRange(user);
                        await _userRepository.UnitOfWork.SaveChangesAsync();
                        returnResult.Success = true;
                        returnResult.Result = ResourceInformation.GetResValue("DataUpdateSuccess");
                    }
                    else
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("DataUpdateFailure");
                    }
                }

                if (tokenData.Role.Contains(UserRoles.SuperAdmin.ToString()))
                {
                    foreach (var users in unlockUserByUserNames)
                    {
                        var unlockUserDetails = await _userRepository.SelectFirstOrDefaultAsync(x => x.UserName.Equals(users.userName.ToString()) && x.IsActive.Value);

                        if (unlockUserDetails == null)
                        {
                            returnResult.Success = false;
                            returnResult.Result = users.userName.ToString() + " " + ResourceInformation.GetResValue("NotExists");
                            continue;
                        }

                        if (users.isAccLock == false)
                        {
                            unlockUserDetails.IsAccLock = false;
                            unlockUserDetails.ModifiedBy = userDetails.UserId;
                            unlockUserDetails.ModifiedOn = DateTime.Now;
                            unlockUserDetails.LockAccountDate = null;
                            unlockUserDetails.LockTypeID = 0;
                            unlockUserDetails.UnlockAccountDate = DateTime.UtcNow;
                            user.Add(unlockUserDetails);
                        }
                    }

                    if (user != null)
                    {
                        await _userRepository.UpdateRange(user);
                        await _userRepository.UnitOfWork.SaveChangesAsync();
                        returnResult.Success = true;
                        returnResult.Result = ResourceInformation.GetResValue("DataUpdateSuccess");
                    }
                    else
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("DataUpdateFailure");
                    }

                }
                return returnResult;
            }
            catch (Exception ex)
            {
                returnResult.Success = false;
                _logger.Error("UserBusiness", "UnlockUserByUserName", ex.Message, ex.StackTrace);
                return returnResult;
            }
        }
        #endregion

        #region ResetAnyUserPassword

        /// <summary>
        /// This method is used to reset any user's password by Superadmin.
        /// </summary>
        /// <param name="userCredentials">userCredentials object</param>
        /// <returns>Returns success or failure message</returns>
        public async Task<ReturnResult> ResetAnyUserPassword(string userNamefromToken, UserCredentials userCredentials)
        {
            ILockAccountRepository lockAccountRepository = _serviceProvider.GetRequiredService<ILockAccountRepository>();
            IAuthProviderRepository authProviderRepository = _serviceProvider.GetRequiredService<IAuthProviderRepository>();
            IPasswordHistoryBusiness passwordHistoryBusiness = _serviceProvider.GetRequiredService<IPasswordHistoryBusiness>();

            ReturnResult returnResult = new ReturnResult();
            try
            {
                var userDetailsofRequester = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userNamefromToken) && u.IsActive.Value);
                var userRoleDetailsofRequester = await _userRoleMappingRepository.SelectFirstOrDefaultAsync(u => u.UserId.Equals(userDetailsofRequester.UserId) && u.IsActive);
                var roleDetailsofRequester = await _roleRepository.SelectFirstOrDefaultAsync(r => r.RoleId == userRoleDetailsofRequester.RoleId);

                if (userDetailsofRequester == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("RequesterNotFound");
                    return returnResult;
                }

                if (userDetailsofRequester.IsAccLock)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("RequesterAccountLocked");
                    return returnResult;
                }

                if (userCredentials == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideAllDetails");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(userCredentials.UserName))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideUserName");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(userCredentials.NewPassword))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideNewPwd");
                    return returnResult;
                }

                var userDetails = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userCredentials.UserName));

                //Allowed site admin to change passowrd of his organization's user
                if (roleDetailsofRequester.RoleName.ToUpper() == UserRoles.SiteAdmin.ToString().ToUpper() && !userDetails.OrgId.Equals(userDetailsofRequester.OrgId))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("OutSideOfOrganization");
                    return returnResult;
                }

                var providerDetails = await authProviderRepository.SelectFirstOrDefaultAsync(p => p.ProviderID == userDetails.ProviderId);
                var providerName = providerDetails.ProviderName;

                if (providerName != ApplicationLevelConstants.IMSAuthProvider)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("UserNotFound");
                    return returnResult;
                }

                if (userDetails.IsAccLock)
                {
                    userDetails.IsAccLock = false;
                }

                if (!userDetails.IsActive.Value)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InactiveRecord");
                    return returnResult;
                }

                if (!_customPasswordHash.ValidatePassword(userCredentials.NewPassword, out string errorMsg))
                {
                    returnResult.Success = false;
                    returnResult.Result = errorMsg;
                    return returnResult;
                }

                var resultReturn = await passwordHistoryBusiness.ManagePassword(userDetails.UserId.ToString(), userCredentials.NewPassword.ToString(), userDetailsofRequester.UserId.ToString());

                if (resultReturn.Success == false)
                {
                    return await Task.FromResult(resultReturn);
                }

                userDetails.PasswordHash = _customPasswordHash.ScryptHash(userCredentials.NewPassword);
                userDetails.LastPasswordChangeOn = DateTime.Now;
                userDetails.PasswordExpiration = DateTime.Now.AddDays(_passwordExpirationDay);
                userDetails.IsFirstTimeLogin = true;
                userDetails.RequiredSecurityQuestion = true;

                var result = await _userRepository.UpdateAsync(userDetails);
                if (result.State.Equals(EntityState.Modified))
                {
                    await _userRepository.UnitOfWork.SaveChangesAsync();

                    var deleteRecord = await lockAccountRepository.SelectAsync(x => x.UserId == userDetails.UserId);
                    if (deleteRecord != null && deleteRecord.Any())
                    {
                        await lockAccountRepository.RemoveRange(deleteRecord);
                        await lockAccountRepository.UnitOfWork.SaveChangesAsync();
                    }

                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("PasswordUpdated");
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("PasswordUpdateFailure");
                }

                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "ResetAnyUserPassword", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataUpdateFailure");
                return returnResult;
            }
        }

        #endregion

        #region First Time LoginCheck
        /// <summary>
        /// Used to set the IsFirstTimeLogin value
        /// <param name="userName">UserName</param>
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnResult> FirstTimeLogin(UserDetails userDetails)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (userDetails.IsFirstTimeLogin)
                {
                    if (userDetails.ProviderName != ApplicationLevelConstants.IMSAuthProvider)
                    {
                        returnResult.Success = true;
                        userDetails.IsFirstTimeLogin = false;
                        return returnResult;
                    }
                    else
                    {
                        var user = (await _userRepository.SelectAsync(u => u.UserName.Equals(userDetails.UserName) && u.IsActive.Value)).FirstOrDefault();
                        user.IsFirstTimeLogin = false;
                        user.RequiredSecurityQuestion = true;
                        var result = await _userRepository.UpdateAsync(user);
                        if (result.State.Equals(EntityState.Modified))
                        {
                            await _userRepository.UnitOfWork.SaveChangesAsync();
                            returnResult.Success = true;
                        }
                    }
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "FirstTimeLogin", ex.Message, ex.StackTrace);
                returnResult.Result = ExceptionLogger.LogException(ex);
                throw ex;
            }
        }
        #endregion

        #region User Organization Mapping  
        /// <summary>
        /// This method is used get the Role Module Access Details for all roles.
        /// </summary>
        /// <returns>returns multiple UserOrganizationMapping details</returns>
        public async Task<ReturnResult> GetAllUserOrgMapping()
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IUserOrganizationMappingRepository _userOrganizationMappingRepository = _serviceProvider.GetRequiredService<IUserOrganizationMappingRepository>();

                var listOfUserOrganizationMapping = (await _userOrganizationMappingRepository.SelectAsync(rma => !rma.Equals(null))).ToList();
                if (listOfUserOrganizationMapping != null && listOfUserOrganizationMapping.Count() > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = listOfUserOrganizationMapping;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ResourceInformation.GetResValue("UserOrganizationMapping")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "GetAllUserOrgMapping", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used get the UserOrganizationMapping details by UserId.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>returns single UserOrganizationMapping details</returns>
        public async Task<ReturnResult> GetUserOrgMappingByUserId(Guid userId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IUserOrganizationMappingRepository _userOrganizationMappingRepository = _serviceProvider.GetRequiredService<IUserOrganizationMappingRepository>();

                var requiredUserOrganizationMappingInfo = (await _userOrganizationMappingRepository.SelectAsync(rma => rma.UserId.Equals(userId))).ToList();
                if (requiredUserOrganizationMappingInfo != null)
                {
                    returnResult.Success = true;
                    returnResult.Data = requiredUserOrganizationMappingInfo;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")} " +
                                          $"{ResourceInformation.GetResValue("UserOrganizationMapping")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "GetUserOrgMappingByUserId", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }
        /// <summary>
        /// This method is used get the OrganizationApplicationMapping details by username.
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns>All application and organization for which the user has access</returns>
        public async Task<List<UserOrgAppDetails>> GetOrgAppMappingByUserName(string userName)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IExecuterStoreProc<UserOrgAppDetails> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<UserOrgAppDetails>>();

                List<Parameters> param = new List<Parameters>() {
                    new Parameters("userName", userName)
                     };

                var orgAppDataBasedOnUserName = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.proc_GetAllOrgAppDetailsByUserName.ToString(), param);
                if (orgAppDataBasedOnUserName != null && orgAppDataBasedOnUserName.Count() > 0)
                {
                    return orgAppDataBasedOnUserName;
                }
                else
                {
                    return null;
                }
            }

            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "GetOrgAppMappingByUserName", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return null;
            }
        }

        /// <summary>
        /// This method is used to save multiple UserOrganizationMapping details
        /// </summary>
        /// <param name="userName">userName object</param>
        /// <param name="userOrganizationMapping">userOrganizationMapping object</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> SaveUserOrgMapping(string userName, UserOrganizationMappingModel userOrganizationMappingModel)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                returnResult = await VerifyUserOrgMappingData(userName, userOrganizationMappingModel);

                if (returnResult.Success == false)
                {
                    return returnResult;
                }
                List<UserOrganizationMapping> userOrganizationMappingList = (List<UserOrganizationMapping>)returnResult.Data;
                IUserOrganizationMappingRepository userOrganizationMappingRepository = _serviceProvider.GetRequiredService<IUserOrganizationMappingRepository>();

                bool result = await userOrganizationMappingRepository.AddRange(userOrganizationMappingList);

                if (result)
                {
                    await userOrganizationMappingRepository.UnitOfWork.SaveChangesAsync();

                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("DataSavedSuccess");
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DataSavedFailure");
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "SaveUserOrgMapping", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }

        }

        /// <summary>
        /// This method is used to update multiple UserOrganizationMapping details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="userId">userId</param>
        /// <param name="userOrganizationMappingModelList">userOrganizationMappingModelList </param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> UpdateUserOrgMapping(string userName, UserOrganizationMappingModel userOrganizationMappingModel)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IUserOrganizationMappingRepository userOrganizationMappingRepository = _serviceProvider.GetRequiredService<IUserOrganizationMappingRepository>();
                if (userOrganizationMappingModel == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideDetails");
                    return returnResult;
                }

                // Check wether data to be updated is correct
                returnResult = await VerifyUserOrgMappingData(userName, userOrganizationMappingModel);

                if (returnResult.Success == false)
                {
                    return returnResult;
                }

                List<UserOrganizationMapping> updateUserOrganizationMappingList = (await userOrganizationMappingRepository.SelectAsync(rma => rma.UserId == userOrganizationMappingModel.UserId)).ToList();

                // Delete all existing record in database for UserId to be updated 
                if (await userOrganizationMappingRepository.DeleteRange(updateUserOrganizationMappingList) != true)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DataUpdateFailure");
                    return returnResult;
                }

                List<UserOrganizationMapping> userOrganizationMappingList = (List<UserOrganizationMapping>)returnResult.Data;
                bool result = await userOrganizationMappingRepository.AddRange(userOrganizationMappingList);

                if (returnResult.Success != true)
                {
                    returnResult.Success = false;
                    return returnResult;
                }
                await userOrganizationMappingRepository.UnitOfWork.SaveChangesAsync();
                returnResult.Success = true;
                returnResult.Result = ResourceInformation.GetResValue("DataUpdateSuccess");
                return returnResult;

            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "UpdateUserOrgMapping", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to delete specific User's UserOrganizationMapping details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="userId">userId</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> DeleteUserOrgMappingByUserId(Guid userId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IUserOrganizationMappingRepository userOrganizationMappingRepository = _serviceProvider.GetRequiredService<IUserOrganizationMappingRepository>();

                List<UserOrganizationMapping> deleteUserOrganizationMappingList = (await userOrganizationMappingRepository.SelectAsync(uom => uom.UserId == userId)).ToList();

                if (deleteUserOrganizationMappingList == null)
                {
                    returnResult.Result = $"{ResourceInformation.GetResValue("UserOrganizationMapping")} {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                IUserRepository userRepository = _serviceProvider.GetRequiredService<IUserRepository>();

                bool result = await userOrganizationMappingRepository.DeleteRange(deleteUserOrganizationMappingList);
                if (result)
                {
                    await userOrganizationMappingRepository.UnitOfWork.SaveChangesAsync();

                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("DataDeleteSuccess");
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DataDeleteFailure");
                }

                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "DeleteUserOrgMappingByUserId", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to save multiple UserOrganizationMapping details
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="userId">userId</param>
        /// <param name="userOrganizationMappingModelList">userOrganizationMappingModelList </param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> SaveMultiUserOrgMapping(string userName, UserOrganizationMappingModel userOrganizationMappingModel)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IUserOrganizationMappingRepository userOrganizationMappingRepository = _serviceProvider.GetRequiredService<IUserOrganizationMappingRepository>();
                if (userOrganizationMappingModel == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideDetails");
                    return returnResult;
                }

                // Check whether data to be updated is correct
                returnResult = await VerifyMultiUserOrgMappingData(userName, userOrganizationMappingModel);

                if (returnResult.Success == false)
                {
                    return returnResult;
                }

                var deleteUserOrganizationMappingList = (await userOrganizationMappingRepository.SelectAsync(rma => userOrganizationMappingModel.UserIdArray.Contains(rma.UserId))).ToList();

                // Delete all existing records in database for provided user list to be updated
                if (await userOrganizationMappingRepository.DeleteRange(deleteUserOrganizationMappingList) != true)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DataUpdateFailure");
                    return returnResult;
                }

                List<UserOrganizationMapping> userOrganizationMappingList = (List<UserOrganizationMapping>)returnResult.Data;
                bool result = await userOrganizationMappingRepository.AddRange(userOrganizationMappingList);

                if (returnResult.Success != true)
                {
                    returnResult.Success = false;
                    return returnResult;
                }
                await userOrganizationMappingRepository.UnitOfWork.SaveChangesAsync();
                returnResult.Success = true;
                returnResult.Result = ResourceInformation.GetResValue("DataUpdateSuccess");
                return returnResult;

            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "UpdateUserOrgMapping", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// Get user by user name for internal use
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<User> GetUserByUserNameInternally(string username)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var user = await _userRepository.SelectFirstOrDefaultAsync(x => x.UserName.Equals(username) && x.IsActive.Value);
                return user;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "GetUserByUserNameInternally", ex.Message, ex.StackTrace);
                return null;
            }
        }
        #endregion

        #region Admin OTP verification
        /// <summary>
        /// verify account and generate otp during login
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="otpType"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ReturnResult> VerifyAccountLoginAsync(string userName, int otpType, string token, bool IsRefreshTokenFlow = false)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    UserDetails existingUser = await GetUserByUserName(userName);
                    string userEmail = string.Empty;

                    await ResetLock(existingUser.UserName.ToString(), (int)LockType.VerificationLock);
                    userEmail = existingUser.EmailId;
                    //If the organization/user has enabled multi - factor authentication then generate OTP and send it over Email
                    if (existingUser.TwoFactorEnabled && !IsRefreshTokenFlow)
                    {
                        JwtSecurityToken tokenDetails = null;
                        var jwtHandler = new JwtSecurityTokenHandler();
                        tokenDetails = jwtHandler.ReadToken(token) as JwtSecurityToken;

                        int otpLength = _applicationSettings.Value.OTPLength;
                        string UniqueKey = CommonMethods.RandomOTP(otpLength);
                        IMSLogOutToken iMSLogOut = new IMSLogOutToken
                        {
                            LogOutTokenId = new Guid(),
                            LogOutToken = token,
                            LogoutOn = DateTime.Now,
                            TokenValidationPeriod = DateTime.Now,
                            OTP = UniqueKey
                        };

                        IUserOTPRepository _userOTPRepository = _serviceProvider.GetRequiredService<IUserOTPRepository>();

                        var result = await _userOTPRepository.AddAsync(new UserOTP
                        {
                            OTPHash = UniqueKey,
                            UserId = existingUser.UserId,
                            OTPCreationDatetime = DateTime.Now,
                            OTPExpirationDatetime = DateTime.Now.AddMinutes(_applicationSettings.Value.ForgotPasswordSession),
                            OTPTypeId = otpType,
                        });

                        #region Email Sending

                        #region Save data into database
                        // Added data into iMSLogOut time
                        IIMSLogOutRepository _iMSLogOutRepository = _serviceProvider.GetRequiredService<IIMSLogOutRepository>();
                        var resultLogOut = await _iMSLogOutRepository.AddAsync(iMSLogOut);
                        if (resultLogOut.State.Equals(EntityState.Added))
                        {
                            await _iMSLogOutRepository.UnitOfWork.SaveChangesAsync();
                        }
                        else
                        {
                            returnResult.Success = false;
                            returnResult.Result = ResourceInformation.GetResValue("DataSavedFailure");
                        }

                        //Added Data into User OTP table
                        if (result.State.Equals(EntityState.Added))
                        {
                            await _userOTPRepository.UnitOfWork.SaveChangesAsync();
                        }
                        else
                        {
                            returnResult.Success = false;
                            returnResult.Result = ResourceInformation.GetResValue("DataSavedFailure");
                        }
                        #endregion

                        IEmailSender _mailSender = _serviceProvider.GetRequiredService<IEmailSender>();

                        //EmailSender sendEmail = new EmailSender();
                        string emailBody = _mailSender.ComposeOTPMailBody(userName, UniqueKey);
                        string emailSubject = ResourceInformation.GetResValue("PasswordRecovery");

                        var emailFlag = await _mailSender.SendEmail(userEmail, ApplicationLevelConstants.FromEmailID, emailSubject, emailBody);

                        if (emailFlag.Success)
                        {
                            returnResult.Success = true;
                            returnResult.Result = ResourceInformation.GetResValue("EmailSentSuccess");
                            returnResult.Data = new VerifyUserModel()
                            {
                                UserId = existingUser.UserId,
                                TwoFactorEnable = existingUser.TwoFactorEnabled
                            };
                        }
                        else
                        {
                            returnResult.Result = ResourceInformation.GetResValue("EmailNotSent");
                        }

                        #endregion
                    }
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidUserNameAndEmail");
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "VerifyAccountLoginAsync", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("NotVerified")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// verify mobile and generate otp during login
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public async Task<ReturnResult> VerifyMobileLoginAsync(string mobile)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                int otpType = Convert.ToInt32(OTPTypeEnum.MobileLogin);
                if (!string.IsNullOrEmpty(mobile))
                {
                    UserDetails existingUser = await GetUserByMobile(mobile);
                    string userEmail = string.Empty;

                    if (existingUser is null)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("MobileNoNotFound");
                        return returnResult;
                    }

                    await ResetLock(existingUser.UserName.ToString(), (int)LockType.VerificationLock);
                    userEmail = existingUser.EmailId;
                    //If the organization/user has enabled multi - factor authentication then generate OTP and send it over Email
                    if (existingUser.MobileLoginEnabled)
                    {
                        int otpLength = _applicationSettings.Value.OTPLength;
                        string UniqueKey = CommonMethods.RandomOTP(otpLength);

                        IUserOTPRepository _userOTPRepository = _serviceProvider.GetRequiredService<IUserOTPRepository>();

                        var result = await _userOTPRepository.AddAsync(new UserOTP
                        {
                            OTPHash = UniqueKey,
                            UserId = existingUser.UserId,
                            OTPCreationDatetime = DateTime.Now,
                            OTPExpirationDatetime = DateTime.Now.AddMinutes(_applicationSettings.Value.ForgotPasswordSession),
                            OTPTypeId = otpType,
                        });

                        #region Email Sending

                        #region Save data into database

                        //Added Data into User OTP table
                        if (result.State.Equals(EntityState.Added))
                        {
                            await _userOTPRepository.UnitOfWork.SaveChangesAsync();
                        }
                        else
                        {
                            returnResult.Success = false;
                            returnResult.Result = ResourceInformation.GetResValue("DataSavedFailure");
                        }
                        #endregion

                        //IEmailSender _mailSender = _serviceProvider.GetRequiredService<IEmailSender>();

                        ////EmailSender sendEmail = new EmailSender();
                        //string emailBody = _mailSender.ComposeOTPMailBody(existingUser.UserName, UniqueKey);
                        //string emailSubject = ResourceInformation.GetResValue("PasswordRecovery");

                        //var emailFlag = await _mailSender.SendEmail(userEmail, ApplicationLevelConstants.FromEmailID, emailSubject, emailBody);

                        //if (emailFlag.Success)
                        //{
                        returnResult.Success = true;
                        returnResult.Result = ResourceInformation.GetResValue("OTPSentSuccess");
                        returnResult.Data = new VerifyMobileModel()
                        {
                            UserId = existingUser.UserId,
                            MobileLoginEnabled = existingUser.MobileLoginEnabled
                        };
                        //}
                        //else
                        //{
                        //    returnResult.Result = ResourceInformation.GetResValue("EmailNotSent");
                        //}

                        #endregion
                    }
                    else
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("MobileNoNotFound");
                    }
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("MobileNoNotFound");
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "VerifyAccountLoginAsync", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("NotVerified")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// Verify otp during login
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="otp"></param>
        /// <returns></returns>
        public async Task<ReturnResult> VerifyOTPLoginAsync(Guid userId, string otp)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if ((userId != Guid.Empty) && (!string.IsNullOrEmpty(otp)))
                {
                    IUserOTPRepository _userOTPRepository = _serviceProvider.GetRequiredService<IUserOTPRepository>();
                    IIMSLogOutRepository _iMSLogOutRepository = _serviceProvider.GetRequiredService<IIMSLogOutRepository>();

                    var user = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserId.Equals(userId) && u.IsActive.Value);
                    //var user = await _iMSLogOutRepository.SelectFirstOrDefaultAsync(u => u.LogOutTokenId.Equals(userId) && u.IsActive.Value);

                    if (user == null)
                    {
                        returnResult.Success = false;
                        returnResult.Result =
                            $"{ResourceInformation.GetResValue("VerifyOTP")} {ResourceInformation.GetResValue("NotExists")}";
                        return returnResult;
                    }
                    /// Check if account is already locked
                    if (user.IsAccLock)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("AccountLocked");
                        return returnResult;
                    }
                    var otpVerification = (await _userOTPRepository.SelectAsync(o => o.UserId.Equals(userId))).OrderByDescending(o => o.OTPCreationDatetime).FirstOrDefault();
                    var otpLogOut = (await _iMSLogOutRepository.SelectAsync(o => o.OTP.Equals(otp))).OrderByDescending(o => o.TokenValidationPeriod).FirstOrDefault();

                    if (otpVerification == null)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("OTPNotExists");
                    }
                    //Check if otp is same as last otp generated for the same user
                    if (otpVerification.OTPHash != otp)
                    {

                        returnResult = await LockAccount(user.UserName.ToString(), (int)LockType.OTPLock);
                        returnResult.Result = ResourceInformation.GetResValue("OTP_Invalid") + " " + returnResult.Result;
                        return returnResult;
                    }

                    //Check if otp expired expire
                    if (otpVerification.OTPExpirationDatetime < DateTime.Now)
                    {
                        returnResult = await LockAccount(user.UserName.ToString(), (int)LockType.OTPLock);
                        returnResult.Result = ResourceInformation.GetResValue("OTP_Expired") + " " + returnResult.Result;
                        return returnResult;
                    }


                    //Create response 
                    await ResetLock(user.UserName.ToString(), (int)LockType.OTPLock);


                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("OTP_Valid");
                    if (returnResult.Success == true)
                    {
                        await _userOTPRepository.DeleteAsync(otpVerification);
                        await _userOTPRepository.UnitOfWork.SaveChangesAsync();

                        if (otpLogOut != null)
                        {
                            await _iMSLogOutRepository.DeleteAsync(otpLogOut);
                            await _iMSLogOutRepository.UnitOfWork.SaveChangesAsync();
                        }
                    }

                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidInput");
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "VerifyOTPLoginAsync", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                return returnResult;
            }
        }

        /// <summary>
        /// Verify User and Organization details
        /// <param name="userName">userName</param>
        /// <param name="userOrganizationMappingModel">userOrganizationMappingModel</param>
        /// </summary>
        /// <returns>response message</returns>
        public async Task<ReturnResult> VerifyTokenExchange(Guid userId, string orgName)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                // Check whether Organization exist or not
                var org = await _organizationRepository.SelectAsync(o => o.OrgName.Equals(orgName, StringComparison.OrdinalIgnoreCase) && o.IsActive.Value);
                if (!org.Any())
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("Organization")} " + $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                int orgId = org.FirstOrDefault().OrgId;

                // Check whether user organization mapping exist or not
                IUserOrganizationMappingRepository userOrganizationMappingRepository = _serviceProvider.GetRequiredService<IUserOrganizationMappingRepository>();
                if (!(await userOrganizationMappingRepository.SelectAsync(uom => uom.UserId.Equals(userId) && uom.OrgId.Equals(orgId))).Any())
                {
                    returnResult.Result = ResourceInformation.GetResValue("NoPermission");
                    return returnResult;
                }

                returnResult.Success = true;
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "VerifyTokenExchange", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                return returnResult;
            }
        }


        #endregion

        #region Email Verification

        public async Task<ReturnResult> VerifyEmailAsync(Guid userId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (userId != Guid.Empty)
                {
                    var existingUser = await _userRepository.SelectFirstOrDefaultAsync(e => e.UserId == userId);
                    if (existingUser != null)
                    {
                        existingUser.EmailVerified = true;

                        await _userRepository.UpdateAsync(existingUser);
                        await _userRepository.UnitOfWork.SaveChangesAsync();

                        returnResult.Success = true;
                        returnResult.Result = ResourceInformation.GetResValue("EmailVerificationSuccess");
                        returnResult.Data = new
                        {
                            UserId = existingUser.UserId,
                            EmailVerified = true
                        };
                    }
                    else
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("InvalidLink");
                    }
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidLink");
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "VerifyAccountLoginAsync", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("NotVerified")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Verify Forgot Password Flow
        /// <param name="userId">UserId</param>
        /// </summary>
        /// <returns>response message</returns>
        private async Task<ReturnResult> VerifyForgotPasswordFlow(Guid userId)
        {

            ReturnResult returnResult = new ReturnResult { Success = true };

            var existingUser = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserId.Equals(userId) && u.IsActive.Value);
            if (existingUser == null)
            {
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("User")} " + $"{ ResourceInformation.GetResValue("NotExists")}";
                return returnResult;
            }

            IForgotPasswordFlowManagementRepository forgotPasswordFlowManagementRepository = _serviceProvider.GetRequiredService<IForgotPasswordFlowManagementRepository>();

            ForgotPasswordFlowManagement flowManagement = await forgotPasswordFlowManagementRepository.SelectFirstOrDefaultAsync(fpm => fpm.UserId.Equals(userId));

            if (flowManagement == null)
            {
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("AccountVerificationRequired")}";
                return returnResult;
            }

            if (flowManagement.VerifiedEmailOn.AddMinutes(_applicationSettings.Value.ForgotPasswordSession) < DateTime.Now)
            {
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("ForgotPasswordTimeOut");
                return returnResult;
            }

            if (existingUser.TwoFactorEnabled == true)
            {
                if (flowManagement.VerifiedOTP == false)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("OTPVerificationRequired");
                    return returnResult;
                }
            }
            if (flowManagement.VerifiedSecurityQuestions == false)
            {
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("SecurityQuestionsRequired");
                return returnResult;
            }

            return returnResult;

        }

        /// <summary>
        /// Verify User and Organization details
        /// <param name="userName">userName</param>
        /// <param name="userOrganizationMappingModel">userOrganizationMappingModel</param>
        /// </summary>
        /// <returns>response message</returns>
        private async Task<ReturnResult> VerifyUserOrgMappingData(string userName, UserOrganizationMappingModel userOrganizationMappingModel)
        {
            ReturnResult returnResult = new ReturnResult();
            List<UserOrganizationMapping> userOrganizationMappingList = new List<UserOrganizationMapping>();
            try
            {

                UserOrganizationMapping userOrganizationMapping;
                // Check wether User exist or not
                if (!(await _userRepository.SelectAsync(u => u.UserId.Equals(userOrganizationMappingModel.UserId) && u.IsActive.Value)).Any())
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("User")} " + $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
                Guid createdBy = (await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value)).UserId;

                IOrganizationRepository organizationRepository = _serviceProvider.GetRequiredService<IOrganizationRepository>();
                // Check wether Organization exist or not and add
                foreach (int orgId in userOrganizationMappingModel.OrgIdArray)
                {
                    if (!(await organizationRepository.SelectAsync(o => o.OrgId.Equals(orgId))).Any())
                    {
                        returnResult.Success = false;
                        returnResult.Result = $"{ResourceInformation.GetResValue("Organization")} " + $"{ ResourceInformation.GetResValue("NotExists")}";
                        return returnResult;
                    }

                    userOrganizationMapping = new UserOrganizationMapping()
                    {
                        UserId = userOrganizationMappingModel.UserId,
                        OrgId = orgId,
                        CreatedBy = createdBy,
                        CreatedOn = DateTime.Now
                    };

                    userOrganizationMappingList.Add(userOrganizationMapping);
                }
                returnResult.Data = userOrganizationMappingList;
                returnResult.Success = true;
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "VerifyUserOrgMappingData", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// Verify User and Organization details
        /// <param name="userName">userName</param>
        /// <param name="userOrganizationMappingModel">userOrganizationMappingModel</param>
        /// </summary>
        /// <returns>response message</returns>
        private async Task<ReturnResult> VerifyMultiUserOrgMappingData(string userName, UserOrganizationMappingModel userOrganizationMappingModel)
        {
            ReturnResult returnResult = new ReturnResult();
            List<UserOrganizationMapping> userOrganizationMappingList = new List<UserOrganizationMapping>();
            int? mstOrgId = 0;
            int userLimit = 5;
            try
            {
                if (userOrganizationMappingModel.UserIdArray.Count() > userLimit)
                {
                    returnResult.Success = false;
                    returnResult.Result = string.Format(ResourceInformation.GetResValue("UserLimitExceeded"), userLimit);
                    return returnResult;
                }

                var mstOrg = await _organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgName == _applicationSettings.Value.MstOrg);
                if (mstOrg != null) mstOrgId = mstOrg.OrgId;
                var mstOrgUsers = await _userRepository.SelectAsync(x => x.OrgId == mstOrgId && x.IsActive.Value);
                var filteredUsers = userOrganizationMappingModel.UserIdArray.Join(mstOrgUsers, a => a, b => b.UserId, (a, b) => b).ToList();
                var filteredSupportUsers = filteredUsers.Join(await _userRoleMappingRepository.SelectAsync(u => u.RoleId == Convert.ToInt32(EIMSRoles.Support) || u.RoleId == Convert.ToInt32(EIMSRoles.SiteAdmin)),
                                        a => a.UserId,
                                        b => b.UserId,
                                        (a, b) => b).ToList();

                // Check whether User exist or not
                if (filteredSupportUsers.Count() != userOrganizationMappingModel.UserIdArray.Count())
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("User")} " + $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                //Check whether requester is siteadmin and belongs to master organization
                var createdByObject = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
                var createdByRole = await _userRoleMappingRepository.SelectFirstOrDefaultAsync(u => u.UserId.Equals(createdByObject.UserId));
                if (!(createdByObject.OrgId == mstOrgId && (createdByRole.RoleId == Convert.ToInt32(EIMSRoles.SiteAdmin) || createdByRole.RoleId == Convert.ToInt32(EIMSRoles.SuperAdmin))))
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("CanNotAssign")}";
                    return returnResult;
                }

                IOrganizationRepository organizationRepository = _serviceProvider.GetRequiredService<IOrganizationRepository>();
                // Check wether Organization exist or not and add
                foreach (string orgName in userOrganizationMappingModel.OrgNameArray)
                {
                    var org = await organizationRepository.SelectAsync(o => o.OrgName.Equals(orgName, StringComparison.OrdinalIgnoreCase) && o.IsActive.Value);
                    if (!org.Any())
                    {
                        returnResult.Success = false;
                        returnResult.Result = $"{ResourceInformation.GetResValue("Organization")} " + $"{ ResourceInformation.GetResValue("NotExists")}";
                        return returnResult;
                    }

                    var userOrgMappingList = userOrganizationMappingModel.UserIdArray.Select(u => new UserOrganizationMapping()
                    {
                        UserId = u,
                        OrgId = org.FirstOrDefault().OrgId,
                        CreatedBy = createdByObject.UserId,
                        CreatedOn = DateTime.Now
                    });

                    userOrganizationMappingList.AddRange(userOrgMappingList);
                }
                returnResult.Data = userOrganizationMappingList;
                returnResult.Success = true;
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("UserBusiness", "VerifyUserOrgMappingData", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        private async Task<bool> SendUserRegistrationNotification(User user, EmailTemplate emailTemplate, string appUrl)
        {
            bool result = false;
            try
            {
                MailEntity mailEntity = new MailEntity();
                mailEntity.Subject = emailTemplate.EmailSubject;
                mailEntity.Body = emailTemplate.EmailBody.Replace("[$#;AppURL$#;]", appUrl).Replace("[$#;FirstName$#;]", user.Name).Replace("[$#;UserId$#;]", user.UserId.ToString());
                mailEntity.MailFrom = emailTemplate.EmailFrom;
                mailEntity.MailTo = user.EmailId;
                SendEmail sendEmail = new SendEmail(_applicationSettings);
                result = await sendEmail.SendUserRegistrationNotification(mailEntity);
            }
            catch
            {
                return false;
            }
            return result;
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
                _userRepository.Dispose();
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

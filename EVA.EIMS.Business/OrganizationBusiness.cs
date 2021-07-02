using EVA.EIMS.Common;
using EVA.EIMS.Common.Constants;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
    public class OrganizationBusiness : IOrganizationBusiness
    {
        #region Private Variable

        private readonly IOrganizationRepository _organizationRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;
        private readonly string _mstOrg;
        private bool _disposed;
        private Guid userId;

        #endregion

        #region Constructor
        public OrganizationBusiness(IOrganizationRepository organizationRepository, IUserRepository userRepository, IOptions<ApplicationSettings> applicationSettings, ILogger logger, IServiceProvider serviceProvider)
        {
            _organizationRepository = organizationRepository;
            _serviceProvider = serviceProvider;
            _userRepository = userRepository;
            _disposed = false;
            _mstOrg = applicationSettings.Value.MstOrg;
            _logger = logger;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// This method is used to get the multiple organization details
        /// <param name="tokenData">tokenData object</param>
        /// </summary>
        /// <returns>returns multiple organization details</returns>
        public async Task<ReturnResult> Get(TokenData tokenData)
        {
            try
            {
                ReturnResult returnResult = new ReturnResult();

                IExecuterStoreProc<Organization> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<Organization>>();
                List<Parameters> param = new List<Parameters>() {
                    new Parameters("p_Roles", tokenData.Role.FirstOrDefault()),
                    new Parameters("p_OrgId", tokenData.OrgId),
                    new Parameters("p_ClientType", String.Join(",", tokenData.UserClientTypes)),
                    new Parameters("p_UserName", tokenData.UserName) };

                var listOfOrganizations = (await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllOrganization.ToString(), param));

                if (listOfOrganizations != null && listOfOrganizations.Count > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = listOfOrganizations;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("Organization")} {ResourceInformation.GetResValue("NotExists")}";
                }
                return returnResult;
            }

            catch (Exception ex)
            {
                _logger.Error("OrganizationBusiness", "Get", ex.Message, ex.StackTrace);
                return null;

            }
        }
        /// <summary>
        /// This method is used to get the organization details by id
        /// </summary>
        /// <param name="orgName">orgName</param>
        /// <param name="tokenData">tokenData object</param>
        /// <returns>returns single organization details</returns>
        public async Task<ReturnResult> GetByOrgName(TokenData tokenData, string orgName)
        {
            try
            {
                ReturnResult returnResult = new ReturnResult();

                IExecuterStoreProc<Organization> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<Organization>>();
                List<Parameters> param = new List<Parameters>() {
                    new Parameters("p_Roles", tokenData.Role.FirstOrDefault()),
                    new Parameters("p_OrgId", tokenData.OrgId),
                    new Parameters("p_ClientType", String.Join(",", tokenData.UserClientTypes)),
                    new Parameters("p_UserName", tokenData.UserName),
                    new Parameters("p_SearchOrg", orgName) };

                var organizationDetails = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllOrganization, param);

                if (organizationDetails != null && organizationDetails.Count > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = organizationDetails;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("Organization")} {ResourceInformation.GetResValue("NotExists")}";
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("OrganizationBusiness", "GetByOrgName", ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// This method is used to save the organization details
        /// </summary>
        /// <param name="org">organization object</param>
        /// <param name="userName">userName</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Save(string userName, Organization org)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (org == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideOrgDetails");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(org.OrgName))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideOrganizationName");
                    return returnResult;
                }

                var existOgnanization = await _organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgName.Equals(org.OrgName) && !o.IsActive.Value);

                if (existOgnanization != null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ExistsAndInActive");
                    return returnResult;
                }

                var existingOgnanization = await _organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgName.Equals(org.OrgName) && o.IsActive.Value);
                if (existingOgnanization != null)
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("Organization")} {org.OrgName} {ResourceInformation.GetResValue("AlreadyExist")}";
                    return returnResult;
                }

                if (org.IsActive == null)
                {
                    org.IsActive = true;
                }
                if (userName.ToUpper() == KeyConstant.Client.ToUpper()) userName = KeyConstant.Superadmin;
                var userDetails = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
                if (userDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("UserDetailsNotFound");
                    return returnResult;
                }
                userId = userDetails.UserId;
                org.CreatedBy = userId;
                org.ModifiedBy = userId;

                var result = await _organizationRepository.AddAsync(org);
                if (result.State.Equals(EntityState.Added))
                {
                    await _organizationRepository.UnitOfWork.SaveChangesAsync();
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
                _logger.Error("OrganizationBusiness", "Save", ex.Message, ex.StackTrace);
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                returnResult.Success = false;
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used update the organization details
        /// </summary>
        /// <param name="orgName">orgName</param>
        /// <param name="org">organization object</param>
        /// <param name="tokenData">tokenData object</param>
        /// <returns>returns response message</returns>
        /// <summary>
        public async Task<ReturnResult> Update(string userName, int orgId, Organization org)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (org == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideOrgDetails");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(org.OrgName))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideOrgName");
                    return returnResult;
                }

                if (org.OrgId == 0)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideOrgId");
                    return returnResult;
                }

                var updateOrg = await _organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgId == orgId);

                if (updateOrg == null)
                {
                    returnResult.Success = false;
                    returnResult.Result =
                        $"{ResourceInformation.GetResValue("Organization")} {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
                if (org.IsActive == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("IsActiveRequired");
                    return returnResult;
                }
                var userDetails = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
                if (userDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("UserDetailsNotFound");
                    return returnResult;
                }

                userId = userDetails.UserId;

                updateOrg.OrgName = org.OrgName;
                updateOrg.Description = org.Description;
                updateOrg.ModifiedBy = userId;
                updateOrg.IsActive = org.IsActive;

                var result = await _organizationRepository.UpdateAsync(updateOrg);
                if (result.State.Equals(EntityState.Modified))
                {
                    await _organizationRepository.UnitOfWork.SaveChangesAsync();
                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("DataUpdateSuccess");
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("DataUpdateFailure");
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("OrganizationBusiness", "Update", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result =
                    $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to delete the organization details
        /// </summary>
        /// <param name="orgName">orgName</param>
        /// <param name="userName">userName</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> Delete(string userName, string orgName)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var userDetails = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
                userId = userDetails.UserId;
                if (string.IsNullOrEmpty(orgName))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideOrgName");
                    return returnResult;
                }

                if (_mstOrg.Equals(orgName))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("MstOrganizationCannotDelete");
                    return returnResult;
                }
                var deleteOrg = await _organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgName.Equals(orgName, StringComparison.OrdinalIgnoreCase) && o.IsActive.Value);

                if (deleteOrg == null)
                {
                    returnResult.Success = false;
                    returnResult.Result =
                        $"{ResourceInformation.GetResValue("Organization")} {orgName} {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
                var orgDetails = await _organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgName.Equals(orgName));
                int orgId = orgDetails.OrgId;

                var existingUsers = await _userRepository.SelectAsync(u => u.OrgId.Equals(orgId) && u.IsActive.Value);
                if (existingUsers != null && existingUsers.Count() != 0)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("AssociatedOrg");
                    return returnResult;
                }
                IDeviceRepository deviceRepository = _serviceProvider.GetRequiredService<IDeviceRepository>();
                var existingDevices = await deviceRepository.SelectAsync(d => d.OrgId.Equals(orgId) & d.IsActive);
                if (existingDevices != null && existingUsers.Count() != 0)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("AssociatedOrg");
                    return returnResult;
                }

                deleteOrg.IsActive = false;
                deleteOrg.ModifiedBy = userId;
                var result = await _organizationRepository.UpdateAsync(deleteOrg);

                if (result.State.Equals(EntityState.Modified))
                {
                    await _organizationRepository.UnitOfWork.SaveChangesAsync();
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
                _logger.Error("OrganizationBusiness", "Delete", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result =
                    $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to delete all users & devices by organization Id    
        /// </summary>
        /// <param name="orgName">orgName</param>
        /// <param name="userName">userName</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> DeleteAllUsersAndDevicesById(string userName, string orgName)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var userDetails = await _userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
                userId = userDetails.UserId;
                if (string.IsNullOrEmpty(orgName))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideOrgId");
                    return returnResult;
                }

                if (_mstOrg.Equals(orgName))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("MstOrganizationCannotDelete");
                    return returnResult;

                }
                var organizationDetails = await _organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgName.Equals(orgName) && o.IsActive.Value);
                if (organizationDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("Organization")} {orgName} { ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
                int orgId = organizationDetails.OrgId;

                var users = await _userRepository.SelectAsync(u => u.OrgId == orgId && u.IsActive.Value);
                users.ToList().ForEach(a => { a.IsActive = false; a.ModifiedBy = userId; });
                IDeviceRepository deviceRepository = _serviceProvider.GetRequiredService<IDeviceRepository>();
                var devices = await deviceRepository.SelectAsync(u => u.OrgId == orgId && u.IsActive);
                devices.ToList().ForEach(a => { a.IsActive = false; a.ModifiedBy = userId; });

                await _userRepository.UpdateRange(users);
                await deviceRepository.UpdateRange(devices);
                await _organizationRepository.UnitOfWork.SaveChangesAsync();
                returnResult.Success = true;
                returnResult.Result = ResourceInformation.GetResValue("DataDeleteSuccess");
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("OrganizationBusiness", "DeleteAllUsersAndDevicesById", ex.Message, ex.StackTrace);
                returnResult.Result =
                $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                returnResult.Success = false;
                return returnResult;
            }
        }

        /// <summary>
        /// To get organization by organizationid
        /// </summary>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        public async Task<Organization> GetOrganizationByOrgId(int? orgId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                return await _organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgId == orgId && o.IsActive.Value);
            }
            catch (Exception ex)
            {
                _logger.Error("OrganizationBusiness", "GetOrganizationByOrgId", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ExceptionLogger.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// to get organization by organization name
        /// </summary>
        /// <param name="OrgName"></param>
        /// <returns></returns>
        public async Task<Organization> GetOrganizationIdByOrgName(string OrgName)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                return await _organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgName.Equals(OrgName) && o.IsActive.Value);
            }
            catch (Exception ex)
            {
                _logger.Error("OrganizationBusiness", "GetOrganizationIdByOrgName", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ExceptionLogger.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// To get all organization by client id
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<List<Organization>> GetAllOrganizationByClientId(string clientId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {

                IExecuterStoreProc<Organization> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<Organization>>();
                List<Parameters> param = new List<Parameters>() { new Parameters("p_clientid", clientId) };

                var listOfOrganizations = (await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllOrganizationByClientId.ToString(), param));

                if (listOfOrganizations != null && listOfOrganizations.Count() > 0)
                {
                    return listOfOrganizations;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("OrganizationBusiness", "GetAllOrganizationByClientId", ex.Message, ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// To get all active organization by client id
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<List<Organization>> GetAllActiveOrganizationByClientId(string clientId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {

                IExecuterStoreProc<Organization> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<Organization>>();
                List<Parameters> param = new List<Parameters>() { new Parameters("p_clientid", clientId) };

                var listOfOrganizations = (await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllActiveOrganizationByClientId.ToString(), param));

                if (listOfOrganizations != null && listOfOrganizations.Count() > 0)
                {
                    return listOfOrganizations;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("OrganizationBusiness", "GetAllActiveOrganizationByClientId", ex.Message, ex.StackTrace);
                return null;
            }
        }

        #endregion
        /// <summary>
        /// This method returns all organization with it associated application list
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns>list of oranization</returns>
        public async Task<ReturnResult> GetOrganizatioApplicationDetails(TokenData tokenData)
        {
            try
            {
                ReturnResult returnResult = new ReturnResult();

                IExecuterStoreProc<OrganizationApplicationDetails> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<OrganizationApplicationDetails>>();

                var organizationAppDetails = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetOrganizationApplicationDetails);

                if (organizationAppDetails != null && organizationAppDetails.Count > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = organizationAppDetails;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("Organization")} {ResourceInformation.GetResValue("NotExists")}";
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("OrganizationBusiness", "GetOrganizatioApplicationDetails", ex.Message, ex.StackTrace);
                throw ex;
            }
        }


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
                _organizationRepository.Dispose();
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

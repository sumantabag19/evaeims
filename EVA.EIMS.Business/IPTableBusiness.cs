using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Repository;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using EVA.EIMS.Entity.ComplexEntities;
using EVA.EIMS.Contract.Infrastructure;
using System.Threading.Tasks;
using EVA.EIMS.Logging;

namespace EVA.EIMS.Business
{
    public class IPTableBusiness : IIPTableBusiness
    {
        #region Private variables
        private IIPTableRepository _iPTableRepository;
        private IServiceProvider _serviceProvider;
        private IPAddress iP;
        private bool _disposed;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public IPTableBusiness(IIPTableRepository iPTableRepository, IServiceProvider serviceProvider, ILogger logger)
        {
            _iPTableRepository = iPTableRepository;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _disposed = false;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// This method is used to get the multiple IP details
        /// </summary>
        /// <returns>returns  multiple IP details</returns>
        public async Task <ReturnResult> Get()
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var listOfIPDetails = await _iPTableRepository.SelectAsync(a => a.IsActive.Value);
                if (listOfIPDetails != null && listOfIPDetails.Count() > 0)
                {
                    returnResult.Success = true;
                    returnResult.Data = listOfIPDetails;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ResourceInformation.GetResValue("Application")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("IPTableBusiness", "Get", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used get the IP details by id
        /// </summary>
        /// <param name="ipAddressId">ipAddressId</param>
        /// <returns>returns single IP details</returns>
        public async Task<ReturnResult> GetById(int ipAddressId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                var requiredIpTableInfo = await _iPTableRepository.SelectFirstOrDefaultAsync(a => a.IPAddressId == ipAddressId && a.IsActive.Value);
                if (requiredIpTableInfo != null)
                {
                    returnResult.Success = true;
                    returnResult.Data = requiredIpTableInfo;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")} " +
                                          $"{ResourceInformation.GetResValue("IPData")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("IPTableBusiness", "GetById", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used get the IP details by orgId and AppId
        /// </summary>
        /// <param name="orgId">orgId</param>
        /// <param name="appId">appId</param>
        /// <returns>returns single IP details</returns>
        public async Task<IPTable> GetByOrgIdAndAppId(int orgId, int appId)
        {
            try
            {
                var requiredIpTableInfo = await _iPTableRepository.SelectFirstOrDefaultAsync(a => a.OrgId == orgId && a.AppId == appId && a.IsActive.Value);
                if (requiredIpTableInfo != null)
                {
                    return requiredIpTableInfo;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("IPTableBusiness", "GetByOrgIdAndAppId", ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// This method is used get the IP details by orgId and multiple AppId 
        /// </summary>
        /// <param name="orgId">orgId</param>
        /// <param name="appId">appId</param>
        /// <returns>returns matching IP Table details</returns>
        public async Task<IEnumerable<IPAddressRange>> GetByOrgIdAndAppIdArray(int orgId, string appId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IExecuterStoreProc<IPAddressRange> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<IPAddressRange>>();
                List<Parameters> param = new List<Parameters>
                    {
                        new Parameters("AppId", appId),
                        new Parameters("OrgId", orgId)
                    };

                var listOfAuthorizedIPs = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllAuthorizedIPByAppId.ToString(), param);
                if (listOfAuthorizedIPs != null && listOfAuthorizedIPs.Count() > 0)
                {
                    return listOfAuthorizedIPs;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("IPTableBusiness", "GetByOrgIdAndAppIdArray", ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// This method is used to save the IP details
        /// </summary>
        /// <param name="iPTable">iPTable</param>
        /// <param name="userName">userName</param>
        /// <returns>returns response  message</returns>
        public async Task<ReturnResult> Save(string userName, IPTable iPTable)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (iPTable == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidData");
                    return returnResult;
                }
                IApplicationRepository applicationRepository = _serviceProvider.GetRequiredService<IApplicationRepository>();
                var existingApp = await applicationRepository.SelectFirstOrDefaultAsync(a => a.AppId == iPTable.AppId && a.IsActive.Value);
                if (iPTable.AppId == 0 || existingApp == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideProperAppId");
                    return returnResult;
                }
                IOrganizationRepository organizationRepository = _serviceProvider.GetRequiredService<IOrganizationRepository>();
                var existingOrg = await organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgId == iPTable.OrgId && o.IsActive.Value);

                if (iPTable.OrgId == 0 || existingOrg == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideProperOrgId");
                    return returnResult;
                }
                if (string.IsNullOrEmpty(iPTable.IPStartAddress) || string.IsNullOrEmpty(iPTable.IPEndAddress))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidIPAddress");
                    return returnResult;
                }
                var existingIpDetails = await _iPTableRepository.SelectAsync(i => i.IsActive.Value);
                foreach (var item in existingIpDetails)
                {
                    if (iPTable.IPStartAddress == item.IPStartAddress &&
                        iPTable.IPEndAddress == item.IPEndAddress &&
                        iPTable.AppId == item.AppId &&
                        iPTable.OrgId == item.OrgId)
                    {

                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("AlreadyExist");
                        return returnResult;
                    }
                }


                //Check if the string is a valid IPAddress
                bool validateIpStartAddress = IPAddress.TryParse(iPTable.IPStartAddress, out iP);
                bool validateIpEndAddress = IPAddress.TryParse(iPTable.IPEndAddress, out iP);

                if (!validateIpStartAddress || !validateIpEndAddress)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidIPAddress");
                    return returnResult;
                }

                //Converting the input strings to IPAddress instances of byte array to check validity.
                var ipStart = IPAddress.Parse(iPTable.IPStartAddress).GetAddressBytes();
                var ipEnd = IPAddress.Parse(iPTable.IPEndAddress).GetAddressBytes();

                //Converting the byte array to unsigned integer and checking for each byte IPV4 range.
                uint ip1 = (uint)(ipStart[0] << 24 | ipStart[1] << 16 | ipStart[2] << 8 | ipStart[3]);
                uint ip2 = (uint)(ipEnd[0] << 24 | ipEnd[1] << 16 | ipEnd[2] << 8 | ipEnd[3]);

                //check for IPStartAddress cannot be bigger than IPEndAddress range.
                if (ip1 > ip2)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidIPRange");
                    return returnResult;
                }

                //Check if the string is a valid IPAddress
                if (!(IPAddress.TryParse(iPTable.IPProxyAddress, out iP)))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidProxyAddress");
                    return returnResult;
                }

                IUserRepository userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                var userDetails = await userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
                if (userDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("UserDetailsNotFound");
                    return returnResult;
                }
                Guid userId = userDetails.UserId ;
                iPTable.CreatedBy = userId;
                iPTable.ModifiedBy = userId;

                if (iPTable.IsIPAllowed == null)
                {
                    iPTable.IsIPAllowed = true;
                }
                if (iPTable.IsActive == null)
                {
                    iPTable.IsActive = true;
                }

                var result = await _iPTableRepository.AddAsync(iPTable);
                if (result.State.Equals(EntityState.Added))
                {
                    await _iPTableRepository.UnitOfWork.SaveChangesAsync();
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
                _logger.Error("IPTableBusiness", "Save", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataSavedFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to update the IP details
        /// </summary>
        /// <param name="iPTable">iPTable</param>
        /// <param name="userName">userName</param>
        /// <returns>returns response  message</returns>
        public async Task <ReturnResult> Update(string userName, IPTable iPTable)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (iPTable == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidData");
                    return returnResult;
                }
                if (iPTable.IPAddressId == 0)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideProperIPAddressId");
                    return returnResult;
                }

                var existingIpTableDetails = await _iPTableRepository.SelectFirstOrDefaultAsync(i => i.IPAddressId == iPTable.IPAddressId);
                if (existingIpTableDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ResourceInformation.GetResValue("IPData")}" +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }

                IApplicationRepository applicationRepository = _serviceProvider.GetRequiredService<IApplicationRepository>();
                var existingApp = await applicationRepository.SelectFirstOrDefaultAsync(a => a.AppId == iPTable.AppId && a.IsActive.Value);
                if (iPTable.AppId == 0 || existingApp == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideProperAppId");
                    return returnResult;
                }
                IOrganizationRepository organizationRepository = _serviceProvider.GetRequiredService<IOrganizationRepository>();
                var existingOrg = await organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgId == iPTable.OrgId && o.IsActive.Value);

                if (iPTable.OrgId == 0 || existingOrg == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideProperOrgId");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(iPTable.IPStartAddress) || string.IsNullOrEmpty(iPTable.IPEndAddress))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidIPAddress");
                    return returnResult;
                }

                //Check if the string is a valid IPAddress
                bool validateIpStartAddress = IPAddress.TryParse(iPTable.IPStartAddress, out iP);
                bool validateIpEndAddress = IPAddress.TryParse(iPTable.IPEndAddress, out iP);

                if (!validateIpStartAddress || !validateIpEndAddress)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidIPAddress");
                    return returnResult;
                }

                //Converting the input strings to IPAddress instances of byte array to check validity.
                var ipStart = IPAddress.Parse(iPTable.IPStartAddress).GetAddressBytes();
                var ipEnd = IPAddress.Parse(iPTable.IPEndAddress).GetAddressBytes();

                //Converting the byte array to unsigned integer and checking for each byte IPV4 range.
                uint ip1 = (uint)(ipStart[0] << 24 | ipStart[1] << 16 | ipStart[2] << 8 | ipStart[3]);
                uint ip2 = (uint)(ipEnd[0] << 24 | ipEnd[1] << 16 | ipEnd[2] << 8 | ipEnd[3]);

                //check for IPStartAddress cannot be bigger than IPEndAddress range.
                if (ip1 > ip2)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidIPRange");
                    return returnResult;
                }

                //Check if the string is a valid IPAddress
                if (!(IPAddress.TryParse(iPTable.IPProxyAddress, out iP)))
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidProxyAddress");
                    return returnResult;
                }

                IUserRepository userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                var userDetails = await userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
                if (userDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("UserDetailsNotFound");
                    return returnResult;
                }
                Guid userId = userDetails.UserId;
                iPTable.ModifiedBy = userId;
                if (iPTable.IsActive == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("IsActiveRequired");
                    return returnResult;
                }

                if (iPTable.IsIPAllowed == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("IsIPAllowRequired");
                    return returnResult;
                }

                existingIpTableDetails.AppId = iPTable.AppId;
                existingIpTableDetails.OrgId = iPTable.OrgId;
                existingIpTableDetails.GatewayDeviceId = iPTable.GatewayDeviceId;
                existingIpTableDetails.IPStartAddress = iPTable.IPStartAddress;
                existingIpTableDetails.IPEndAddress = iPTable.IPEndAddress;
                existingIpTableDetails.IPProxyAddress = iPTable.IPProxyAddress;
                existingIpTableDetails.IsIPAllowed = iPTable.IsIPAllowed.Value;
                existingIpTableDetails.IsProxyEnabled = iPTable.IsProxyEnabled;
                existingIpTableDetails.PortNo = iPTable.PortNo;
                existingIpTableDetails.IsActive = iPTable.IsActive.Value;

                var result = await _iPTableRepository.UpdateAsync(existingIpTableDetails);
                if (result.State.Equals(EntityState.Modified))
                {
                    await _iPTableRepository.UnitOfWork.SaveChangesAsync();
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
                _logger.Error("IPTableBusiness", "Update", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataUpdateFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to delete the IP details
        /// </summary>
        /// <param name="iPTable">iPTable</param>
        /// <param name="userName">userName</param>
        /// <returns>returns response  message</returns>
        public async Task<ReturnResult> Delete(string userName, int ipAddressId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (ipAddressId == 0)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideProperIPAddressId");
                    return returnResult;
                }

                var existingIpTableDetails = await _iPTableRepository.SelectFirstOrDefaultAsync(i => i.IPAddressId == ipAddressId && i.IsActive.Value);

                IUserRepository userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                var userDetails = await userRepository.SelectFirstOrDefaultAsync(u => u.UserName.Equals(userName) && u.IsActive.Value);
                if (userDetails == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("UserDetailsNotFound");
                    return returnResult;
                }
                Guid userId = userDetails.UserId;

                existingIpTableDetails.ModifiedBy = userId;
                existingIpTableDetails.IsActive = false;
                var result = await _iPTableRepository.UpdateAsync(existingIpTableDetails);

                if (result.State.Equals(EntityState.Modified))
                {
                    await _iPTableRepository.UnitOfWork.SaveChangesAsync();
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
                _logger.Error("IPTableBusiness", "Delete", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataDeleteFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// To check whether the remote ip address is authorized
        /// </summary>
        /// <param name="ipAddress">ipAddress</param>
        /// <param name="orgId">orgId</param>
        /// <param name="appId">appId</param>
        /// <returns>true or false</returns>
        public async Task<ReturnResult> IsIPAuthorized(IPAddress ipAddress, int orgId, int appId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                bool isAllowed = false;

                var allowedIp = await GetByOrgIdAndAppId(orgId, appId);
                
                if (allowedIp != null)
                {
                    //convert the IPaddress from string to an instance of IPAddress and check if it falls in the range of authorized IPs
                    isAllowed = await IsInRange(IPAddress.Parse(allowedIp.IPStartAddress), IPAddress.Parse(allowedIp.IPEndAddress), ipAddress);
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("NoMappinfOfAppWithOrg");
                    return returnResult;
                }

                returnResult.Success = isAllowed;

                _logger.Info("IPTableBusiness", "IsIPAuthorized", $"RequestFrom ={ ipAddress } | IsAllowd = { isAllowed }", "Access time :" + DateTime.Now);
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("IPTableBusiness", "IsIPAuthorized", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                return returnResult;
            }
        }

        /// <summary>
        /// To check whether the remote ip address is authorized for ServiceClient with Password GrantType
        /// </summary>
        /// <param name="ipAddress">ipAddress</param>
        /// <param name="orgId">orgId</param>
        /// <param name="appId">appId</param>
        /// <returns>true or false</returns>
        public async Task<ReturnResult> IPAuthorizedForServiceClient(IPAddress ipAddress, int orgId, string appId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                bool isAllowed = false;
                var allowedIp = await GetByOrgIdAndAppIdArray(orgId, appId);
                if (allowedIp != null && allowedIp.Count() > 0)
                {
                    foreach (var ip in allowedIp)
                    {
                        //convert the IPaddress from string to an instance of IPAddress and check if it falls in the range of authorized IPs
                        isAllowed = await IsInRange(IPAddress.Parse(ip.IPStartAddress), IPAddress.Parse(ip.IPEndAddress), ipAddress);
                        if (isAllowed == true)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("Notexists");
                    return returnResult;
                }

                returnResult.Success = isAllowed;
                _logger.Info("IPTableBusiness", "IPAuthorizedForServiceClient", $"RequestFrom ={ ipAddress } | IsAllowd = { isAllowed }", "Access time :" + DateTime.Now);
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("IPTableBusiness", "IPAuthorizedForServiceClient", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                return returnResult;
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// To check whether the remote ip address is in the range of authorized addresses.
        /// </summary>
        /// <param name="lowerInclusive"></param>
        /// <param name="upperInclusive"></param>
        /// <param name="address"></param>
        /// <returns>true or false</returns>
        private Task<bool> IsInRange(IPAddress lowerInclusive, IPAddress upperInclusive, IPAddress address)
        {
            try
            {
                var addressFamily = lowerInclusive.AddressFamily;
                var lowerBytes = lowerInclusive.GetAddressBytes();
                var upperBytes = upperInclusive.GetAddressBytes();
                
                byte[] addressBytes = address.GetAddressBytes();

                bool lowerBoundary = true, upperBoundary = true;

                //check whether the remote ip falls within the range of authorized IP addresses
                for (int i = 0; i < lowerBytes.Length &&
                    (lowerBoundary || upperBoundary); i++)
                {
                    //check if the IP address falls before the IpStartAddress or after the IpEndAddress
                    if ((lowerBoundary && addressBytes[i] < lowerBytes[i]) ||
                        (upperBoundary && addressBytes[i] > upperBytes[i]))
                    {
                        _logger.Info("IPTableBusiness", "IsInRange", $"RequestFrom ={ upperBoundary } | IsAllowd = { lowerBoundary }", "Access time :" + DateTime.Now);
                        return Task.FromResult<bool>(false);
                    }
                    //set Boundaries to true only if the addressBytes of IP address are equal to the range at every iteration.
                    lowerBoundary &= (addressBytes[i] == lowerBytes[i]);
                    upperBoundary &= (addressBytes[i] == upperBytes[i]);
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.Error("IPTableBusiness", "IsInRange", ex.Message, ex.StackTrace);
                return Task.FromResult(false);
            }
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
                _iPTableRepository.Dispose();
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

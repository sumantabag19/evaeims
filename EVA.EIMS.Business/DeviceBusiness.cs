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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Business
{
    public class DeviceBusiness : IDeviceBusiness
    {
        #region Private Variable
        private IDeviceRepository _deviceRepository;
        private IOrganizationRepository _organizationRepository;
        private bool _disposed;
        private ICustomPasswordHash _customPasswordHash;
        private IApplicationRepository _applicationRepository;
        private readonly string _mstOrg;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserRepository _userRepository;
        private readonly IOptions<ApplicationSettings> _applicationSettings;
        private readonly ILogger _logger;
        private readonly IUserBusiness _userBusiness;
        #endregion

        #region Constructor
        public DeviceBusiness(IDeviceRepository deviceRepository, IOptions<ApplicationSettings> applicationSettings, IOrganizationRepository organizationRepository, ICustomPasswordHash customPasswordHash, IApplicationRepository applicationRepository, IServiceProvider serviceProvider, IUserRepository userRepository, IUserBusiness userBusiness, ILogger logger)
        {
            _deviceRepository = deviceRepository;
            _organizationRepository = organizationRepository;
            _disposed = false;
            _mstOrg = applicationSettings.Value.MstOrg;
            _customPasswordHash = customPasswordHash;
            _applicationRepository = applicationRepository;
            _serviceProvider = serviceProvider;
            _userBusiness = userBusiness;
            _userRepository = userRepository;
            _applicationSettings = applicationSettings;
            _logger = logger;
        }
        #endregion

        #region Public Method
        /// <summary>
        /// This method is used to get the device details by device id
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="deviceId"></param>
        /// <returns>returns single device details</returns>
        public async Task<ReturnResult> GetDevice(TokenData tokenData, string deviceId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                IExecuterStoreProc<DeviceModel> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<DeviceModel>>();
                List<Parameters> param = new List<Parameters>() {
                    new Parameters("p_Roles", tokenData.Role.FirstOrDefault()),
                    new Parameters("p_OrgName", tokenData.OrgId),
                    new Parameters("p_UserName", tokenData.UserName),
                    new Parameters("p_SearchDevice", deviceId==null?string.Empty:deviceId)};

                var deviceData = await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.proc_GetDeviceBasedOnRole, param);


                //var deviceData = await _deviceRepository.SelectAsync(d => (string.IsNullOrEmpty(deviceId) ? d.DeviceId != string.Empty : d.DeviceId.Equals(deviceId)) && d.IsActive);
                if (deviceData != null && deviceData.Count() > 0)
                {
                    var listOfDevices = deviceData.Select(x => new DeviceModel
                    {
                        OrgId = x.OrgId,
                        Subject = x.Subject,
                        DeviceId = x.DeviceId,
                        ClientTypeId = x.ClientTypeId,
                        PrimaryKey = _customPasswordHash.Decrypt(x.PrimaryKey, x.Subject.ToString()),
                        SerialKey = x.SerialKey,
                        IsUsed = x.IsUsed,
                        GatewayDeviceId = x.GatewayDeviceId,
                        AppId = x.AppId,
                        IsActive = x.IsActive,
                        OrgName = x.OrgName,
                        AppName = x.AppName

                    });

                    returnResult.Success = true;
                    returnResult.Data = listOfDevices;
                    return returnResult;
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = $"{ResourceInformation.GetResValue("RequestedDetails")}" +
                                          $"{ResourceInformation.GetResValue("Device")} " +
                                          $"{ ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("DeviceBusiness", "GetDevice", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to save the device details
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="device">device object</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> SaveDevice(TokenData tokenData, DeviceModel deviceModel)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                Device device = new Device();

                if (deviceModel == null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideDeviceDetails");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(deviceModel.DeviceId))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideDeviceId");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(deviceModel.OrgName))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideOrgId");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(deviceModel.OrgName))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideAppId");
                    return returnResult;
                }

                device.DeviceId = deviceModel.DeviceId;
                var organization = await _organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgName.Equals(deviceModel.OrgName, StringComparison.OrdinalIgnoreCase) && o.IsActive.Value);

                if (organization == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidOrgId");
                    return returnResult;
                }

                device.OrgId = organization.OrgId;
                var createdBy = await _userBusiness.GetUserByUserName(tokenData.UserName.ToString());

                device.ModifiedBy = createdBy.UserId;
                device.CreatedBy = createdBy.UserId;


                var application = await _applicationRepository.SelectFirstOrDefaultAsync(a => a.AppName.Equals(deviceModel.AppName, StringComparison.OrdinalIgnoreCase) && a.IsActive.Value);
                if (application == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("ProvideValidAppId");
                    return returnResult;
                }

                device.AppId = application.AppId;
                device.GatewayDeviceId = deviceModel.GatewayDeviceId;
                device.IsUsed = deviceModel.IsUsed;

                if (deviceModel.IsActive == null)
                {
                    device.IsActive = true;
                }
                else
                {
                    device.IsActive = deviceModel.IsActive.Value;
                }

                device.Subject = Guid.NewGuid();
                device.ClientTypeId = (int)ClientTypes.DeviceClient;

                device.PrimaryKey = _customPasswordHash.GenerateRandomPasswordForDeviceUser(device.Subject.ToString());
                device.SerialKey = Guid.NewGuid();

                //if (!tokenData.OrgId.Equals(_mstOrg))
                //{
                //    if (!tokenData.OrgId.Equals(Convert.ToString(deviceModel.OrgId)))
                //    {
                //        returnResult.Result = ResourceInformation.GetResValue("MismatchOrganization");
                //        return returnResult;
                //    }
                //}

                if (await _deviceRepository.SelectFirstOrDefaultAsync(d => d.DeviceId.Equals(device.DeviceId) && !d.IsActive) != null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ExistsAndInActive");
                    return returnResult;
                }

                if (await _deviceRepository.SelectFirstOrDefaultAsync(d => d.DeviceId.Equals(device.DeviceId) && d.IsActive) != null)
                {
                    returnResult.Result =
                        $"Device {ResourceInformation.GetResValue("AlreadyExist")}";
                    return returnResult;
                }
                if (await _organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgName.Equals(tokenData.OrgId) && o.IsActive.Value) == null)
                {
                    returnResult.Result =
                        $"{ResourceInformation.GetResValue("Organization")} {ResourceInformation.GetResValue("NotExists")}";
                    return returnResult;
                }
                var result = await _deviceRepository.AddAsync(device);
                if (result.State.Equals(EntityState.Added))
                {
                    await _deviceRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("DeviceBusiness", "SaveDevice", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result =
                    $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }


        /// <summary>
        /// This method is used to update the device details
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="deviceId"></param>
        /// <param name="device"></param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> UpdateDevice(TokenData tokenData, string deviceId, DeviceModel deviceModel)
        {
            ReturnResult returnResult = new ReturnResult();

            try
            {
                if (deviceModel == null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideDeviceDetails");
                    return returnResult;
                }

                if (string.IsNullOrEmpty(deviceId))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideDeviceId");
                    return returnResult;
                }
                if (string.IsNullOrEmpty(deviceModel.OrgName))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideOrganizationName");
                    return returnResult;
                }

                //if (deviceModel.DeviceId != deviceId)
                //{
                //    returnResult.Result = $"Mismatch device id. ";
                //    return returnResult;
                //}

                if (deviceModel.IsActive == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("IsActiveRequired");
                    return returnResult;
                }

                var updatedevice = await _deviceRepository.SelectFirstOrDefaultAsync(x => x.DeviceId.Equals(deviceId));

                if (updatedevice == null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("NotExists");
                    return returnResult;
                }

                //if (deviceModel.DeviceId != updatedevice.DeviceId)
                //{
                //    returnResult.Result = ResourceInformation.GetResValue("MismatchDeviceId");
                //    return returnResult;
                //}

                var organization = await _organizationRepository.SelectFirstOrDefaultAsync(x => x.OrgName.Equals(deviceModel.OrgName, StringComparison.OrdinalIgnoreCase) && x.IsActive.Value);

                if (organization != null)
                {
                    if (updatedevice.OrgId != organization.OrgId)
                    {
                        returnResult.Success = false;
                        returnResult.Result = ResourceInformation.GetResValue("MismatchOrganization");
                        return returnResult;
                    }
                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("OrganizationNotExists");
                    return returnResult;
                }
                //if (orgId != _mstOrg)
                //{
                //    if (orgId != device.OrgId || orgId != updatedevice.OrgId)
                //    {
                //        _returnResult.Result = _resInfo.GetResValue("MismatchOrganization");
                //        return _returnResult;
                //    }
                //}

                if (deviceModel.ClientTypeId != updatedevice.ClientTypeId)
                {
                    returnResult.Result = ResourceInformation.GetResValue("NoPermissionToUpdateClientType");
                    return returnResult;
                }

                updatedevice.IsUsed = deviceModel.IsUsed;
                updatedevice.IsActive = deviceModel.IsActive.Value;

                var modifiedBy = await _userBusiness.GetUserByUserName(tokenData.UserName.ToString());

                updatedevice.ModifiedBy = modifiedBy.UserId;

                var result = await _deviceRepository.UpdateAsync(updatedevice);

                if (result.State.Equals(EntityState.Modified))
                {
                    await _deviceRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("DeviceBusiness", "UpdateDevice", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = $"{ResourceInformation.GetResValue("DataUpdateFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to delete the device details
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="deviceId"></param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> DeleteDevice(TokenData tokenData, string deviceId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (string.IsNullOrEmpty(deviceId))
                    returnResult.Result = ResourceInformation.GetResValue("ProvideDeviceId");

                var deletedevice = await _deviceRepository.SelectFirstOrDefaultAsync(x => x.DeviceId.Equals(deviceId) && x.IsActive);

                if (deletedevice == null)
                {
                    returnResult.Result = ResourceInformation.GetResValue("NotExists");
                    return returnResult;
                }

                deletedevice.IsActive = false;

                var modifiedBy = await _userBusiness.GetUserByUserName(tokenData.UserName.ToString());
                deletedevice.ModifiedBy = modifiedBy.UserId;


                var result = await _deviceRepository.UpdateAsync(deletedevice);
                if (result.State.Equals(EntityState.Modified))
                {
                    await _deviceRepository.UnitOfWork.SaveChangesAsync();

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
                _logger.Error("DeviceBusiness", "DeleteDevice", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result =
                    $"{ResourceInformation.GetResValue("DataDeleteFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to get devoce details by serialkey
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="serialKey">userName</param>
        /// <param name="orgId"></param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> GetDeviceBySerialKey(TokenData tokenData, Guid SerialKey, string orgId)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                Device deviceData = null;
                bool isAlreadyInUse = false;
                Organization organizationData = null;

                if (!string.IsNullOrEmpty(SerialKey.ToString()))
                {
                    organizationData = await _organizationRepository.SelectFirstOrDefaultAsync(y => y.OrgName == orgId);
                    deviceData = await _deviceRepository.SelectFirstOrDefaultAsync(x => x.SerialKey == SerialKey && (x.OrgId == organizationData.OrgId));

                    //TODO:Need to discuss if device should get blocked if requested device data on serial key

                    if (deviceData != null)
                    {
                        isAlreadyInUse = deviceData.IsUsed;

                        deviceData.IsUsed = true;
                        await _deviceRepository.UpdateAsync(deviceData);
                        await _deviceRepository.UnitOfWork.SaveChangesAsync();
                        deviceData.PrimaryKey = _customPasswordHash.Decrypt(deviceData.PrimaryKey, deviceData.Subject.ToString());
                    }
                }

                if (deviceData == null)
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("InvalidSerialKey");
                    return returnResult;
                }
                else if (isAlreadyInUse)//Check if serial key already in use
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("SerialKeyInUse");
                    return returnResult;
                }
                else
                {
                    //Return device details only if device already not in use
                    returnResult.Success = true;
                    returnResult.Result = ResourceInformation.GetResValue("DeviceStatusSetToUsed");
                    returnResult.Data = deviceData;
                    return returnResult;
                }

            }
            catch (Exception ex)
            {
                _logger.Error("DeviceBusiness", "GetDeviceBySerialKey", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// Method to add SerialKey
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        public async Task<ReturnResult> AddSerialkey(TokenData tokenData)
        {
            ReturnResult returnResult = new ReturnResult();
            var devices = await _deviceRepository.SelectAllAsync();
            try
            {
                var modifiedBy = await _userBusiness.GetUserByUserName(tokenData.UserName.ToString());
                foreach (var device in devices)
                {
                    if (device.SerialKey == null)
                    {
                        device.SerialKey = new Guid();
                    }
                    device.ModifiedBy = modifiedBy.UserId;

                }
                await _deviceRepository.UpdateRange(devices);
                await _deviceRepository.UnitOfWork.SaveChangesAsync();
                returnResult.Success = true;
                returnResult.Result = $"Device {ResourceInformation.GetResValue("DataSavedSuccess")}";
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("DeviceBusiness", "AddSerialkey", ex.Message, ex.StackTrace);
                ExceptionLogger.LogException(ex);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// Method to get device details by organization name
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="OrgName"></param>
        /// <returns></returns>
        public async Task<ReturnResult> GetDeviceByOrg(TokenData tokenData, string OrgName)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                if (string.IsNullOrEmpty(OrgName))
                {
                    returnResult.Result = ResourceInformation.GetResValue("ProvideOrgDetails");
                    return returnResult;
                }

                var deviceOrg = await _organizationRepository.SelectFirstOrDefaultAsync(x => (x.OrgName == OrgName.Trim()) && x.IsActive.Value);
                if (deviceOrg != null)
                {
                    var deviceData = await _deviceRepository.SelectAsync(x => (x.OrgId == deviceOrg.OrgId && x.IsActive));

                    #region Unused code as PrimaryKey is not property of new Device entity
                    deviceData = deviceData.Select(x => new Device
                    {
                        OrgId = x.OrgId,
                        Subject = x.Subject,
                        DeviceId = x.DeviceId,
                        ClientTypeId = x.ClientTypeId,
                        PrimaryKey = _customPasswordHash.Decrypt(x.PrimaryKey, x.Subject.ToString()),
                        SerialKey = x.SerialKey,
                        IsUsed = x.IsUsed
                    });
                    #endregion

                    returnResult.Success = true;
                    returnResult.Data = deviceData;

                }
                else
                {
                    returnResult.Success = false;
                    returnResult.Result = ResourceInformation.GetResValue("NotExists");
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("DeviceBusiness", "GetDeviceByOrg", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// Method to update device IsUsed status
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="serialKey"></param>
        /// <param name="isUsed"></param>
        /// <returns></returns>
        public async Task<ReturnResult> UpdateDeviceUsedStatus(TokenData tokenData, Guid? serialKey, bool isUsed)
        {
            ReturnResult returnResult = new ReturnResult()
            {
                Success = false
            };
            try
            {
                Device deviceData = null;
                if (!string.IsNullOrEmpty(serialKey.ToString()))
                {
                    var organization = await _organizationRepository.SelectFirstOrDefaultAsync(o => o.OrgName == tokenData.OrgId && o.IsActive.Value);
                    deviceData = await _deviceRepository.SelectFirstOrDefaultAsync(x => x.SerialKey == serialKey &&
                                                       (_mstOrg != tokenData.OrgId
                                                           ? x.OrgId.Equals(organization.OrgId)
                                                           : x.OrgId.Equals(null)));
                    if (deviceData != null)
                    {
                        deviceData.IsUsed = isUsed;
                        deviceData.ModifiedBy = deviceData.SerialKey;
                        returnResult.Success = true;
                        await _deviceRepository.UpdateAsync(deviceData);
                        await _deviceRepository.UnitOfWork.SaveChangesAsync();
                    }
                }

                returnResult.Result = deviceData == null
                    ? ResourceInformation.GetResValue("NoPermissionOrMismatchOrganization")
                    : ResourceInformation.GetResValue("DeviceStatusSuccessfulUpdate");

                return returnResult;
            }
            catch (Exception ex)
            {
                _logger.Error("DeviceBusiness", "UpdateDeviceUsedStatus", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
            }
        }

        /// <summary>
        /// Method for range of operations
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public async Task<ReturnResult> UpdateRangeDevice(IEnumerable<Device> devices)
        {
            ReturnResult returnResult = new ReturnResult();
            try
            {
                await _deviceRepository.UpdateRange(devices);
                await _deviceRepository.UnitOfWork.SaveChangesAsync();

                returnResult.Success = true;
                returnResult.Result = $"{ResourceInformation.GetResValue("dataupdatesuccess")}";
                return returnResult;
            }

            catch (Exception ex)
            {
                _logger.Error("DeviceBusiness", "UpdateRangeDevice", ex.Message, ex.StackTrace);
                returnResult.Result = $"{ResourceInformation.GetResValue("DataSavedFailure")} : {ExceptionLogger.LogException(ex)}";
                return returnResult;
            }
        }

        /// <summary>
        /// This method is used to user details by User Name
        /// </summary>
        /// <param name="deviceId">userName</param>
        /// <returns>returns response message</returns>
        public async Task<ReturnResult> GetUserByDeviceId(string deviceId)
        {
            ReturnResult returnResult = new ReturnResult();
            Device device = new Device();
            try
            {
                //using (var sc = new SecurityContext())
                //{
                //    return await Task.FromResult(
                //        sc.Device.FirstOrDefault(
                //            x =>
                //                x.DeviceId.Equals(deviceId) &&
                //                //(orgId != null ? u.OrgId.Equals(orgId) : u.OrgId != string.Empty) &&
                //                !x.IsDeleted));
                //}
                returnResult.Data = await _deviceRepository.SelectFirstOrDefaultAsync(x => x.DeviceId.Equals(deviceId) && x.IsActive);
                returnResult.Success = true;
                return returnResult;

            }
            catch (Exception ex)
            {
                _logger.Error("DeviceBusiness", "GetUserByDeviceId", ex.Message, ex.StackTrace);
                returnResult.Success = false;
                returnResult.Result = ResourceInformation.GetResValue("DataLoadFailure");
                return returnResult;
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
                _deviceRepository.Dispose();
                _organizationRepository.Dispose();
                _applicationRepository.Dispose();
                _userRepository.Dispose();
                _userBusiness.Dispose();

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

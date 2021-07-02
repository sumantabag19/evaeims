using EVA.EIMS.Common;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IDeviceBusiness : IDisposable
    {
        Task<ReturnResult> GetDevice(TokenData tokenData, string deviceId);
        Task<ReturnResult> SaveDevice(TokenData tokenData, DeviceModel deviceModel);
        Task<ReturnResult> UpdateDevice(TokenData tokenData, string deviceId, DeviceModel device);
        Task<ReturnResult> DeleteDevice(TokenData tokenData, string deviceId);
        Task<ReturnResult> GetDeviceBySerialKey(TokenData tokenData, Guid SerialKey, string orgID);
        Task<ReturnResult> AddSerialkey(TokenData tokenData);
        Task<ReturnResult> GetDeviceByOrg(TokenData tokenData, string OrgId);
        Task<ReturnResult> UpdateDeviceUsedStatus(TokenData tokenData, Guid? serialKey, bool isUsed);
        Task<ReturnResult> UpdateRangeDevice(IEnumerable<Device> devices);
        Task<ReturnResult> GetUserByDeviceId(string deviceId);
        //Device GetDeviceBySerialKey(TokenData tokenData,Guid SerialKey);
        //ReturnResult ResetPassword(string userName,string oldPwd,string newPwd, string orgId, int clientTypeId);
        //ReturnResult ResetAnyDevicePassword(Guid userId, string newPwd, string orgId, int clientTypeId);
    }
}

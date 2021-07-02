using EVA.EIMS.Common;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ComplexEntities;
using EVA.EIMS.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IUserBusiness : IDisposable
    {
        Task<ReturnResult> GetUser(TokenData tokenData);
        Task<ReturnResult> GetUser(TokenData tokenData, Guid id);
        Task<ReturnResult> GetUserForUI(TokenData tokenData);
        Task<ReturnResult> GetUser(TokenData tokenData, string name);
        Task<ReturnResult> SaveUser(TokenData tokenData, User user);
        Task<ReturnResult> UpdateUser(TokenData tokenData, Guid id, User user);
        Task<ReturnResult> DeleteUser(TokenData tokenData, Guid id);
        Task<UserDetails> GetUserByUserName(string userName);
        Task<UserDetails> GetUserByMobile(string mobile);
        Task<ReturnResult> GetTwoFactorByUserName(string userName);
        Task<ReturnResult> GetUserNameById(Guid userId);

        Task<ReturnResult> UpdatePassword(UserCredentials userCredentials);
        Task<ReturnResult> ResetAnyUserPassword(string userNamefromToken, UserCredentials userCredentials);
        Task<ReturnResult> GetUsersByOrganization(TokenData tokenData, string orgId);
        Task<ReturnResult> AddOrUpdateUserApplication(TokenData tokenData, ApplicationUserMappingModel applicationUserMappingModel);
        //Task<ReturnResult> UpdateUserApplication(string userName, ApplicationUserMappingModel applicationUserMappingModel);
        Task<ReturnResult> DeleteUserApplication(TokenData tokenData, int mappingId);
        Task<ReturnResult> VerifyAccount(string userName, string userEmail);
        Task<ReturnResult> VerifyOTP(Guid userId, string otp);
        Task<ReturnResult> LockAccount(string userName, int lockType);
        Task<ReturnResult> ResetLock(string userName, int lockType);
        Task<ReturnResult> GetAllLockedUser(TokenData tokenData);
        Task<ReturnResult> FirstTimeLogin(UserDetails userDetails);
        Task<ReturnResult> UnlockUsers(TokenData tokenData, List<UnlockUsers> unlockUsers);
        Task<ReturnResult> UnlockUserByUserName(TokenData tokenData, List<UnlockUserByUserName> unlockUserByUserNames);
        Task<ReturnResult> ChangePassword(string UserName, UserCredentials userCredentials);
        Task SendEmailPwdExpNotify(TokenData tokenData);
        Task<User> GetUserByUserNameInternally(string username);
        Task<ReturnResult> VerifyAccountLoginAsync(string userName, int otpType, string token, bool IsRefreshTokenFlow = false);
        Task<ReturnResult> VerifyOTPLoginAsync(Guid userId, string otp);
        Task<ReturnResult> VerifyMobileLoginAsync(string mobile);
        Task<ReturnResult> VerifyEmailAsync(Guid userId);

        #region UserOrganizationMapping Methods
        Task<ReturnResult> GetAllUserOrgMapping();
        Task<ReturnResult> GetUserOrgMappingByUserId(Guid userId);
        Task<List<UserOrgAppDetails>> GetOrgAppMappingByUserName(string userName);
        Task<ReturnResult> SaveUserOrgMapping(string userName, UserOrganizationMappingModel userOrganizationMappingModel);
        Task<ReturnResult> UpdateUserOrgMapping(string userName, UserOrganizationMappingModel userOrganizationMappingModel);
        Task<ReturnResult> DeleteUserOrgMappingByUserId(Guid userId);
        Task<ReturnResult> SaveMultiUserOrgMapping(string userName, UserOrganizationMappingModel userOrganizationMappingModel);
        Task<ReturnResult> VerifyTokenExchange(Guid userId, string orgName);

        #endregion
    }
}

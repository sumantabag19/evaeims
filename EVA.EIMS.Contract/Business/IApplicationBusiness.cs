using EVA.EIMS.Common;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IApplicationBusiness : IDisposable
    {
        Task<ReturnResult> Get(TokenData tokenData);
        Task<ReturnResult> GetById(int appId);
        Task<ReturnResult> Save(string userName, Application application);
        Task<ReturnResult> Update(string userName, int appId, Application application);
        Task<ReturnResult> Delete(string userName, int appId);
        Task<int> GetByAppName(string appName);
        Task<string> GetAppNameFromClientId(string clientId);
        Task<OauthClient> GetClientIdByAzureAppId(string azureAppId);
        Task<Application> GetAppById(int appid);
        Task<ReturnResult> GetAllApplicationByUserId(Guid userId, TokenData tokenData);
        Task<int> GetByApplicationName(string appName);
        Task<ReturnResult> SetAzureAppId(int appid, Guid adid);

    }
}

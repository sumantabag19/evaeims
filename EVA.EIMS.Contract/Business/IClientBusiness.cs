using EVA.EIMS.Common;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IClientBusiness : IDisposable
    {
        Task<ReturnResult> Get();
        Task<OauthClient> GetById(string clientId);
        Task<ReturnResult> Save(string userName, OauthClient client);
        Task<ReturnResult> Update(string userName, string clientId, OauthClient client);
        Task<ReturnResult> Delete(string userName, string clientId);
        Task<ReturnResult> DynamicClientCreation(TokenData tokenData, OauthClient dynamicClient);
        Task<ReturnResult> GetClient(int oauthClientId);
        Task<OauthClient> GetByIdForInActiveClient(string clientId);
        Task<List<ClientApplicationDetails>> GetAzureAppIdByClientId(string clientId);
        Task<ReturnResult> UpdateClientSecret(TokenData tokenData, OAuthClientViewModel updateClientModel);
      }
}

using EVA.EIMS.Common;
using EVA.EIMS.Entity.ViewModel;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IAccountBusiness
    {
        Task<ReturnResult> GetEIMSTokenFromADToken(string clientId, string adAccessToken, string idToken);
        Task<ReturnResult> GetTokenFromSupportToken(AccessTokenModel accessToken, string org);
    }
}

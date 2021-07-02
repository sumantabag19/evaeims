using EVA.EIMS.Common;
using EVA.EIMS.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IIMSLogOutBusiness : IDisposable
    {
        Task<ReturnResult> GetLoggedOutUserToken();
        Task<ReturnResult> IsUserLoggedOut(string token);
        Task<ReturnResult> SaveLogOutUserToken(string token);
        Task DeleteLoggedOutToken(TokenData tokenData);
    }
}

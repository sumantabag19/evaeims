using EVA.EIMS.Common;
using EVA.EIMS.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IRefreshTokenBusiness: IDisposable
    {
        Task<ReturnResult> Get(Guid refreshTokenId);
        Task<ReturnResult> GetClientwiseTokenCount();
        Task<ReturnResult> Save(RefreshToken refreshToken);
        Task<ReturnResult> Delete(Guid refreshTokenId);
        Task<ReturnResult> DeleteExpiredRefreshTokens();
        Task SendTokenRequestCountEmail(TokenData tokenData);

    }
}

using EVA.EIMS.Common;
using EVA.EIMS.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface ILanguageBusiness : IDisposable
    {
        Task<ReturnResult> Delete(Guid languageId);
        Task<ReturnResult> Update(Guid languageId, Language language);
        Task<ReturnResult> Save(Language language);
        Task<ReturnResult> Get();
        Task<ReturnResult> GetById(Guid languageId);
        Task<ReturnResult> GetByCode(string languageCode);
    }
}

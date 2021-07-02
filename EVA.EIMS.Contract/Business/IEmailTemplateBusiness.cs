using EVA.EIMS.Common;
using EVA.EIMS.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IEmailTemplateBusiness : IDisposable
    {
        Task<ReturnResult> Delete(string userName,int clientId);
        Task<ReturnResult> Get();
        Task<ReturnResult> GetById(int emailTemplateId);
        Task<ReturnResult> Save(string userName, EmailTemplate emailTemplate);
		Task<ReturnResult> Update(string userName, int emailTemplateId, EmailTemplate emaliTemplate);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EVA.EIMS.Common;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ComplexEntities;
using EVA.EIMS.Entity.ViewModel;

namespace EVA.EIMS.Contract.Business
{
    public interface ISecurityQuestionBusiness : IDisposable
    {

        Task<ReturnResult> Get();
        Task<ReturnResult> GetQuestionById(int questionId);
        Task<ReturnResult> Save(string userName, SecurityQuestion question);
        Task<ReturnResult> Update(string userName, int questionId, SecurityQuestion question);
        Task<ReturnResult> Delete(string userName, int questionId);
        Task<ReturnResult> GetRandomSecurityQuestions(Guid userId);
        Task<ReturnResult> VerifySecurityQuestionsAnswer(List<SecurityAnswerFromUserModel> userAnswers);
        Task<ReturnResult> GetbyUserId(string userNameFromToken, Guid userId);

    }
}

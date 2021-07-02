using EVA.EIMS.Common;
using EVA.EIMS.Entity;
using EVA.EIMS.Entity.ComplexEntities;
using EVA.EIMS.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IUserAnswerBusiness : IDisposable
    {
           Task <ReturnResult> AddOrUpdate(string userName, List<UserAnswer> userAns);
           Task <ReturnResult> UpdateUserQuestionAnswer(string userName, List<UserAnswer> userAns);
        

    }
}

using EVA.EIMS.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface IPasswordHistoryBusiness: IDisposable
    {
		Task<ReturnResult> ManagePassword(string UserId, string newPwd, string CreatedBy);
    }
}

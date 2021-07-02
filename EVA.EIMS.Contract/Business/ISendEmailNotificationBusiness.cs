using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Contract.Business
{
    public interface ISendEmailNotificationBusiness
    {
        Task<bool> SendRequestAccessEmail(string orgId, string username, string usermailId);
    }
}

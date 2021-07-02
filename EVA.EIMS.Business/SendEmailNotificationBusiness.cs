using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity;
using EVA.EIMS.Helper;
using Microsoft.Extensions.DependencyInjection;

namespace EVA.EIMS.Business
{

    public class SendEmailNotificationBusiness : ISendEmailNotificationBusiness
    {
        #region Private Variables

        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region Constructor
        public SendEmailNotificationBusiness(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        } 
        #endregion
        public async Task<bool> SendRequestAccessEmail(string orgId, string username, string usermailId)
        {
            try
            {
                IExecuterStoreProc<User> procExecuterRepository = _serviceProvider.GetRequiredService<IExecuterStoreProc<User>>();
                List<Parameters> param = new List<Parameters>
                {
                    new Parameters("orgId", orgId)
                };
                var listOfSiteAdmin = (await procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procGetAllSiteAdmin.ToString(), param));
                SendEmail sendEmailObj = new SendEmail();
                return await sendEmailObj.SendRequestAccessNotification(listOfSiteAdmin, username, usermailId);
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}

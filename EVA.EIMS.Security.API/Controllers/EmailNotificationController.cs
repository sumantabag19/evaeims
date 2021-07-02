using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVA.EIMS.Contract.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVA.EIMS.Security.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class EmailNotificationController : Controller
    {
        #region Private Variable
        private readonly ISendEmailNotificationBusiness _sendEmailNotification;
        #endregion

        #region Constructor
        public EmailNotificationController(ISendEmailNotificationBusiness sendEmailNotification)
        {
            _sendEmailNotification = sendEmailNotification;
        }
        #endregion

        // POST: api/EmailNotification/SendMailNotification
        [HttpPost]
        [ActionName("SendMailNotification")]
        [AllowAnonymous]
        public async Task<IActionResult> Post(string orgId, string userName, string usermailId)
        {
            return Ok(await _sendEmailNotification.SendRequestAccessEmail(orgId, userName, usermailId));
        }


    }
}

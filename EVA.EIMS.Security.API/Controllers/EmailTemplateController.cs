using EVA.EIMS.Common;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EVA.EIMS.Security.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]/[action]")]
    public class EmailTemplateController : Controller
    {
        #region Private Variables
        private IEmailTemplateBusiness _emailTemplateBusiness;
        #endregion

        #region Constructor
        public EmailTemplateController(IEmailTemplateBusiness emailTemplateBusiness)
        {
            _emailTemplateBusiness = emailTemplateBusiness;
        }
        #endregion

        #region Public API Methods
        /// <summary>
        /// This method is used to get the multiple emailTemplate details
        /// </summary>
        /// <returns>returns multiple emailTemplate details</returns>
        [HttpGet]
        [ActionName("GetEmailTemplate")]
        public async Task<IActionResult> Get()
        {
            var result = await _emailTemplateBusiness.Get();
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used get the emailTemplate details by id
        /// </summary>
        /// <param name="emailTemplateId">emailTemplateId</param>
        /// <returns>returns single emailTemplate details</returns>
        [HttpGet]
        [ActionName("GetEmailTemplateById")]
        public async Task<IActionResult> Get(int emailTemplateId)
        {
            var result = await _emailTemplateBusiness.GetById(emailTemplateId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to save the emailTemplate details
        /// </summary>
        /// <param name="emailTemplate">emailTemplate object</param>
        /// <returns>returns response message</returns>
        [HttpPost]
        [ActionName("SaveEmailTemplate")]
        public async Task<IActionResult> Post(EmailTemplate emailTemplate)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _emailTemplateBusiness.Save(tokenData.UserName, emailTemplate);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to update the emailTemplate details
        /// </summary>
        /// <param name="emailTemplateId">emailTemplateId</param>
        /// <param name="emailTemplate">emailTemplate object</param>
        /// <returns>returns response message</returns>
        [HttpPut]
        [ActionName("UpdateEmailTemplate")]
        public async Task<IActionResult> Put(int emailTemplateId, EmailTemplate emaliTemplate)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _emailTemplateBusiness.Update(tokenData.UserName, emailTemplateId, emaliTemplate);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to delete the emailTemplate details
        /// </summary>
        /// <param name="emailTemplateId">emailTemplate</param>
        /// <returns>returns response message</returns>
        [HttpDelete]
        [ActionName("DeleteEmailTemplate")]
        public async Task<IActionResult> Delete(int emailTemplateId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _emailTemplateBusiness.Delete(tokenData.UserName, emailTemplateId);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Method to dispose by parameter.
        /// </summary>
        /// <param name="disposing"></param>
        /// 
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _emailTemplateBusiness.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
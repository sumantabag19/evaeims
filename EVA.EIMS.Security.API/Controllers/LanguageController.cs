using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EVA.EIMS.Security.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]/[action]")]
    public class LanguageController : Controller, IDisposable
    {
        #region Private variables
        private ILanguageBusiness _languageBusiness;
        #endregion

        #region Constructor
        public LanguageController(ILanguageBusiness languageBusiness)
        {
            _languageBusiness = languageBusiness;
        }
        #endregion

        #region Public API Methods
        /// <summary>
        /// This method is used to get the multiple language details
        /// </summary>
        /// <returns>returns multiple language details</returns>
        /// 
        [HttpGet]
        [ActionName("GetLanguage")]
        public async Task<IActionResult> Get()
        {
            var result = await _languageBusiness.Get();
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);

        }

        /// <summary>
        /// This method is used get the language details by id
        /// </summary>
        /// <param name="languageId">languageId</param>
        /// <returns>returns single language details</returns>
        [HttpGet]
        [ActionName("GetLanguageById")]
        public async Task<IActionResult> Get(Guid languageId)
        {
            var result = await _languageBusiness.GetById(languageId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);

        }

        /// <summary>
        /// This method is used get the language details by languageCode
        /// </summary>
        /// <param name="languageCode">languageCode</param>
        /// <returns>returns single language details</returns>
        [HttpGet]
        [ActionName("GetLanguageByCode")]
        public async Task<IActionResult> Get(string languageCode)
        {
            var result = await _languageBusiness.GetByCode(languageCode);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to save the language details
        /// </summary>
        /// <param name="language">language object</param>
        /// <returns>returns response message</returns>
        [HttpPost]
        [ActionName("SaveLanguage")]
        public async Task<IActionResult> Post(Language language)
        {
            var result = await _languageBusiness.Save(language);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }


        /// <summary>
        /// This method is used to update the language details
        /// </summary>
        /// <param name="languageId">languageId</param>
        /// <param name="language">language object</param>
        /// <returns>returns response message</returns>
        [HttpPut]
        [ActionName("UpdateLanguage")]
        public async Task<IActionResult> Put(Guid languageId, Language language)
        {
            var result = await _languageBusiness.Update(languageId, language);

            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// This method is used to delete the language details
        /// </summary>
        /// <param name="languageId">languageId</param>
        /// <returns>returns response message</returns>
        [HttpDelete]
        [ActionName("DeleteLanguage")]
        public async Task<IActionResult> Delete(Guid languageId)
        {
            var result = await _languageBusiness.Delete(languageId);

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
                _languageBusiness.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
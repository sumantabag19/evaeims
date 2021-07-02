using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EVA.EIMS.Contract.Business;
using EVA.EIMS.Entity;
using EVA.EIMS.Common;
using EVA.EIMS.Common.Constants;
using EVA.EIMS.Entity.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using EVA.EIMS.Helper;
using System.Threading.Tasks;

namespace EVA.EIMS.Security.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]/[action]")]
    public class SecurityQuestionController : Controller, IDisposable
    {
        #region Instance Variables

        private readonly ISecurityQuestionBusiness _securityQuestionBusiness;
        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region Constructor
        public SecurityQuestionController(ISecurityQuestionBusiness securityQuestionBusiness, IServiceProvider serviceProvider)
        {
            _securityQuestionBusiness = securityQuestionBusiness;
            _serviceProvider = serviceProvider;

        }
        #endregion

        #region Public API Methods
        /// <summary>
        /// This method is used to get all security questions
        /// </summary>
        /// <returns>returns multiple security questions</returns>
        [HttpGet]
        [ActionName("GetSecurityQuestion")]
        public async Task<IActionResult> Get()
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _securityQuestionBusiness.Get();
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);


        }

        /// <summary>
        /// This method is used to get the security questions by id
        /// </summary>
        /// <param name="questionId">questionId</param>
        /// <returns>returns single security question details</returns>
        [HttpGet]
        [ActionName("GetQuestionById")]
        public async Task<IActionResult> GetQuestion([FromQuery] int questionId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _securityQuestionBusiness.GetQuestionById(questionId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);

        }

        /// <summary>
        /// This method is used to save the security question details
        /// </summary>
        /// <param name="securityQuestion"> securityQuestion object</param>
        /// <returns>returns response message</returns>
        [HttpPost]
        [ActionName("SaveSecurityQuestion")]
        public async Task<IActionResult> Post([FromBody] SecurityQuestion securityQuestion)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            ModelState.Remove("QuestionId");
            if (ModelState.IsValid)
            {
                var result = await _securityQuestionBusiness.Save(tokenData.UserName, securityQuestion);
                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);

        }

        /// <summary>
        /// This method is used to update the security question details
        /// </summary>
        /// <param name="quesionId">quesionId</param>
        /// <param name="securityQuestion">securityQuestion object</param>
        /// <returns>returns response message</returns>
        [HttpPut]
        [ActionName("UpdateSecurityQuestion")]
        public async Task<IActionResult> Put([FromBody] SecurityQuestion securityQuestion)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            if (ModelState.IsValid)
            {
                var result = await _securityQuestionBusiness.Update(tokenData.UserName, securityQuestion.QuestionId, securityQuestion);
                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// This method is used to delete the security question details
        /// </summary>
        /// <param name="questionId">questionId</param>
        /// <returns>returns response message</returns>
        [HttpDelete]
        [ActionName("DeleteSecurityQuestion")]
        public async Task<IActionResult> Delete([FromQuery] int questionId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);

            var result = await _securityQuestionBusiness.Delete(tokenData.UserName, questionId);
            if (result.Success)
                return Ok(result.Result);
            else
                return BadRequest(result.Result);

        }

        /// <summary>
        /// Get random User questions using stored procedure
        /// <param name="userId">UserId</param>
        /// </summary>
        /// <returns>UserQuestions</returns>
        [HttpGet]
        [AllowAnonymous]
        [ActionName("GetRandomSecurityQuestions")]
        public async Task<IActionResult> GetRandomSecurityQuestions([FromQuery] Guid userId)
        {
            var result = await _securityQuestionBusiness.GetRandomSecurityQuestions(userId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// Verify security questions answer by User
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userAnswers"></param>
        /// <returns>true or false</returns>

        [HttpPost]
        [AllowAnonymous]
        [ActionName("VerifySecurityQuestionsAnswer")]
        public async Task<IActionResult> VerifySecurityQuestionsAnswer([FromBody] List<SecurityAnswerFromUserModel> userAnswers)
        {
            if (ModelState.IsValid)
            {
                var result = await _securityQuestionBusiness.VerifySecurityQuestionsAnswer(userAnswers);
                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Get all the User questions using stored procedure
        /// <param name="userId">UserId</param>
        /// </summary>
        /// <returns>UserQuestions</returns>
        [HttpGet]
        [ActionName("GetAllSecurityQuestionsByUserId")]
        public async Task<IActionResult> GetbyUserId([FromQuery] Guid userId)
        {
            var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
            var result = await _securityQuestionBusiness.GetbyUserId(tokenData.UserName, userId);
            if (result.Success)
                return Ok(result.Data);
            else
                return BadRequest(result.Result);
        }

        /// <summary>
        /// Add or Updates user's security questions and/or answers
        /// <param name="userName">UserId</param>
        /// <param name="userAns">List of user's question and answer</param>
        /// </summary>
        /// <returns> true or false</returns>
        [HttpPost]
        [ActionName("AddOrUpdateUserAnswer")]
        public async Task<IActionResult> AddOrUpdate([FromBody] List<UserAnswer> userAns)
        {
            if (ModelState.IsValid)
            {
                IUserAnswerBusiness _userAnswerBusiness = _serviceProvider.GetRequiredService<IUserAnswerBusiness>();
                var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
                var result = await _userAnswerBusiness.AddOrUpdate(tokenData.UserName, userAns);
                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Updates user's security questions and/or answers
        /// <param name="userName">UserId</param>
        /// <param name="userAns">List of user's question and answer</param>
        /// </summary>
        /// <returns> true or false</returns>
        [HttpPost]
        [ActionName("UpdateUserQuestionAnswer")]
        public async Task<IActionResult> UpdateUserQuestionAnswer([FromBody] List<UserAnswer> userAns)
        {
            foreach (var key in ModelState.Keys.Where(m => m.EndsWith("UserAnswerText")).ToList())
                ModelState.Remove(key);

            if (ModelState.IsValid)
            {
                IUserAnswerBusiness _userAnswerBusiness = _serviceProvider.GetRequiredService<IUserAnswerBusiness>();
                var tokenData = TokenData.GetRequestContextRouteData(HttpContext);
                var result = await _userAnswerBusiness.UpdateUserQuestionAnswer(tokenData.UserName, userAns);
                if (result.Success)
                    return Ok(result.Result);
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
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
                _securityQuestionBusiness.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

    }
}
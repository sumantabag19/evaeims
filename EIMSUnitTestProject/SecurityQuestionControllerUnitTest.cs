//using FluentAssertions;
//using EVA.EIMS.Entity;
//using EVA.EIMS.Entity.ViewModel;
//using EVA.EIMS.Security.API.Controllers;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using Xunit;

//namespace EIMSUnitTestProject
//{
//    public class SecurityQuestionControllerUnitTest
//    {
//        #region Private Variables
//        private SecurityQuestionController _securityQuestionController;

//        #endregion

//        #region Constructor
//        public SecurityQuestionControllerUnitTest()
//        {
//            StartupUnitTest<SecurityQuestionController> startUp = new StartupUnitTest<SecurityQuestionController>();

//            _securityQuestionController = startUp.ConfigureServices();
//        }

//        #endregion

//        /// <summary>
//        /// This method is used to test to get all security questions
//        /// </summary>
//        /// <returns>returns IEnumerableSecurityQuestion </returns>
//        [Fact]
//        public void GetAllSecurityQuestions_ReturnsIEnumerableSecurityQuestion()
//        {
//            var result = _securityQuestionController.Get();
//            //Assert
//            var okResult = result.Should().BeAssignableTo<IEnumerable<SecurityQuestion>>().Subject;

//        }

//        /// <summary>
//        /// This method is used to test to get the security questions by id
//        /// </summary>
//        /// <param name="questionId">questionId</param>
//        /// <returns>returns security question type object</returns>
//        [Fact]


//        public void GetSpecificSecurityQuestion_BasedOnQuestionId_ReturnsSecurityQuestion()
//        {
//            var result = _securityQuestionController.GetQuestion(3);

//            var okResult = result.Should().BeOfType<SecurityQuestion>().Subject;

//        }

//        /// <summary>
//        /// This method is used to test to save the security question details
//        /// </summary>
//        /// <param name="newQuestion"> securityQuestion object</param>
//        /// <returns>returns OkObjectResult</returns>

//        [Fact]
//        public void SaveSecurityQuestionDetails_ReturnsOkObject()
//        {
//            var newQuestion = new SecurityQuestion { Question = "trialtest", IsActive = true };
//            var result = _securityQuestionController.Post(newQuestion);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

//        }

//        /// <summary>
//        /// This method is used test to update the security question details
//        /// </summary>
//        /// <param name="quesionId">quesionId</param>
//        /// <param name="updatedQuestion">securityQuestion object</param>
//        /// <returns>returns OkObjectResult</returns>
//        [Fact]
//        public void UpdateSecurityQuestion_BasedOnQuestionId_ReturnsOkObject()
//        {
//            var updatedQuestion = new SecurityQuestion { Question = "trialtestupdate", IsActive = true };
//            var result = _securityQuestionController.Put(updatedQuestion);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// This method is used to test to delete the security question details
//        /// </summary>
//        /// <param name="questionId">questionId</param>
//        /// <returns>returns OkObjectResult</returns>
//        [Fact]
//        public void DeleteSecurityQuestion_BasedOnQuestionId_ReturnsOkObject()
//        {
//            var result = _securityQuestionController.Delete(3);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// This method is used for negative test while saving a security question
//        /// </summary>
//        /// <returns>returns BadRequestObjectResult</returns>
//        [Fact]
//        public void SaveSecurityQuestionDetails_ReturnsBadRequest()
//        {
//            var newQuestion = new SecurityQuestion { IsActive = true };
//            var result = _securityQuestionController.Post(newQuestion);
//            var okResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;

//        }

//        /// <summary>
//        /// This method is used to test to get random User questions using stored procedure
//        /// <param name="userId">UserId</param>
//        /// </summary>
//        /// <returns>OkObject</returns>

//        [Fact]
//        public void GetRandomSecurityQuestions_ReturnsOkObject()
//        {
//            var result = _securityQuestionController.GetRandomSecurityQuestions(new Guid("03AB0396-82BD-49D8-A893-1340AB727E5A"));
//            var okResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
//        }

//        /// <summary>
//        /// This method is used to test to verify security questions answer by User
//        /// </summary>
//        /// <param name="userId"></param>
//        /// <param name="userAnswers"></param>
//        /// <returns>OkObject</returns>

//        [Fact]
//        public void VerifySecurityQuestionsAnswer_ReturnsOkObject()
//        {
//            List<SecurityAnswerFromUserModel> verifyQuestion = new List<SecurityAnswerFromUserModel>
//            {
//                new SecurityAnswerFromUserModel
//                {
//                    QuestionId = 1,
//                    UserAnswerText = "firstanswer",
//                    UserId = new Guid("03AB0396-82BD-49D8-A893-1340AB727E5A")

//                },
//                new SecurityAnswerFromUserModel
//                {
//                    QuestionId = 15,
//                    UserAnswerText = "fifteenthanswer",
//                   UserId = new Guid("03AB0396-82BD-49D8-A893-1340AB727E5A")

//                }   
//            };
//            var result = _securityQuestionController.VerifySecurityQuestionsAnswer(verifyQuestion);
//        }

//        /// <summary>
//        /// Test to get all the User questions using stored procedure
//        /// <param name="userId">UserId</param>
//        /// </summary>
//        /// <returns>UserQuestions</returns>
        
//        [Fact]
//        public void GetAllSecurityQuestionsByUserId_ReturnsSecurityQuestion()
//        {
//            var result = _securityQuestionController.GetbyUserId(new Guid("03AB0396-82BD-49D8-A893-1340AB727E5A"));

//            var okResult = result.Should().BeOfType<SecurityQuestion>().Subject;

//        }

//        /// <summary>
//        /// Used to test to Add or Updates user's security questions and/or answers
//        /// <param name="userName">UserId</param>
//        /// <param name="userAns">List of user's question and answer</param>
//        /// </summary>
//        /// <returns> true or false</returns>
        
//        [Fact]
//        public void AddOrUpdateUserAnswer_ReturnsOkObject()
//        {
//            List<UserAnswer> addOrUpdateQuestion = new List<UserAnswer>
//            {
//                new UserAnswer
//                {
//                    QuestionId = 1,
//                    UserAnswerText = "first answer",
//                    UserId = new Guid("6528461B-7986-4BD3-8C80-012082E50F7F")

//                },
//                new UserAnswer
//                {
//                    QuestionId = 15,
//                    UserAnswerText = "fifteenth answer",
//                   UserId = new Guid("6528461b-7986-4bd3-8c80-012082e50f7f")

//                },
//                new UserAnswer
//                {
//                    QuestionId = 19,
//                    UserAnswerText = "nineteenth answer",
//                    UserId = new Guid("6528461b-7986-4bd3-8c80-012082e50f7f")

//                },
//                new UserAnswer
//                {
//                    QuestionId = 20,
//                    UserAnswerText = "twentieth answer",
//                    UserId = new Guid("6528461b-7986-4bd3-8c80-012082e50f7f")

//                },
//                new UserAnswer
//                {
//                    QuestionId = 22,
//                    UserAnswerText = "twenty first answer",
//                    UserId = new Guid("6528461b-7986-4bd3-8c80-012082e50f7f")

//                }
//            };
//            var result = _securityQuestionController.AddOrUpdate(addOrUpdateQuestion);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }
//    }



//}



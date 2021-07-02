//using FluentAssertions;
//using EVA.EIMS.Common;
//using EVA.EIMS.Entity;
//using EVA.EIMS.Entity.ViewModel;
//using EVA.EIMS.Security.API.Controllers;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using Xunit;

//namespace EIMSUnitTestProject
//{
//    public class UserControllerUnitTest
//    {
//        private UserController _userController;
//        public UserControllerUnitTest()
//        {
//            StartupUnitTest<UserController> startUpUnittTest = new StartupUnitTest<UserController>();

//            _userController = startUpUnittTest.ConfigureServices();

//        }

//        [Fact]
//        public void GetAllUser_ReturnsIEnumerableUser()
//        {
//            var result = _userController.Get();
//            var okResult = result.Should().BeAssignableTo<IEnumerable<User>>().Subject;
//        }

//        //[Fact]
//        //public void GetUserById_ReturnsUser()
//        //{
//        //    var result = _userController.Get("08d5e022 - 184c - acfb - e906 - 2e97358963b6");
//        //    var okResult = result.Should().BeOfType<User>().Subject;
//        //}

//        //[Fact]
//        //public void GetUserById_ReturnsNull_WhenUserIdDoesNotExists()
//        //{
//        //    var result = _userController.Get("08d5e022-184c-acfb-e906-2e97358963b6");
//        //    var okResult = result.Should().BeNull();
//        //}

//        [Fact]
//        public void GetUserById_ReturnsNull_WhenEmpltyUserIdPassed()
//        {
//            var result = _userController.Get();
//            var okResult = result.Should().BeNull();
//        }

//        [Fact]
//        public void SaveUser_ReturnsOkObjectResult()
//        {
//            var newUser = new User { UserId = Guid.NewGuid(), UserName = "TempUser" };
//            var result = _userController.Post(newUser);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        [Fact]
//        public void SaveUser_ReturnsBadRequestObjectResult_WhenIdAlreadyExists()
//        {
//            var newUser = new User { UserId = Guid.NewGuid(), UserName = "User"};
//            var result = _userController.Post(newUser);
//            var Result = result.Should().BeOfType<BadRequestObjectResult>().Subject;
//        }

//        [Fact]
//        public void SaveUser_ReturnsBadRequestObjectResult_WhenRollIsNull()
//        {
//            User newUser = null;
//            var result = _userController.Post(newUser);
//            var Result = result.Should().BeOfType<BadRequestObjectResult>().Subject;
//        }

//        [Fact]
//        public void DeleteUser_ReturnsOkObjectResult()
//        {
//            var result = _userController.Delete(Guid.NewGuid());
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        [Fact]
//        public void DeleteUser_ReturnsBadRequestObjectResult_WhenIdNotExist()
//        {
//            var result = _userController.Delete(Guid.NewGuid());
//            var okResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
//        }

//        [Fact]
//        public void UpdateUser_ReturnsOkObjectResult()
//        {
//            var newUser = new User { UserId = Guid.NewGuid(), UserName = "User"};
//            var result = _userController.Put(newUser);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        [Fact]
//        public void UpdateUser_ReturnsBadRequestObjectResult_WhenIdMismatch()
//        {
//            var newUser = new User { UserId = Guid.NewGuid(), UserName = "User" };
//            var result = _userController.Put(newUser);
//            var okResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
//        }

//        [Fact]
//        public void UpdateUser_ReturnsBadRequestObjectResult_WhenIdNotExists()
//        {
//            var newUser = new User { UserId = Guid.NewGuid(), UserName = "User"};
//            var result = _userController.Put(newUser);
//            var okResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Used to test the verification of account on the basis of user name and email
//        /// </summary>
//        /// <param name="userName"></param>
//        /// <param name="userEmail"></param>
//        /// <returns>userId as OkResult</returns>
//        /// 
//        [Fact]
//        public void VerifyAccount_ReturnsOkObjectResult_WithUserId()
//        {
//            UserCredentials credentials = new UserCredentials()
//            {
//                UserName = "cdeshmd",
//                EmailId = "cdeshmd@eva.com"
//            };

//            var result = _userController.VerifyAccount(credentials);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Used to negatively test the verification of account on the basis of inactive user name and email
//        /// </summary>
//        /// <param name="userName"></param>
//        /// <param name="userEmail"></param>
//        /// <returns>BadReuestObjectResult</returns>
//        [Fact]
//        public void VerifyAccount_ReturnsBadRequestObjectResult_WhenInactiveUser()
//        {
//            UserCredentials credentials = new UserCredentials()
//            { UserName = "cdeshmd",
//                EmailId = "cdeshmd@eva.com"
//            };

//            var result = _userController.VerifyAccount(credentials);
//            var okResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Used to test the verification of token against provided user id in database
//        /// If token is expired or invalid return responce accordingly
//        /// </summary>
//        /// <param name="userId"></param>
//        /// <param name="otp"></param>
//        /// <returns></returns>
//        [Fact]
//        public void VerifyOTP_ReturnsOkObjectResult()
//        {
//            var result = _userController.VerifyOTP(new VerifyOTP { UserId = new Guid("50B4E503-C3E1-4FD6-AFBD-00024478F293"), OTPString = "vBPzQRvfgoq8NHXrqpUt%2fg%3d%3d" });
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Used to negavtive test the verification of token against provided wrong user id
//        /// If token is expired or invalid return responce accordingly
//        /// </summary>
//        /// <param name="userId"></param>
//        /// <param name="otp"></param>
//        /// <returns></returns>
//        [Fact]
//        public void VerifyOTP_ReturnsBadRequestObjectResult_WhenInvalidUserId()
//        {

//            var result = _userController.VerifyOTP(new VerifyOTP { UserId = new Guid("50B4E503-C3E1-4FD6-AFBD-00024478F293"), OTPString = "vBPzQRvfgoq8NHXrqpUt%2fg%3d%3d" });
//            var okResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Test case when user enters wrong otp more than certain times the user gets locked
//        [Fact]
//        public void VerifyOTP_LockUserWhenUserEntersWrongOTPMoreThanThreeTimes_Returns_BadRequestObjectResult()
//        {

//            var result = _userController.VerifyOTP(new VerifyOTP { UserId = new Guid("50B4E503-C3E1-4FD6-AFBD-00024478F293"), OTPString = "vBPzQRvfgoq8NH" });
//            var okResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Test case when user enters wrong emailid more than certain times the user gets locked during verification
//        /// </summary>
//        [Fact]
//        public void VerifyAccount_LockUserWhenUserEntersWrongEmaildIdMoreThanThreeTimes_ReturnsBadRequestObjectResult()
//        {
//            UserCredentials credentials = new UserCredentials()
//            {
//                UserName = "cdeshmd",
//                EmailId = "cdeshmd@eva.com"
//            };
//            var result = _userController.VerifyAccount(credentials);
//            var okResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
//        }
//    }

//}
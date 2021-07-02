//using FluentAssertions;
//using EVA.EIMS.Common;
//using EVA.EIMS.Security.API.Controllers;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using Xunit;

//namespace EIMSUnitTestProject
//{
//    public class PasswordManagementControllerUnitTest
//    {
//        #region Private Variables
//        private PasswordManagementController _passwordManagementController;
//        private UserController _userController;
//        private CustomPasswordHash _customPasswordHash;
//        #endregion

//        #region Constructor
//        public PasswordManagementControllerUnitTest()
//        {
//            StartupUnitTest<PasswordManagementController> startupPasswordManagement = new StartupUnitTest<PasswordManagementController>();
//            StartupUnitTest<UserController> startupUser = new StartupUnitTest<UserController>();

//            _passwordManagementController = startupPasswordManagement.ConfigureServices();
//            _userController = startupUser.ConfigureServices();
//            _customPasswordHash = new CustomPasswordHash();
//        }
//        #endregion

//        #region Positive Tests

//        /// <summary>
//        /// This method is used to test the update of password in case of forgot password.
//        /// </summary>
//        /// <param name="userCredentials">userCredentials object</param>
//        /// <returns>Returns success or failure message</returns>
//        [Fact]
//        public void UpdatePassword_returnsReturnsOkObjectResult()
//        {
//            var newUserCredentials = new UserCredentials { UserId = new Guid("3631041A-CD44-4C23-3BE1-08D612F3A919"), UserName = "Indra3" , NewPassword = "PasswordChanged@01" };
//            var result = _passwordManagementController.UpdatePassword(newUserCredentials);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

//        }

//        /// <summary>
//        /// This method is used to test to change the password
//        /// </summary>
//        /// <param name="userCredentials">userCredentials object</param>
//        /// <returns>Returns success or failure message</returns>
//        [Fact]
//        public void ChangePassword_returnsReturnsOkObjectResult()
//        {
//            var newUserCredentials = new UserCredentials { UserId = new Guid("3631041A-CD44-4C23-3BE1-08D612F3A919"), UserName = "indra3", OldPassword = "PasswordChanged@01", NewPassword = "PasswordChanged@02" };
//            var result = _passwordManagementController.UpdatePassword(newUserCredentials);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

//        }

//        /// <summary>
//        /// Test case to manage password when user changes password , the old password goes to the password history
//        /// </summary>
//        [Fact]
//        public void PasswordManagement_PasswordGoestoPasswordHistoryTable_Returns_OkObjectResult()
//        {
//            var userCredentials = new UserCredentials(){ UserName = "indra3", NewPassword = "Change@123456" };
//            var result = _passwordManagementController.ResetAnyUserPassword(userCredentials);
//            result.Should().BeOfType<OkObjectResult>();
//        }

//        /// <summary>
//        /// Test case to manage password when user changes password which is present in its recent passwords
//        /// </summary>
//        [Fact]
//        public void PasswordManagement_NewPasswordPresentInPasswordHistory_Returns_BadRequestObjectResult()
//        {
//            var userCredentials = new UserCredentials() { UserName = "indra3", NewPassword = "Change@123" };
//            var result = _passwordManagementController.ResetAnyUserPassword(userCredentials);
//            result.Should().BeOfType<BadRequestObjectResult>();

//        }

//        [Fact]
//        public void LockAccount_WhenUserEntersWrongPasswordDuringPasswordChange_Returns_OkObjectResult()
//        {
//            var userCredentials = new UserCredentials() { UserName = "indra3", OldPassword = "asdsds" };
//            var result = _passwordManagementController.ChangePassword(userCredentials);
//            result.Should().BeOfType<BadRequestObjectResult>();
//        }
//        #region Password Management Old Code
//        ///// <summary>
//        ///// Tests whether ResetPassword method returns ReturnResult object or not and also check whether password is updated or not.
//        ///// </summary>
//        //[Fact]
//        //public void ResetPassword_ReturnsReusultObject()
//        //{
//        //    string newPassword = "sdjvfcsjvhcvjvj@12313";
//        //    string newPasswordHash = _customPasswordHash.ScryptHash(newPassword);

//        //    //Act
//        //    UserCredentials userCredentials = new UserCredentials { UserName = "SuperAdmin11" , OldPassword= "dsjjkgfkuygg_543" };
//        //    var result = _passwordManagementController.ResetPassword(userCredentials);

//        //    var user = _userController.Get(Guid.Parse("03ab0396-82bd-49d8-a893-1340ab727ebb"));

//        //    //Assert
//        //    result.Should().BeOfType<OkObjectResult>();
//        //    user.PasswordHash.Should().BeSameAs(newPasswordHash);
//        //}

//        /// <summary>
//        /// This method is used to test the reset of any user's password by Superadmin.
//        /// </summary>
//        /// <param name="userCredentials">userCredentials object</param>
//        /// <returns>Returns success or failure message</returns>
//        [Fact]
//        public void ResetAnyUserPassword_returnsReturnsOkObjectResult()
//        {
//            var newUserCredentials = new UserCredentials { UserId = new Guid("3631041A-CD44-4C23-3BE1-08D612F3A919"), UserName = "indra3", NewPassword = "PasswordChanged@03" };
//            var result = _passwordManagementController.UpdatePassword(newUserCredentials);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

//        }
//        #endregion

//        #endregion

//    }
//}

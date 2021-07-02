//using FluentAssertions;
//using EVA.EIMS.Entity;
//using EVA.EIMS.Security.API.Controllers;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Text;
//using Xunit;

//namespace EIMSUnitTestProject
//{
//    public class RefreshTokenControllerUnitTest
//    {
//        #region Private Variables
//        private RefreshTokenController _refreshTokenController;
//        #endregion

//        #region Constructor
//        public RefreshTokenControllerUnitTest()
//        {
//            StartupUnitTest<RefreshTokenController> startup = new StartupUnitTest<RefreshTokenController>();

//            _refreshTokenController = startup.ConfigureServices();
//        }
//        #endregion

////        #region Positive Tests
////        /// <summary>
////        /// Tests whether GetRefreshTokenById method returns RefreshToken object or not.
////        /// </summary>
////        [Fact]
////        public void GetRefreshTokenById_ReturnsRefreshTokenObject()
////        {
////            //var result = _refreshTokenController.Get(Guid.Parse("08d5e26e-1dfd-6961-fc59-37ea15d3a29c"));

////            result.Should().BeAssignableTo<RefreshToken>();
////        }

////        /// <summary>
////        /// Tests whether SaveRefreshToken method returns Ok status in response or not.
////        /// </summary>
////        [Fact]
////        public void SaveRefreshToken_ReturnsReturnResult()
////        {
////            var newRefreshToken = new RefreshToken { AppId = 1, OrgId = 1, RefreshAuthenticationTicket = Encoding.ASCII.GetBytes("eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9"), TokenExpirationDateTime = DateTime.Now, UserId = Guid.Parse("03ab0396-82bd-49d8-a893-1340ab727e5a") };

////            //var result = _refreshTokenController.Post(newRefreshToken);

////            result.Should().BeOfType<OkObjectResult>();
////        }

////        /// <summary>
////        /// Tests whether DeleteRefreshToken method returns Ok status in response or not.
////        /// </summary>
////        [Fact]
////        public void DeleteRefreshToken_ReturnsReturnResult()
////        {
////            //var result = _refreshTokenController.Delete(Guid.Parse("08d5e26e-1dfd-6961-fc59-37ea15d3a29c"));

////            result.Should().BeOfType<OkObjectResult>();
////        }
////        #endregion

////        #region Negative Tests
////        /// <summary>
////        /// Tests whether GetRefreshTokenById method returns null in response or not after providing invalid refreshtoken id.
////        /// </summary>
////        [Fact]
////        public void GetRefreshTokenById_NonExistingRefreshToken_ReturnsRefreshTokenObject()
////        {
////            var result = _refreshTokenController.Get(Guid.Parse("03ab0396-82bd-49d8-a893-1340ab727e5a"));

////            result.Should().Be(null);
////        }

////        /// <summary>
////        /// Tests whether DeleteRefreshToken method returns BadRequest status code in response or not after providing invalid refreshtoken id.
////        /// </summary>
////        [Fact]
////        public void DeleteRefreshToken_RefreshTokenNotExist_ReturnsReturnResult()
////        {
////            var result = _refreshTokenController.Delete(Guid.Parse("03ab0396-82bd-49d8-a893-1340ab727e5a"));

////            result.Should().BeOfType<BadRequestObjectResult>();
////        }
////        #endregion
//    }
//}

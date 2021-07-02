//using FluentAssertions;
//using EVA.EIMS.Entity;
//using EVA.EIMS.Security.API.Controllers;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using Xunit;

//namespace EIMSUnitTestProject
//{
//    public class ApplicationControllerUnitTest
//    {
//        #region Private Variables
//        private ApplicationController _applicationController;
//        #endregion

//        #region Constructor
//        public ApplicationControllerUnitTest()
//        {
//            StartupUnitTest<ApplicationController> startup = new StartupUnitTest<ApplicationController>();

//            _applicationController = startup.ConfigureServices();
//        }
//        #endregion

//        #region Positive Tests
//        /// <summary>
//        /// Tests whether GetApplication method returns IEnumerable<Application> or not.
//        /// </summary>
//        [Fact]
//        public void GetApplication_ReturnsIEnumerableApplication()
//        {
//            //Act
//            var result = _applicationController.Get().Result.As<OkObjectResult>();

//            //Assert
//            result.Value.Should().BeAssignableTo<IEnumerable<Application>>();
//        }

//        /// <summary>
//        /// Tests whether GetApplicationById method returns Application object or not.
//        /// </summary>
//        [Fact]
//        public void GetApplicationById_ReturnsApplicationObject()
//        {
//            var result = _applicationController.GetById(1).Result.As<OkObjectResult>();

//            result.Value.Should().BeAssignableTo<Application>();
//        }

//        /// <summary>
//        /// Tests whether SaveApplication method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void SaveApplication_ReturnsReturnResult()
//        {
//           var newApplication = new Application { AppName = "imstest5", AppUrl = "http://", CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now,Description="New app", IsActive=true, ModifiedBy=Guid.NewGuid(), ModifiedOn=DateTime.Now };

//            var result = _applicationController.Post(newApplication).Result.As<OkObjectResult>();

//            result.Should().BeOfType<OkObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether DeleteApplication method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void DeleteApplication_ReturnsReturnResult()
//        {
//            var result = _applicationController.Delete(1);

//            result.Should().BeOfType<OkObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether UpdateApplication method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void UpdateApplication_ReturnsReturnResult()
//        {
//            var newApplication = new Application { AppName = "ims", AppUrl = "http://", CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, Description = "New app", IsActive = true, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _applicationController.Put(newApplication);

//            result.Should().BeOfType<OkObjectResult>();
//        }
//        #endregion

//        #region Negative Tests
//        /// <summary>
//        /// Tests whether GetApplicationById method returns null in response or not after providing invalid application id.
//        /// </summary>
//        [Fact]
//        public void GetApplicationById_NonExistingApplication_ReturnsApplicationObject()
//        {
//            var result = _applicationController.GetById(50).Result.As<OkObjectResult>();

//            result.Should().Be(null);
//        }

//        /// <summary>
//        /// Tests whether SaveApplication method returns BadRequest status code in response or not after providing information matching with existing application.
//        /// </summary>
//        [Fact]
//        public void SaveApplication_DuplicateApplication_ReturnsReturnResult()
//        {
//            var newApplication = new Application { AppName = "imstest", AppUrl = "http://", CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, Description = "New app", IsActive = true, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _applicationController.Post(newApplication).Result.As<OkObjectResult>();

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether SaveApplication method returns BadRequest status code in response or not after not providing application name.
//        /// </summary>
//        [Fact]
//        public void SaveApplication_ApplicationNameMissing_ReturnsReturnResult()
//        {
//            var newApplication = new Application { AppUrl = "http://", CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, Description = "New app", IsActive = true, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _applicationController.Post(newApplication).Result;

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether DeleteApplication method returns BadRequest status code in response or not after providing invalid application id.
//        /// </summary>
//        [Fact]
//        public void DeleteApplication_ApplicationNotExist_ReturnsReturnResult()
//        {
//            var result = _applicationController.Delete(50);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether UpdateApplication method returns BadRequest status code in response or not after providing invalid application id.
//        /// </summary>
//        [Fact]
//        public void UpdateApplication_ApplicationNotExist_ReturnsReturnResult()
//        {
//            var newApplication = new Application { AppName = "ims", AppUrl = "http://", CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, Description = "New app", IsActive = true, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _applicationController.Put(newApplication);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }
//        #endregion
//    }
//}

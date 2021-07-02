//using FluentAssertions;
//using EVA.EIMS.Entity;
//using EVA.EIMS.Security.API.Controllers;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using Xunit;

//namespace EIMSUnitTestProject
//{
//    public class IPTableControllerUnitTest
//    {
//        #region Private Variables
//        private readonly IPTableController _iPTableController;

//        #endregion

//        #region Constructor
//        public IPTableControllerUnitTest()
//        {
//            StartupUnitTest<IPTableController> startUp = new StartupUnitTest<IPTableController>();

//            _iPTableController = startUp.ConfigureServices();
//        }

//        #endregion

//        /// <summary>
//        /// This method is used to test to get all IP Table Details
//        /// </summary>
//        /// <returns>returns IActionResult IP Table</returns>
//        [Fact]
//        public void GetAllIPTableDetails_ReturnsIActionResult_IPTable()
//        {
//            var result = _iPTableController.Get();
//            //Assert
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

//        }

//        /// <summary>
//        /// This method is used to test to get the IP Table details by id
//        /// </summary>
//        /// <param name="ipAddressId">ipAddressId</param>
//        /// <returns>returns security question type object</returns>
//        [Fact]


//        public void GetSpecificIPTableDetails_BasedOnipAddressId_ReturnsIPTable()
//        {
//           var result = _iPTableController.GetById(6);

//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

//        }

//        /// <summary>
//        /// This method is used to test to save the IP Table details
//        /// </summary>
//        /// <param name="iPTable"> iPTable object</param>
//        /// <returns>returns OkObjectResult</returns>

//        [Fact]
//        public void SaveIPTableDetails_ReturnsOkObject()
//        {
//            var newIpDetails = new IPTable { AppId = 10, OrgId = 3, IPStartAddress = "10.240.255.111", IPEndAddress = "10.240.255.115", GatewayDeviceId = 1, IPProxyAddress = "10.10.5.18", IsIPAllowed = true, PortNo = 587 };
//            var result = _iPTableController.Post(newIpDetails);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

//        }

//        /// <summary>
//        /// This method is used test to update the IP Table details
//        /// </summary>
//        /// <param name="quesionId">quesionId</param>
//        /// <param name="updatedQuestion">securityQuestion object</param>
//        /// <returns>returns OkObjectResult</returns>
//        [Fact]
//        public void UpdateIPTable_BasedOnIPAddressId_ReturnsOkObject()
//        {
//            var updatedIpDetails = new IPTable { IPAddressId = 15, IsProxyEnabled = false, AppId = 10, OrgId = 3, IPStartAddress = "10.240.255.111", IPEndAddress = "10.240.255.115", GatewayDeviceId = 1, IPProxyAddress = "10.10.5.18", IsIPAllowed = true, PortNo = 587, IsActive = true };
//            var result = _iPTableController.Put(updatedIpDetails);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// This method is used to test to delete the IP Table details
//        /// </summary>
//        /// <param name="questionId">questionId</param>
//        /// <returns>returns OkObjectResult</returns>
//        [Fact]
//        public void DeleteIPTableDetails_BasedOnIpAddressId_ReturnsOkObject()
//        {
//            var result = _iPTableController.Delete(15);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// This method is used for negative test while saving a IP Table Detail
//        /// </summary>
//        /// <returns>returns BadRequestObjectResult</returns>
//        [Fact]
//        public void SaveIPTableDetails_ReturnsBadRequest()
//        {
//            var newIpDetails = new IPTable { AppId = 10, OrgId = 3, IPStartAddress = "11.240.255.111", IPEndAddress = "09.240.255.111", GatewayDeviceId = 1, IPProxyAddress = "10.10.5.18", IsIPAllowed = true, PortNo = 587 };
//            var result = _iPTableController.Post(newIpDetails);
//            var okResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;

//        }
//    }

//}

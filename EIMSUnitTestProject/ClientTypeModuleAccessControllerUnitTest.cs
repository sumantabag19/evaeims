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
//    public class ClientTypeModuleAccessControllerUnitTest
//    {
//        #region Private Variables
//        private ClientTypeModuleAccessController _clientTypeModuleAccess;
//        #endregion

//        #region Constructor
//        public ClientTypeModuleAccessControllerUnitTest()
//        {
//            StartupUnitTest<ClientTypeModuleAccessController> startup = new StartupUnitTest<ClientTypeModuleAccessController>();

//            _clientTypeModuleAccess = startup.ConfigureServices();
//        }
//        #endregion

//        #region Positive Tests
//        /// <summary>
//        /// Tests whether GetClient method returns IEnumerable<Client> or not.
//        /// </summary>
//        [Fact]
//        public void GetClientTypeModuleAccess_ReturnsIEnumerableClientTypeModuleAccess()
//        {
//            //Act
//            var result = _clientTypeModuleAccess.Get();

//            //Assert
//            var okResult = result.Should().BeAssignableTo<IEnumerable<ClientTypeModuleAccess>>();
//        }

//        /// <summary>
//        /// Tests whether ClientTypeModuleAccessById method returns Client object or not.
//        /// </summary>
//        [Fact]
//        public void GetByAccessId_ReturnsClientObject()
//        {
//            var result = _clientTypeModuleAccess.GetbyId(2);

//            var okResult = result.Should().BeAssignableTo<ClientTypeModuleAccess>().Subject;

//            okResult.Should().NotBeNull();
//        }

//        /// <summary>
//        /// Tests whether GetClientById method returns Client object or not.
//        /// </summary>
//        [Fact]
//        public void GetByClientTypeId_ReturnsClientObject()
//        {
//            var result = _clientTypeModuleAccess.GetbyClientTypeId(2);

//            var okResult = result.Should().BeAssignableTo<ClientTypeModuleAccess>().Subject;

//            okResult.Should().NotBeNull();
//        }


//        /// <summary>
//        /// Tests whether SaveClientTypeModuleAccess method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void SaveClientTypeModuleAccess_ReturnsReturnResult()
//        {
//            var newClientModuleAccess = new ClientTypeModuleAccessModel
//            {
//                ClientTypeId = 4,
//                ModuleId = 1,
//                ReadAccess = true,
//                WriteAccess = false,
//                EditAccess = false,
//                DeleteAccess = false,
//                IsActive = true,
//            };

//            var result = _clientTypeModuleAccess.Post(newClientModuleAccess);

//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Tests whether SaveClientTypeModuleAccess method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void SaveMultipleClientTypeModuleAccess_ReturnsReturnResult()
//        {
//            List<ClientTypeModuleAccessModel> newClientModuleAccess = new List<ClientTypeModuleAccessModel>();
//            newClientModuleAccess.Add(new ClientTypeModuleAccessModel
//            {
//                ClientTypeId = 4,
//                ModuleId = 1,
//                ReadAccess = true,
//                WriteAccess = false,
//                EditAccess = false,
//                DeleteAccess = false,
//                IsActive = true,
//            });

//            var result = _clientTypeModuleAccess.PostRange(newClientModuleAccess);

//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Tests whether UpdateClientTypeModuleAccess method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void UpdateClientTypeModuleAccess_ReturnsReturnResult()
//        {
//            var newClientModuleAccess = new ClientTypeModuleAccessModel
//            {
//                ClientTypeId = 4,
//                ModuleId = 1,
//                ReadAccess = true,
//                WriteAccess = false,
//                EditAccess = false,
//                DeleteAccess = false,
//                IsActive = true,
//            };

//            var result = _clientTypeModuleAccess.Put(1, newClientModuleAccess);

//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Tests whether UpdateClientTypeModuleAccess method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void UpdateMultipleClientTypeModuleAccess_ReturnsReturnResult()
//        {
//            List<ClientTypeModuleAccessModel> newClientModuleAccess = new List<ClientTypeModuleAccessModel>();
//            newClientModuleAccess.Add(new ClientTypeModuleAccessModel
//            {
//                ClientTypeId = 4,
//                ModuleId = 1,
//                ReadAccess = true,
//                WriteAccess = false,
//                EditAccess = false,
//                DeleteAccess = false,
//                IsActive = true,
//            });

//            var result = _clientTypeModuleAccess.PutRange(4, newClientModuleAccess);

//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Tests whether DeleteClientTypeMooduleAccess method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void DeleteClientTypeModuleAccess_ReturnsReturnResult()
//        {
//            var result = _clientTypeModuleAccess.Delete(1);

//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Tests whether DeleteClientTypeMooduleAccess method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void DeleteRangeClientTypeModuleAccess_ReturnsReturnResult()
//        {
//            var result = _clientTypeModuleAccess.DeleteRange(11);

//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }
//        #endregion

//        #region Negative Tests
//        /// <summary>
//        /// Tests whether GetClientById method returns null in response or not after providing invalid client id.
//        /// </summary>
//        [Fact]
//        public void GetByClientTypeAccessId_NonExistingClient_ReturnsClientObject()
//        {
//            var result = _clientTypeModuleAccess.GetbyId(1234567);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        #region Negative Tests
//        /// <summary>
//        /// Tests whether GetClientTypeModuleAccessById method returns null in response or not after providing invalid client id.
//        /// </summary>
//        [Fact]
//        public void GetbyClientTypeId_NonExistingClient_ReturnsClientObject()
//        {
//            var result = _clientTypeModuleAccess.GetbyClientTypeId(1234567);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether SaveClientTypeModuleAccess  method returns BadRequest status code in response or not after not providing client id.
//        /// </summary>
//        [Fact]
//        public void SaveClientTypeModuleAccess_DuplicateClientType_ReturnsReturnResult()
//        {
//            var newClientModuleAccess = new ClientTypeModuleAccessModel
//            {
//                ClientTypeId = 4,
//                ModuleId = 1,
//                ReadAccess = true,
//                WriteAccess = false,
//                EditAccess = false,
//                DeleteAccess = false,
//                IsActive = true,
//            };
//            var result = _clientTypeModuleAccess.Post(newClientModuleAccess);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether SaveClientTypeModuleAccess  method returns BadRequest status code in response or not after not providing client id.
//        /// </summary>
//        [Fact]
//        public void SaveClientTypeModuleAccess_ClientTypeIdMissing_ReturnsReturnResult()
//        {
//            var newClientModuleAccess = new ClientTypeModuleAccessModel
//            {
//                ClientTypeId = 0,
//                ModuleId = 1,
//                ReadAccess = true,
//                WriteAccess = false,
//                EditAccess = false,
//                DeleteAccess = false,
//                IsActive = true,
//            };
//            var result = _clientTypeModuleAccess.Post(newClientModuleAccess);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether UpdateClientTypeModuleAccess method returns BadRequest status code in response or not after not providing client id.
//        /// </summary>
//        [Fact]
//        public void UpdateClientTypeModuleAccess_ClientTypeIdMissing_ReturnsReturnResult()
//        {
//            var newClientModuleAccess = new ClientTypeModuleAccessModel
//            {
//                ClientTypeId = 4,
//                ModuleId = 1,
//                ReadAccess = true,
//                WriteAccess = false,
//                EditAccess = false,
//                DeleteAccess = false,
//                IsActive = true,
//            };
//            var result = _clientTypeModuleAccess.Put(1234, newClientModuleAccess);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether DeleteClient method returns BadRequest status code in response or not after providing invalid client id.
//        /// </summary>
//        [Fact]
//        public void DeleteClientTypeModuleAccess_ClientTypeModuleAccessIdNotExist_ReturnsReturnResult()
//        {
//            var result = _clientTypeModuleAccess.Delete(123456);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }
//        /// <summary>
//        /// Tests whether DeleteClient method returns BadRequest status code in response or not after providing invalid client id.
//        /// </summary>
//        [Fact]
//        public void DeleteClientTypeModuleAccess_ClientTypeIdNotExist_ReturnsReturnResult()
//        {
//            var result = _clientTypeModuleAccess.DeleteRange(123456);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }
//        #endregion
//        #endregion
//    }
//}

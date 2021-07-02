//using FluentAssertions;
//using EVA.EIMS.Common;
//using EVA.EIMS.Data;
//using EVA.EIMS.Entity;
//using EVA.EIMS.Helper;
//using EVA.EIMS.Security.API;
//using EVA.EIMS.Security.API.Controllers;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Routing;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using Xunit;

//namespace EIMSUnitTestProject
//{
//    public class ClientTypeControllerUnitTest
//    {
//        #region Private Variables
//        private ClientTypeController _clientTypeController;
//        #endregion

//        #region Constructor


//        public ClientTypeControllerUnitTest()
//        {
//            StartupUnitTest<ClientTypeController> startUp = new StartupUnitTest<ClientTypeController>();

//            _clientTypeController = startUp.ConfigureServices(); 
//        }

//        #endregion

//        #region Positive tests
//        /// <summary>
//        /// Tests whether GetClientType method returns IEnumerable<ClientType> or not.
//        /// </summary>
//        [Fact]
//        public void GetClientType_ReturnsIEnumerableClientType()
//        {
//            //Act
//            var result = _clientTypeController.Get();

//            //Assert
//            var okResult = result.Should().BeAssignableTo<IEnumerable<ClientType>>().Subject;
//        }

//        /// <summary>
//        /// Tests whether GetClientTypeById method returns ClientType object or not.
//        /// </summary>
//        [Fact]
//        public void GetClientTypeById_ReturnsClientTypeObject()
//        {
//            var result = _clientTypeController.Get(1);

//            result.Should().BeAssignableTo<ClientType>();
//        }

//        /// <summary>
//        /// Tests whether SaveClientType method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void SaveClientType_ReturnsReturnResult()
//        {
//            var newClientType = new ClientType { ClientTypeName = "newclienttype1", CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, Description = "testclienttype", IsActive = true, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _clientTypeController.Post(newClientType);

//            result.Should().BeOfType<OkObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether DeleteClientType method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void DeleteClientType_ReturnsReturnResult()
//        {
//            var result = _clientTypeController.Delete(1);

//            result.Should().BeOfType<OkObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether UpdateClientType method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void UpdateClientType_ReturnsReturnResult()
//        {
//            var newClientType = new ClientType { ClientTypeName = "testclienttype", CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, Description = "testclienttype", IsActive = true, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _clientTypeController.Put(newClientType);

//            result.Should().BeOfType<OkObjectResult>();
//        }
//        #endregion

//        #region Negative Tests
//        /// <summary>
//        /// Tests whether GetClientTypeById method returns null in response or not after providing invalid clienttype id.
//        /// </summary>
//        [Fact]
//        public void GetClientTypeById_NonExistingClientType_ReturnsClientTypeObject()
//        {
//            var result = _clientTypeController.Get(50);

//            result.Should().Be(null);
//        }

//        /// <summary>
//        /// Tests whether SaveClientType method returns BadRequest status code in response or not after providing information matching with existing clienttype.
//        /// </summary>
//        [Fact]
//        public void SaveClientType_DuplicateClientType_ReturnsReturnResult()
//        {
//            var newClientType = new ClientType { ClientTypeName = "webui", CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, Description = "New app", IsActive = true, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _clientTypeController.Post(newClientType);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether SaveClientType method returns BadRequest status code in response or not after not providing clienttype name.
//        /// </summary>
//        [Fact]
//        public void SaveClientType_ClientTypeNameMissing_ReturnsReturnResult()
//        {
//            var newClientType = new ClientType { CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, Description = "test client", IsActive = true, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _clientTypeController.Post(newClientType);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether DeleteClientType method returns BadRequest status code in response or not after providing invalid clienttype id.
//        /// </summary>
//        [Fact]
//        public void DeleteClientType_ClientTypeNotExist_ReturnsReturnResult()
//        {
//            var result = _clientTypeController.Delete(50);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether UpdateClientType method returns BadRequest status code in response or not after providing invalid clienttype id.
//        /// </summary>
//        [Fact]
//        public void UpdateClientType_ClientTypeNotExist_ReturnsReturnResult()
//        {
//            var newClientType = new ClientType { ClientTypeName = "testupdateclient", CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, Description = "test clienttype", IsActive = true, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _clientTypeController.Put(newClientType);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }
//        #endregion
//    }
//}

//using FluentAssertions;
//using EVA.EIMS.Entity;
//using EVA.EIMS.Security.API.Controllers;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using Xunit;
//using Xunit.Abstractions;

//namespace EIMSUnitTestProject
//{
   
//    public class ClientControllerUnitTest:ITestCollectionOrderer
//    {
//        #region Private Variables
//        private ClientController _clientController;
//        #endregion

//        #region Constructor
//        public ClientControllerUnitTest()
//        {
//            StartupUnitTest<ClientController> startup = new StartupUnitTest<ClientController>();

//            _clientController = startup.ConfigureServices();
//        }
//        #endregion

//        #region Positive Tests
//        /// <summary>
//        /// Tests whether GetClient method returns IEnumerable<Client> or not.
//        /// </summary>
//        [Fact]          
//        public void aGetClient_ReturnsIEnumerableClient()
//        {
//            //Act
//            var result = _clientController.Get().Result.As<OkObjectResult>();

//            //Assert
//            var okResult = result.Value.Should().BeAssignableTo<IEnumerable<OauthClient>>();
//        }

//        /// <summary>
//        /// Tests whether GetClientById method returns Client object or not.
//        /// </summary>
//        [Fact]
//        public void bGetClientById_ReturnsClientObject()
//        {
//            var result = _clientController.Get("mem.cloud.UI.web").Result.As<OkObjectResult>();

//            var okResult = result.Value.Should().BeAssignableTo<OauthClient>().Subject;

//            okResult.Should().NotBeNull();
//        }

//        /// <summary>
//        /// Tests whether SaveClient method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void cSaveClient_ReturnsReturnResult()
//        {
//            //var temp = _clientController.Delete("mem.cloud").Result;
//            string clientId = "1.test" + DateTime.Now.ToString();

//            var newClient = new OauthClient { ClientName = clientId, AllowedScopes = "read,write", ClientSecret = "IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Xaw", ClientTypeId = 1, Flow = "", ClientId = clientId, AppId = 1, CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, IsActive = true, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _clientController.Post(newClient).Result;

//            StartupUnitTest<ClientController> startup = new StartupUnitTest<ClientController>();

//           var _clientController2 = startup.ConfigureServices();
//            var result1 = _clientController2.Delete(clientId).Result;

//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

//        }


//        /// <summary>
//        /// Tests whether UpdateClient method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void dUpdateClient_ReturnsReturnResult()
//        {
//            var newClient = new OauthClient { ClientName = "newclienttype1", AllowedScopes = "read,write", ClientSecret = "IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Xaw", ClientTypeId = 1, Flow = "test", ClientId = "mem.cloud", AppId = 1, CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, IsActive = true, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now,DeleteRefreshToken=true,TokenValidationPeriod=5,ClientValidationPeriod=5 };

//            var result = _clientController.Put(newClient).Result;

//            result.Should().BeOfType<OkObjectResult>();
//        }
//        #endregion

//        #region Negative Tests
//        /// <summary>
//        /// Tests whether GetClientById method returns null in response or not after providing invalid client id.
//        /// </summary>
//        [Fact]
//        public void eGetClientById_NonExistingClient_ReturnsClientObject()
//        {
//            var result = _clientController.Get("abc.sdf.xyz").Result.As<OkObjectResult>();

//            result.Should().Be(null);
//        }

//        /// <summary>
//        /// Tests whether SaveClient method returns BadRequest status code in response or not after providing information matching with existing client.
//        /// </summary>
//        [Fact]
//        public void fSaveClient_DuplicateClient_ReturnsReturnResult()
//        {
//            var newClient = new OauthClient { ClientName = "newclienttype1", AllowedScopes = "read,write", ClientSecret = "IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Xaw", ClientTypeId = 1, Flow = "", ClientId = "mem.cloud", AppId = 1, CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, IsActive = true, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _clientController.Post(newClient).Result;

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether SaveClient method returns BadRequest status code in response or not after not providing client id.
//        /// </summary>
//        [Fact]
//        public void gSaveClient_ClientIdMissing_ReturnsReturnResult()
//        {
//            var newClient = new OauthClient { ClientName = "newclienttype1", AllowedScopes = "read,write", ClientSecret = "IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Xaw", ClientTypeId = 1, Flow = "", AppId = 1, CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, IsActive = true, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _clientController.Post(newClient).Result;

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether DeleteClient method returns BadRequest status code in response or not after providing invalid client id.
//        /// </summary>
//        [Fact]
//        public void hDeleteClient_ClientNotExist_ReturnsReturnResult()
//        {
//            var result = _clientController.Delete("mem.cloud.123").Result;

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether UpdateClient method returns BadRequest status code in response or not after providing invalid client id.
//        /// </summary>
//        [Fact]
//        public void iUpdateClient_ClientNotExist_ReturnsReturnResult()
//        {
//            var newClient = new OauthClient { ClientName = "newclienttype1", AllowedScopes = "read,write", ClientSecret = "IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Xaw", ClientTypeId = 1, Flow = "", AppId = 1, CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, IsActive = true, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _clientController.Put(newClient).Result;

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests method to save client dynamically by superadmin
//        /// </summary>
//        [Fact]
//        public void jSaveClientDynamically_BySuperAdmin_ReturnsReturnResult()
//        {

//            string clientId = "1.test"+ DateTime.Now.ToString();
//            var newClient = new OauthClient { ClientId = clientId, ClientSecret = "IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Abc", ClientName = "ims test client",AllowedScopes = "read write", ClientTypeId = 1, Flow = "password", AppId = 1, CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, IsActive = true, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _clientController.CreateDynamicClient(newClient).Result;

//            StartupUnitTest<ClientController> startup = new StartupUnitTest<ClientController>();

//            var _clientController2 = startup.ConfigureServices();
//            var result1 = _clientController2.Delete(clientId).Result;

//            result.Should().BeOfType<OkObjectResult>();
//        }


//        /// <summary>
//        /// Tests method to update client dynamically by superadmin when deleterefreshtoken field not given
//        /// </summary>
//        [Fact]
//        public void kUpdateClientDynamically_BySuperAdmin_DeleteRefreshTokenNotGiven_ReturnsReturnResult()
//        {
//            var newClient = new OauthClient { ClientId = "ims.test", ClientSecret = "IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Mno", ClientName = "ims changed client", AllowedScopes = "read write uiapi_all", ClientTypeId = 2, Flow = "password", AppId = 9,ClientValidationPeriod = 10, ClientExpireOn = DateTime.Parse("2018-10-19 11:21:44.267"),CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, IsActive = true, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _clientController.Put(newClient).Result;

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests method to update client dynamically by superadmin when IsActive field not given
//        /// </summary>
//        [Fact]
//        public void lUpdateClientDynamically_BySuperAdmin_IsActiveFieldNotGiven_ReturnsReturnResult()
//        {
//            var newClient = new OauthClient { ClientId = "ims.test", ClientSecret = "IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Mno", ClientName = "ims changed client", AllowedScopes = "read write uiapi_all", ClientTypeId = 2, Flow = "password", AppId = 9, ClientValidationPeriod = 10, ClientExpireOn = DateTime.Parse("2018-10-19 11:21:44.267"), DeleteRefreshToken = false, CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _clientController.Put(newClient).Result;

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests method to update client dynamically by superadmin
//        /// </summary>
//        //[Fact]
//        //public void UpdateClientDynamically_BySuperAdmin_ReturnsReturnResult()
//        //{
//        //    var newClient = new OauthClient { ClientId = "ims.test", ClientSecret = "IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Xyz", ClientName = "ims changed client", AllowedScopes = "read write uiapi_all", ClientTypeId = 2, Flow = "password", AppId = 9, ClientValidationPeriod = 10, ClientExpireOn = DateTime.Parse("2018-10-19 11:21:44.267"), DeleteRefreshToken = false,IsActive = true, CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//        //    var result = _clientController.CreateDynamicClient(newClient).Result;

//        //    result.Should().BeOfType<OkObjectResult>();
//        //}

//        /// <summary>
//        /// Tests method to update client dynamically by service client type when client already exists
//        /// </summary>
//        [Fact]
//        public void mSaveClientDynamically_ByServiceClientType_ClientAlreadyExists_ReturnsReturnResult()
//        {
//            var newClient = new OauthClient { ClientId = "ims.test" };

//            var result = _clientController.CreateDynamicClient(newClient).Result;

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }
//        /// <summary>
//        /// Tests method to update client dynamically by service client type
//        /// </summary>
//        //[Fact]
//        //public void SaveClientDynamically_ByServiceClientType_ReturnsReturnResult()
//        //{
//        //    var newClient = new OauthClient { ClientId = "ims.test"};

//        //    var result = _clientController.CreateDynamicClient(newClient).Result;

//        //    result.Should().BeOfType<OkObjectResult>();
//        //}

//        /// <summary>
//        /// Tests whether DeleteClient method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void nDeleteClient_ReturnsReturnResult()
//        {
//            var result = _clientController.Delete("mem.cloud").Result;

//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }
//        /// <summary>
//        /// Tests whether DeleteClient method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void oDeleteClientDynamicallybeforeupdate_ReturnsReturnResult()
//        {
//            var result = _clientController.Delete("ims.test").Result;

//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections)
//        {
            
//            throw new NotImplementedException();
//        }
//        ///// <summary>
//        ///// Tests whether DeleteClient method returns Ok status in response or not.
//        ///// </summary>
//        //[Fact]
//        //public void DeleteClientDynamicallyafterupdate_ReturnsReturnResult()
//        //{
//        //    var result = _clientController.Delete("ims changed client").Result;

//        //    var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        //}
//        #endregion
//    }
//}

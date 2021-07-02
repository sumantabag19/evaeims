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
//using System.Collections.Generic;
//using System.IO;
//using Xunit;

//namespace EIMSUnitTestProject
//{
//    public class RoleControllerUnitTest
//    {
//        #region Private Variables
//        private RoleController _roleController;
//        #endregion

//        #region Constructor
//        public RoleControllerUnitTest()
//        {
//            StartupUnitTest<RoleController> startUp = new StartupUnitTest<RoleController>();

//            _roleController = startUp.ConfigureServices();
//        }
//        #endregion


//        [Fact]
//        public void GetAllRole_ReturnsIEnumerableRole()
//        {
//            var result = _roleController.Get();
//            var okResult = result.Should().BeAssignableTo<IEnumerable<Role>>().Subject;
//            //okResult.Count().Should().Be();  
//        }

//        [Fact]
//        public void GetRoleById_ReturnsRole()
//        {
//            var result = _roleController.Get(1);
//            var okResult = result.Should().BeOfType<Role>().Subject;
//        }

//        [Fact]
//        public void GetRoleById_ReturnsNull_WhenRoleIdDoesNotExists ()
//        {
//            var result = _roleController.Get(7);
//            var okResult = result.Should().BeNull();
//        }

//        [Fact]
//        public void GetRoleById_ReturnsNull_WhenEmpltyRoleIdPassed()
//        {
//            var result = _roleController.Get(0);
//            var okResult = result.Should().BeNull();
//        }

//        [Fact]
//        public void SaveRole_ReturnsOkObjectResult()
//        {
//            var newRole = new Role { RoleId = 6, RoleName = "User", Description = "End User"};
//            var result = _roleController.Post(newRole);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;



//            //var newRole3 = new Role { RoleId = 4, RoleName = "User", Description = "End User"};
//            //var result3 = _roleController.Post(newRole);
//            //var okResult3 = result3.Should().BeOfType<ObjectResult>().Subject;
//        }

//        [Fact]
//        public void SaveRole_ReturnsBadRequestObjectResult_WhenIdAlreadyExists()
//        {
//            var newRole = new Role { RoleId = 3, RoleName = "User", Description = "End User"};
//            var result = _roleController.Post(newRole);
//            var Result = result.Should().BeOfType<BadRequestObjectResult>().Subject;
//        }

//        [Fact]
//        public void SaveRole_ReturnsBadRequestObjectResult_WhenRollIsNull()
//        {
//            Role newRole = null;
//            var result = _roleController.Post(newRole);
//            var Result = result.Should().BeOfType<BadRequestObjectResult>().Subject;
//        }

//        //    [Fact]
//        //    public void DeleteDevice()
//        //    {
//        //        var result = _deviceController.Delete("device013.tdbank");
//        //        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        //    }

//        //    [Fact]
//        //    public void UpdateDevice()
//        //    {
//        //        var newdevice = new DeviceModel { DeviceId = "device012.tdbank", ClientTypeId = 2, OrgId = "eims.eva.com", IsUsed = false };
//        //        var result = _deviceController.Put("device012.tdbank", newdevice);
//        //        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        //    }


//        //}

//    }

//}
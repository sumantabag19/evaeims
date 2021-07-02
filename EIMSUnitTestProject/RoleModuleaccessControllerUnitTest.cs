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
//    public class RoleModuleAccessControllerUnitTest
//    {
//        #region Private Variables
//        private RoleModuleAccessController _roleModuleAccess;
//        #endregion

//        #region Constructor
//        public RoleModuleAccessControllerUnitTest()
//        {
//            StartupUnitTest<RoleModuleAccessController> startup = new StartupUnitTest<RoleModuleAccessController>();

//            _roleModuleAccess = startup.ConfigureServices();
//        }
//        #endregion

//        #region Positive Tests
//        /// <summary>
//        /// Tests whether GetRole method returns IEnumerable<Role> or not.
//        /// </summary>
//        [Fact]
//        public void GetRoleModuleAccess_ReturnsIEnumerableRoleModuleAccess()
//        {
//            //Act
//            var result = _roleModuleAccess.Get();

//            //Assert
//            var okResult = result.Should().BeAssignableTo<IEnumerable<RoleModuleAccess>>();
//        }

//        /// <summary>
//        /// Tests whether RoleModuleAccessById method returns Role object or not.
//        /// </summary>
//        [Fact]
//        public void GetByAccessId_ReturnsRoleObject()
//        {
//            var result = _roleModuleAccess.GetbyId(2);

//            var okResult = result.Should().BeAssignableTo<RoleModuleAccess>().Subject;

//            okResult.Should().NotBeNull();
//        }

//        /// <summary>
//        /// Tests whether GetRoleById method returns Role object or not.
//        /// </summary>
//        [Fact]
//        public void GetByRoleId_ReturnsRoleObject()
//        {
//            var result = _roleModuleAccess.GetbyRoleId(2);

//            var okResult = result.Should().BeAssignableTo<RoleModuleAccess>().Subject;

//            okResult.Should().NotBeNull();
//        }


//        /// <summary>
//        /// Tests whether SaveRoleModuleAccess method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void SaveRoleModuleAccess_ReturnsReturnResult()
//        {
//            var newRoleModuleAccess = new RoleModuleAccessModel
//            {
//                RoleId = 4,
//                ModuleId = 1,
//                ReadAccess = true,
//                WriteAccess = false,
//                EditAccess = false,
//                DeleteAccess = false,
//                IsActive = true,
//            };

//            var result = _roleModuleAccess.Post(newRoleModuleAccess);

//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Tests whether SaveRoleModuleAccess method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void SaveMultipleRoleModuleAccess_ReturnsReturnResult()
//        {
//            List<RoleModuleAccessModel> newRoleModuleAccess = new List<RoleModuleAccessModel>();
//            newRoleModuleAccess.Add(new RoleModuleAccessModel
//            {
//                RoleId = 4,
//                ModuleId = 1,
//                ReadAccess = true,
//                WriteAccess = false,
//                EditAccess = false,
//                DeleteAccess = false,
//                IsActive = true,
//            });

//            var result = _roleModuleAccess.PostRange(newRoleModuleAccess);

//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Tests whether UpdateRoleModuleAccess method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void UpdateRoleModuleAccess_ReturnsReturnResult()
//        {
//            var newRoleModuleAccess = new RoleModuleAccessModel
//            {
//                RoleId = 4,
//                ModuleId = 1,
//                ReadAccess = true,
//                WriteAccess = false,
//                EditAccess = false,
//                DeleteAccess = false,
//                IsActive = true,
//            };

//            var result = _roleModuleAccess.Put(1, newRoleModuleAccess);

//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Tests whether UpdateRoleModuleAccess method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void UpdateMultipleRoleModuleAccess_ReturnsReturnResult()
//        {
//            List<RoleModuleAccessModel> newRoleModuleAccess = new List<RoleModuleAccessModel>();
//            newRoleModuleAccess.Add(new RoleModuleAccessModel
//            {
//                RoleId = 4,
//                ModuleId = 1,
//                ReadAccess = true,
//                WriteAccess = false,
//                EditAccess = false,
//                DeleteAccess = false,
//                IsActive = true,
//            });

//            var result = _roleModuleAccess.PutRange(4, newRoleModuleAccess);

//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Tests whether DeleteRoleMooduleAccess method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void DeleteRoleModuleAccess_ReturnsReturnResult()
//        {
//            var result = _roleModuleAccess.Delete(1);

//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Tests whether DeleteRoleMooduleAccess method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void DeleteRangeRoleModuleAccess_ReturnsReturnResult()
//        {
//            var result = _roleModuleAccess.DeleteRange(11);

//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }
//        #endregion

//        #region Negative Tests
//        /// <summary>
//        /// Tests whether GetRoleById method returns null in response or not after providing invalid Role id.
//        /// </summary>
//        [Fact]
//        public void GetByRoleAccessId_NonExistingRole_ReturnsRoleObject()
//        {
//            var result = _roleModuleAccess.GetbyId(1234567);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        #region Negative Tests
//        /// <summary>
//        /// Tests whether GetRoleModuleAccessById method returns null in response or not after providing invalid Role id.
//        /// </summary>
//        [Fact]
//        public void GetbyRoleId_NonExistingRole_ReturnsRoleObject()
//        {
//            var result = _roleModuleAccess.GetbyRoleId(1234567);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether SaveRoleModuleAccess  method returns BadRequest status code in response or not after not providing Role id.
//        /// </summary>
//        [Fact]
//        public void SaveRoleModuleAccess_DuplicateRole_ReturnsReturnResult()
//        {
//            var newRoleModuleAccess = new RoleModuleAccessModel
//            {
//                RoleId = 4,
//                ModuleId = 1,
//                ReadAccess = true,
//                WriteAccess = false,
//                EditAccess = false,
//                DeleteAccess = false,
//                IsActive = true,
//            };
//            var result = _roleModuleAccess.Post(newRoleModuleAccess);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether SaveRoleModuleAccess  method returns BadRequest status code in response or not after not providing Role id.
//        /// </summary>
//        [Fact]
//        public void SaveRoleModuleAccess_roleIdMissing_ReturnsReturnResult()
//        {
//            var newRoleModuleAccess = new RoleModuleAccessModel
//            {
//                RoleId = 0,
//                ModuleId = 1,
//                ReadAccess = true,
//                WriteAccess = false,
//                EditAccess = false,
//                DeleteAccess = false,
//                IsActive = true,
//            };
//            var result = _roleModuleAccess.Post(newRoleModuleAccess);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether UpdateRoleModuleAccess method returns BadRequest status code in response or not after not providing Role id.
//        /// </summary>
//        [Fact]
//        public void UpdateRoleModuleAccess_roleIdMissing_ReturnsReturnResult()
//        {
//            var newRoleModuleAccess = new RoleModuleAccessModel
//            {
//                RoleId = 4,
//                ModuleId = 1,
//                ReadAccess = true,
//                WriteAccess = false,
//                EditAccess = false,
//                DeleteAccess = false,
//                IsActive = true,
//            };
//            var result = _roleModuleAccess.Put(1234, newRoleModuleAccess);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether DeleteRole method returns BadRequest status code in response or not after providing invalid Role id.
//        /// </summary>
//        [Fact]
//        public void DeleteRoleModuleAccess_roleModuleAccessIdNotExist_ReturnsReturnResult()
//        {
//            var result = _roleModuleAccess.Delete(123456);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }
//        /// <summary>
//        /// Tests whether DeleteRole method returns BadRequest status code in response or not after providing invalid Role id.
//        /// </summary>
//        [Fact]
//        public void DeleteRoleModuleAccess_roleIdNotExist_ReturnsReturnResult()
//        {
//            var result = _roleModuleAccess.DeleteRange(123456);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }
//        #endregion
//        #endregion
//    }
//}

//using FluentAssertions;
//using EVA.EIMS.Entity;
//using EVA.EIMS.Security.API.Controllers;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using Xunit;

//namespace EIMSUnitTestProject
//{
//    public class OrganizationApplicationMappingUnitTest
//    {
//        #region Private variable
//        private OrganizationController _organizationController;
//        #endregion

//        #region Public Constructor
//        public OrganizationApplicationMappingUnitTest()
//        {
//            StartupUnitTest<OrganizationController> startup = new StartupUnitTest<OrganizationController>();

//            _organizationController = startup.ConfigureServices();
//        }
//        #endregion

//        #region Positve Test Cases
//        [Fact]
//        public void GetAllOrgAppMapping_Reeturns_OkObjectResult()
//        {
//            var result = _organizationController.GetAllOrgAppMapping();
//            var okResult = result.Should().BeAssignableTo<OkObjectResult>();
//        }

//        [Fact]
//        public void GetAllAppByOrgId_Returns_OkObjectResult()
//        {
//            var result = _organizationController.GetAllAppByOrgId(3);
//            var okResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;

//            okResult.Should().NotBeNull();
//        }

//        [Fact]
//        public void SaveOrganizationApplicationMapping_Returns_OkObjectResult()
//        {
//            var orgAppMapping = new OrganizationApplicationmapping { OrgId = 3, AppId = 10, IsActive = true, CreatedBy = new Guid(), CreatedOn = DateTime.Now, ModiefiedBy = new Guid(), ModiefiedOn = DateTime.Now };
//            var result = _organizationController.SaveOrganizationApplicationMapping(orgAppMapping);

//            var okReslt = result.Should().BeAssignableTo<OkObjectResult>().Subject;
//        }
//        [Fact]
//        public void UpdateOrganizationApplicationMapping_Returns_OkObjectResult()
//        {
//            var orgAppMapping = new OrganizationApplicationmapping { OrganizationApplicationId =11,OrgId = 9, AppId = 10, IsActive = true, CreatedBy = new Guid(), CreatedOn = DateTime.Now, ModiefiedBy = new Guid(), ModiefiedOn = DateTime.Now };
//            var result = _organizationController.UpdateOrganizationApplicationMapping(orgAppMapping);

//            var okResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
//        }
//        [Fact]
//        public void DeleteOrganizationApplicationMapping_Returns_OkObjectResult()
//        {
//            var result = _organizationController.DeleteOrganizationApplicationMapping(11);
//            var okResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
//        }
//        #endregion

//        #region Negative Test Cases
//        [Fact]
//        public void SaveOrganizationApplicationMapping_RecordExistsAndActive_Returns_BadRequestObjectResult()
//        {
//            var orgAppMapping = new OrganizationApplicationmapping { OrgId = 3, AppId = 9, IsActive = true, CreatedBy = new Guid(), CreatedOn = DateTime.Now, ModiefiedBy = new Guid(), ModiefiedOn = DateTime.Now };
//            var result = _organizationController.SaveOrganizationApplicationMapping(orgAppMapping);

//            var okReslt = result.Should().BeAssignableTo<BadRequestObjectResult>().Subject;
//        }
//        [Fact]
//        public void DeleteOrganizationApplicationMapping_Returns_BadRequestObjectResult()
//        {
//            var result = _organizationController.DeleteOrganizationApplicationMapping(6);
//            var okResult = result.Should().BeAssignableTo<BadRequestObjectResult>().Subject;
//        }
//        [Fact]
//        public void SaveOrganizationApplicationMapping_RecordExistsAndInActive_Returns_BadRequestObjectResult()
//        {
//            var orgAppMapping = new OrganizationApplicationmapping { OrgId = 1, AppId = 9, IsActive = true, CreatedBy = new Guid(), CreatedOn = DateTime.Now, ModiefiedBy = new Guid(), ModiefiedOn = DateTime.Now };
//            var result = _organizationController.SaveOrganizationApplicationMapping(orgAppMapping);

//            var okReslt = result.Should().BeAssignableTo<BadRequestObjectResult>().Subject;
//        }
//        [Fact]
//        public void GetAllAppByOrgId_OrganizationNotExists_Returns_OkObjectResult()
//        {
//            var result = _organizationController.GetAllAppByOrgId(4);
//            var okResult = result.Should().BeAssignableTo<BadRequestObjectResult>().Subject;

//        }
//        #endregion
//    }
//}

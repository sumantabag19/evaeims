//using FluentAssertions;
//using EVA.EIMS.Entity;
//using EVA.EIMS.Security.API.Controllers;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Xunit;

//namespace EIMSUnitTestProject
//{
//    public class OrganizationControllerUnitTest
//    {

//        #region Private Variables

//        private OrganizationController _organizationController;

//        #endregion

//        #region Constructor
//        public OrganizationControllerUnitTest()
//        {
//            StartupUnitTest<OrganizationController> startUp = new StartupUnitTest<OrganizationController>();

//            _organizationController = startUp.ConfigureServices();
//        }

//        #endregion

//        /// <summary>
//        /// This method is used to test to get the multiple organization details
//        /// </summary>
//        /// <returns>returns multiple organization details</returns>
//        [Fact]
//        public void GetAllOrganizations_ReturnsIEnumerableOrganization()
//        {
//            var result = _organizationController.Get();
//            //Assert
//            var okResult = result.Should().BeAssignableTo<IEnumerable<Organization>>().Subject;
//        }


//        /// <summary>
//        /// This method is used to test to get the organization details by id
//        /// </summary>
//        /// <param name="orgId">orgId</param>
//        /// <returns>returns single organization details</returns>
//        [Fact]
//        public async System.Threading.Tasks.Task GetSpecificOrganization_BasedOnOrgId_ReturnsOrganizationAsync()
//        {
//            var result = await _organizationController.GetOrg("eims.eva.com");

//            var okResult = result.Should().BeOfType<Organization>().Subject;

//        }

//        /// <summary>
//        /// This method is used to test to save the organization details
//        /// </summary>
//        /// <param name="newOrg">organization object</param>
//        /// <returns>returns response message</returns>

//        [Fact]
//        public void SaveOrganizationDetails_ReturnsOkObjectResult()
//        {
//            var newOrg = new Organization { OrgName = "unittest1.eva.com",  Description = "trial", IsActive = true };
//            var result = _organizationController.Post(newOrg);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;


//        }

//        /// <summary>
//        /// This method is used to test to update the organization details
//        /// </summary>
//        /// <param name="orgId">orgId</param>
//        /// <param name="updateOrg">organization object</param>
//        /// <returns>returns response message</returns>

//        [Fact]
//        public void UpdateOrganization_BasedOnOrgId_ReturnsOkObject()
//        {
//            var updatedOrg = new Organization { OrgName = "unittest.eva.com", Description = "trialupdate", IsActive = true };
//            var result = _organizationController.Put(updatedOrg);
//            var okResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
//            updatedOrg.OrgName.Should().Be("unittest.eva.com");
//            updatedOrg.Description.Should().Be("trialupdate");
//        }

//        /// <summary>
//        /// This method is used to  test to delete the organization details
//        /// </summary>
//        /// <param name="orgId">orgId</param>
//        /// <returns>returns response message</returns>
//        [Fact]
//        public void DeleteOrganization_BasedOnOrgId_ReturnsOkObject()
//        {
//            var result = _organizationController.Delete("unittest.eva.com");
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// This method is used to test to delete all users & devices by organization Id
//        /// </summary>
//        /// <param name="orgId">orgId</param>
//        /// <returns>returns response message</returns>

//        [Fact]
//        public void DeleteAllUsersAndDevices_BasedOnOrgId_ReturnsOkObject()
//        {
//            var result = _organizationController.DeleteAllUsersAndDevicesByOrgId("ytfh");
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// This method is used as a negative test to save the organization details
//        /// </summary>
//        /// <param name="newOrg">organization object</param>
//        /// <returns>BadRequestObjectResult</returns>

//        [Fact]
//        public void SaveOrganizationDetails_ReturnsBadRequest()
//        {
//            var newOrg = new Organization { CreatedBy = new Guid("56aa2e79-2f9f-41d1-9622-e3911b2e100a"), ModifiedBy = new Guid("56aa2e79-2f9f-41d1-9622-e3911b2e100a"), Description = "trial", IsActive = true };
//            var result = _organizationController.Post(newOrg);
//            var okResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;

//        }

//    }
//}

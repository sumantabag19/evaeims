//using FluentAssertions;
//using EVA.EIMS.Entity;
//using EVA.EIMS.Security.API.Controllers;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using Xunit;

//namespace EIMSUnitTestProject
//{
//    public class EmailTemplateControllerUnitTest
//    {
//        #region Private Variables
//        private EmailTemplateController _emailTemplateController;
//        #endregion

//        #region Constructor
//        public EmailTemplateControllerUnitTest()
//        {
//            StartupUnitTest<EmailTemplateController> startup = new StartupUnitTest<EmailTemplateController>();

//            _emailTemplateController = startup.ConfigureServices();
//        }
//        #endregion

//        #region Positive Tests
//        /// <summary>
//        /// Tests whether GetEmailTemplate method returns IEnumerable<EmailTemplate> or not.
//        /// </summary>
//        [Fact]
//        public void GetEmailTemplate_ReturnsIEnumerableEmailTemplate()
//        {
//            //Act
//            var result = _emailTemplateController.Get();

//            //Assert
//            result.Should().BeAssignableTo<IEnumerable<EmailTemplate>>();
//        }

//        /// <summary>
//        /// Tests whether GetEmailTemplateById method returns EmailTemplate object or not.
//        /// </summary>
//        [Fact]
//        public void GetEmailTemplateById_ReturnsEmailTemplateObject()
//        {
//            var result = _emailTemplateController.Get(5);

//            result.Should().BeAssignableTo<EmailTemplate>();
//        }

//        /// <summary>
//        /// Tests whether SaveEmailTemplate method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void SaveEmailTemplate_ReturnsReturnResult()
//        {
//            var newEmailTemplate = new EmailTemplate { EmailBody = "test", AppId = 1, EmailConfidentialMsg = "test", EmailFooter = "test", EmailFrom = "abc@eva.com", EmailSubject = "test", LanguageId = Guid.Parse("03ab0396-82bd-49d8-a893-1340ab727e5a"), CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _emailTemplateController.Post(newEmailTemplate);

//            result.Should().BeOfType<OkObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether DeleteEmailTemplate method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void DeleteEmailTemplate_ReturnsReturnResult()
//        {
//            var result = _emailTemplateController.Delete(5);

//            result.Should().BeOfType<OkObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether UpdateEmailTemplate method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void UpdateEmailTemplate_ReturnsReturnResult()
//        {
//            var newEmailTemplate = new EmailTemplate { EmailBody = "test123", AppId = 1, EmailConfidentialMsg = "test1331", EmailFooter = "test121", EmailFrom = "abc@eva.com", EmailSubject = "testSubject", LanguageId = Guid.Parse("03ab0396-82bd-49d8-a893-1340ab727e5a"), CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _emailTemplateController.Put(5, newEmailTemplate);

//            result.Should().BeOfType<OkObjectResult>();
//        }
//        #endregion

//        #region Negative Tests
//        /// <summary>
//        /// Tests whether GetEmailTemplateById method returns null in response or not after providing invalid emailtemplate id.
//        /// </summary>
//        [Fact]
//        public void GetEmailTemplateById_NonExistingEmailTemplate_ReturnsEmailTemplateObject()
//        {
//            var result = _emailTemplateController.Get(50);

//            result.Should().Be(null);
//        }

//        /// <summary>
//        /// Tests whether DeleteEmailTemplate method returns BadRequest status code in response or not after providing invalid emailtemplate id.
//        /// </summary>
//        [Fact]
//        public void DeleteEmailTemplate_EmailTemplateNotExist_ReturnsReturnResult()
//        {
//            var result = _emailTemplateController.Delete(50);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether UpdateEmailTemplate method returns BadRequest status code in response or not after providing invalid emailtemplate id.
//        /// </summary>
//        [Fact]
//        public void UpdateEmailTemplate_EmailTemplateNotExist_ReturnsReturnResult()
//        {
//            var newEmailTemplate = new EmailTemplate { EmailBody = "test", EmailConfidentialMsg = "test", EmailFooter = "test", EmailFrom = "abc@eva.com", EmailSubject = "test", LanguageId = Guid.Parse("03ab0396-82bd-49d8-a893-1340ab727e5a"), CreatedBy = Guid.NewGuid(), CreatedOn = DateTime.Now, ModifiedBy = Guid.NewGuid(), ModifiedOn = DateTime.Now };

//            var result = _emailTemplateController.Put(50, newEmailTemplate);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }
//        #endregion
//    }
//}

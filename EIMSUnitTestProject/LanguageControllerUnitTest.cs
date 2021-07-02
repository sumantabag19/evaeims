//using FluentAssertions;
//using EVA.EIMS.Entity;
//using EVA.EIMS.Security.API.Controllers;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using Xunit;

//namespace EIMSUnitTestProject
//{
//    public class LanguageControllerUnitTest
//    {
//        #region Private Variables
//        private LanguageController _languageController;
//        #endregion

//        #region Constructor
//        public LanguageControllerUnitTest()
//        {
//            StartupUnitTest<LanguageController> startup = new StartupUnitTest<LanguageController>();

//            _languageController = startup.ConfigureServices();
//        }
//        #endregion

//        #region Positive Tests
//        /// <summary>
//        /// Tests whether GetLanguage method returns IEnumerable<Language> or not.
//        /// </summary>
        
//        [Fact]
//        public void GetLanguage_ReturnsIEnumerableLanguage()
//        {
//            //Act
//            var result = _languageController.Get();

//            //Assert
//            result.Should().BeAssignableTo<IEnumerable<Language>>();
//        }

//        /// <summary>
//        /// Tests whether GetLanguageById method returns Language object or not.
//        /// </summary>
//        [Fact]
//        public void GetLanguageById_ReturnsLanguageObject()
//        {
//            var result = _languageController.Get(Guid.Parse("7ab36a29-3391-4e6d-bb17-3979fc01e627"));

//            result.Should().BeAssignableTo<Language>();
//        }

//        /// <summary>
//        /// Tests whether GetLanguageByCode method returns Language object or not.
//        /// </summary>
//        [Fact]
//        public void GetLanguageByCode_ReturnsLanguageObject()
//        {
//            var result = _languageController.Get("en");

//            result.Should().BeAssignableTo<Language>();
//        }

       
//        /// <summary>
//        /// Tests whether SaveLanguage method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void SaveLanguage_ReturnsReturnResult()
//        {
//            var newLanguage = new Language { LanguageName = "newlanguage1", LanguageCode = "nl", LanguageId = Guid.NewGuid() };

//            var result = _languageController.Post(newLanguage);

//            result.Should().BeOfType<OkObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether DeleteLanguage method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void DeleteLanguage_ReturnsReturnResult()
//        {
//            var result = _languageController.Delete(Guid.Parse("7ab36a29-3391-4e6d-bb17-3979fc01e627"));

//            result.Should().BeOfType<OkObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether UpdateLanguage method returns Ok status in response or not.
//        /// </summary>
//        [Fact]
//        public void UpdateLanguage_ReturnsReturnResult()
//        {
//            var newLanguage = new Language { LanguageName = "testlanguage",LanguageCode="tl"};

//            var result = _languageController.Put(Guid.Parse("7ab36a29-3391-4e6d-bb17-3979fc01e627"), newLanguage);

//            result.Should().BeOfType<OkObjectResult>();
//        }
//        #endregion

//        #region Negative Tests
//        /// <summary>
//        /// Tests whether GetLanguageById method returns null in response or not after providing invalid language id.
//        /// </summary>
//        [Fact]
//        public void GetLanguageById_NonExistingLanguage_ReturnsLanguageObject()
//        {
//            var result = _languageController.Get(Guid.Parse("03ab0398-82bd-49d8-a893-1340ab727e5a"));

//            result.Should().Be(null);
//        }

//        /// <summary>
//        /// Tests whether GetLanguageById method returns null in response or not after providing invalid language id.
//        /// </summary>
//        [Fact]
//        public void GetLanguageByCode_NonExistingLanguage_ReturnsLanguageObject()
//        {
//            var result = _languageController.Get("ch");

//            result.Should().Be(null);
//        }

//        /// <summary>
//        /// Tests whether DeleteLanguage method returns BadRequest status code in response or not after providing invalid language id.
//        /// </summary>
//        [Fact]
//        public void DeleteLanguage_LanguageNotExist_ReturnsReturnResult()
//        {
//            var result = _languageController.Delete(Guid.Parse("03ab0396-82bd-49d8-a893-1340ab727e5a"));

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }

//        /// <summary>
//        /// Tests whether UpdateLanguage method returns BadRequest status code in response or not after providing invalid language id.
//        /// </summary>
//        [Fact]
//        public void UpdateLanguage_LanguageNotExist_ReturnsReturnResult()
//        {
//            var newLanguage = new Language { LanguageName = "testupdatelanguage", LanguageCode="tl"};

//            var result = _languageController.Put(Guid.Parse("03ab0396-82bd-49d8-a893-1340ab727e5a"), newLanguage);

//            result.Should().BeOfType<BadRequestObjectResult>();
//        }
//        #endregion
//    }
//}

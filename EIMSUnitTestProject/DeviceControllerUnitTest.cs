//using FluentAssertions;
//using EVA.EIMS.Common;
//using EVA.EIMS.Entity;
//using EVA.EIMS.Entity.ViewModel;
//using EVA.EIMS.Security.API.Controllers;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using Xunit;

//namespace EIMSUnitTestProject
//{
//    public class DeviceControllerUnitTest
//    {
//        #region Private Variables
//        private DeviceController _deviceController;
//        #endregion

//        #region Constructor
//        public DeviceControllerUnitTest()
//        {
//            StartupUnitTest<DeviceController> startUpUnittTest = new StartupUnitTest<DeviceController>();

//            _deviceController = startUpUnittTest.ConfigureServices();

//        }
//        #endregion

//        /// <summary>
//        /// Method to test Get() for all device
//        /// </summary>
//        [Fact]
//        public void GetAllDevice_ReturnsIEnumerableDevice()
//        {
//            var result = _deviceController.Get();

//            var okResult = result.Should().BeAssignableTo<IEnumerable<Device>>().Subject;
//        }

//        /// <summary>
//        /// Method to thet Get() for specific device
//        /// </summary>
//        [Fact]
//        public void GetSpecificDevice_ReturnsIEnumerable()
//        {
//            var result = _deviceController.Get("device012.tdbank");

//            var okResult = result.Should().BeAssignableTo<IEnumerable<Device>>().Subject;
//        }

//        /// <summary>
//        /// Method to test Post() 
//        /// </summary>
//        [Fact]
//        public void SaveDeviceDetails_ReturnsOkObjectResult()
//        {
//            var newdevice = new DeviceModel { DeviceId = "device013.tdbank", ClientTypeId = 2, OrgName = "eims.eva.com", AppName = "NBS", IsActive = true, GatewayDeviceId = 1, IsUsed = true };
//            var result = _deviceController.Post(newdevice);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Method to test Delete() 
//        /// </summary>
//        [Fact]
//        public void DeleteDevice_ReturnsOkObjectResult()
//        {
//            var result = _deviceController.Delete("device013.tdbank");
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Method to test Put() 
//        /// </summary>
//        [Fact]
//        public void UpdateDevice_ReturnOkObjectResult()
//        {
//            var newdevice = new DeviceModel { DeviceId = "device012.tdbank", ClientTypeId = 2, OrgName = "eims.eva.com", IsUsed = false };
//            var result = _deviceController.Put(newdevice);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Method to test UpdateDeviceUsedStatus() 
//        /// </summary>
//        [Fact]
//        public void UpdateDeviceUsedStatus_ReturnsReturnResult()
//        {
//            var newdevice = new DeviceModel { DeviceId = "device012.tdbank", SerialKey = new Guid("6a95e429-703a-40c4-971f-2bd71df599c6"), IsUsed = true };
//            var result = _deviceController.UpdateDeviceUsedStatus(newdevice);
//            var okResult = result.Should().BeOfType<ReturnResult>().Subject;
//            okResult.Success.Should().Be(true);
//        }

//        /// <summary>
//        /// Method to test GetDeviceBySerialKey()
//        /// </summary>
//        [Fact]
//        public void GetDeviceBySerialKey_ReturnReturnResult()
//        {
//            var result = _deviceController.GetDeviceBySerialKey(new Guid("6a95e429-703a-40c4-971f-2bd71df599c6"));
//            var okResult = result.Should().BeOfType<ReturnResult>().Subject;
//            okResult.Success.Should().Be(true);
//        }

//        /// <summary>
//        /// Method to test GetDeviceBySerialKey() when device is already in use
//        /// </summary>
//        [Fact]
//        public void GetDeviceBySerialKey_WhenSerialKeyInUse_ReturnResult()
//        {
//            var result = _deviceController.GetDeviceBySerialKey(new Guid("6a95e429-703a-40c4-971f-2bd71df599c6"));
//            var okResult = result.Should().BeOfType<ReturnResult>().Subject;
//            okResult.Success.Should().Be(false);
//        }

//        /// <summary>
//        /// Method to test GetDeviceByOrg()
//        /// </summary>
//        [Fact]
//        public void GetDeviceByOrgId_ReturnReturnResult()
//        {
//            var result = _deviceController.GetDeviceByOrg("eims.eva.com");
//            var okResult = result.Should().BeOfType<ReturnResult>().Subject;
//            okResult.Success.Should().Be(true);
//        }

//        /// <summary>
//        /// Method to test Delete() when DeviceId is invalid
//        /// </summary>
//        [Fact]
//        public void DeleteDevice_WhenDeviceIdInvalid_ReturnsBadRequest()
//        {
//            var result = _deviceController.Delete("afc");
//            var okResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;

//        }

//        /// <summary>
//        /// Method to test Put() when DeviceId is invalid 
//        /// </summary>
//        [Fact]
//        public void UpdateDevice_WhenDeviceIdInvalid_ReturnsBadRequest()
//        {
//            var newdevice = new DeviceModel { DeviceId = "device014.tdbank", ClientTypeId = 2, OrgName = "eims.eva.com", IsUsed = false };
//            var result = _deviceController.Put(newdevice);
//            var okResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Method to test Put() when DeviceId is invalid
//        /// </summary>
//        [Fact]
//        public void SaveDeviceDetails_WhenDeviceIdIsInvalid_ReturnsBadRequest()
//        {
//            var newdevice = new DeviceModel { ClientTypeId = 2, OrgName = "eims.eva.com", AppName = "NBS", IsActive = true, GatewayDeviceId = 1, IsUsed = true };
//            var result = _deviceController.Post(newdevice);
//            var okResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
//        }

//        /// <summary>
//        /// Method to test GetDeviceByOrg() when organization name is invalid
//        /// </summary>
//        [Fact]
//        public void GetDeviceByOrgId_WhenOrganizationNameInvalid_ReturnsReturnResult()
//        {
//            var result = _deviceController.GetDeviceByOrg("hgk");
//            var okResult = result.Should().BeOfType<ReturnResult>().Subject;
//            okResult.Success.Should().Be(false);
//        }

//        /// <summary>
//        /// Method to test GetDeviceBySerialKey() when SerialKey is invalid
//        /// </summary>
//        [Fact]
//        public void GetDeviceBySerialKey_WhenSerialKeyInvalid_ReturnsReturnResult()
//        {
//            var result = _deviceController.GetDeviceBySerialKey(new Guid("6a95e429-703a-40c4-971f-2bd71df599c7"));
//            var okResult = result.Should().BeOfType<ReturnResult>().Subject;
//            okResult.Success.Should().Be(true);
//        }

//        /// <summary>
//        /// Method to test UpdateDeviceUsedStatus() when DeviceId is invalid
//        /// </summary>
//        [Fact]
//        public void UpdateDeviceUsedStatus_WhenDeviceIdInvalid_ReturnsReturnResult()
//        {
//            var newdevice = new DeviceModel { SerialKey = new Guid("6a95e429-703a-40c4-971f-2bd71df599c6"), IsUsed = true };
//            var result = _deviceController.UpdateDeviceUsedStatus(newdevice);
//            var okResult = result.Should().BeOfType<ReturnResult>().Subject;
//            okResult.Success.Should().Be(true);
//        }

//        /// <summary>
//        /// Method to test UpdateDeviceUsedStatus() when SerialKey is invalid
//        /// </summary>
//        [Fact]
//        public void UpdateDeviceUsedStatus_WhenSerialKeyInvalid_ReturnsReturnResult()
//        {
//            var newdevice = new DeviceModel { DeviceId = "device012.tdbank", IsUsed = true };
//            var result = _deviceController.UpdateDeviceUsedStatus(newdevice);
//            var okResult = result.Should().BeOfType<ReturnResult>().Subject;
//            okResult.Success.Should().Be(true);
//        }
//    }
//}


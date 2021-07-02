using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace EVA.EIMS.Entity.ViewModel
{
    public class DeviceModel
    {
        [Key]        
        public string DeviceId { get; set; }        
        public int GatewayDeviceId { get; set; }
        public Guid Subject { get; set; }
        public Guid SerialKey { get; set; }
        [JsonProperty(PropertyName = "DeviceKey")]
        public string PrimaryKey { get; set; }        
        public int OrgId { get; set; }
        [JsonProperty(PropertyName = "Organization")]
        public string OrgName { get; set; }
        [JsonProperty(PropertyName = "ClientType")]
        public int? ClientTypeId { get; set; }       
        public int AppId { get; set; }
        public string AppName { get; set; }        
        public bool IsUsed { get; set; }       
        public bool? IsActive { get; set; }
        public Guid CreatedBy { get; set; }
        private DateTime? CreatedOn { get; set; }
        public Guid ModifiedBy { get; set; }
        private DateTime? ModifiedOn { get; set; }
    }
}

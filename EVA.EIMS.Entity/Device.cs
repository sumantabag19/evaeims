using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EVA.EIMS.Entity
{
    public class Device
    {
        [Key]
        [StringLength(50)]
        public string DeviceId { get; set; }
        public int GatewayDeviceId { get; set; }
        public Guid Subject { get; set; }
        [Required]
        public Guid SerialKey { get; set; }
        [JsonProperty(PropertyName = "DeviceKey")]
        public string PrimaryKey { get; set; }
        [JsonProperty(PropertyName = "Organization")]
        public int OrgId { get; set; }
        [JsonProperty(PropertyName = "ClientType")]
        public int ClientTypeId { get; set; }
        public int AppId { get; set; }        
        public bool IsUsed { get; set; }
        public bool IsActive { get; set; }
        [IgnoreDataMember]
        public Guid CreatedBy { get; set; }
        private DateTime? _createdOn;
        [IgnoreDataMember]
        public DateTime? CreatedOn
        {
            get
            {
                if (_createdOn == null)
                    return DateTime.Now;
                else
                    return _createdOn;
            }
            set { _createdOn = value; }
        }
        [IgnoreDataMember]
        public Guid ModifiedBy { get; set; }

        private DateTime? _modifiedOn;
        [IgnoreDataMember]
        public DateTime? ModifiedOn
        {
            get
            {

                _modifiedOn = DateTime.Now;
                return _modifiedOn;

            }
            set { _modifiedOn = value; }
        }
        
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EVA.EIMS.Entity
{
    public class IPTable
    {
        [Key]
        [Required]
        [Range(1, Double.MaxValue, ErrorMessage = ("Please provide valid IP address id"))]
        public int IPAddressId { get; set; }
        [Required]
        public int GatewayDeviceId { get; set; }
        [Required]
        public int OrgId { get; set; }
        [Required]
        public int AppId { get; set; }
        [Required]
        [StringLength(25)]
        public string IPStartAddress { get; set; }
        [Required]
        [StringLength(25)]
        public string IPEndAddress { get; set; }
        [Required]
        [StringLength(25)]
        public string IPProxyAddress { get; set; }
        [Required]
        public int PortNo { get; set; }
        [Required]
        public bool IsProxyEnabled { get; set; }
        [Required]
        public bool? IsIPAllowed { get; set; }
        [Required]
        public bool? IsActive { get; set; }
        [IgnoreDataMember]
        public Guid CreatedBy { get; set; }
        [IgnoreDataMember]
        public Guid ModifiedBy { get; set; }

        private DateTime? _createdDate;
        [IgnoreDataMember]
        public DateTime? CreatedOn
        {
            get
            {
                if (_createdDate == null)
                    return DateTime.Now;
                else
                    return _createdDate;
            }
            set { _createdDate = value; }
        }
        
        private DateTime? _modifiedDate;
        [IgnoreDataMember]
        public DateTime? ModifiedOn
        {
            get
            {
                _modifiedDate = DateTime.Now;
                return _modifiedDate;

            }
            set { _modifiedDate = value; }
        }

        public virtual Organization Organization { get; set; }
        public virtual Application Application { get; set; }
    }
}

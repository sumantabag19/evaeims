using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EVA.EIMS.Entity
{
    public class Role
    {
        [Key]
        [Required]
        [Range(1, Double.MaxValue, ErrorMessage = ("Please provide valid role id"))]
        public int RoleId { get; set; }
        [Required]
        [StringLength(150)]
        public string RoleName { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [Required]
        public bool? MultipleOrgAccess { get; set; }
        [Required]
        public bool? IsActive { get; set; }
        [IgnoreDataMember]
        public Guid CreatedBy { get; set; }
        [IgnoreDataMember]
        public Guid ModifiedBy { get; set; }
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

        private DateTime? _modifiedOn;
        [IgnoreDataMember]
        public DateTime? ModifiedOn
        {
            get
            {
                return DateTime.Now;
            }
            set { _modifiedOn = value; }
        }
    }
}
  
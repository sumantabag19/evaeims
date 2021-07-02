using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace EVA.EIMS.Entity
{
    public class OrganizationApplicationmapping
    {
        [Key]
        [Required]
        [Range(1, Double.MaxValue, ErrorMessage = ("Please provide valid organization application id"))]
        public int OrganizationApplicationId { get; set; }
        [Required]
        public int OrgId { get; set; }
        [Required]
        public int AppId { get; set; }
        public bool? CanAccessAllUsers { get; set; }
        [Required]
        public bool? IsActive { get; set; }
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
        public Guid ModiefiedBy { get; set; }

        private DateTime? _modifiedOn;
        [IgnoreDataMember]
        public DateTime? ModiefiedOn
        {
            get
            {
                //if (_modifiedOn == null)
                return DateTime.Now;
                //else
                //  return _modifiedOn;
            }
            set { _modifiedOn = value; }
        }
    }
}

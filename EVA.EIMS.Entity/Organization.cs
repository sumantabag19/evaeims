using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace EVA.EIMS.Entity
{
    public class Organization
    {
        [Key]
        [Required]
        [Range(1, Double.MaxValue, ErrorMessage = ("Please provide valid organization id"))]
        public int OrgId { get; set; }
        [Required]
        [StringLength(150)]
        public string OrgName { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [Required]
        public bool? IsActive { get; set; }
        [Required]
        public string TenantDB { get; set; }
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





    }
}
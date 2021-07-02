using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EVA.EIMS.Entity
{
    public class ClientType
    {
        [Key]
        [Required]
        [Range(1, Double.MaxValue, ErrorMessage = ("Please provide valid client type id"))]
        public int ClientTypeId { get; set; }
        [Required]
        [StringLength(50)]
        public string ClientTypeName { get; set; }
        [StringLength(150)]
        public string Description { get; set; }
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
    }
}

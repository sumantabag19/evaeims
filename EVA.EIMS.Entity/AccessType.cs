using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace EVA.EIMS.Entity
{
    public class AccessType
    {
        [Key]
        public int AccessTypeId { get; set; }
        [StringLength(100)]
        public string AccessTypeName { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        [IgnoreDataMember]
        public Guid CreatedBy { get; set; }
        [IgnoreDataMember]
        public DateTime CreatedOn { get; set; }
        [IgnoreDataMember]
        public Guid ModifiedBy { get; set; }
        [IgnoreDataMember]
        public DateTime ModifiedOn { get; set; }
    }
}

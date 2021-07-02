using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace EVA.EIMS.Entity
{
    public class Actions
    {
        [Key]
        public int ActionId { get; set; }
        [StringLength(100)]
        public string ActionName { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
        public int ModuleId { get; set; }
        public int AccessTypeId { get; set; }
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

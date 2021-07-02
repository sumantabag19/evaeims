using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace EVA.EIMS.Entity
{
    public class RoleModuleAccess
    {
        [Key]
        public int RoleAccessId { get; set; }
        public int RoleId { get; set; }
        public int ModuleId { get; set; }
        public bool ReadAccess { get; set; }
        public bool WriteAccess { get; set; }
        public bool EditAccess { get; set; }
        public bool DeleteAccess { get; set; }
        public bool? IsActive { get; set; }
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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace EVA.EIMS.Entity
{
    public class ApplicationRoleMapping
    {
        [Key]
        public int ApplicationRoleId { get; set; }
        public int AppId { get; set; }
        public int RoleId { get; set; }
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

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EVA.EIMS.Entity
{
    public class RoleAccessException
    {
        [Key]
        public int AccessExceptionId { get; set; }
        public int RoleId { get; set; }
        public int ModuleId { get; set; }
        public int ActionId { get; set; }
        [IgnoreDataMember]
        public Guid CreatedBy { get; set; }
        [IgnoreDataMember]
        public DateTime CreatedOn { get; set; }
    }
}

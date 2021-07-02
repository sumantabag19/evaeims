using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace EVA.EIMS.Entity
{
    public class ClientTypeAccessException
    {
        [Key]
        public int AccessExceptionId { get; set; }
        public int ClientTypeId { get; set; }
        public int ModuleId { get; set; }
        public int ActionId { get; set; }
        [IgnoreDataMember]
        public Guid CreatedBy { get; set; }
        [IgnoreDataMember]
        public DateTime CreatedOn { get; set; }
    }
}


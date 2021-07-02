using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity
{
    public class AuditUserLogin
    {
        [Key]
        public Guid LoginHistoryId { get; set; }
        public Guid UserId { get; set; }
        public DateTime LastLoginDatetime { get; set; }
    }
}

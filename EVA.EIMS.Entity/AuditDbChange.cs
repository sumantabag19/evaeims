using System;
using System.ComponentModel.DataAnnotations;


namespace EVA.EIMS.Entity
{
    public class AuditDbChange
    {
        [Key]
        public int DbChangeId { get; set; }
        public Int16 ChangeType { get; set; }
        public DateTime ChangeDatetime { get; set; }
        public string ChangeQuery { get; set; }
        public string ChangeRequestIp { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EVA.EIMS.Entity
{
    public class PasswordHistory
    {
        [Key]
        public int PasswordHistoryId { get; set; }
        public Guid UserId { get; set; }
        [StringLength(250)]
        public string PasswordHash { get; set; }
        public DateTime LastPasswordChangeDate { get; set; }
        [IgnoreDataMember]
        public Guid CreateBy { get; set; }
    }
}

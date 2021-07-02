using System;
using System.ComponentModel.DataAnnotations;

namespace EVA.EIMS.Entity
{
    public class UserClientTypeMapping
    {
        [Key]
        public int UserClientId { get; set; }
        public Guid UserId { get; set; }
        public int ClientTypeId { get; set; }
        public bool? IsActive { get; set; }

        public virtual ClientType ClientType { get; set; }
        public virtual User User { get; set; }
    }
}

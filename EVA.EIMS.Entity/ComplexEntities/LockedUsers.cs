using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ComplexEntities
{
    public class LockedUsers
    {
        [Key]
        public Guid userId { get; set; }
        public string userName { get; set; }
        public string name { get; set; }
        public DateTime? LockAccountDate { get; set; }
        public int? lockTypeId { get; set; }
    }
}
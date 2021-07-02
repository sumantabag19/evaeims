using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EVA.EIMS.Entity
{
    public class LockAccount
    {
        public Guid UserId { get; set; }
        public int LockTypeId { get; set; }
        public int FailedLockCount { get; set; }
        public DateTime FailedLockDate { get; set; }
    }
    
}

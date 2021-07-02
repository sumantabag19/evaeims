using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
    public class UnlockUserByUserName
    {
        [Key]
        [Required]
        public string userName { get; set; }
        [Required]
        public bool isAccLock { get; set; }
    }
}

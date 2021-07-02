using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
    public class VerifyOTP
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string OTPString { get; set; }
    }
}

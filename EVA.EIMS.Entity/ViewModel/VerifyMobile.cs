using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
    public class VerifyMobile
    {
        [Required]
        [RegularExpression(@"^(\d{10}){1}?$")]
        public string Mobile { get; set; }
        //[Required]
        public string OTPString { get; set; }
    }
}

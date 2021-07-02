using System;
using System.ComponentModel.DataAnnotations;

namespace EVA.EIMS.Entity
{

    public class IMSLogOutToken
    {
        [Key]
        [Required]
        public Guid LogOutTokenId { get; set; }
        [Required]
        public string LogOutToken { get; set; }
        [Required]
        public DateTime LogoutOn { get; set; }
        [Required]
        public DateTime TokenValidationPeriod { get; set; }
        public string OTP { get; set; }


    }
}

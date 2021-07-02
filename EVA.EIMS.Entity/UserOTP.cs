using System;
using System.ComponentModel.DataAnnotations;

namespace EVA.EIMS.Entity
{
    public class UserOTP
    {
        [Key]
        public int OTPId { get; set; }
        public string OTPHash { get; set; }
        public Guid UserId { get; set; }
        public DateTime OTPCreationDatetime { get; set; }
        public int OTPVerificationCount { get; set; }
		public DateTime OTPExpirationDatetime { get; set; }
        public int OTPTypeId { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace EVA.EIMS.Entity
{
    public class ForgotPasswordFlowManagement
    {
        [Key]
        public Guid UserId { get; set; }
        public bool VerifiedEmail { get; set; }
        public DateTime VerifiedEmailOn { get; set; }
        public bool VerifiedOTP { get; set; }
        public DateTime? VerifiedOTPOn { get; set; }
        public bool VerifiedSecurityQuestions { get; set; }
        public DateTime? VerifiedSecurityQuestionsOn { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
    public class SecurityAnswerFromUserModel
    {
        [Key]
        [Required]
        public int QuestionId { get; set; }
        public string Question { get; set; }
        [Required]
        public string UserAnswerText { get; set; }
        [Required]
        public Guid UserId { get; set; }
    }
}

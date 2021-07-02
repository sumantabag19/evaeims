using System;
using System.ComponentModel.DataAnnotations;

namespace EVA.EIMS.Common
{
    /// <summary>
    /// User Credentials
    /// </summary>
    public class UserCredentials
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string EmailId { get; set; }

    }
}

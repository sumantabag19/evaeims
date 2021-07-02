using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ComplexEntities
{
    public class SendEmailPwdExpNotify
    {
        [Key]
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public DateTime PasswordExpiration { get; set; }
        public string AppName { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string Name { get; set; }
    }
}

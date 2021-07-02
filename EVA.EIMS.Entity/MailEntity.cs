using System;
using System.Collections.Generic;
using System.Text;

namespace EVA.EIMS.Entity
{
    public class MailEntity
    {
        public string Body { get; set; }
        public string MailFrom { get; set; }
        public string MailTo { get; set; }
        public string Subject { get; set; }

        public string UserName { get; set; }
        public string UserMailid { get; set; }
    }
}

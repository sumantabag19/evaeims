using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ComplexEntities
{
    public class ApplicationUserDetails
    {
        [Key]
        public int OauthClientId { get; set; }
        public int AppId { get; set; }
    }
}

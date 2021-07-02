using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ComplexEntities
{
    public class UserOrgAppDetails
    {
        [Key]
        public int AppId { get; set; }
        public string OrgId { get; set; }
        public string AppName { get; set; }
    }
}

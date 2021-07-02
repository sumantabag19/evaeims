using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
    public class OrganizationApplicationModel
    {
        [Key]
        public int OrganizationApplicationId { get; set; }
        public string AppName { get; set; }
        public int AppId { get; set; }
        public string OrgName { get; set; }
        public int OrgId { get; set; }
        public bool IsActive { get; set; }
    }
}
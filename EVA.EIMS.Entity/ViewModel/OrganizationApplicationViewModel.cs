using EVA.EIMS.Entity.ComplexEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
    public class OrganizationApplicationViewModel
    {
        public string OrgName { get; set; }
        public bool? IsActive { get; set; }
        public MultiSelect[] AppName { get; set; }
    }
}

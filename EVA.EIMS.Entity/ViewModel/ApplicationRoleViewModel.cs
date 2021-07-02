using EVA.EIMS.Entity.ComplexEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
    public class ApplicationRoleViewModel
    {
        public string AppName { get; set; }
        public bool? IsActive { get; set; }
        public MultiSelect[] RoleName { get; set; }
    }
}

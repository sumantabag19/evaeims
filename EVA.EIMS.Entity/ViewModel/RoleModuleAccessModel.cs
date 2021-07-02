using System;
using System.Collections.Generic;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
    public class RoleModuleAccessModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public bool ReadAccess { get; set; }
        public bool WriteAccess { get; set; }
        public bool EditAccess { get; set; }
        public bool DeleteAccess { get; set; }
        public bool? IsActive { get; set; }

    }
}

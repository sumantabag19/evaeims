using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
    public class RoleModel
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }

    }
}


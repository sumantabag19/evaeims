using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
    public class AccessPermission
    {
        [Key]
        public int Authorized { get; set; }
    }
}

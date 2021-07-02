using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
    public class VerifyMobileModel
    {
        public Guid UserId { get; set; }
        public bool MobileLoginEnabled { get; set; }
    }
}

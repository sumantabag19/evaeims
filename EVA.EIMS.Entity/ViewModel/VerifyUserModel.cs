using System;
using System.Collections.Generic;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
    public class VerifyUserModel
    {
        public Guid UserId { get; set; }
        public bool TwoFactorEnable { get; set; }
    }
}

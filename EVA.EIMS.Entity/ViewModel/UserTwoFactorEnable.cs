using System;
using System.Collections.Generic;
using System.Text;

namespace EVA.EIMS.Entity.ViewModel
{
    public class UserTwoFactorEnable
    {
        public Guid userId { get; set; }
        public bool? twoFactorEnabled { get; set; }

    }
}

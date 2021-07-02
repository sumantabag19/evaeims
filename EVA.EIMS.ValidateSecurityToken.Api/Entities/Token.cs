using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.ValidateSecurityToken.Api.Entities
{
    public class Token
    {
        public string JwtToken { get; set; }

        public bool TokenValidationSuccess { get; set; }

        public string ValidationMessage { get; set; }

    }


}

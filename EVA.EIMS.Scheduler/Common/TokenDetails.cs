using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Scheduler.Common
{
    /// <summary>
    /// Maps with token data format
    /// </summary>
    public class TokenDetails
    {
        public string scope { get; set; }
        public string token_type { get; set; }
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
    }
}

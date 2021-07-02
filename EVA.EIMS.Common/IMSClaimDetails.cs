using System;
using System.Collections.Generic;
using System.Text;

namespace EVA.EIMS.Common
{
    public class IMSClaimDetails
    {
        public string client_id { get; set; }
        public string sub { get; set; }
        public string subid { get; set; }
        public string role { get; set; }
        public string org { get; set; }
        public string client_type { get; set; }
        public List<string> scope { get; set; }
        public string iss { get; set; }
        public string aud { get; set; }
        public int exp { get; set; }
        public int nbf { get; set; }
    }
}

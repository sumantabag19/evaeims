using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeleteLoggedOutTokensScheduler.Common
{
    public class SchedulerSettings
    {
        public string GetEIMSTokenUrl { get; set; }
        public string DeleteLoggedOutTokenCronTab { get; set; }
        public string DeleteLoggedOutTokenUrl { get; set; }
        public string cli { get; set; }
        public string sts { get; set; }
        public string UseLogging { get; set; }
    }
}

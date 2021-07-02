using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeleteLoggedOutTokensScheduler.SchedulerConfiguration.CronSettings
{

    public delegate void CrontabFieldAccumulator(int start, int end, int interval); 

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeleteLoggedOutTokensScheduler.SchedulerConfiguration.CronSettings
{
    [Serializable]
    public enum CrontabFieldKind
    {
        Minute,
        Hour,
        Day,
        Month,
        DayOfWeek
    }
}

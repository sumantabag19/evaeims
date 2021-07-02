using System;

namespace EVA.EIMS.Scheduler.SchedulerConfiguration.CronSettings
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
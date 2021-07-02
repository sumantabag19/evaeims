using System;

namespace EVA.EIMS.AutoEmailerScheduler.SchedulerConfiguration.CronSettings
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
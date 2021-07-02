namespace EVA.EIMS.AutoEmailerScheduler.Common
{
    /// <summary>
    /// Maps setting with appsettings.json file
    /// </summary>
    public class SchedulerSettings
    {
        public string GetEIMSTokenUrl { get; set; }
        public string AutoEmailerNotifyCronTab { get; set; }
        public string AutoEmailerUrl { get; set; }
        public string cli { get; set; }
        public string sts { get; set; }
        public string UseLogging { get; set; }
    }
}

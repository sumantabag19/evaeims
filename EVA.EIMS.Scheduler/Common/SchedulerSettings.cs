namespace EVA.EIMS.Scheduler.Common
{
    /// <summary>
    /// Maps setting with appsettings.json file
    /// </summary>
    public class SchedulerSettings
    {
        public string GetEIMSTokenUrl { get; set; }
        public string RefreshTokenUrl { get; set; }
        public string DeleteRefreshTokenCronTab { get; set; }
        public string DeleteRefreshTokenCronTab1 { get; set; }
        public string cli { get; set; }
        public string sts { get; set; }
        public string UseLogging { get; set; }
    }
}

namespace EVA.EIMS.Scheduler.Common
{
    /// <summary>
    /// Refresh token count model format
    /// </summary>
    public class RefreshTokenCountModel
    {
        public string ClientId { get; set; }
        public int RequestThreshold { get; set; }
        public int TokenCount { get; set; }
    }
}

namespace EVA.EIMS.Logging
{
    public class LoggerStorage
    {
        public string BlobConnectionString { get; set; }
        public string ElasticSearchURL { get; set; }
        public bool IsInfoLog { get; set; }
        public bool IsWarnLog { get; set; }
        public bool IsErrorLog { get; set; }
        public bool IsDebugLog { get; set; }
    }
}

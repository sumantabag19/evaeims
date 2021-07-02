using System;

namespace EVA.EIMS.Logging
{
    public class LogClass
    {
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime LogDateTime { get; set; }
        public string Application { get; set; }
        public string IPAddress { get; set; }
        public string LogLevel { get; set; }
    }
}

namespace EVA.EIMS.Logging
{
    public interface ILogger
    {
        void Info(LogClass logClass);
        void Debug(LogClass logClass);
        void Error(LogClass logClass);
        void Warn(LogClass logClass);
        void Info(string className, string methodName, string message, string stackTrace);
        void Debug(string className, string methodName, string message, string stackTrace);
        void Error(string className, string methodName, string message, string stackTrace);
        void Warn(string className, string methodName, string message, string stackTrace);
    }
}

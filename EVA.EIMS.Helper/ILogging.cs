using EVA.EIMS.Helper.Constants;

namespace EVA.EIMS.Helper
{
    public interface ILogging
    {
        void Log(LogType type, string className, string methodName, string message, string stackTrace);
    }
}

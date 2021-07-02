using System;

namespace EVA.EIMS.Helper
{
    public class ExceptionLogger
    {
        public static string LogException(Exception ex)
        {
            var returnString = ex.Message;
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                returnString += $" {ex.Message}";
            }
            return returnString;
        }
    }
}

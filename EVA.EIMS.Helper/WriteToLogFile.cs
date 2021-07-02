using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace EVA.EIMS.Helper
{
    public static class WriteToLogFile
    {

        public static string m_exePath = string.Empty;
        public static void LogMessageToFile(string logMessage, string type)
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);
            var root = configurationBuilder.Build();
            var appSetting = root.GetSection("ApplicationSettings");
            try
            {
                string wwwrootPath = appSetting["ErrorFolderPath"];//"D:/home/site/wwwroot/";
                m_exePath = wwwrootPath + "ERRORLogException";

                if (!Directory.Exists(m_exePath))
                {
                    Directory.CreateDirectory(m_exePath);
                }

                if (!m_exePath.EndsWith("\\")) m_exePath += Path.DirectorySeparatorChar;

                string filePath = m_exePath + DateTime.Now.ToString("MM-dd-yyyy") + "_" + type + ".log";
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Dispose();
                    using (StreamWriter writer = File.AppendText(filePath))
                    {
                        WriteLog(logMessage, writer);
                        writer.Close();
                    }
                }
                else if (File.Exists(filePath))
                {
                    using (StreamWriter writer = File.AppendText(filePath))
                    {
                        WriteLog(logMessage, writer);
                        writer.Close();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public async static void WriteLog(String logMessage, TextWriter writer)
        {
            //writer.WriteAsync("\r\nLog Entry : ");
            await writer.WriteLineAsync(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture));
            //writer.WriteLineAsync("  :");
            await writer.WriteAsync("  :" + logMessage);
        }
    }

}

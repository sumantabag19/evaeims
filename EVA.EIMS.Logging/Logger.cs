using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace EVA.EIMS.Logging
{
    #region Old Code
    ///// <summary>
    ///// Logger Helper class
    ///// </summary>
    //public static class Logger
    //{
    //    /// <summary>
    //    /// Add log details by types(Info, Error and Warning)
    //    /// </summary>
    //    public static void Log(LogType type, string className, string methodName, string message, string stackTrace, string userName)
    //    {
    //        //Use below switch condition to work on log information
    //        switch (type)
    //        {
    //            case LogType.INFO:
    //                //type + "Log: Class Name: " + className + " Method:" + method + "Info:" + message;
    //                break;
    //            case LogType.ERROR:
    //                //type + "Log: Class Name: " + className + " Method:" + method + " Exception:" + message;

    //                break;
    //            case LogType.WARNING:
    //                //type + "Log: Class Name: " + className + " Method:" + method + " Warning:" + message;                    
    //                break;
    //        }

    //        //Push log info to centralized loger service
    //        //logHandlerService.pushLog(type, message, component, method, this.userName);
    //        WriteLogFile.LogMessageToFile(type, message, methodName, message, stackTrace);

    //    }

    //    /// <summary>
    //    /// Add log details by types(Info, Error and Warning)
    //    /// </summary>
    //    /// <param name="type"></param>
    //    /// <param name="className"></param>
    //    /// <param name="methodName"></param>
    //    /// <param name="message"></param>
    //    /// <param name="stackTrace"></param>
    //    public static void Log(LogType type, string className, string methodName, string message, string stackTrace)
    //    {
    //        //Use below switch condition to work on log information
    //        switch (type)
    //        {
    //            case LogType.INFO:
    //                //type + "Log: Class Name: " + className + " Method:" + method + "Info:" + message;
    //                break;
    //            case LogType.ERROR:
    //                //type + "Log: Class Name: " + className + " Method:" + method + " Exception:" + message;

    //                break;
    //            case LogType.WARNING:
    //                //type + "Log: Class Name: " + className + " Method:" + method + " Warning:" + message;                    
    //                break;
    //        }

    //        //Push log info to centralized liger service
    //        //logHandlerService.pushLog(type, message, component, method, this.userName);
    //        WriteLogFile.LogMessageToFile(type, message, methodName, message, stackTrace);

    //    }
    //}

    ///// <summary>
    ///// Define Log Types
    ///// </summary>
    //public enum LogType
    //{
    //    INFO,
    //    WARNING,
    //    ERROR,
    //    DEBUG
    //}

    //public enum LogWriteTypeEnum
    //{
    //    BlobLog,
    //    ElsaticLog,
    //    FileLog
    //}
    //public static class WriteLogFile
    //{
    //    public static string m_exePath = string.Empty;
    //    public static string LogPath()
    //    {
    //        m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    //        if (!m_exePath.EndsWith("\\")) m_exePath += "\\";
    //        return m_exePath;
    //    }
    //    public static void LogMessageToFile(LogType type, string className, string methodName, string message, string stackTrace)
    //    {
    //        //System.IO.StreamWriter sw = System.IO.File.AppendText(
    //        //    LogPath() + "MyLogFile.txt");
    //        //try
    //        //{
    //        //    sw.Write("\r\nLog Entry : ");
    //        //    sw.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
    //        //        DateTime.Now.ToLongDateString());
    //        //    sw.WriteLine("  :");
    //        //    sw.WriteLine("\n Log Type:" + type + "\n ClassName :" + className + "\n MethodName :" + methodName + "\n Message :" + message + "\n StackTrace" + stackTrace);
    //        //    sw.WriteLine("-------------------------------");

    //        //}
    //        //catch (Exception ex)
    //        //{
    //        //}


    //        m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    //        if (!m_exePath.EndsWith("\\")) m_exePath += "\\";

    //        if (!File.Exists(m_exePath + "EntityApi.log"))
    //        {
    //            File.Create(m_exePath + "EntityApi.log").Dispose();
    //            using (StreamWriter writer = File.AppendText(m_exePath + "EntityApi.log"))
    //            {
    //                WriteLog(message, writer, className, methodName, stackTrace);
    //                writer.Close();
    //            }

    //        }
    //        else if (File.Exists(m_exePath + "EntityApi.log"))
    //        {
    //            using (StreamWriter writer = File.AppendText(m_exePath + "EntityApi.log"))
    //            {
    //                WriteLog(message, writer, className, methodName, stackTrace);
    //                writer.Close();
    //            }
    //        }


    //    }

    //    public static void WriteLog(String logMessage, TextWriter writer, string className, string methodName, string stackTrace)
    //    {
    //        writer.Write("\r\nLog Entry : ");

    //        writer.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),

    //        DateTime.Now.ToLongDateString());

    //        writer.WriteLine("  :");

    //        writer.WriteLine("\n ClassName :" + className + "\n MethodName :" + methodName + "\n Message :" + logMessage + "\n StackTrace" + stackTrace);

    //        //writer.WriteLine("  :{0}", logMessage);

    //        writer.WriteLine("-------------------------------");
    //    }
    //}
    #endregion

    public class Logger : ILogger
    {
        #region Class Variables
        private readonly IOptions<LoggerConfig> _applicationSettings;
        private readonly IHttpClientFactory _clientFactory;
        private ILogger<Logger> _logger = null;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public Logger(IServiceProvider provider, IHttpClientFactory clientFactory, ILogger<Logger> logger)
        {
            _applicationSettings = provider.GetRequiredService<IOptions<LoggerConfig>>();
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public Logger(IHttpContextAccessor httpContextAccessor, IServiceProvider provider, IHttpClientFactory clientFactory, ILogger<Logger> logger)
        {
            _applicationSettings = provider.GetRequiredService<IOptions<LoggerConfig>>();
            _clientFactory = clientFactory;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Private function
        private void ExecuteLog(LogClass logClass)
        {
            switch (_applicationSettings.Value.LogWriteTypeEnum)
            {
                case LogWriteTypeEnum.BlobLog:
                    LogBlob(logClass);
                    break;
                case LogWriteTypeEnum.ElsaticLog:
                    LogElastic(logClass);
                    break;
                case LogWriteTypeEnum.FileLog:
                case LogWriteTypeEnum.Console:
                    LogFile(logClass);
                    break;
                default:
                    LogElastic(logClass);
                    break;
            }
        }

        private void LogElastic(LogClass logClass)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                client.PostAsync(_applicationSettings.Value.LoggerStorage.ElasticSearchURL, new StringContent(JsonConvert.SerializeObject(logClass)));
            }
            catch (Exception ex)
            { }
        }

        private void LogBlob(LogClass logClass)
        {
            string logFileName = LogConstants.BlobFileNameSuffix + DateTime.Now.ToShortDateString().Replace("/", "_") + "_" + DateTime.Now.Hour + LogConstants.BlobFileExtension;

            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_applicationSettings.Value.LoggerStorage.BlobConnectionString);

            // Create a CloudFile_client object for credentialed access to File storage.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_applicationSettings.Value.AzureBlobSettings.Container);
            container.CreateIfNotExistsAsync();

            // This creates a reference to the append blob we are going to use.
            CloudAppendBlob appendBlob = container.GetAppendBlobReference(_applicationSettings.Value.ServiceName + "/" + DateTime.Now.ToShortDateString().Replace("/", "_") + "/" + logFileName);

            // Now we are going to check if todays file exists and if it doesn't we create it.
            if (!appendBlob.ExistsAsync().GetAwaiter().GetResult())
            {
                appendBlob.CreateOrReplaceAsync();
            }

            // Add the entry to our log.
            appendBlob.AppendTextAsync(JsonConvert.SerializeObject(logClass));
        }

        private void LogFile(LogClass logClass)
        {
            switch (logClass.LogLevel)
            {
                case LogConstants.Error:
                    _logger.LogError(JsonConvert.SerializeObject(logClass));
                    break;
                case LogConstants.Debug:
                    _logger.LogDebug(JsonConvert.SerializeObject(logClass));
                    break;
                case LogConstants.Info:
                    _logger.LogInformation(JsonConvert.SerializeObject(logClass));
                    break;
                case LogConstants.Warn:
                    _logger.LogWarning(JsonConvert.SerializeObject(logClass));
                    break;
                default:
                    _logger.LogError(JsonConvert.SerializeObject(logClass));
                    break;
            }
        }
        #endregion

        #region Public function
        public void Info(LogClass logClass)
        {
            if (_applicationSettings.Value.LoggerStorage.IsInfoLog)
            {
                logClass.LogLevel = LogConstants.Info;
                ExecuteLog(logClass);
            }
        }

        public void Debug(LogClass logClass)
        {
            if (_applicationSettings.Value.LoggerStorage.IsDebugLog)
            {
                logClass.LogLevel = LogConstants.Debug;
                ExecuteLog(logClass);
            }
        }

        public void Error(LogClass logClass)
        {
            if (_applicationSettings.Value.LoggerStorage.IsErrorLog)
            {
                logClass.LogLevel = LogConstants.Error;
                ExecuteLog(logClass);
            }
        }

        public void Warn(LogClass logClass)
        {
            if (_applicationSettings.Value.LoggerStorage.IsWarnLog)
            {
                logClass.LogLevel = LogConstants.Warn;
                ExecuteLog(logClass);
            }
        }

        public void Info(string className, string methodName, string message, string stackTrace)
        {
            if (_applicationSettings.Value.LoggerStorage.IsInfoLog)
            {
                LogClass logClass = new LogClass
                {
                    Application = _applicationSettings.Value.ServiceName,
                    LogLevel = LogConstants.Info,
                    ClassName = className,
                    MethodName = methodName,
                    Message = message,
                    StackTrace = stackTrace,
                    LogDateTime = DateTime.UtcNow,
                    IPAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()
                };
                ExecuteLog(logClass);
            }
        }

        public void Debug(string className, string methodName, string message, string stackTrace)
        {
            if (_applicationSettings.Value.LoggerStorage.IsDebugLog)
            {
                LogClass logClass = new LogClass
                {
                    Application = _applicationSettings.Value.ServiceName,
                    LogLevel = LogConstants.Debug,
                    ClassName = className,
                    MethodName = methodName,
                    Message = message,
                    StackTrace = stackTrace,
                    LogDateTime = DateTime.UtcNow,
                    IPAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()
                };
                ExecuteLog(logClass);
            }
        }

        public void Error(string className, string methodName, string message, string stackTrace)
        {
            if (_applicationSettings.Value.LoggerStorage.IsErrorLog)
            {
                LogClass logClass = new LogClass
                {
                    Application = _applicationSettings.Value.ServiceName,
                    LogLevel = LogConstants.Error,
                    ClassName = className,
                    MethodName = methodName,
                    Message = message,
                    StackTrace = stackTrace,
                    LogDateTime = DateTime.UtcNow,
                    IPAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()
                };
                ExecuteLog(logClass);
            }
        }

        public void Warn(string className, string methodName, string message, string stackTrace)
        {
            if (_applicationSettings.Value.LoggerStorage.IsWarnLog)
            {
                LogClass logClass = new LogClass
                {
                    Application = _applicationSettings.Value.ServiceName,
                    LogLevel = LogConstants.Warn,
                    ClassName = className,
                    MethodName = methodName,
                    Message = message,
                    StackTrace = stackTrace,
                    LogDateTime = DateTime.UtcNow,
                    IPAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()
                };
                ExecuteLog(logClass);
            }
        }
        #endregion
    }
}

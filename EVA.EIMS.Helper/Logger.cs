
using EVA.EIMS.Helper.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace EVA.EIMS.Helper
{
    /// <summary>
    /// Logger Helper class
    /// </summary>
    public class Logger : ILogging
    {
        #region Private Properties
        private readonly IHttpContextAccessor _httpContextAccessor;
        //public IConfigurationRoot Configuration { get; set; }
        private readonly IOptions<ApplicationSettings> _applicationSettings;
        //private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly ILogger<Logger> _logger;
        private readonly IDistributedCache _distributedCache;

        #endregion

        #region Constructor
        public Logger(IHttpContextAccessor httpContextAccessor, IServiceProvider provider, ILogger<Logger> logger, IDistributedCache distributedCache)
        {
            _httpContextAccessor = httpContextAccessor;
            _applicationSettings = provider.GetRequiredService<IOptions<ApplicationSettings>>();
            _logger = logger;
            _distributedCache = distributedCache;
        }
        #endregion

        /// <summary>
        /// Add log details by types(Info, Error and Warning)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="message"></param>
        /// <param name="stackTrace"></param>
        public void Log(LogType type, string className, string methodName, string message, string stackTrace)
        {
            var client = new HttpClient();

            LogClass logClass = new LogClass
            {
                Application = ApplicationLevelConstants.ApplicationName,
                LogLevel = type.ToString(),
                ClassName = className,
                MethodName = methodName,
                Message = message,
                StackTrace = stackTrace,
                LogDateTime = DateTime.UtcNow,
                IPAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                APIName = _applicationSettings.Value.LoggerStorage.APIFolder
            };

            //if (type == LogType.ERROR)
            //    WriteToLogFile.LogMessageToFile(message + "\n" + stackTrace, ApplicationLevelConstants.logtype);
            LogToDatabase(type, logClass);
            var logClassJson = JsonConvert.SerializeObject(logClass);
            switch (_applicationSettings.Value.UseLogging)
            {
                case "BlobLogger":

                    string logPath = Environment.CurrentDirectory + @"\Logs";
                    string logFileName = "Log_" + DateTime.Now.ToShortDateString().Replace("/", "_") + "_" + DateTime.Now.Hour + ".txt";

                    // Parse the connection string and return a reference to the storage account.
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_applicationSettings.Value.LoggerStorage.BlobConnectionString);

                    // Create a CloudFile_client object for credentialed access to File storage.
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference("eimslogcontainer");
                    container.CreateIfNotExistsAsync();

                    // This creates a reference to the append blob we are going to use.
                    CloudAppendBlob appendBlob = container.GetAppendBlobReference(_applicationSettings.Value.LoggerStorage.APIFolder + Path.AltDirectorySeparatorChar + DateTime.Now.ToShortDateString().Replace("/", "_") + Path.AltDirectorySeparatorChar + logFileName);

                    // Now we are going to check if todays file exists and if it doesn't we create it.
                    if (!appendBlob.ExistsAsync().GetAwaiter().GetResult())
                    {
                        appendBlob.CreateOrReplaceAsync();
                    }

                    // Add the entry to our log.
                    appendBlob.AppendTextAsync(logClassJson);
                    break;

                case "ElasticSearch":
                    try
                    {

                        var response = client.PostAsync(_applicationSettings.Value.LoggerStorage.ElasticSearchURL,
                            new StringContent(logClassJson, Encoding.UTF8, "application/json"));
                        //response.Wait();
                        break;
                    }
                    catch (Exception)
                    {
                        break;
                    }
                case "File":
                    if (type == LogType.ERROR)
                        WriteToLogFile.LogMessageToFile(message + "\n" + stackTrace, type.ToString());
                    break;
                default:
                    if (type == LogType.ERROR)
                        WriteToLogFile.LogMessageToFile(message + "\n" + stackTrace, type.ToString());
                    break;
            }

        }

        public async void LogToDatabase(LogType type, LogClass logClass)
        {
            //Log to database only if database logging is enabled
            try
            {
                //var cachedData = await _distributedCache.GetAsync("IsDatabaseLogEnabled");
                //if (cachedData != null)
                //{
                //var cachedMessage = Encoding.UTF8.GetString(cachedData);
                //if (cachedMessage.ToLower() == "true")
                //{
                var logLevel = type == LogType.INFO ? NLog.LogLevel.Info : (type == LogType.ERROR ? NLog.LogLevel.Error : NLog.LogLevel.Debug);
                var msgArray = new string[] { logClass.ClassName, logClass.MethodName, logClass.Message };
                LogEventInfo theEvent = new LogEventInfo(logLevel, "", String.Join("|", msgArray));
                if (type == LogType.ERROR)
                    theEvent.Properties["StatusCode"] = Convert.ToInt32(HttpStatusCode.InternalServerError);
                theEvent.Properties["exception"] = logClass.StackTrace;
                LogManager.GetCurrentClassLogger().Log(theEvent);
                //}
                //}
            }
            catch (Exception)
            {
            }
        }
    }
}

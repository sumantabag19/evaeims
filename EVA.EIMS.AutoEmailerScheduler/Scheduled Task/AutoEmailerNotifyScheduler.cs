using EVA.EIMS.AutoEmailerScheduler.Common;
using EVA.EIMS.AutoEmailerScheduler.Common.Constants;
using EVA.EIMS.AutoEmailerScheduler.SchedulerConfiguration;
using EVA.EIMS.Logging;
using EVA.EIMS.AutoEmailerScheduler.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EVA.EIMS.AutoEmailerScheduler.ScheduledTask
{
    public class AutoEmailerNotifyScheduler : IScheduledTask
    {
        #region Private Properties
        private readonly IOptions<SchedulerSettings> _schedulerSettings;
        private bool _isExecutingFirstTime = false;
        private readonly ILogger _logger;
        #endregion

        public AutoEmailerNotifyScheduler(IServiceProvider provider, ILogger logger)
        {
            _schedulerSettings = provider.GetRequiredService<IOptions<SchedulerSettings>>();
            _logger = logger;
        }
        public string Schedule => _schedulerSettings.Value.AutoEmailerNotifyCronTab;


        #region Get IP Address
        /// <summary>
        /// Get Ip Address
        /// </summary>
        /// <returns></returns>
        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return string.Empty;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// This method executes the sheduled task
        /// </summary>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>Task</returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!_isExecutingFirstTime)
                {
                    //_logger.Log(LogType.INFO, "AutoEmailerNotifyScheduler", "ExecuteAsync", "AutoEmailerNotifyScheduler Execution Started", "Execution Start Time :" + DateTime.Now);

                    TokenDetails tokenDetails = JsonConvert.DeserializeObject<TokenDetails>(await GetEIMSToken());

                    await SendAutoEmailNotificatn(tokenDetails);

                }
                _isExecutingFirstTime = false;

            }
            catch (Exception exception)
            {
                _logger.Error(new EVA.EIMS.Logging.LogClass()
                {
                    Application = exception.Source,
                    ClassName = exception.TargetSite.DeclaringType.FullName,
                    IPAddress = GetLocalIPAddress(),
                    LogDateTime = DateTime.Now,
                    LogLevel = string.Empty,
                    Message = exception.Message,
                    MethodName = exception.TargetSite.Name,
                    StackTrace = exception.StackTrace
                });
            }
        }


        #endregion


        #region Private Methods
        /// <summary>
        /// This method gets the EIMS token 
        /// </summary>
        /// <param name="tokenDetails">tokenDetails</param>
        /// <returns>Task<string></returns>
        private async Task<string> GetEIMSToken()
        {
            string result = string.Empty;

            try
            {
                HttpClient client = new HttpClient();

                client.BaseAddress = new Uri(_schedulerSettings.Value.GetEIMSTokenUrl);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlenCoded"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _schedulerSettings.Value.cli, _schedulerSettings.Value.sts))));

                string parturl = (string.IsNullOrWhiteSpace(client.BaseAddress.AbsolutePath) || client.BaseAddress.AbsolutePath == "/") ?
                                        "/connect/token" : client.BaseAddress.AbsolutePath + "/connect/token";
                var request = new HttpRequestMessage(HttpMethod.Post, parturl);

                var keyValues = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("scope", "read write serviceapi_all")

                };
                request.Content = new FormUrlEncodedContent(keyValues);
                var response = await client.SendAsync(request);

                result = await response.Content.ReadAsStringAsync();

            }
            catch (Exception exception)
            {
                _logger.Error(new EVA.EIMS.Logging.LogClass()
                {
                    Application = exception.Source,
                    ClassName = exception.TargetSite.DeclaringType.FullName,
                    IPAddress = GetLocalIPAddress(),
                    LogDateTime = DateTime.Now,
                    LogLevel = string.Empty,
                    Message = exception.Message,
                    MethodName = exception.TargetSite.Name,
                    StackTrace = exception.StackTrace
                });
            }

            return result;
        }

        /// <summary>
        /// This method calls the sendpasswordexpnotification api
        /// </summary>
        /// <param name="tokenDetails">tokenDetails</param>
        /// <returns>HTTP response message</returns>
        private async Task SendAutoEmailNotificatn(TokenDetails tokenDetails)
        {
            try
            {

                HttpClient client = new HttpClient();

                client.BaseAddress = new Uri(_schedulerSettings.Value.AutoEmailerUrl);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenDetails.access_token);

                string parturl = (string.IsNullOrWhiteSpace(client.BaseAddress.AbsolutePath) || client.BaseAddress.AbsolutePath == "/") ?
                                        "/api/User/sendpasswordexpnotification" : client.BaseAddress.AbsolutePath + "/api/User/sendpasswordexpnotification";
                var request = new HttpRequestMessage(HttpMethod.Post, parturl);

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    //_logger.Log(LogType.INFO, "AutoEmailerNotifyScheduler", "ExecuteAsync", Schedule, "Cron Setting");
                    _logger.Info("AutoEmailerNotifyScheduler", "ExecuteAsync", "Scheduler Executed Successfully", "Sent mail to notify user of Password Expiration");
                }

            }
            catch (Exception exception)
            {
                _logger.Error(new EVA.EIMS.Logging.LogClass()
                {
                    Application = exception.Source,
                    ClassName = exception.TargetSite.DeclaringType.FullName,
                    IPAddress = GetLocalIPAddress(),
                    LogDateTime = DateTime.Now,
                    LogLevel = string.Empty,
                    Message = exception.Message,
                    MethodName = exception.TargetSite.Name,
                    StackTrace = exception.StackTrace
                });
            }
        }
        #endregion
    }
}

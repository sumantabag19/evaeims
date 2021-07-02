using EVA.EIMS.Scheduler.SchedulerConfiguration;
using EVA.EIMS.Scheduler.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using EVA.EIMS.Logging;
using System.Net;
using System.Net.Sockets;

namespace EVA.EIMS.Scheduler.ScheduledTask
{
    public class RefreshTokenScheduler : IScheduledTask
    {
        #region Private Properties
        private readonly IOptions<SchedulerSettings> _schedulerSettings;
        private bool _isExecutingFirstTime = false;
        private ILogger _logger;
        #endregion

        public string Schedule => _schedulerSettings.Value.DeleteRefreshTokenCronTab;

        #region Constructor
        public RefreshTokenScheduler(IServiceProvider provider, ILogger logger)
        {
            _schedulerSettings = provider.GetRequiredService<IOptions<SchedulerSettings>>();
            _logger = logger;
            var A = _schedulerSettings.Value.DeleteRefreshTokenCronTab1;
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
                    HttpClient client = new HttpClient();

                    _logger.Info(GetLogClassObject("RefreshTokenScheduler", "ExecuteAsync", "RefreshTokenScheduler Execution Started"));
                    string token = await GetEIMSToken();
                    TokenDetails tokenDetails = JsonConvert.DeserializeObject<TokenDetails>(token);
                    //Send client wise token request count email notification
                    await SendTokenRequestCountEmail(tokenDetails);
                    //Delete old refresh tokens
                    await DeleteRefreshTokens(tokenDetails);
                    _logger.Info(GetLogClassObject("RefreshTokenScheduler", "ExecuteAsync", "Scheduler Executed Successfully"));
                }
                else
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

                throw exception;
            }
        }
        #endregion

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

        #region Private Methods
        /// <summary>
        /// This method calls deleterefreshtoken API
        /// </summary>
        /// <param name="tokenDetails">tokenDetails</param>
        /// <returns>Task</returns>
        private async Task DeleteRefreshTokens(TokenDetails tokenDetails)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(_schedulerSettings.Value.RefreshTokenUrl)
                };

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenDetails.access_token);

                string parturl = (string.IsNullOrWhiteSpace(client.BaseAddress.AbsolutePath) || client.BaseAddress.AbsolutePath == "/") ?
                                         "/api/RefreshToken/deletesheduledrefreshtoken" : client.BaseAddress.AbsolutePath + "/api/RefreshToken/deletesheduledrefreshtoken";
                var request = new HttpRequestMessage(HttpMethod.Delete, parturl);

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    _logger.Info(GetLogClassObject("RefreshTokenScheduler", "DeleteRefreshTokens", "Deleted RefreshToken data before " + (DateTime.Now.AddDays(-2))));
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
                throw exception;
            }

        }

        /// <summary>
        /// This method calls deleterefreshtoken API
        /// </summary>
        /// <param name="tokenDetails">tokenDetails</param>
        /// <returns>Task</returns>
        private async Task SendTokenRequestCountEmail(TokenDetails tokenDetails)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(_schedulerSettings.Value.RefreshTokenUrl)
                };

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenDetails.access_token);

                string parturl = (string.IsNullOrWhiteSpace(client.BaseAddress.AbsolutePath) || client.BaseAddress.AbsolutePath == "/") ?
                                         "/api/RefreshToken/SendTokenRequestCountNotification" : client.BaseAddress.AbsolutePath + "/api/RefreshToken/SendTokenRequestCountNotification";
                var request = new HttpRequestMessage(HttpMethod.Post, parturl);

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.Info(GetLogClassObject("RefreshTokenScheduler", "SendTokenRequestCountEmail", "Sent mail to notify token request threshold reached"));
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
                throw exception;
            }

        }

        /// <summary>
        /// This method gets the EIMS token 
        /// </summary>
        /// <param name="tokenDetails">tokenDetails</param>
        /// <returns>Task<string></returns>
        private async Task<string> GetEIMSToken()
        {
            try
            {

                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(_schedulerSettings.Value.GetEIMSTokenUrl)
                };

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

                return await response.Content.ReadAsStringAsync();
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

                throw exception;
            }
        }

        /// <summary>
        /// Get Log Class Object
        /// </summary>
        /// <param name="ClassName"></param>
        /// <param name="MethodName"></param>
        /// <param name="Message"></param>
        /// <param name="Source"></param>
        /// <param name="StackTrace"></param>
        /// <returns></returns>
        private LogClass GetLogClassObject(string ClassName, string MethodName, string Message, string Source = "", string StackTrace = "")
        {
            LogClass logClassObject = new LogClass();

            logClassObject.Application = Source;
            logClassObject.ClassName = ClassName;
            logClassObject.IPAddress = GetLocalIPAddress();
            logClassObject.LogDateTime = DateTime.Now;
            logClassObject.LogLevel = string.Empty;
            logClassObject.Message = Message;
            logClassObject.MethodName = MethodName;
            logClassObject.StackTrace = StackTrace;

            return logClassObject;
        }
        #endregion
    }
}
using EVA.EIMS.Common;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using EVA.EIMS.Logging;
using System.Net.Sockets;

namespace EVA.EIMS.ValidateSecurityToken.Api.Filter
{
    /// <summary>
    /// This middleware class handels all type of exceptions
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        #region Private Variables
        private readonly RequestDelegate _next;
        private ILogger _logger;

        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next">It will get populated by framework</param>
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;


        }
        #endregion

        #region Public Methods

        public async Task Invoke(HttpContext context, ILogger logger /* other scoped dependencies */)
        {
            try
            {
                _logger = logger;

                // call next middleware
                await _next(context);

            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handle exception and provide appropriate response
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = JsonConvert.SerializeObject(new { error = exception.Message });

            switch (exception.Message)
            {
                case MessageConstants.UnauthorizedAccess:
                    code = HttpStatusCode.Unauthorized;
                    result = JsonConvert.SerializeObject(new { error = MessageConstants.UnauthorizedAccessException });
                    _logger.Error("RolePermissionFilter", "OnActionExecuting", exception.InnerException == null ? exception.Message : exception.InnerException.Message, exception.StackTrace);
                    break;
                case MessageConstants.InvalidAccessToken:
                    code = HttpStatusCode.Unauthorized;
                    result = JsonConvert.SerializeObject(new { error = MessageConstants.InvalidAccessTokenException });
                    _logger.Error("AuthorizationTokenFilter", "OnAuthorization", exception.InnerException == null ? exception.Message : exception.InnerException.Message, exception.StackTrace);
                    break;
                default:
                    _logger.Error(exception.TargetSite.DeclaringType.FullName, exception.TargetSite.Name,
                        exception.InnerException == null ? exception.Message : exception.InnerException.Message, exception.StackTrace);
                    break;
            }

            //_logger.Error(new EVA.EIMS.Logging.LogClass()
            //{
            //    Application = exception.Source,
            //    ClassName = exception.TargetSite.DeclaringType.FullName,
            //    IPAddress = GetLocalIPAddress(),
            //    LogDateTime = DateTime.Now,
            //    LogLevel = string.Empty,
            //    Message = exception.Message,
            //    MethodName = exception.TargetSite.Name,
            //    StackTrace = exception.StackTrace
            //});

            return context.Response.WriteAsync(result);
        }

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
    }
}

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

namespace EVA.EIMS.Security.API.Filters
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
                    code = HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(new { error = MessageConstants.InvalidAccessTokenException });
                    _logger.Error("AuthorizationTokenFilter", "OnAuthorization", exception.InnerException == null ? exception.Message : exception.InnerException.Message, exception.StackTrace);
                    break;
                case MessageConstants.AccountLocked:
                    code = HttpStatusCode.Unauthorized;
                    result = JsonConvert.SerializeObject(new { error = exception.InnerException.InnerException.Message });
                    _logger.Error("AuthorizationTokenFilter", "OnAuthorization", exception.InnerException == null ? exception.Message : exception.InnerException.Message, exception.StackTrace);
                    break;
                case MessageConstants.InvalidClient:
                    code = HttpStatusCode.Unauthorized;
                    result = JsonConvert.SerializeObject(new { error = exception.InnerException.InnerException.Message });
                    _logger.Error("AuthorizationTokenFilter", "OnAuthorization", exception.InnerException == null ? exception.Message : exception.InnerException.Message, exception.StackTrace);
                    break;
                case MessageConstants.InternalServerError:
                    code = HttpStatusCode.InternalServerError;
                    result = JsonConvert.SerializeObject(new { error = exception.InnerException.Message });
                    _logger.Error("ErrorHandlingMiddleware", "HandleExceptionAsync", exception.InnerException == null ? exception.Message : exception.InnerException.Message, exception.StackTrace);
                    break;
                default:
                    _logger.Error(exception.TargetSite.DeclaringType.FullName, exception.TargetSite.Name,
                        exception.InnerException == null ? exception.Message : exception.InnerException.Message, exception.StackTrace);
                    break;
            }

            return context.Response.WriteAsync(result);
        }
        #endregion
    }
}

using EVA.EIMS.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServer.Api.Filter
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private ILogger _logger;

        public ApiExceptionFilter(ILogger logger)
        {
            this._logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.Error(context.Exception.TargetSite.DeclaringType.FullName, context.Exception.TargetSite.Name,
                          context.Exception.InnerException == null ? context.Exception.Message : context.Exception.InnerException.Message, context.Exception.StackTrace);

        }
    }
}

using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace EVA.EIMS.Security.API.Filters
{
    /// <summary>
    /// This exception filter calss used to handel all exception logs
    /// </summary>
    public class CustomExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// Pass all exception logs to logger
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            //ILogging logger = context.HttpContext.RequestServices.GetService<ILogging>();

            //ControllerActionDescriptor descriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            //string actionName = descriptor.ActionName;
            //string controllerName = descriptor.ControllerName;
            //logger.Log(LogType.ERROR, descriptor.ControllerName, descriptor.ActionName, context.Exception.Message, context.Exception.StackTrace);
        }
    }
}

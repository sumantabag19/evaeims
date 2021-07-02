
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc.Filters;

//namespace AuthorizationServer.Api
//{
//    public class RequestInformationAttribute : ActionFilterAttribute
//    {
//      //  private static readonly Logger Log = LogManager.GetCurrentClassLogger();

//        private string _requestUri = string.Empty;
//        private readonly string _endpoint = string.Empty;
//        private string _methodType = string.Empty;

//        /// <summary>
//        /// This filter method is used to track the request information
//        /// </summary>
//        /// <param name="actionContext">actionContext</param>
//        public override void OnActionExecuting(ActionExecutingContext actionContext)
//        {
//            base.OnActionExecuting(actionContext);
//            actionContext.Request.Properties[actionContext.ActionDescriptor.ActionName] = Stopwatch.StartNew();
//            _requestUri = actionContext.Request.RequestUri.ToString();
//            _methodType = actionContext.ActionDescriptor.ActionName;
//        }

//        /// <summary>
//        /// This filter method is used to track the request information
//        /// </summary>
//        /// <param name="actionExecutedContext">actionExecutedContext</param>
//        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
//        {
//            base.OnActionExecuted(actionExecutedContext);
//            var watch = (Stopwatch)actionExecutedContext.Request.Properties[actionExecutedContext.ActionContext.ActionDescriptor.ActionName];
//            var msg = $"Method={_methodType}| Elapsed={watch.ElapsedMilliseconds} | RequestUri={this._requestUri} | Endpoint={this._endpoint} - End";
//            if (actionExecutedContext.Exception != null)
//            {
//              //  Log.Error(actionExecutedContext.Exception.Message);
//            }
//            else
//            {
//                var objectContent = actionExecutedContext.ActionContext.Response.Content as ObjectContent;
//                if (objectContent?.Value != null)
//                {
//                  //  Log.Error(((ObjectContent)actionExecutedContext.ActionContext.Response.Content).Value);
//                }
//            }
//           // Log.Trace(msg);
//        }
//    }
//}

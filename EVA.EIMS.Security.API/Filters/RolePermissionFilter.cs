using EVA.EIMS.Common;
using EVA.EIMS.Contract.Infrastructure;
using EVA.EIMS.Entity.ViewModel;
using EVA.EIMS.Helper;
using EVA.EIMS.Helper.Constants;
using EVA.EIMS.Logging;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EVA.EIMS.Security.API.Filters
{

    /// <summary>
    /// This is a global action filter to check user access permission 
    /// Also it logs action execution details
    /// </summary>
    public class RolePermissionFilter : ActionFilterAttribute
    {
        #region Public Methods
        /// <summary>
        /// Logs required information after executing filter action  
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>void</returns>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            ILogger logger = context.HttpContext.RequestServices.GetService<ILogger>();
            ControllerActionDescriptor descriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            string actionName = descriptor.ActionName;
            string controllerName = descriptor.ControllerName;

            logger.Info(descriptor.ControllerName, descriptor.ActionName, string.Format(MessageConstants.EndMethodExecution, descriptor.ActionName), "");
        }

        /// <summary>
        /// Filter request on basis of role and client type 
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>void</returns>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger>();
            try
            {
                //For Anonymous access, check if the api has [AllowAnonymous] attribute
                if (!context.Filters.Any(item => item is IAllowAnonymousFilter))
                {
                    ControllerActionDescriptor descriptor = (ControllerActionDescriptor)context.ActionDescriptor;
                    string actionName = descriptor.ActionName;
                    string controllerName = descriptor.ControllerName;
                    logger.Info(descriptor.ControllerName, descriptor.ActionName, string.Format(MessageConstants.StartMethodExecution, descriptor.ActionName), "");

                    if (context.HttpContext.Items != null && context.HttpContext.Items.Count > 0)
                    {
                        AccessPermission AccessPermission = new AccessPermission() { Authorized = 0 };
                        IExecuterStoreProc<AccessPermission> procExecuterRepository = context.HttpContext.RequestServices.GetService<IExecuterStoreProc<AccessPermission>>();
                        // Check wether the token contains role  
                        if (context.HttpContext.Items.Any(v => v.Key.Equals(KeyConstant.Role)))
                        {
                            List<string> role = new List<string>();
                            foreach (var item in (List<string>)context.HttpContext.Items[KeyConstant.Role])
                            {
                                role.Add(item);
                            }
                            if (role.Contains("SuperAdmin"))
                            {
                                AccessPermission.Authorized = 1;
                            }
                            else
                            {
                                // Pass Role name Controller and Action to store procedure
                                List<Parameters> roleParam = new List<Parameters>() {
                                                    new Parameters("p_Role", role.FirstOrDefault().ToString()),
                                                    new Parameters("p_ModuleName", controllerName),
                                                    new Parameters("p_ActionName", actionName) };

                                // procVerifyRoleBaseAccess returns 1 if Role has access else 0
                                AccessPermission = procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procVerifyRoleBaseAccess.ToString(), roleParam).GetAwaiter().GetResult().FirstOrDefault();
                                if(AccessPermission == null || AccessPermission.Authorized != 1)
                                {
                                    //Check for Role Access Exceptions
                                    AccessPermission = procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procVerifyRoleAccessExceptions.ToString(), roleParam).GetAwaiter().GetResult().FirstOrDefault();
                                }
                            }
                        }
                        else
                        {
                            if (context.HttpContext.Items.Any(v => v.Key.Equals(KeyConstant.Client_Type)))
                            {
                                List<string> clientType = new List<string>();
                                foreach (var item in (List<string>)context.HttpContext.Items[KeyConstant.Client_Type])
                                {
                                    clientType.Add(item);
                                }

                                // Pass Client Type name Controller and Action to store procedure
                                List<Parameters> clientTypeParam = new List<Parameters>() {
                                                    new Parameters("p_ClientType", clientType.FirstOrDefault().ToString()),
                                                    new Parameters("p_ModuleName", controllerName),
                                                    new Parameters("p_ActionName", actionName) };

                                // procVerifyClientTypeBaseAccess returns 1 if ClientType has access else 0
                                AccessPermission = procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procVerifyClientTypeBaseAccess.ToString(), clientTypeParam).GetAwaiter().GetResult().FirstOrDefault();
                                if (AccessPermission != null && AccessPermission.Authorized != 1)
                                {
                                    //Check for ClientType Access Exceptions
                                    AccessPermission = procExecuterRepository.ExecuteProcedureAsync(ProcedureConstants.procVerifyClientTypeAccessExceptions.ToString(), clientTypeParam).GetAwaiter().GetResult().FirstOrDefault();
                                }
                            }
                        }
                        if (AccessPermission == null || AccessPermission.Authorized != 1)
                        {
                            //throw UnauthorizedAccess exception if user does not have access permission
                            throw new UnauthorizedAccessException(MessageConstants.UnauthorizedAccess);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("RolePermissionFilter", "OnActionExecuting", ex.Message, ex.StackTrace);
                throw new UnauthorizedAccessException(MessageConstants.UnauthorizedAccess, ex);
            }

            base.OnActionExecuting(context);
        }
        #endregion
    }
}

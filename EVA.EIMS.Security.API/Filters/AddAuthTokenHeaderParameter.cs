using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVA.EIMS.Security.API.Filters
{
    public class AddAuthTokenHeaderParameter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();
            if (context.ControllerActionDescriptor.ActionName != "VerifyAccount" &&
                context.ControllerActionDescriptor.ActionName != "VerifyOTP" &&
                context.ControllerActionDescriptor.ActionName != "GetRandomSecurityQuestions" &&
                context.ControllerActionDescriptor.ActionName != "VerifySecurityQuestionsAnswer" &&
                context.ControllerActionDescriptor.ActionName != "UpdatePassword" )
            {
                operation.Parameters.Add(new HeaderParameter()
                {
                    Name = "Authorization",
                    In = "header",
                    Type = "string",
                    Required = true
                });
            }
        }
    }

    class HeaderParameter : NonBodyParameter
    {
    }
}

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace EVA.EIMS.ValidateSecurityToken.Api.Filter
{
	public class AddAuthTokenHeaderParameter : IOperationFilter
	{
		public void Apply(Operation operation, OperationFilterContext context)
		{
			if (operation.Parameters == null)
				operation.Parameters = new List<IParameter>();
			if (context.ControllerActionDescriptor.ActionName != "GET")
			{
				operation.Parameters.Add(new HeaderParameter()
				{
					Name = "appName",
					In = "header",
					Type = "string",
					Required = true
				});

                operation.Parameters.Add(new HeaderParameter()
                {
                    Name = "orgName",
                    In = "header",
                    Type = "string",
                    Required = false
                });
            }
        }
	}

	class HeaderParameter : NonBodyParameter
	{
	}
}

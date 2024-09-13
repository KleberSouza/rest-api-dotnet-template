using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace rest_api_dotnet_template.Filters
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Verifica se o endpoint tem o atributo [Authorize]
            var hasAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
                                || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

            if (hasAuthorize)
            {
                // Verifica e adiciona a resposta 401 somente se ainda não foi adicionada
                if (!operation.Responses.ContainsKey("401"))
                {
                    operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                }

                // Verifica e adiciona a resposta 403 somente se ainda não foi adicionada
                if (!operation.Responses.ContainsKey("403"))
                {
                    operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
                }

                var bearerScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                };

                operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [bearerScheme] = new string[] { }
                }
            };
            }
        }
    }
}

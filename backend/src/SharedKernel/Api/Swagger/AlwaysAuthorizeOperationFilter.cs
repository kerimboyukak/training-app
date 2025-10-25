using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Swagger
{
    public class AlwaysAuthorizeOperationFilter : IOperationFilter
    {
        private readonly string _securityScheme;
        private readonly string[] _scopes;

        public AlwaysAuthorizeOperationFilter(string securityScheme, string[] scopes)
        {
            _securityScheme = securityScheme;
            _scopes = scopes;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme {Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = _securityScheme}
                        }
                    ] = _scopes
                }
            };
        }
    }
}
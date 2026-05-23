using Abp.Collections.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace Eaf.Middleware.Web.Swagger
{
    /// <summary>
    /// Representa a classe SwaggerOperationFilter.
    /// </summary>
    public class SwaggerOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Apply.
        /// </summary>
        /// <param name="operation">Parâmetro operation.</param>
        /// <param name="context">Parâmetro context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                return;
            }

            for (var i = 0; i < operation.Parameters.Count; ++i)
            {
                var parameter = operation.Parameters[i];

                var enumType = context.ApiDescription.ParameterDescriptions[i].ParameterDescriptor.ParameterType;
                if (!enumType.IsEnum)
                {
                    continue;
                }

                var schema = context.SchemaRepository.Schemas.GetOrAdd($"#/definitions/{enumType.Name}", () =>
                    context.SchemaGenerator.GenerateSchema(enumType, context.SchemaRepository)
                );

                parameter.Schema = schema;
            }
        }
    }
}
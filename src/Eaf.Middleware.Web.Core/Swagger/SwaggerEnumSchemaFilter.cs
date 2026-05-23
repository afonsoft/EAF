using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace Eaf.Middleware.Web.Swagger
{
    /// <summary>
    /// Representa a classe SwaggerEnumSchemaFilter.
    /// </summary>
    public class SwaggerEnumSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// Apply.
        /// </summary>
        /// <param name="schema">Parâmetro schema.</param>
        /// <param name="context">Parâmetro context.</param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;
            if (!type.IsEnum || schema.Extensions.ContainsKey("x-enumNames"))
            {
                return;
            }

            var enumNames = new OpenApiArray();
            enumNames.AddRange(Enum.GetNames(type).Select(_ => new OpenApiString(_)));
            schema.Extensions.Add("x-enumNames", enumNames);
        }
    }
}
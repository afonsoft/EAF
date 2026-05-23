using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace Eaf.Middleware.Web.Swagger
{
    /// <summary>
    /// Representa a classe SwaggerNullableParameterFilter.
    /// </summary>
    public class SwaggerNullableParameterFilter : IParameterFilter
    {
        /// <summary>
        /// Apply.
        /// </summary>
        /// <param name="parameter">Parâmetro parameter.</param>
        /// <param name="context">Parâmetro context.</param>
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            if (!parameter.Schema.Nullable &&
                (Nullable.GetUnderlyingType(context.ApiParameterDescription.Type) != null
                    || !context.ApiParameterDescription.Type.IsValueType
                    || (context.ApiParameterDescription.Type.IsGenericType
                        && context.ApiParameterDescription.Type.GetGenericTypeDefinition() == typeof(Nullable<>))))
            {
                parameter.Schema.Nullable = true;
            }
        }
    }
}
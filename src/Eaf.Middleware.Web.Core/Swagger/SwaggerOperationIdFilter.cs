using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Eaf.Middleware.Swagger
{
    /// <summary>
    /// Representa a classe SwaggerOperationIdFilter.
    /// </summary>
    public class SwaggerOperationIdFilter : IOperationFilter
    {
        /// <summary>
        /// Apply.
        /// </summary>
        /// <param name="operation">Parâmetro operation.</param>
        /// <param name="context">Parâmetro context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.OperationId = FriendlyId(context.ApiDescription);
        }

        private static string FriendlyId(ApiDescription apiDescription)
        {
            var parts = (RelativePathSansQueryString(apiDescription) + "/" + apiDescription.HttpMethod.ToLower())
                .Split('/');

            var builder = new StringBuilder();
            foreach (var part in parts)
            {
                var trimmed = part.Trim('{', '}');
                builder.AppendFormat("{0}{1}",
                    (part.StartsWith('{') ? "By" : string.Empty),
                    CultureInfo.InvariantCulture.TextInfo.ToTitleCase(trimmed)
                );
            }

            return builder.ToString();
        }

        private static string RelativePathSansQueryString(ApiDescription apiDescription)
        {
            return apiDescription.RelativePath.Split('?')[0];
        }
    }
}
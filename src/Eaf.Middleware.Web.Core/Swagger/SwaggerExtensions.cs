using Abp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Linq;
using System.Text;

namespace Eaf.Middleware.Web.Swagger
{
    /// <summary>
    /// Representa a classe SwaggerExtensions.
    /// </summary>
    public static class SwaggerExtensions
    {
        /// <summary>
        /// https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/752#issuecomment-467817189
        /// When Swashbuckle.AspNetCore 5.0 is released, we can remove it.
        /// </summary>
        /// <param name="options"></param>
        public static void CustomDefaultSchemaIdSelector(this SwaggerGenOptions options)
        {
            string SchemaIdSelector(Type modelType)
            {
                if (!modelType.IsConstructedGenericType)
                {
                    return modelType.Name;
                }

                var prefix = modelType.GetGenericArguments()
                    .Select(SchemaIdSelector)
                    .Aggregate<string>((previous, current) => previous + current);

                return modelType.Name.Split('`')[0] + "Of" + prefix;
            }

            options.CustomSchemaIds(SchemaIdSelector);
        }

        /// <summary>
        /// Injects EAF base URI into the index.html page
        /// </summary>
        /// <param name="options"></param>
        /// <param name="pathBase">base path (URL) to application API</param>
        public static void InjectBaseUrl(this SwaggerUIOptions options, string pathBase)
        {
            pathBase = pathBase.EnsureEndsWith('/');

            options.HeadContent = new StringBuilder(options.HeadContent)
                .AppendLine($"<script> var eaf = eaf || {{}}; eaf.appPath = eaf.appPath || '{pathBase}'; </script>")
                .ToString();
        }
    }
}
using Abp.Dependency;
using Abp.IO.Extensions;
using Abp.Reflection.Extensions;
using Abp.Timing;
using System;

using System.Collections.Concurrent;
using System.Text;

namespace Eaf.Middleware.Net.Emailing
{
    /// <summary>
    /// Representa a classe EmailTemplateProvider.
    /// </summary>
    public class EmailTemplateProvider : IEmailTemplateProvider, ISingletonDependency
    {
        private readonly ConcurrentDictionary<string, string> _defaultTemplates;

        /// <summary>
        /// EmailTemplateProvider.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public EmailTemplateProvider()
        {
            _defaultTemplates = new ConcurrentDictionary<string, string>();
        }

        /// <summary>
        /// GetDefaultTemplate.
        /// </summary>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <returns>Resultado da operação.</returns>
        public string GetDefaultTemplate(int? tenantId)
        {
            var tenancyKey = tenantId.HasValue ? tenantId.Value.ToString() : "host";

            return _defaultTemplates.GetOrAdd(tenancyKey, key =>
            {
                using (var stream = typeof(EmailTemplateProvider).GetAssembly().GetManifestResourceStream($"{typeof(EmailTemplateProvider).GetAssembly().GetName().Name}.Net.Emailing.EmailTemplates.default.html"))
                {
                    var bytes = stream.GetAllBytes();
                    var template = Encoding.UTF8.GetString(bytes);
                    return template.Replace("{THIS_YEAR}", Clock.Now.Year.ToString());
                }
            });
        }
    }
}
using System;
using Abp.AspNetCore.Webhook;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eaf.Webhooks.Configuration
{
    /// <summary>
    /// Extensões para registro dos serviços Eaf.Webhooks no IServiceCollection.
    /// </summary>
    public static class EafWebhooksServiceCollectionExtensions
    {
        /// <summary>
        /// Registra opções e HttpClient nomeado usado pelo sender de webhooks.
        /// </summary>
        public static IServiceCollection AddEafWebhooks(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            services.Configure<EafWebhooksOptions>(configuration.GetSection("EafWebhooks"));
            services.AddHttpClient();
            services.AddHttpClient(AspNetCoreWebhookSender.WebhookSenderHttpClientName);

            return services;
        }
    }
}

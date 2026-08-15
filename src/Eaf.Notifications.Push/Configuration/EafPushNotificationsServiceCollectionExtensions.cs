using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eaf.Notifications.Push.Configuration
{
    /// <summary>
    /// Extensions to register EAF push notification services.
    /// </summary>
    public static class EafPushNotificationsServiceCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="PushOptions"/> and the named <see cref="HttpClient"/> used by push providers.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">Application configuration.</param>
        /// <param name="isEnabled">Whether push notifications are enabled.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddEafPushNotifications(this IServiceCollection services, IConfiguration configuration, bool isEnabled = true)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            services.Configure<PushOptions>(configuration.GetSection("Eaf:Push"));
            services.PostConfigure<PushOptions>(options => options.IsEnabled = isEnabled && options.IsEnabled);
            services.AddHttpClient("EafPush");

            return services;
        }
    }
}

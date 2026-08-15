using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eaf.Notifications.Sms.Configuration
{
    /// <summary>
    /// Extensions to register EAF SMS notification services.
    /// </summary>
    public static class EafSmsNotificationsServiceCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="SmsOptions"/> and the named <see cref="HttpClient"/> used by SMS providers.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">Application configuration.</param>
        /// <param name="isEnabled">Whether SMS notifications are enabled.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddEafSmsNotifications(this IServiceCollection services, IConfiguration configuration, bool isEnabled = true)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            services.Configure<SmsOptions>(configuration.GetSection("Eaf:Sms"));
            services.PostConfigure<SmsOptions>(options => options.IsEnabled = isEnabled && options.IsEnabled);
            services.AddHttpClient("EafSms");

            return services;
        }
    }
}

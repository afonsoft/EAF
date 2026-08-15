using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.UI;
using Eaf.Notifications.Push.Configuration;
using Microsoft.Extensions.Options;

namespace Eaf.Notifications.Push
{
    /// <summary>
    /// Default implementation of <see cref="IPushNotificationSender"/>.
    /// Selects the active provider from <see cref="PushOptions.Provider"/>.
    /// </summary>
    public class EafPushNotificationSender : IPushNotificationSender, ITransientDependency
    {
        private readonly PushOptions _options;
        private readonly IEnumerable<IPushNotificationProvider> _providers;

        /// <summary>
        /// Creates a new <see cref="EafPushNotificationSender"/>.
        /// </summary>
        /// <param name="optionsAccessor">Push options.</param>
        /// <param name="providers">Registered providers.</param>
        public EafPushNotificationSender(IOptions<PushOptions> optionsAccessor, IEnumerable<IPushNotificationProvider> providers)
        {
            _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
            _providers = providers ?? throw new ArgumentNullException(nameof(providers));
        }

        /// <inheritdoc/>
        public async Task<PushSendResult> SendAsync(PushSubscription subscription, PushNotificationMessage message)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (!_options.IsEnabled)
                throw new UserFriendlyException("As notificações push estão desabilitadas.");
            if (string.IsNullOrWhiteSpace(subscription.Endpoint))
                throw new UserFriendlyException("O endpoint da inscrição push é obrigatório.");

            var provider = ResolveProvider();
            return await provider.SendAsync(subscription, message);
        }

        private IPushNotificationProvider ResolveProvider()
        {
            var providers = _providers.ToList();
            if (providers.Count == 0)
                throw new UserFriendlyException("Nenhum provider de push foi registrado.");

            if (!string.IsNullOrWhiteSpace(_options.Provider))
            {
                var selected = providers.FirstOrDefault(p =>
                    string.Equals(p.Name, _options.Provider, StringComparison.OrdinalIgnoreCase));

                if (selected != null)
                    return selected;

                throw new UserFriendlyException($"Provider de push '{_options.Provider}' não encontrado.");
            }

            return providers.First();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.UI;
using Eaf.Notifications.Sms.Configuration;
using Microsoft.Extensions.Options;

namespace Eaf.Notifications.Sms
{
    /// <summary>
    /// Default implementation of <see cref="ISmsSender"/>.
    /// Selects the active provider from <see cref="SmsOptions.Provider"/>.
    /// </summary>
    public class EafSmsSender : ISmsSender, ITransientDependency
    {
        private readonly SmsOptions _options;
        private readonly IEnumerable<ISmsProvider> _providers;

        /// <summary>
        /// Creates a new <see cref="EafSmsSender"/>.
        /// </summary>
        /// <param name="optionsAccessor">SMS options.</param>
        /// <param name="providers">Registered providers.</param>
        public EafSmsSender(IOptions<SmsOptions> optionsAccessor, IEnumerable<ISmsProvider> providers)
        {
            _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
            _providers = providers ?? throw new ArgumentNullException(nameof(providers));
        }

        /// <inheritdoc/>
        public async Task<SmsSendResult> SendAsync(SmsMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (!_options.IsEnabled)
                throw new UserFriendlyException("As notificações por SMS estão desabilitadas.");
            if (string.IsNullOrWhiteSpace(message.PhoneNumber))
                throw new UserFriendlyException("O número de telefone é obrigatório.");
            if (string.IsNullOrWhiteSpace(message.Body))
                throw new UserFriendlyException("A mensagem do SMS é obrigatória.");

            var provider = ResolveProvider();
            return await provider.SendAsync(message);
        }

        private ISmsProvider ResolveProvider()
        {
            var providers = _providers.ToList();
            if (providers.Count == 0)
                throw new UserFriendlyException("Nenhum provider de SMS foi registrado.");

            if (!string.IsNullOrWhiteSpace(_options.Provider))
            {
                var selected = providers.FirstOrDefault(p =>
                    string.Equals(p.Name, _options.Provider, StringComparison.OrdinalIgnoreCase));

                if (selected != null)
                    return selected;

                throw new UserFriendlyException($"Provider de SMS '{_options.Provider}' não encontrado.");
            }

            return providers.First();
        }
    }
}

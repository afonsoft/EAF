using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.UI;
using Eaf.Notifications.Sms.Configuration;
using Microsoft.Extensions.Options;

namespace Eaf.Notifications.Sms.Providers
{
    /// <summary>
    /// Provider for the Twilio SMS REST API.
    /// </summary>
    public class TwilioSmsProvider : ISmsProvider, ITransientDependency
    {
        /// <inheritdoc/>
        public string Name => "Twilio";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SmsOptions _options;

        /// <summary>
        /// Creates a new <see cref="TwilioSmsProvider"/>.
        /// </summary>
        /// <param name="httpClientFactory">HTTP client factory.</param>
        /// <param name="optionsAccessor">SMS options.</param>
        public TwilioSmsProvider(IHttpClientFactory httpClientFactory, IOptions<SmsOptions> optionsAccessor)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
        }

        /// <inheritdoc/>
        public async Task<SmsSendResult> SendAsync(SmsMessage message, CancellationToken cancellationToken = default)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var opts = _options.Twilio ?? throw new UserFriendlyException("A seção Eaf:Sms:Twilio não foi configurada.");

            if (string.IsNullOrWhiteSpace(opts.AccountSid))
                throw new UserFriendlyException("Eaf:Sms:Twilio:AccountSid é obrigatório.");
            if (string.IsNullOrWhiteSpace(opts.AuthToken))
                throw new UserFriendlyException("Eaf:Sms:Twilio:AuthToken é obrigatório.");
            if (string.IsNullOrWhiteSpace(message.PhoneNumber))
                throw new UserFriendlyException("O número de telefone é obrigatório.");
            if (string.IsNullOrWhiteSpace(message.Body))
                throw new UserFriendlyException("A mensagem do SMS é obrigatória.");

            var from = string.IsNullOrWhiteSpace(message.From)
                ? (!string.IsNullOrWhiteSpace(opts.From) ? opts.From : _options.DefaultFrom)
                : message.From;

            if (string.IsNullOrWhiteSpace(from))
                throw new UserFriendlyException("O remetente é obrigatório para o Twilio.");

            var client = _httpClientFactory.CreateClient("EafSms");
            var content = new StringContent(
                $"To={Uri.EscapeDataString(message.PhoneNumber)}&From={Uri.EscapeDataString(from)}&Body={Uri.EscapeDataString(message.Body)}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.twilio.com/2010-04-01/Accounts/{opts.AccountSid}/Messages.json")
            {
                Content = content
            };

            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{opts.AccountSid}:{opts.AuthToken}"));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new SmsSendResult
                {
                    Succeeded = true,
                    MessageId = responseBody
                };
            }

            return new SmsSendResult
            {
                Succeeded = false,
                ErrorMessage = $"HTTP {(int)response.StatusCode}: {responseBody}"
            };
        }
    }
}

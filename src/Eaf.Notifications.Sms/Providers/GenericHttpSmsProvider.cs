using System;
using System.Net;
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
    /// Generic HTTP provider for REST SMS gateways such as Zenvia.
    /// </summary>
    public class GenericHttpSmsProvider : ISmsProvider, ITransientDependency
    {
        /// <inheritdoc/>
        public string Name => "GenericHttp";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SmsOptions _options;

        /// <summary>
        /// Creates a new <see cref="GenericHttpSmsProvider"/>.
        /// </summary>
        /// <param name="httpClientFactory">HTTP client factory.</param>
        /// <param name="optionsAccessor">SMS options.</param>
        public GenericHttpSmsProvider(IHttpClientFactory httpClientFactory, IOptions<SmsOptions> optionsAccessor)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
        }

        /// <inheritdoc/>
        public async Task<SmsSendResult> SendAsync(SmsMessage message, CancellationToken cancellationToken = default)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var opts = _options.GenericHttp ?? throw new UserFriendlyException("A seção Eaf:Sms:GenericHttp não foi configurada.");

            if (string.IsNullOrWhiteSpace(opts.BaseUrl))
                throw new UserFriendlyException("Eaf:Sms:GenericHttp:BaseUrl é obrigatório.");
            if (string.IsNullOrWhiteSpace(opts.Endpoint))
                throw new UserFriendlyException("Eaf:Sms:GenericHttp:Endpoint é obrigatório.");
            if (string.IsNullOrWhiteSpace(opts.Template))
                throw new UserFriendlyException("Eaf:Sms:GenericHttp:Template é obrigatório.");

            var client = _httpClientFactory.CreateClient("EafSms");
            client.Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds > 0 ? opts.TimeoutSeconds : 30);

            var from = string.IsNullOrWhiteSpace(message.From) ? _options.DefaultFrom : message.From;
            var content = opts.Template
                .Replace("{{phoneNumber}}", message.PhoneNumber ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("{{body}}", message.Body ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("{{from}}", from ?? string.Empty, StringComparison.OrdinalIgnoreCase);

            var request = new HttpRequestMessage(HttpMethod.Post, BuildUrl(opts))
            {
                Content = BuildContent(opts, content)
            };

            ApplyAuthentication(request, opts);

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

        private static Uri BuildUrl(GenericHttpSmsProviderOptions opts)
        {
            var baseUrl = opts.BaseUrl.TrimEnd('/');
            var endpoint = opts.Endpoint.TrimStart('/');
            return new Uri($"{baseUrl}/{endpoint}", UriKind.Absolute);
        }

        private static HttpContent BuildContent(GenericHttpSmsProviderOptions opts, string content)
        {
            if (string.Equals(opts.ContentType, "Form", StringComparison.OrdinalIgnoreCase))
                return new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");

            return new StringContent(content, Encoding.UTF8, "application/json");
        }

        private static void ApplyAuthentication(HttpRequestMessage request, GenericHttpSmsProviderOptions opts)
        {
            switch (opts.AuthenticationType?.ToLowerInvariant())
            {
                case "basic":
                    var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{opts.Username}:{opts.Password}"));
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
                    break;

                case "bearer":
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", opts.Token);
                    break;

                case "header":
                    if (!string.IsNullOrWhiteSpace(opts.ApiKeyHeaderName) && !string.IsNullOrWhiteSpace(opts.ApiKey))
                        request.Headers.Add(opts.ApiKeyHeaderName, opts.ApiKey);
                    break;
            }
        }
    }
}

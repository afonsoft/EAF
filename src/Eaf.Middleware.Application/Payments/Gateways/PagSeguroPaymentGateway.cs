using Abp.Configuration;
using Abp.Dependency;
using Abp.UI;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Payments.Dto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Eaf.Middleware.Payments.Gateways
{
    /// <summary>
    /// Gateway de pagamento PagSeguro para assinaturas.
    /// </summary>
    public class PagSeguroPaymentGateway : IPaymentGateway, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// PagSeguroPaymentGateway.
        /// </summary>
        /// <param name="settingManager">Gerenciador de configurações.</param>
        public PagSeguroPaymentGateway(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        /// <summary>
        /// Cria uma transação de checkout no PagSeguro (sandbox).
        /// </summary>
        public async Task<PaymentRequestDto> CreatePaymentAsync(CreatePaymentRequestInput input)
        {
            var (email, token) = await GetCredentialsAsync();
            var reference = Guid.NewGuid().ToString("N");

            var formData = new Dictionary<string, string>
            {
                { "currency", "BRL" },
                { "itemId1", input.EditionId.ToString() },
                { "itemDescription1", input.Description ?? $"Assinatura {input.EditionId}" },
                { "itemAmount1", input.Amount.ToString("F2", CultureInfo.InvariantCulture) },
                { "itemQuantity1", "1" },
                { "reference", reference }
            };

            var url = $"https://ws.sandbox.pagseguro.uol.com.br/v2/checkout?email={email}&token={token}";

            using var httpClient = new HttpClient();
            var content = new FormUrlEncodedContent(formData);
            var response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var xml = XDocument.Parse(responseBody);
            var code = xml.Root?.Element("code")?.Value;

            return new PaymentRequestDto
            {
                PaymentId = code,
                Gateway = "PagSeguro",
                IsSuccess = !string.IsNullOrEmpty(code),
                CheckoutUrl = !string.IsNullOrEmpty(code)
                    ? $"https://sandbox.pagseguro.uol.com.br/v2/checkout/payment.html?code={code}"
                    : null
            };
        }

        /// <summary>
        /// Consulta a transação e verifica se foi paga.
        /// </summary>
        public async Task<PaymentResultDto> ProcessPaymentAsync(ProcessPaymentInput input)
        {
            var (email, token) = await GetCredentialsAsync();
            var url = $"https://ws.sandbox.pagseguro.uol.com.br/v3/transactions/{input.ExternalPaymentId}?email={email}&token={token}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var xml = XDocument.Parse(responseBody);
            var statusText = xml.Root?.Element("status")?.Value;

            _ = int.TryParse(statusText, out var status);

            // 3 = Paga, 4 = Disponível
            return new PaymentResultDto
            {
                ExternalPaymentId = input.ExternalPaymentId,
                Gateway = "PagSeguro",
                IsSuccess = status == 3 || status == 4
            };
        }

        private async Task<(string Email, string Token)> GetCredentialsAsync()
        {
            var email = await _settingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.PagSeguro.Email);
            var token = await _settingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.PagSeguro.Token);

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                throw new UserFriendlyException("PagSeguro email/token is not configured.");
            }

            return (email, token);
        }
    }
}

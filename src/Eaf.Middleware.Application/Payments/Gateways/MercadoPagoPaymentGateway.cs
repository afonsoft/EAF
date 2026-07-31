using Abp.Configuration;
using Abp.Dependency;
using Abp.UI;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Payments.Dto;
using MercadoPago.Client;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Payments.Gateways
{
    /// <summary>
    /// Gateway de pagamento MercadoPago para assinaturas.
    /// </summary>
    public class MercadoPagoPaymentGateway : IPaymentGateway, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// MercadoPagoPaymentGateway.
        /// </summary>
        /// <param name="settingManager">Gerenciador de configurações.</param>
        public MercadoPagoPaymentGateway(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        /// <summary>
        /// Cria uma preferência de pagamento no MercadoPago.
        /// </summary>
        public async Task<PaymentRequestDto> CreatePaymentAsync(CreatePaymentRequestInput input)
        {
            var accessToken = await GetAccessTokenAsync();
            var requestOptions = new RequestOptions { AccessToken = accessToken };

            var client = new PreferenceClient();
            var request = new PreferenceRequest
            {
                ExternalReference = $"edition:{input.EditionId}",
                Items = new List<PreferenceItemRequest>
                {
                    new PreferenceItemRequest
                    {
                        Title = input.Description ?? $"Assinatura {input.EditionId}",
                        Quantity = 1,
                        UnitPrice = input.Amount,
                        CurrencyId = "BRL"
                    }
                },
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = "https://localhost/payment/success",
                    Pending = "https://localhost/payment/pending",
                    Failure = "https://localhost/payment/failure"
                },
                AutoReturn = "approved"
            };

            Preference preference = await client.CreateAsync(request, requestOptions);

            return new PaymentRequestDto
            {
                PaymentId = preference.Id,
                Gateway = "MercadoPago",
                IsSuccess = !string.IsNullOrEmpty(preference.Id),
                CheckoutUrl = preference.InitPoint ?? preference.SandboxInitPoint
            };
        }

        /// <summary>
        /// Consulta o pagamento e verifica se foi aprovado.
        /// </summary>
        public async Task<PaymentResultDto> ProcessPaymentAsync(ProcessPaymentInput input)
        {
            var accessToken = await GetAccessTokenAsync();
            var requestOptions = new RequestOptions { AccessToken = accessToken };

            if (!long.TryParse(input.ExternalPaymentId, out var paymentId))
            {
                throw new UserFriendlyException("Invalid MercadoPago payment id.");
            }

            var client = new PaymentClient();
            var payment = await client.GetAsync(paymentId, requestOptions);

            return new PaymentResultDto
            {
                ExternalPaymentId = input.ExternalPaymentId,
                Gateway = "MercadoPago",
                IsSuccess = payment.Status == "approved"
            };
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var accessToken = await _settingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.MercadoPago.AccessToken);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new UserFriendlyException("MercadoPago access token is not configured.");
            }

            return accessToken;
        }
    }
}

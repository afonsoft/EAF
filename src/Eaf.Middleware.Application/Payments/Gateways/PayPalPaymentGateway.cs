using Abp.Configuration;
using Abp.Dependency;
using Abp.UI;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Payments.Dto;
using PaypalServerSdk.Standard;
using PaypalServerSdk.Standard.Authentication;
using PaypalServerSdk.Standard.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Eaf.Middleware.Payments.Gateways
{
    /// <summary>
    /// Gateway de pagamento PayPal para assinaturas.
    /// </summary>
    public class PayPalPaymentGateway : IPaymentGateway, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// PayPalPaymentGateway.
        /// </summary>
        /// <param name="settingManager">Gerenciador de configurações.</param>
        public PayPalPaymentGateway(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        /// <summary>
        /// Cria uma ordem de pagamento no PayPal.
        /// </summary>
        public async Task<PaymentRequestDto> CreatePaymentAsync(CreatePaymentRequestInput input)
        {
            var (clientId, clientSecret) = await GetCredentialsAsync();
            var client = new PaypalServerSdkClient.Builder()
                .ClientCredentialsAuth(new ClientCredentialsAuthModel.Builder(clientId, clientSecret).Build())
                .Environment(PaypalServerSdk.Standard.Environment.Sandbox)
                .Build();

            var orderInput = new CreateOrderInput
            {
                Prefer = "return=representation",
                Body = new OrderRequest
                {
                    Intent = CheckoutPaymentIntent.Capture,
                    PurchaseUnits = new List<PurchaseUnitRequest>
                    {
                        new PurchaseUnitRequest
                        {
                            Description = input.Description,
                            Amount = new AmountWithBreakdown
                            {
                                CurrencyCode = "BRL",
                                MValue = input.Amount.ToString("F2", CultureInfo.InvariantCulture)
                            }
                        }
                    }
                }
            };

            var response = await client.OrdersController.CreateOrderAsync(orderInput);
            var order = response.Data;

            return new PaymentRequestDto
            {
                PaymentId = order.Id,
                Gateway = "PayPal",
                IsSuccess = response.StatusCode == 200 && !string.IsNullOrEmpty(order.Id),
                CheckoutUrl = $"https://www.sandbox.paypal.com/checkoutnow?token={order.Id}"
            };
        }

        /// <summary>
        /// Captura a ordem e verifica se foi concluída.
        /// </summary>
        public async Task<PaymentResultDto> ProcessPaymentAsync(ProcessPaymentInput input)
        {
            var (clientId, clientSecret) = await GetCredentialsAsync();
            var client = new PaypalServerSdkClient.Builder()
                .ClientCredentialsAuth(new ClientCredentialsAuthModel.Builder(clientId, clientSecret).Build())
                .Environment(PaypalServerSdk.Standard.Environment.Sandbox)
                .Build();

            var captureInput = new CaptureOrderInput { Id = input.ExternalPaymentId };
            var response = await client.OrdersController.CaptureOrderAsync(captureInput);
            var order = response.Data;

            return new PaymentResultDto
            {
                ExternalPaymentId = input.ExternalPaymentId,
                Gateway = "PayPal",
                IsSuccess = order.Status == OrderStatus.Completed
            };
        }

        private async Task<(string ClientId, string ClientSecret)> GetCredentialsAsync()
        {
            var clientId = await _settingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.PayPal.ClientId);
            var clientSecret = await _settingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.PayPal.ClientSecret);

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new UserFriendlyException("PayPal client id/secret is not configured.");
            }

            return (clientId, clientSecret);
        }
    }
}

using Abp.Configuration;
using Abp.Dependency;
using Abp.UI;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Payments.Dto;
using Stripe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Payments.Gateways
{
    /// <summary>
    /// Gateway de pagamento Stripe para assinaturas.
    /// </summary>
    public class StripePaymentGateway : IPaymentGateway, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// StripePaymentGateway.
        /// </summary>
        /// <param name="settingManager">Gerenciador de configurações.</param>
        public StripePaymentGateway(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        /// <summary>
        /// Cria um PaymentIntent no Stripe.
        /// </summary>
        public async Task<PaymentRequestDto> CreatePaymentAsync(CreatePaymentRequestInput input)
        {
            var secretKey = await GetSecretKeyAsync();
            var client = new StripeClient(secretKey);
            var service = new PaymentIntentService(client);

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(input.Amount * 100),
                Currency = "brl",
                Description = input.Description,
                Metadata = new Dictionary<string, string>
                {
                    { "editionId", input.EditionId.ToString() },
                    { "paymentPeriodType", input.PaymentPeriodType.ToString() }
                }
            };

            var intent = await service.CreateAsync(options);

            return new PaymentRequestDto
            {
                PaymentId = intent.Id,
                Gateway = "Stripe",
                IsSuccess = !string.IsNullOrEmpty(intent.Id),
                CheckoutUrl = null
            };
        }

        /// <summary>
        /// Recupera o PaymentIntent e verifica se foi confirmado.
        /// </summary>
        public async Task<PaymentResultDto> ProcessPaymentAsync(ProcessPaymentInput input)
        {
            var secretKey = await GetSecretKeyAsync();
            var client = new StripeClient(secretKey);
            var service = new PaymentIntentService(client);

            var intent = await service.GetAsync(input.ExternalPaymentId);

            return new PaymentResultDto
            {
                ExternalPaymentId = input.ExternalPaymentId,
                Gateway = "Stripe",
                IsSuccess = intent.Status == "succeeded"
            };
        }

        private async Task<string> GetSecretKeyAsync()
        {
            var secretKey = await _settingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.Stripe.SecretKey);
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new UserFriendlyException("Stripe secret key is not configured.");
            }

            return secretKey;
        }
    }
}

using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Timing;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Core.Editions;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Payments.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Payments
{
    /// <summary>
    /// Serviço de aplicação para gerenciamento de pagamentos de assinatura.
    /// </summary>
    [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Payments)]
    public class PaymentAppService : MiddlewareAppServiceBase, IPaymentAppService
    {
        private readonly IRepository<SubscriptionPayment, long> _subscriptionPaymentRepository;
        private readonly IRepository<SubscribableEdition, int> _editionRepository;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IPaymentGatewayResolver _paymentGatewayResolver;

        /// <summary>
        /// PaymentAppService.
        /// </summary>
        public PaymentAppService(
            IRepository<SubscriptionPayment, long> subscriptionPaymentRepository,
            IRepository<SubscribableEdition, int> editionRepository,
            IRepository<Tenant, int> tenantRepository,
            IPaymentGatewayResolver paymentGatewayResolver)
        {
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
            _editionRepository = editionRepository;
            _tenantRepository = tenantRepository;
            _paymentGatewayResolver = paymentGatewayResolver;
        }

        /// <summary>
        /// Obtém os pagamentos de assinatura paginados.
        /// </summary>
        public virtual async Task<PagedResultDto<SubscriptionPaymentDto>> GetAllAsync(GetSubscriptionPaymentsInput input)
        {
            var query = (await _subscriptionPaymentRepository.GetAllAsync())
                .WhereIf(!input.Filter.IsNullOrWhiteSpace(), p =>
                    p.Gateway.Contains(input.Filter) ||
                    p.Status.ToString() == input.Filter ||
                    p.ExternalPaymentId.Contains(input.Filter));

            var total = await query.CountAsync();
            var ordered = System.Linq.Dynamic.Core.DynamicQueryableExtensions.OrderBy(query, input.Sorting ?? "CreationTime desc");
            var items = await ordered.PageBy(input).ToListAsync();

            return new PagedResultDto<SubscriptionPaymentDto>(total, ObjectMapper.Map<List<SubscriptionPaymentDto>>(items));
        }

        /// <summary>
        /// Cria uma solicitação de pagamento para assinatura.
        /// </summary>
        public virtual async Task<PaymentRequestDto> CreatePaymentAsync(CreateSubscriptionPaymentInput input)
        {
            var edition = await _editionRepository.GetAsync(input.EditionId);
            var amount = edition.GetPaymentAmount(input.PaymentPeriodType);

            var gateway = _paymentGatewayResolver.Resolve(input.Gateway);
            var request = await gateway.CreatePaymentAsync(new CreatePaymentRequestInput
            {
                EditionId = input.EditionId,
                EditionPaymentType = input.EditionPaymentType,
                PaymentPeriodType = input.PaymentPeriodType,
                Amount = amount,
                Description = input.Description,
                Gateway = input.Gateway,
            });

            var payment = new SubscriptionPayment
            {
                TenantId = AbpSession.TenantId,
                EditionId = input.EditionId,
                EditionPaymentType = input.EditionPaymentType,
                PaymentPeriodType = input.PaymentPeriodType,
                Amount = amount,
                Status = SubscriptionPaymentStatus.Pending,
                Gateway = input.Gateway,
                Description = input.Description,
                ExternalPaymentId = request.PaymentId,
            };

            await _subscriptionPaymentRepository.InsertAsync(payment);

            return request;
        }

        /// <summary>
        /// Processa o retorno de pagamento e ativa a assinatura.
        /// </summary>
        public virtual async Task<SubscriptionPaymentDto> ProcessPaymentAsync(long paymentId, ProcessPaymentInput input)
        {
            var payment = await _subscriptionPaymentRepository.GetAsync(paymentId);
            var gateway = _paymentGatewayResolver.Resolve(input.Gateway);

            var result = await gateway.ProcessPaymentAsync(input);
            payment.GatewayResponse = input.GatewayResponse;

            if (result.IsSuccess)
            {
                payment.Status = SubscriptionPaymentStatus.Completed;
                payment.PaymentTime = Clock.Now;
                payment.SubscriptionStartDate = Clock.Now;
                payment.SubscriptionEndDate = CalculateEndDate(Clock.Now, payment.PaymentPeriodType);

                var edition = await _editionRepository.GetAsync(payment.EditionId);
                await ActivateTenantSubscriptionAsync(payment, edition);
            }
            else
            {
                payment.Status = SubscriptionPaymentStatus.Failed;
            }

            await _subscriptionPaymentRepository.UpdateAsync(payment);

            return ObjectMapper.Map<SubscriptionPaymentDto>(payment);
        }

        /// <summary>
        /// Lista os gateways de pagamento disponíveis e suas configurações.
        /// </summary>
        public virtual async Task<List<PaymentGatewayDto>> GetGatewayListAsync()
        {
            var defaultGateway = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.DefaultGateway);
            var gateways = new List<PaymentGatewayDto>();

            await AddGatewayIfConfiguredAsync(gateways, "Stripe", defaultGateway, AppSettings.Payment.Stripe.SecretKey);
            await AddGatewayIfConfiguredAsync(gateways, "PayPal", defaultGateway, AppSettings.Payment.PayPal.ClientId);
            await AddGatewayIfConfiguredAsync(gateways, "MercadoPago", defaultGateway, AppSettings.Payment.MercadoPago.AccessToken);
            await AddGatewayIfConfiguredAsync(gateways, "PagSeguro", defaultGateway, AppSettings.Payment.PagSeguro.Token);

            return gateways;
        }

        /// <summary>
        /// Obtém as configurações dos gateways de pagamento.
        /// </summary>
        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Payments_GatewaySettings)]
        public virtual async Task<PaymentGatewaySettingsDto> GetGatewaySettingsAsync()
        {
            return new PaymentGatewaySettingsDto
            {
                DefaultGateway = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.DefaultGateway),
                Stripe = new StripePaymentGatewaySettingsDto
                {
                    SecretKey = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.Stripe.SecretKey),
                    PublishableKey = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.Stripe.PublishableKey),
                    WebhookSecret = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.Stripe.WebhookSecret)
                },
                PayPal = new PayPalPaymentGatewaySettingsDto
                {
                    ClientId = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.PayPal.ClientId),
                    ClientSecret = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.PayPal.ClientSecret),
                    WebhookId = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.PayPal.WebhookId)
                },
                MercadoPago = new MercadoPagoPaymentGatewaySettingsDto
                {
                    AccessToken = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.MercadoPago.AccessToken),
                    PublicKey = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.MercadoPago.PublicKey)
                },
                PagSeguro = new PagSeguroPaymentGatewaySettingsDto
                {
                    Token = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.PagSeguro.Token),
                    Email = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.PagSeguro.Email)
                }
            };
        }

        /// <summary>
        /// Atualiza as configurações dos gateways de pagamento.
        /// </summary>
        [AbpAuthorize(MiddlewarePermissions.Pages_Administration_Payments_GatewaySettings)]
        public virtual async Task UpdateGatewaySettingsAsync(PaymentGatewaySettingsDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.Payment.DefaultGateway, input.DefaultGateway ?? string.Empty);

            if (input.Stripe != null)
            {
                await SettingManager.ChangeSettingForApplicationAsync(AppSettings.Payment.Stripe.SecretKey, input.Stripe.SecretKey ?? string.Empty);
                await SettingManager.ChangeSettingForApplicationAsync(AppSettings.Payment.Stripe.PublishableKey, input.Stripe.PublishableKey ?? string.Empty);
                await SettingManager.ChangeSettingForApplicationAsync(AppSettings.Payment.Stripe.WebhookSecret, input.Stripe.WebhookSecret ?? string.Empty);
            }

            if (input.PayPal != null)
            {
                await SettingManager.ChangeSettingForApplicationAsync(AppSettings.Payment.PayPal.ClientId, input.PayPal.ClientId ?? string.Empty);
                await SettingManager.ChangeSettingForApplicationAsync(AppSettings.Payment.PayPal.ClientSecret, input.PayPal.ClientSecret ?? string.Empty);
                await SettingManager.ChangeSettingForApplicationAsync(AppSettings.Payment.PayPal.WebhookId, input.PayPal.WebhookId ?? string.Empty);
            }

            if (input.MercadoPago != null)
            {
                await SettingManager.ChangeSettingForApplicationAsync(AppSettings.Payment.MercadoPago.AccessToken, input.MercadoPago.AccessToken ?? string.Empty);
                await SettingManager.ChangeSettingForApplicationAsync(AppSettings.Payment.MercadoPago.PublicKey, input.MercadoPago.PublicKey ?? string.Empty);
            }

            if (input.PagSeguro != null)
            {
                await SettingManager.ChangeSettingForApplicationAsync(AppSettings.Payment.PagSeguro.Token, input.PagSeguro.Token ?? string.Empty);
                await SettingManager.ChangeSettingForApplicationAsync(AppSettings.Payment.PagSeguro.Email, input.PagSeguro.Email ?? string.Empty);
            }
        }

        private async Task AddGatewayIfConfiguredAsync(List<PaymentGatewayDto> gateways, string name, string defaultGateway, string requiredSettingKey)
        {
            var value = await SettingManager.GetSettingValueForApplicationAsync(requiredSettingKey);
            gateways.Add(new PaymentGatewayDto
            {
                Name = name,
                DisplayName = name,
                IsConfigured = !string.IsNullOrWhiteSpace(value),
                IsDefault = name.Equals(defaultGateway, StringComparison.OrdinalIgnoreCase)
            });
        }

        private async Task ActivateTenantSubscriptionAsync(SubscriptionPayment payment, SubscribableEdition edition)
        {
            if (!payment.TenantId.HasValue)
            {
                return;
            }

            var tenant = await _tenantRepository.GetAsync(payment.TenantId.Value);
            tenant.EditionId = edition.Id;
            tenant.SubscriptionEndDateUtc = payment.SubscriptionEndDate;
            await _tenantRepository.UpdateAsync(tenant);
        }

        private static DateTime? CalculateEndDate(DateTime start, PaymentPeriodType period)
        {
            return period switch
            {
                PaymentPeriodType.Daily => start.AddDays(1),
                PaymentPeriodType.Weekly => start.AddDays(7),
                PaymentPeriodType.Monthly => start.AddMonths(1),
                PaymentPeriodType.Quarterly => start.AddMonths(3),
                PaymentPeriodType.Biannual => start.AddMonths(6),
                PaymentPeriodType.Annual => start.AddYears(1),
                PaymentPeriodType.Permanent => null,
                _ => start,
            };
        }
    }
}

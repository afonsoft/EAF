using Abp.Application.Editions;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Timing;
using Eaf.Middleware.Authorization;
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
            };

            await _subscriptionPaymentRepository.InsertAsync(payment);

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

            payment.ExternalPaymentId = request.PaymentId;
            await _subscriptionPaymentRepository.UpdateAsync(payment);

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

                var edition = await _editionRepository.GetAsync(payment.EditionId);
                payment.SubscriptionStartDate = Clock.Now;
                payment.SubscriptionEndDate = CalculateEndDate(Clock.Now, payment.PaymentPeriodType);

                await ActivateTenantSubscriptionAsync(payment, edition);
            }
            else
            {
                payment.Status = SubscriptionPaymentStatus.Failed;
            }

            await _subscriptionPaymentRepository.UpdateAsync(payment);

            return ObjectMapper.Map<SubscriptionPaymentDto>(payment);
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

        private static DateTime CalculateEndDate(DateTime start, PaymentPeriodType period)
        {
            return period switch
            {
                PaymentPeriodType.Daily => start.AddDays(1),
                PaymentPeriodType.Weekly => start.AddDays(7),
                PaymentPeriodType.Monthly => start.AddMonths(1),
                PaymentPeriodType.Annual => start.AddYears(1),
                _ => start,
            };
        }
    }
}

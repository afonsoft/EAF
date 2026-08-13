using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Eaf.Middleware.Core.Editions;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Payments.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Payments
{
    /// <summary>
    /// Gerenciador de ciclo de vida de pagamentos de assinatura.
    /// </summary>
    [RemoteService(false)]
    public class PaymentManager : MiddlewareAppServiceBase, IPaymentManager
    {
        private readonly IRepository<SubscriptionPayment, long> _subscriptionPaymentRepository;
        private readonly IRepository<SubscriptionPaymentProduct, long> _subscriptionPaymentProductRepository;
        private readonly IRepository<SubscribableEdition, int> _editionRepository;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IPaymentGatewayResolver _paymentGatewayResolver;

        /// <summary>
        /// PaymentManager.
        /// </summary>
        public PaymentManager(
            IRepository<SubscriptionPayment, long> subscriptionPaymentRepository,
            IRepository<SubscriptionPaymentProduct, long> subscriptionPaymentProductRepository,
            IRepository<SubscribableEdition, int> editionRepository,
            IRepository<Tenant, int> tenantRepository,
            IPaymentGatewayResolver paymentGatewayResolver)
        {
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
            _subscriptionPaymentProductRepository = subscriptionPaymentProductRepository;
            _editionRepository = editionRepository;
            _tenantRepository = tenantRepository;
            _paymentGatewayResolver = paymentGatewayResolver;
        }

        /// <summary>
        /// Cria um pagamento de assinatura pendente e solicita o gateway.
        /// </summary>
        public virtual async Task<PaymentRequestDto> CreatePaymentAsync(CreateSubscriptionPaymentInput input)
        {
            var edition = await _editionRepository.GetAsync(input.EditionId);
            var products = BuildProducts(input.Products, edition, input.PaymentPeriodType);
            var amount = products.Sum(p => p.TotalAmount);

            var payment = new SubscriptionPayment
            {
                TenantId = AbpSession.TenantId,
                EditionId = input.EditionId,
                EditionPaymentType = input.EditionPaymentType,
                PaymentPeriodType = input.PaymentPeriodType,
                Amount = amount,
                IsRecurring = input.IsRecurring,
                Status = SubscriptionPaymentStatus.Pending,
                Gateway = input.Gateway,
                Description = input.Description,
                SuccessUrl = input.SuccessUrl,
                ErrorUrl = input.ErrorUrl,
            };

            foreach (var product in products)
            {
                product.TenantId = payment.TenantId;
                payment.Products.Add(product);
            }

            var gateway = _paymentGatewayResolver.Resolve(input.Gateway);
            var request = await gateway.CreatePaymentAsync(new CreatePaymentRequestInput
            {
                EditionId = input.EditionId,
                EditionPaymentType = input.EditionPaymentType,
                PaymentPeriodType = input.PaymentPeriodType,
                Amount = amount,
                IsRecurring = input.IsRecurring,
                Description = input.Description,
                Gateway = input.Gateway,
                SuccessUrl = input.SuccessUrl,
                ErrorUrl = input.ErrorUrl,
                Products = input.Products ?? new List<SubscriptionPaymentProductInput>(),
            });

            payment.ExternalPaymentId = request.PaymentId;

            await _subscriptionPaymentRepository.InsertAsync(payment);
            await CurrentUnitOfWork.SaveChangesAsync();

            return new PaymentRequestDto
            {
                SubscriptionPaymentId = payment.Id,
                PaymentId = payment.ExternalPaymentId,
                GatewayPaymentId = request.GatewayPaymentId,
                Gateway = input.Gateway,
                CheckoutUrl = request.CheckoutUrl,
                IsSuccess = request.IsSuccess && payment.Id > 0,
            };
        }

        /// <summary>
        /// Processa o retorno do gateway e ativa a assinatura.
        /// </summary>
        public virtual async Task<SubscriptionPaymentDto> ProcessPaymentAsync(long paymentId, ProcessPaymentInput input)
        {
            var payment = await _subscriptionPaymentRepository.GetAsync(paymentId);
            var products = await _subscriptionPaymentProductRepository.GetAllListAsync(p => p.SubscriptionPaymentId == paymentId);

            var gateway = _paymentGatewayResolver.Resolve(input.Gateway);
            var result = await gateway.ProcessPaymentAsync(input);

            payment.GatewayResponse = input.GatewayResponse;

            if (result.IsSuccess)
            {
                payment.Status = SubscriptionPaymentStatus.Completed;
                payment.PaymentTime = Clock.Now;
                payment.SubscriptionStartDate = Clock.Now;
                payment.SubscriptionEndDate = result.SubscriptionEndDate ?? PaymentPeriodHelper.GetEndDate(Clock.Now, payment.PaymentPeriodType);
                payment.InvoiceNo = GenerateInvoiceNo(payment.Id);
                payment.GatewaySubscriptionId = result.GatewaySubscriptionId ?? payment.GatewaySubscriptionId;

                var edition = await _editionRepository.GetAsync(payment.EditionId);
                await ActivateTenantSubscriptionAsync(payment, edition);
            }
            else
            {
                payment.Status = SubscriptionPaymentStatus.Failed;
            }

            await _subscriptionPaymentRepository.UpdateAsync(payment);

            var dto = ObjectMapper.Map<SubscriptionPaymentDto>(payment);
            dto.Products = ObjectMapper.Map<List<SubscriptionPaymentProductDto>>(products);
            return dto;
        }

        /// <summary>
        /// Upgrade/downgrade de edição com cálculo de prorração.
        /// </summary>
        public virtual async Task<PaymentRequestDto> UpgradeSubscriptionAsync(UpgradeSubscriptionInput input)
        {
            var tenantId = input.TenantId ?? AbpSession.TenantId ?? throw new UserFriendlyException(L("TenantIdRequired"));

            var currentPayment = (await _subscriptionPaymentRepository
                .GetAllListAsync(p => p.TenantId == tenantId && p.Status == SubscriptionPaymentStatus.Completed))
                .OrderByDescending(p => p.SubscriptionEndDate)
                .FirstOrDefault();

            var newEdition = await _editionRepository.GetAsync(input.NewEditionId);
            var newAmount = newEdition.GetPaymentAmount(input.PaymentPeriodType);

            if (currentPayment == null || !currentPayment.SubscriptionEndDate.HasValue || currentPayment.SubscriptionEndDate.Value <= Clock.Now)
            {
                return await CreatePaymentAsync(new CreateSubscriptionPaymentInput
                {
                    EditionId = input.NewEditionId,
                    EditionPaymentType = EditionPaymentType.Upgrade,
                    PaymentPeriodType = input.PaymentPeriodType,
                    Gateway = input.Gateway,
                    IsRecurring = false,
                    Products = new List<SubscriptionPaymentProductInput>
                    {
                        new() { Description = newEdition.DisplayName, Count = 1, Amount = newAmount },
                    },
                });
            }

            var (prorationAmount, isUpgrade) = CalculateProration(currentPayment, newAmount);

            var prorationPayment = new SubscriptionPayment
            {
                TenantId = tenantId,
                EditionId = input.NewEditionId,
                EditionPaymentType = isUpgrade ? EditionPaymentType.Upgrade : EditionPaymentType.Downgrade,
                PaymentPeriodType = input.PaymentPeriodType,
                Amount = prorationAmount,
                IsProrationPayment = true,
                Status = SubscriptionPaymentStatus.Pending,
                Gateway = input.Gateway,
                SubscriptionStartDate = Clock.Now,
                SubscriptionEndDate = currentPayment.SubscriptionEndDate.Value,
                Description = $"{(isUpgrade ? "Upgrade" : "Downgrade")} to {newEdition.DisplayName}",
            };

            prorationPayment.Products.Add(new SubscriptionPaymentProduct
            {
                TenantId = tenantId,
                Description = $"Proration: {newEdition.DisplayName}",
                Count = 1,
                Amount = prorationAmount,
                TotalAmount = prorationAmount,
            });

            if (!isUpgrade && prorationAmount <= 0)
            {
                prorationPayment.Status = SubscriptionPaymentStatus.Completed;
                prorationPayment.PaymentTime = Clock.Now;
                await _tenantRepository.UpdateAsync(await ActivateTenantEditionAsync(tenantId, input.NewEditionId, currentPayment.SubscriptionEndDate.Value));
                await _subscriptionPaymentRepository.InsertAsync(prorationPayment);
                await CurrentUnitOfWork.SaveChangesAsync();

                return new PaymentRequestDto
                {
                    SubscriptionPaymentId = prorationPayment.Id,
                    Gateway = input.Gateway,
                    IsSuccess = true,
                };
            }

            var gateway = _paymentGatewayResolver.Resolve(input.Gateway);
            var gatewayRequest = await gateway.CreatePaymentAsync(new CreatePaymentRequestInput
            {
                EditionId = input.NewEditionId,
                EditionPaymentType = prorationPayment.EditionPaymentType,
                PaymentPeriodType = input.PaymentPeriodType,
                Amount = prorationAmount,
                Description = prorationPayment.Description,
                Gateway = input.Gateway,
                Products = new List<SubscriptionPaymentProductInput>
                {
                    new() { Description = prorationPayment.Description, Count = 1, Amount = prorationAmount },
                },
            });

            prorationPayment.ExternalPaymentId = gatewayRequest.PaymentId;
            await _subscriptionPaymentRepository.InsertAsync(prorationPayment);
            await CurrentUnitOfWork.SaveChangesAsync();

            return new PaymentRequestDto
            {
                SubscriptionPaymentId = prorationPayment.Id,
                PaymentId = prorationPayment.ExternalPaymentId,
                GatewayPaymentId = gatewayRequest.GatewayPaymentId,
                Gateway = input.Gateway,
                CheckoutUrl = gatewayRequest.CheckoutUrl,
                IsSuccess = gatewayRequest.IsSuccess,
            };
        }

        /// <summary>
        /// Cancela uma assinatura recorrente.
        /// </summary>
        public virtual async Task<SubscriptionPaymentDto> CancelRecurringAsync(long paymentId)
        {
            var payment = await _subscriptionPaymentRepository.GetAsync(paymentId);
            var products = await _subscriptionPaymentProductRepository.GetAllListAsync(p => p.SubscriptionPaymentId == paymentId);

            if (!payment.IsRecurring)
            {
                throw new UserFriendlyException(L("PaymentIsNotRecurring"));
            }

            if (!string.IsNullOrWhiteSpace(payment.GatewaySubscriptionId))
            {
                var gateway = _paymentGatewayResolver.Resolve(payment.Gateway);
                if (gateway is ISubscriptionPaymentGateway subscriptionGateway)
                {
                    await subscriptionGateway.CancelSubscriptionAsync(payment.GatewaySubscriptionId);
                }
            }

            payment.IsRecurring = false;
            payment.Status = SubscriptionPaymentStatus.Canceled;

            await _subscriptionPaymentRepository.UpdateAsync(payment);

            var dto = ObjectMapper.Map<SubscriptionPaymentDto>(payment);
            dto.Products = ObjectMapper.Map<List<SubscriptionPaymentProductDto>>(products);
            return dto;
        }

        /// <summary>
        /// Renova/estende assinaturas recorrentes ativas consultando o gateway.
        /// </summary>
        public virtual async Task RenewActiveSubscriptionsAsync()
        {
            var today = Clock.Now.Date;
            var payments = await _subscriptionPaymentRepository.GetAllListAsync(p =>
                p.IsRecurring &&
                p.Status == SubscriptionPaymentStatus.Completed &&
                p.GatewaySubscriptionId != null &&
                p.SubscriptionEndDate.HasValue &&
                p.SubscriptionEndDate.Value.Date <= today);

            foreach (var payment in payments)
            {
                var gateway = _paymentGatewayResolver.Resolve(payment.Gateway);
                if (gateway is not ISubscriptionPaymentGateway subscriptionGateway)
                {
                    continue;
                }

                var status = await subscriptionGateway.GetSubscriptionStatusAsync(payment.GatewaySubscriptionId);
                if (status.IsActive && status.CurrentPeriodEnd.HasValue)
                {
                    payment.SubscriptionEndDate = status.CurrentPeriodEnd.Value;
                }
                else if (!status.IsActive)
                {
                    payment.Status = status.Status?.ToLowerInvariant() == "past_due"
                        ? SubscriptionPaymentStatus.PastDue
                        : SubscriptionPaymentStatus.Canceled;
                }

                await _subscriptionPaymentRepository.UpdateAsync(payment);
            }
        }

        /// <summary>
        /// Processa um webhook do gateway de pagamento e atualiza a assinatura.
        /// </summary>
        public virtual async Task<SubscriptionPaymentDto> ProcessWebhookAsync(string gateway, string json, string signature)
        {
            var gatewayImpl = _paymentGatewayResolver.Resolve(gateway);
            if (gatewayImpl is not ISubscriptionPaymentGateway subscriptionGateway)
            {
                throw new UserFriendlyException($"Gateway {gateway} does not support subscription webhooks.");
            }

            var result = await subscriptionGateway.ProcessWebhookAsync(null, json, signature);

            if (string.IsNullOrWhiteSpace(result.GatewaySubscriptionId) && string.IsNullOrWhiteSpace(result.ExternalPaymentId))
            {
                throw new UserFriendlyException("Webhook does not contain a payment reference.");
            }

            SubscriptionPayment payment = null;

            if (!string.IsNullOrWhiteSpace(result.GatewaySubscriptionId))
            {
                var payments = await _subscriptionPaymentRepository.GetAllListAsync(p => p.GatewaySubscriptionId == result.GatewaySubscriptionId);
                payment = payments.FirstOrDefault();
            }

            if (payment == null && !string.IsNullOrWhiteSpace(result.ExternalPaymentId))
            {
                var payments = await _subscriptionPaymentRepository.GetAllListAsync(p => p.ExternalPaymentId == result.ExternalPaymentId);
                payment = payments.FirstOrDefault();
            }

            if (payment == null)
            {
                throw new UserFriendlyException("Subscription payment not found for webhook.");
            }

            var products = await _subscriptionPaymentProductRepository.GetAllListAsync(p => p.SubscriptionPaymentId == payment.Id);

            if (result.IsSuccess)
            {
                payment.Status = SubscriptionPaymentStatus.Completed;
                payment.PaymentTime = Clock.Now;
                payment.InvoiceNo = result.InvoiceNo ?? payment.InvoiceNo;
                if (result.SubscriptionEndDate.HasValue)
                {
                    payment.SubscriptionEndDate = result.SubscriptionEndDate.Value;
                }

                var edition = await _editionRepository.GetAsync(payment.EditionId);
                await ActivateTenantSubscriptionAsync(payment, edition);
            }
            else if (payment.IsRecurring)
            {
                payment.Status = SubscriptionPaymentStatus.PastDue;
            }
            else
            {
                payment.Status = SubscriptionPaymentStatus.Failed;
            }

            await _subscriptionPaymentRepository.UpdateAsync(payment);

            var dto = ObjectMapper.Map<SubscriptionPaymentDto>(payment);
            dto.Products = ObjectMapper.Map<List<SubscriptionPaymentProductDto>>(products);
            return dto;
        }

        private List<SubscriptionPaymentProduct> BuildProducts(List<SubscriptionPaymentProductInput> inputs, SubscribableEdition edition, PaymentPeriodType period)
        {
            if (inputs != null && inputs.Count > 0)
            {
                return inputs.Select(i => new SubscriptionPaymentProduct
                {
                    Description = i.Description,
                    Count = i.Count,
                    Amount = i.Amount,
                    TotalAmount = i.Count * i.Amount,
                }).ToList();
            }

            var amount = edition.GetPaymentAmount(period);
            return new List<SubscriptionPaymentProduct>
            {
                new()
                {
                    Description = edition.DisplayName,
                    Count = 1,
                    Amount = amount,
                    TotalAmount = amount,
                },
            };
        }

        private (decimal Amount, bool IsUpgrade) CalculateProration(SubscriptionPayment currentPayment, decimal newAmount)
        {
            if (!currentPayment.SubscriptionEndDate.HasValue || !currentPayment.SubscriptionStartDate.HasValue)
            {
                return (newAmount, newAmount >= currentPayment.Amount);
            }

            var totalDays = PaymentPeriodHelper.GetDaysInPeriod(currentPayment.PaymentPeriodType, currentPayment.SubscriptionStartDate.Value);
            var remainingDays = Math.Max(0, (currentPayment.SubscriptionEndDate.Value - Clock.Now).Days);
            var dailyRate = currentPayment.Amount / totalDays;
            var unusedValue = dailyRate * remainingDays;
            var difference = newAmount - unusedValue;

            var isUpgrade = newAmount >= currentPayment.Amount;
            return (Math.Max(0, difference), isUpgrade);
        }

        private async Task ActivateTenantSubscriptionAsync(SubscriptionPayment payment, SubscribableEdition edition)
        {
            if (!payment.TenantId.HasValue)
            {
                return;
            }

            await ActivateTenantEditionAsync(payment.TenantId.Value, edition.Id, payment.SubscriptionEndDate);
        }

        private async Task<Tenant> ActivateTenantEditionAsync(int tenantId, int editionId, DateTime? subscriptionEndDate)
        {
            var tenant = await _tenantRepository.GetAsync(tenantId);
            tenant.EditionId = editionId;
            tenant.SubscriptionEndDateUtc = subscriptionEndDate;
            return await _tenantRepository.UpdateAsync(tenant);
        }

        private string GenerateInvoiceNo(long paymentId)
        {
            return $"EAF{Clock.Now:yyyy}-{paymentId:D6}";
        }
    }
}

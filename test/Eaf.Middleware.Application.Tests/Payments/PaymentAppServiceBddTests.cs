using Abp.Application.Editions;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Core.Editions;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Payments;
using Eaf.Middleware.Payments.Dto;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Payments
{
    /// <summary>
    /// Testes BDD para PaymentAppService.
    /// </summary>
    public class PaymentAppServiceBddTests
    {
        private readonly PaymentAppService _sut;
        private readonly IRepository<SubscriptionPayment, long> _subscriptionPaymentRepository;
        private readonly IRepository<SubscribableEdition, int> _editionRepository;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IPaymentGatewayResolver _paymentGatewayResolver;
        private readonly IPaymentGateway _paymentGateway;

        public PaymentAppServiceBddTests()
        {
            _subscriptionPaymentRepository = Substitute.For<IRepository<SubscriptionPayment, long>>();
            _editionRepository = Substitute.For<IRepository<SubscribableEdition, int>>();
            _tenantRepository = Substitute.For<IRepository<Tenant, int>>();
            _paymentGatewayResolver = Substitute.For<IPaymentGatewayResolver>();
            _paymentGateway = Substitute.For<IPaymentGateway>();

            _paymentGatewayResolver.Resolve(Arg.Any<string>()).Returns(_paymentGateway);
            _paymentGateway.CreatePaymentAsync(Arg.Any<CreatePaymentRequestInput>()).Returns(new PaymentRequestDto
            {
                PaymentId = "PAY-123",
                Gateway = "Null",
                IsSuccess = true,
            });
            _paymentGateway.ProcessPaymentAsync(Arg.Any<ProcessPaymentInput>()).Returns(new PaymentResultDto
            {
                ExternalPaymentId = "PAY-123",
                Gateway = "Null",
                IsSuccess = true,
            });

            _sut = new PaymentAppService(
                _subscriptionPaymentRepository,
                _editionRepository,
                _tenantRepository,
                _paymentGatewayResolver);

            _sut.ObjectMapper = CreateObjectMapper();
            _sut.UnitOfWorkManager = ManagerTestHelper.CreateUnitOfWorkManager();
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)1);
            _sut.AbpSession = abpSession;
        }

        private static IObjectMapper CreateObjectMapper()
        {
            var mapper = Substitute.For<IObjectMapper>();
            mapper.Map<SubscriptionPaymentDto>(Arg.Any<SubscriptionPayment>()).Returns(ci =>
            {
                var p = (SubscriptionPayment)ci[0];
                return new SubscriptionPaymentDto
                {
                    Id = p.Id,
                    TenantId = p.TenantId,
                    EditionId = p.EditionId,
                    Amount = p.Amount,
                    Status = p.Status.ToString(),
                    Gateway = p.Gateway,
                    ExternalPaymentId = p.ExternalPaymentId,
                };
            });
            mapper.Map<List<SubscriptionPaymentDto>>(Arg.Any<IEnumerable<SubscriptionPayment>>()).Returns(ci =>
            {
                var payments = (IEnumerable<SubscriptionPayment>)ci[0];
                return payments.Select(p => mapper.Map<SubscriptionPaymentDto>(p)).ToList();
            });
            return mapper;
        }

        [Fact]
        public async Task Dado_InputValido_Quando_CreatePaymentAsync_Entao_DeveCriarPagamentoPendenteEGerarRequest()
        {
            // Dado
            var edition = new SubscribableEdition { Id = 1, DisplayName = "Pro", MonthlyPrice = 100 };
            _editionRepository.GetAsync(1).Returns(edition);

            var input = new CreateSubscriptionPaymentInput
            {
                EditionId = 1,
                EditionPaymentType = EditionPaymentType.Upgrade,
                PaymentPeriodType = PaymentPeriodType.Monthly,
                Gateway = "Null",
            };

            SubscriptionPayment inserted = null;
            await _subscriptionPaymentRepository.InsertAsync(Arg.Do<SubscriptionPayment>(p => inserted = p));

            // Quando
            var result = await _sut.CreatePaymentAsync(input);

            // Então
            inserted.ShouldNotBeNull();
            inserted.Amount.ShouldBe(100);
            inserted.Status.ShouldBe(SubscriptionPaymentStatus.Pending);
            inserted.ExternalPaymentId.ShouldBe("PAY-123");
            result.PaymentId.ShouldBe("PAY-123");
        }

        [Fact]
        public async Task Dado_PagamentoProcessadoComSucesso_Quando_ProcessPaymentAsync_Entao_DeveAtivarAssinaturaDoTenant()
        {
            // Dado
            var tenant = new Tenant("acme", "ACME");
            _tenantRepository.GetAsync(1).Returns(tenant);

            var edition = new SubscribableEdition { Id = 1, DisplayName = "Pro" };
            _editionRepository.GetAsync(1).Returns(edition);

            var payment = new SubscriptionPayment
            {
                Id = 1,
                TenantId = 1,
                EditionId = 1,
                Amount = 100,
                Status = SubscriptionPaymentStatus.Pending,
                Gateway = "Null",
                ExternalPaymentId = "PAY-123",
                PaymentPeriodType = PaymentPeriodType.Monthly,
            };
            _subscriptionPaymentRepository.GetAsync(1).Returns(payment);

            var input = new ProcessPaymentInput
            {
                ExternalPaymentId = "PAY-123",
                Gateway = "Null",
                IsSuccess = true,
            };

            // Quando
            var result = await _sut.ProcessPaymentAsync(1, input);

            // Então
            payment.Status.ShouldBe(SubscriptionPaymentStatus.Completed);
            tenant.EditionId.ShouldBe(1);
            tenant.SubscriptionEndDateUtc.HasValue.ShouldBeTrue();
            result.Status.ShouldBe(SubscriptionPaymentStatus.Completed.ToString());
        }
    }
}

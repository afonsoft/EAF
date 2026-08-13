using Abp.Domain.Repositories;
using Abp.Localization;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Abp.Timing;
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
    /// Testes BDD para PaymentManager.
    /// </summary>
    public class PaymentManagerBddTests
    {
        private readonly PaymentManager _sut;
        private readonly IRepository<SubscriptionPayment, long> _subscriptionPaymentRepository;
        private readonly IRepository<SubscriptionPaymentProduct, long> _subscriptionPaymentProductRepository;
        private readonly IRepository<SubscribableEdition, int> _editionRepository;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IPaymentGatewayResolver _paymentGatewayResolver;
        private readonly IPaymentGateway _paymentGateway;

        public PaymentManagerBddTests()
        {
            _subscriptionPaymentRepository = Substitute.For<IRepository<SubscriptionPayment, long>>();
            _subscriptionPaymentProductRepository = Substitute.For<IRepository<SubscriptionPaymentProduct, long>>();
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

            _sut = new PaymentManager(
                _subscriptionPaymentRepository,
                _subscriptionPaymentProductRepository,
                _editionRepository,
                _tenantRepository,
                _paymentGatewayResolver);

            _sut.ObjectMapper = CreateObjectMapper();
            _sut.UnitOfWorkManager = ManagerTestHelper.CreateUnitOfWorkManager();
            _sut.LocalizationManager = Substitute.For<ILocalizationManager>();
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
            mapper.Map<List<SubscriptionPaymentProductDto>>(Arg.Any<IEnumerable<SubscriptionPaymentProduct>>()).Returns(new List<SubscriptionPaymentProductDto>());
            mapper.Map<List<SubscriptionPaymentProductDto>>(Arg.Any<List<SubscriptionPaymentProduct>>()).Returns(new List<SubscriptionPaymentProductDto>());
            return mapper;
        }

        [Fact]
        public async Task Dado_EdicaoPagaValida_Quando_CreatePaymentAsync_Entao_DeveCriarPagamentoPendenteComProdutos()
        {
            // Dado
            var edition = new SubscribableEdition { Id = 1, DisplayName = "Pro", MonthlyPrice = 100 };
            _editionRepository.GetAsync(1).Returns(edition);

            var input = new CreateSubscriptionPaymentInput
            {
                EditionId = 1,
                EditionPaymentType = EditionPaymentType.NewRegistration,
                PaymentPeriodType = PaymentPeriodType.Monthly,
                Gateway = "Null",
                IsRecurring = false,
            };

            SubscriptionPayment inserted = null;
            await _subscriptionPaymentRepository.InsertAsync(Arg.Do<SubscriptionPayment>(p => inserted = p));

            // Quando
            var result = await _sut.CreatePaymentAsync(input);

            // Então
            inserted.ShouldNotBeNull();
            inserted.Amount.ShouldBe(100);
            inserted.Products.Count.ShouldBe(1);
            inserted.Products.First().Amount.ShouldBe(100);
            inserted.Status.ShouldBe(SubscriptionPaymentStatus.Pending);
            result.PaymentId.ShouldBe("PAY-123");
        }

        [Fact]
        public async Task Dado_PagamentoPendente_Quando_ProcessPaymentAsync_Entao_DeveCompletarEAtivarTenant()
        {
            // Dado
            var edition = new SubscribableEdition { Id = 1, DisplayName = "Pro" };
            _editionRepository.GetAsync(1).Returns(edition);

            var tenant = new Tenant("acme", "ACME");
            _tenantRepository.GetAsync(1).Returns(tenant);

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
            _subscriptionPaymentProductRepository.GetAllListAsync(Arg.Any<System.Linq.Expressions.Expression<Func<SubscriptionPaymentProduct, bool>>>())
                .Returns(new List<SubscriptionPaymentProduct>());

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
            payment.SubscriptionEndDate.ShouldNotBeNull();
            tenant.EditionId.ShouldBe(1);
            tenant.SubscriptionEndDateUtc.HasValue.ShouldBeTrue();
            result.Status.ShouldBe(SubscriptionPaymentStatus.Completed.ToString());
        }

        [Fact]
        public async Task Dado_TenantComAssinaturaAtiva_Quando_UpgradeSubscriptionAsync_Entao_DeveCriarPagamentoDeProracao()
        {
            // Dado
            var edition = new SubscribableEdition { Id = 1, DisplayName = "Pro", MonthlyPrice = 100 };
            var newEdition = new SubscribableEdition { Id = 2, DisplayName = "Enterprise", MonthlyPrice = 200 };
            _editionRepository.GetAsync(1).Returns(edition, newEdition);
            _editionRepository.GetAsync(2).Returns(newEdition);

            var currentPayment = new SubscriptionPayment
            {
                Id = 1,
                TenantId = 1,
                EditionId = 1,
                Amount = 100,
                Status = SubscriptionPaymentStatus.Completed,
                PaymentPeriodType = PaymentPeriodType.Monthly,
                SubscriptionStartDate = Clock.Now.AddDays(-15),
                SubscriptionEndDate = Clock.Now.AddDays(15),
            };
            _subscriptionPaymentRepository.GetAllListAsync(Arg.Any<System.Linq.Expressions.Expression<Func<SubscriptionPayment, bool>>>())
                .Returns(new List<SubscriptionPayment> { currentPayment });

            var tenant = new Tenant("acme", "ACME");
            _tenantRepository.GetAsync(1).Returns(tenant);

            var input = new UpgradeSubscriptionInput
            {
                TenantId = 1,
                NewEditionId = 2,
                PaymentPeriodType = PaymentPeriodType.Monthly,
                Gateway = "Null",
            };

            // Quando
            var result = await _sut.UpgradeSubscriptionAsync(input);

            // Então
            result.ShouldNotBeNull();
            await _subscriptionPaymentRepository.Received(1).InsertAsync(Arg.Is<SubscriptionPayment>(p => p.IsProrationPayment));
        }
    }
}

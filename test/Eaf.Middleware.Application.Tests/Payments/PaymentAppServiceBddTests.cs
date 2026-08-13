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
        private readonly IPaymentManager _paymentManager;

        public PaymentAppServiceBddTests()
        {
            _subscriptionPaymentRepository = Substitute.For<IRepository<SubscriptionPayment, long>>();
            _paymentManager = Substitute.For<IPaymentManager>();

            _sut = new PaymentAppService(
                _subscriptionPaymentRepository,
                _paymentManager);

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
        public async Task Dado_InputValido_Quando_CreatePaymentAsync_Entao_DeveDelegarParaPaymentManager()
        {
            // Dado
            var input = new CreateSubscriptionPaymentInput
            {
                EditionId = 1,
                EditionPaymentType = EditionPaymentType.Upgrade,
                PaymentPeriodType = PaymentPeriodType.Monthly,
                Gateway = "Null",
            };

            _paymentManager.CreatePaymentAsync(input).Returns(new PaymentRequestDto
            {
                PaymentId = "PAY-123",
                Gateway = "Null",
                IsSuccess = true,
            });

            // Quando
            var result = await _sut.CreatePaymentAsync(input);

            // Então
            result.PaymentId.ShouldBe("PAY-123");
            await _paymentManager.Received(1).CreatePaymentAsync(input);
        }

        [Fact]
        public async Task Dado_PagamentoProcessadoComSucesso_Quando_ProcessPaymentAsync_Entao_DeveDelegarParaPaymentManagerEAtivarAssinatura()
        {
            // Dado
            var input = new ProcessPaymentInput
            {
                ExternalPaymentId = "PAY-123",
                Gateway = "Null",
                IsSuccess = true,
            };

            _paymentManager.ProcessPaymentAsync(1, input).Returns(new SubscriptionPaymentDto
            {
                Id = 1,
                Status = SubscriptionPaymentStatus.Completed.ToString(),
            });

            // Quando
            var result = await _sut.ProcessPaymentAsync(1, input);

            // Então
            result.Status.ShouldBe(SubscriptionPaymentStatus.Completed.ToString());
            await _paymentManager.Received(1).ProcessPaymentAsync(1, input);
        }

        [Fact]
        public async Task Dado_PagamentoExistente_Quando_UpgradeSubscriptionAsync_Entao_DeveDelegarParaPaymentManager()
        {
            // Dado
            var input = new UpgradeSubscriptionInput
            {
                TenantId = 1,
                NewEditionId = 2,
                PaymentPeriodType = PaymentPeriodType.Monthly,
                Gateway = "Stripe",
            };

            _paymentManager.UpgradeSubscriptionAsync(input).Returns(new PaymentRequestDto
            {
                PaymentId = "UPG-123",
                Gateway = "Stripe",
                IsSuccess = true,
            });

            // Quando
            var result = await _sut.UpgradeSubscriptionAsync(input);

            // Então
            result.PaymentId.ShouldBe("UPG-123");
            await _paymentManager.Received(1).UpgradeSubscriptionAsync(input);
        }

        [Fact]
        public async Task Dado_AssinaturaRecorrenteAtiva_Quando_CancelRecurringAsync_Entao_DeveDelegarParaPaymentManager()
        {
            // Dado
            _paymentManager.CancelRecurringAsync(1).Returns(new SubscriptionPaymentDto
            {
                Id = 1,
                IsRecurring = false,
                Status = SubscriptionPaymentStatus.Canceled.ToString(),
            });

            // Quando
            var result = await _sut.CancelRecurringAsync(1);

            // Então
            result.IsRecurring.ShouldBeFalse();
            result.Status.ShouldBe(SubscriptionPaymentStatus.Canceled.ToString());
            await _paymentManager.Received(1).CancelRecurringAsync(1);
        }
    }
}

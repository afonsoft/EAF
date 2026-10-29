using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Payments;
using Eaf.Middleware.Payments.Dto;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Payments
{
    /// <summary>
    /// Testes BDD para PaymentAppService e guards dos gateways seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class PaymentAppServiceBddTests
    {
        private readonly IRepository<SubscriptionPayment, long> _repository;
        private readonly IPaymentManager _paymentManager;
        private readonly ISettingManager _settingManager;
        private readonly IPermissionChecker _permissionChecker;
        private readonly PaymentAppService _sut;

        public PaymentAppServiceBddTests()
        {
            _repository = Substitute.For<IRepository<SubscriptionPayment, long>>();
            _paymentManager = Substitute.For<IPaymentManager>();
            _settingManager = Substitute.For<ISettingManager>();
            _permissionChecker = Substitute.For<IPermissionChecker>();

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)1);
            abpSession.UserId.Returns((long?)7);

            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<SubscriptionPaymentDto>(Arg.Any<SubscriptionPayment>())
                .Returns(ci =>
                {
                    var p = (SubscriptionPayment)ci[0];
                    return new SubscriptionPaymentDto { Id = p.Id, TenantId = p.TenantId, Gateway = p.Gateway };
                });

            _sut = new PaymentAppService(_repository, _paymentManager)
            {
                AbpSession = abpSession,
                ObjectMapper = objectMapper,
                SettingManager = _settingManager,
                PermissionChecker = _permissionChecker,
            };
        }

        [Fact]
        public async Task Dado_PagamentoInexistente_Quando_Obter_Entao_DeveLancarAbpAuthorizationException()
        {
            // Dado
            _repository.GetAll().Returns(new List<SubscriptionPayment>().AsAsyncQueryable());

            // Quando / Então
            await Should.ThrowAsync<AbpAuthorizationException>(() => _sut.GetPaymentAsync(42));
        }

        [Fact]
        public async Task Dado_PagamentoDeOutroTenantSemPermissao_Quando_Obter_Entao_DeveLancarExcecao()
        {
            // Dado
            var pagamento = new SubscriptionPayment { Id = 3, TenantId = 9, Gateway = "Stripe" };
            _repository.GetAll().Returns(new List<SubscriptionPayment> { pagamento }.AsAsyncQueryable());
            _permissionChecker.IsGrantedAsync(Arg.Any<string>()).Returns(false);

            // Quando / Então
            await Should.ThrowAsync<AbpAuthorizationException>(() => _sut.GetPaymentAsync(3));
        }

        [Fact]
        public async Task Dado_PagamentoDoMesmoTenant_Quando_Obter_Entao_DeveRetornarDto()
        {
            // Dado
            var pagamento = new SubscriptionPayment { Id = 4, TenantId = 1, Gateway = "Stripe" };
            _repository.GetAll().Returns(new List<SubscriptionPayment> { pagamento }.AsAsyncQueryable());

            // Quando
            var resultado = await _sut.GetPaymentAsync(4);

            // Então
            resultado.Id.ShouldBe(4);
            resultado.Gateway.ShouldBe("Stripe");
        }

        [Fact]
        public async Task Dado_GatewaysParcialmenteConfigurados_Quando_Listar_Entao_DeveMarcarConfigurados()
        {
            // Dado
            _settingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.DefaultGateway)
                .Returns("Stripe");
            _settingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.Stripe.SecretKey)
                .Returns("sk_test_x");
            _settingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.PayPal.ClientId)
                .Returns((string)null);
            _settingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.MercadoPago.AccessToken)
                .Returns((string)null);
            _settingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.PagSeguro.Token)
                .Returns("token123");

            // Quando
            var gateways = await _sut.GetGatewayListAsync();

            // Então
            gateways.Count.ShouldBe(4);
            var stripe = gateways.Find(g => g.Name == "Stripe");
            stripe.IsConfigured.ShouldBeTrue();
            stripe.IsDefault.ShouldBeTrue();
            gateways.Find(g => g.Name == "PagSeguro").IsConfigured.ShouldBeTrue();
            gateways.Find(g => g.Name == "PayPal").IsConfigured.ShouldBeFalse();
            gateways.Find(g => g.Name == "MercadoPago").IsConfigured.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_SettingsConfigurados_Quando_ObterGatewaySettings_Entao_DeveRetornarTodos()
        {
            // Dado
            _settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>())
                .Returns(ci => "valor-" + (string)ci[0]);

            // Quando
            var settings = await _sut.GetGatewaySettingsAsync();

            // Então
            settings.Stripe.SecretKey.ShouldNotBeNullOrWhiteSpace();
            settings.PayPal.ClientId.ShouldNotBeNullOrWhiteSpace();
            settings.MercadoPago.AccessToken.ShouldNotBeNullOrWhiteSpace();
            settings.PagSeguro.Token.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Dado_SettingsNulos_Quando_Atualizar_Entao_DeveGravarSomenteDefault()
        {
            // Dado
            var input = new PaymentGatewaySettingsDto { DefaultGateway = "PayPal" };

            // Quando
            await _sut.UpdateGatewaySettingsAsync(input);

            // Então
            await _settingManager.Received(1).ChangeSettingForApplicationAsync(
                AppSettings.Payment.DefaultGateway, "PayPal");
            await _settingManager.DidNotReceive().ChangeSettingForApplicationAsync(
                AppSettings.Payment.Stripe.SecretKey, Arg.Any<string>());
        }

        [Fact]
        public async Task Dado_MetodosDePagamento_Quando_Delegar_Entao_DeveChamarPaymentManager()
        {
            // Quando
            await _sut.CreatePaymentAsync(new CreateSubscriptionPaymentInput());
            await _sut.ProcessPaymentAsync(5, new ProcessPaymentInput());
            await _sut.UpgradeSubscriptionAsync(new UpgradeSubscriptionInput());
            await _sut.CancelRecurringAsync(6);

            // Então
            await _paymentManager.Received(1).CreatePaymentAsync(Arg.Any<CreateSubscriptionPaymentInput>());
            await _paymentManager.Received(1).ProcessPaymentAsync(5, Arg.Any<ProcessPaymentInput>());
            await _paymentManager.Received(1).UpgradeSubscriptionAsync(Arg.Any<UpgradeSubscriptionInput>());
            await _paymentManager.Received(1).CancelRecurringAsync(6);
        }
    }
}

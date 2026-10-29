using Abp.Configuration;
using Abp.UI;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Payments.Dto;
using Eaf.Middleware.Payments.Gateways;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Payments
{
    /// <summary>
    /// Testes BDD dos guards de configuração dos gateways de pagamento.
    /// Os gateways chamam APIs externas; estes testes cobrem os caminhos
    /// determinísticos anteriores à chamada remota.
    /// </summary>
    public class PaymentGatewayGuardsBddTests
    {
        private readonly ISettingManager _settingManager;

        public PaymentGatewayGuardsBddTests()
        {
            _settingManager = Substitute.For<ISettingManager>();
            _settingManager.GetSettingValueForApplicationAsync(Arg.Any<string>())
                .Returns((string)null);
        }

        [Fact]
        public async Task Dado_StripeSemSecretKey_Quando_CriarPagamento_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var sut = new StripePaymentGateway(_settingManager);

            // Quando / Então
            var ex = await Should.ThrowAsync<UserFriendlyException>(
                () => sut.CreatePaymentAsync(new CreatePaymentRequestInput()));
            ex.Message.ShouldContain("Stripe");
        }

        [Fact]
        public async Task Dado_StripeSemSecretKey_Quando_Processar_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var sut = new StripePaymentGateway(_settingManager);

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(
                () => sut.ProcessPaymentAsync(new ProcessPaymentInput { ExternalPaymentId = "pi_x" }));
        }

        [Fact]
        public async Task Dado_StripeSemSecretKey_Quando_CancelarOuConsultar_Entao_DeveLancarExcecao()
        {
            // Dado
            var sut = new StripePaymentGateway(_settingManager);

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(() => sut.CancelSubscriptionAsync("sub_1"));
            await Should.ThrowAsync<UserFriendlyException>(() => sut.GetSubscriptionStatusAsync("sub_1"));
        }

        [Fact]
        public async Task Dado_StripeComSecretKey_Quando_ProcessarIdDesconhecido_Entao_DeveRetornarFalha()
        {
            // Dado
            _settingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.Stripe.SecretKey)
                .Returns("sk_test_dummy");
            var sut = new StripePaymentGateway(_settingManager);

            // Quando
            var resultado = await sut.ProcessPaymentAsync(new ProcessPaymentInput { ExternalPaymentId = "desconhecido_123" });

            // Então
            resultado.IsSuccess.ShouldBeFalse();
            resultado.Gateway.ShouldBe("Stripe");
            resultado.ExternalPaymentId.ShouldBe("desconhecido_123");
        }

        [Fact]
        public async Task Dado_StripeWebhookSemSecret_Quando_EventoDesconhecido_Entao_DeveRetornarFalha()
        {
            // Dado
            var sut = new StripePaymentGateway(_settingManager);
            var json = "{\"id\":\"evt_1\",\"object\":\"event\",\"api_version\":\"2026-06-24.dahlia\",\"type\":\"customer.created\",\"data\":{\"object\":{}}}";

            // Quando
            var resultado = await sut.ProcessWebhookAsync("customer.created", json, null);

            // Então
            resultado.IsSuccess.ShouldBeFalse();
            resultado.Gateway.ShouldBe("Stripe");
        }

        [Fact]
        public async Task Dado_PayPalSemCredenciais_Quando_CriarOuProcessar_Entao_DeveLancarExcecao()
        {
            // Dado
            var sut = new PayPalPaymentGateway(_settingManager);

            // Quando / Então
            var ex = await Should.ThrowAsync<UserFriendlyException>(
                () => sut.CreatePaymentAsync(new CreatePaymentRequestInput()));
            ex.Message.ShouldContain("PayPal");
            await Should.ThrowAsync<UserFriendlyException>(
                () => sut.ProcessPaymentAsync(new ProcessPaymentInput { ExternalPaymentId = "ORDER-1" }));
        }

        [Fact]
        public async Task Dado_MercadoPagoSemToken_Quando_CriarOuProcessar_Entao_DeveLancarExcecao()
        {
            // Dado
            var sut = new MercadoPagoPaymentGateway(_settingManager);

            // Quando / Então
            var ex = await Should.ThrowAsync<UserFriendlyException>(
                () => sut.CreatePaymentAsync(new CreatePaymentRequestInput()));
            ex.Message.ShouldContain("MercadoPago");
            await Should.ThrowAsync<UserFriendlyException>(
                () => sut.ProcessPaymentAsync(new ProcessPaymentInput { ExternalPaymentId = "123" }));
        }

        [Fact]
        public async Task Dado_MercadoPagoComToken_Quando_IdInvalido_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            _settingManager.GetSettingValueForApplicationAsync(AppSettings.Payment.MercadoPago.AccessToken)
                .Returns("APP_USR-dummy");
            var sut = new MercadoPagoPaymentGateway(_settingManager);

            // Quando / Então
            var ex = await Should.ThrowAsync<UserFriendlyException>(
                () => sut.ProcessPaymentAsync(new ProcessPaymentInput { ExternalPaymentId = "abc" }));
            ex.Message.ShouldContain("Invalid MercadoPago payment id");
        }
    }
}

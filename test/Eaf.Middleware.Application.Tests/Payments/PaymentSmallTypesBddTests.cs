using Eaf.Middleware.Core.Editions;
using Eaf.Middleware.Payments;
using Eaf.Middleware.Payments.Dto;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Payments
{
    /// <summary>
    /// Testes BDD para NullPaymentGateway, PaymentGatewayResolver, PaymentPeriodHelper e DTOs de pagamento.
    /// </summary>
    public class PaymentSmallTypesBddTests
    {
        private sealed class StripeGatewayFake : IPaymentGateway
        {
            public Task<PaymentRequestDto> CreatePaymentAsync(CreatePaymentRequestInput input)
                => Task.FromResult(new PaymentRequestDto { IsSuccess = true, Gateway = "Stripe" });

            public Task<PaymentResultDto> ProcessPaymentAsync(ProcessPaymentInput input)
                => Task.FromResult(new PaymentResultDto { IsSuccess = true });
        }

        [Fact]
        public async Task Dado_NullPaymentGateway_Quando_CriarPagamento_Entao_DeveRetornarSucessoComGatewayNull()
        {
            // Dado
            var sut = new NullPaymentGateway();

            // Quando
            var resultado = await sut.CreatePaymentAsync(new CreatePaymentRequestInput());

            // Então
            resultado.IsSuccess.ShouldBeTrue();
            resultado.Gateway.ShouldBe("Null");
            resultado.PaymentId.ShouldNotBeNullOrEmpty();
            resultado.CheckoutUrl.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_NullPaymentGateway_Quando_ProcessarPagamento_Entao_DeveEspelharIdsDoInput()
        {
            // Dado
            var sut = new NullPaymentGateway();
            var input = new ProcessPaymentInput { ExternalPaymentId = "EXT-1", Gateway = "Null" };

            // Quando
            var resultado = await sut.ProcessPaymentAsync(input);

            // Então
            resultado.IsSuccess.ShouldBeTrue();
            resultado.ExternalPaymentId.ShouldBe("EXT-1");
            resultado.Gateway.ShouldBe("Null");
        }

        [Fact]
        public void Dado_ResolverComGatewayCompativel_Quando_Resolver_Entao_DeveRetornarGateway()
        {
            // Dado
            var stripe = new StripeGatewayFake();
            var sut = new PaymentGatewayResolver(new IPaymentGateway[] { stripe }, new NullPaymentGateway());

            // Quando
            var resolvido = sut.Resolve("stripe");

            // Então
            resolvido.ShouldBeSameAs(stripe);
        }

        [Fact]
        public void Dado_ResolverSemGatewayCompativel_Quando_Resolver_Entao_DeveRetornarNullGateway()
        {
            // Dado
            var nullGateway = new NullPaymentGateway();
            var sut = new PaymentGatewayResolver(new IPaymentGateway[] { new StripeGatewayFake() }, nullGateway);

            // Quando
            var resolvido = sut.Resolve("paypal");

            // Então
            resolvido.ShouldBeSameAs(nullGateway);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Dado_NomeVazio_Quando_Resolver_Entao_DeveRetornarNullGateway(string nome)
        {
            // Dado
            var nullGateway = new NullPaymentGateway();
            var sut = new PaymentGatewayResolver(new IPaymentGateway[] { new StripeGatewayFake() }, nullGateway);

            // Quando
            var resolvido = sut.Resolve(nome);

            // Então
            resolvido.ShouldBeSameAs(nullGateway);
        }

        [Theory]
        [InlineData(PaymentPeriodType.Daily, 1)]
        [InlineData(PaymentPeriodType.Weekly, 7)]
        [InlineData(PaymentPeriodType.Biannual, 181)]
        [InlineData(PaymentPeriodType.Annual, 365)]
        public void Dado_PeriodoComDataFim_Quando_ObterDiasNoPeriodo_Entao_DeveRetornarDias(PaymentPeriodType periodo, int minimo)
        {
            // Dado
            var referencia = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Quando
            var dias = PaymentPeriodHelper.GetDaysInPeriod(periodo, referencia);
            var fim = PaymentPeriodHelper.GetEndDate(referencia, periodo);

            // Então
            dias.ShouldBeGreaterThanOrEqualTo(minimo);
            fim.ShouldNotBeNull();
            fim.Value.ShouldBeGreaterThan(referencia);
        }

        [Fact]
        public void Dado_PeriodoMensal_Quando_ObterDataFim_Entao_DeveAdicionarUmMes()
        {
            // Dado
            var referencia = new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc);

            // Quando / Então
            PaymentPeriodHelper.GetEndDate(referencia, PaymentPeriodType.Monthly).ShouldBe(new DateTime(2026, 4, 15));
            PaymentPeriodHelper.GetEndDate(referencia, PaymentPeriodType.Quarterly).ShouldBe(new DateTime(2026, 6, 15));
        }

        [Fact]
        public void Dado_PeriodoPermanente_Quando_ObterDataFimEDias_Entao_DeveRetornarNuloE365()
        {
            // Quando / Então
            PaymentPeriodHelper.GetEndDate(DateTime.UtcNow, PaymentPeriodType.Permanent).ShouldBeNull();
            PaymentPeriodHelper.GetDaysInPeriod(PaymentPeriodType.Permanent).ShouldBe(365);
        }

        [Fact]
        public void Dado_PeriodoInvalido_Quando_ObterDataFim_Entao_DeveRetornarPropriaData()
        {
            // Dado
            var referencia = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);

            // Quando / Então
            PaymentPeriodHelper.GetEndDate(referencia, (PaymentPeriodType)999).ShouldBe(referencia);
        }

        [Fact]
        public void Dado_SubscriptionStatusResult_Quando_DefinirPropriedades_Entao_DeveManterValores()
        {
            // Dado
            var fim = new DateTime(2026, 12, 31);

            // Quando
            var resultado = new SubscriptionStatusResult
            {
                IsActive = true,
                CurrentPeriodEnd = fim,
                Status = "active",
            };

            // Então
            resultado.IsActive.ShouldBeTrue();
            resultado.CurrentPeriodEnd.ShouldBe(fim);
            resultado.Status.ShouldBe("active");
        }
    }
}

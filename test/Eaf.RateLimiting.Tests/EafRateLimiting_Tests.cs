using System;
using System.Threading.Tasks;
using Eaf.RateLimiting;
using Shouldly;
using Xunit;

namespace Eaf.RateLimiting.Tests
{
    public class EafRateLimiting_Tests
    {
        // Dado uma regra válida, quando o período é convertido, então o TimeSpan é correto.
        [Theory]
        [InlineData("30s", 30)]
        [InlineData("1m", 60)]
        [InlineData("2h", 7200)]
        [InlineData("1d", 86400)]
        public void Dado_PeriodoValido_Quando_GetWindow_Entao_RetornaTimeSpanCorreto(string period, double seconds)
        {
            new EafRateLimitRule { Period = period }.GetWindow().ShouldBe(TimeSpan.FromSeconds(seconds));
        }

        // Dado um período inválido, quando convertido, então lança UserFriendlyException.
        [Fact]
        public void Dado_PeriodoInvalido_Quando_GetWindow_Entao_LancaUserFriendlyException()
        {
            Should.Throw<Abp.UI.UserFriendlyException>(() => new EafRateLimitRule { Period = "5x" }.GetWindow());
        }

        // Dado um store em memória, quando hits dentro do limite, então todas são permitidas.
        [Fact]
        public async Task Dado_StoreEmMemoria_Quando_HitsDentroDoLimite_Entao_Permite()
        {
            var store = new InMemoryRateLimitingStore();
            var window = TimeSpan.FromMinutes(1);

            var r1 = await store.HitAsync("ip:1", window, 2);
            var r2 = await store.HitAsync("ip:1", window, 2);

            r1.Allowed.ShouldBeTrue();
            r2.Allowed.ShouldBeTrue();
            r2.Count.ShouldBe(2);
        }

        // Dado um store em memória, quando o limite é excedido, então a requisição é negada.
        [Fact]
        public async Task Dado_StoreEmMemoria_Quando_LimiteExcedido_Entao_Nega()
        {
            var store = new InMemoryRateLimitingStore();
            var window = TimeSpan.FromMinutes(1);

            await store.HitAsync("ip:1", window, 1);
            var result = await store.HitAsync("ip:1", window, 1);

            result.Allowed.ShouldBeFalse();
            result.ResetsAt.ShouldBeGreaterThan(DateTimeOffset.UtcNow);
        }

        // Dado chaves diferentes, quando hits são feitos, então os contadores são independentes.
        [Fact]
        public async Task Dado_ChavesDistintas_Quando_Hits_Entao_ContadoresIndependentes()
        {
            var store = new InMemoryRateLimitingStore();
            var window = TimeSpan.FromMinutes(1);

            await store.HitAsync("ip:1", window, 1);
            var other = await store.HitAsync("ip:2", window, 1);

            other.Allowed.ShouldBeTrue();
        }

        // Dado um store em memória, quando a janela expira, então o contador reinicia.
        [Fact]
        public async Task Dado_JanelaExpirada_Quando_NovoHit_Entao_ContadorReinicia()
        {
            var store = new InMemoryRateLimitingStore();
            var tiny = TimeSpan.FromMilliseconds(1);

            await store.HitAsync("ip:1", tiny, 1);
            await Task.Delay(50);
            var result = await store.HitAsync("ip:1", TimeSpan.FromMinutes(1), 1);

            result.Allowed.ShouldBeTrue();
            result.Count.ShouldBe(1);
        }

        // Dado as opções padrão, quando inspecionadas, então os defaults estão corretos.
        [Fact]
        public void Dado_OpcoesPadrao_Quando_Inspecionadas_Entao_DefaultsCorretos()
        {
            var options = new EafRateLimitingOptions();
            options.LimitByUser.ShouldBeTrue();
            options.Rules.ShouldBeEmpty();
            options.Value.ShouldBeSameAs(options);
        }
    }
}

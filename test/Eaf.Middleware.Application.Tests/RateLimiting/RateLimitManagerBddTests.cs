using Abp.Runtime.Caching;
using Eaf.Middleware.Application.RateLimiting;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.RateLimiting
{
    /// <summary>
    /// Testes BDD para RateLimitManager seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class RateLimitManagerBddTests
    {
        private readonly ConcurrentDictionary<string, object> _store = new();
        private readonly ICacheManager _cacheManager;

        public RateLimitManagerBddTests()
        {
            var cache = Substitute.For<ICache>();

            cache.GetOrDefaultAsync(Arg.Any<string>())
                .Returns(ci =>
                {
                    _store.TryGetValue(ci.Arg<string>(), out var value);
                    return Task.FromResult(value);
                });

            cache.SetAsync(
                    Arg.Any<string>(),
                    Arg.Any<object>(),
                    Arg.Any<TimeSpan?>(),
                    Arg.Any<DateTimeOffset?>())
                .Returns(ci =>
                {
                    _store[ci.Arg<string>()] = ci.Arg<object>();
                    return Task.CompletedTask;
                });

            _cacheManager = Substitute.For<ICacheManager>();
            _cacheManager.GetCache(Arg.Any<string>()).Returns(cache);
        }

        private RateLimitManager CriarGerenciador() => new RateLimitManager(_cacheManager);

        [Fact]
        public async Task Dado_LimiteZeroOuNegativo_Quando_Verificar_Entao_DevePermitirSemConsultarCache()
        {
            // Dado
            var sut = CriarGerenciador();

            // Quando
            var decisaoZero = await sut.CheckAsync("policy", "subject", TimeSpan.FromMinutes(1), 0);
            var decisaoNegativa = await sut.CheckAsync("policy", "subject", TimeSpan.FromMinutes(1), -5);

            // Então
            decisaoZero.Allowed.ShouldBeTrue();
            decisaoNegativa.Allowed.ShouldBeTrue();
            _store.ShouldBeEmpty();
        }

        [Fact]
        public async Task Dado_PrimeiraChamadaNaJanela_Quando_Verificar_Entao_DevePermitirEIncrementarContador()
        {
            // Dado
            var sut = CriarGerenciador();
            var janela = TimeSpan.FromMinutes(1);

            // Quando
            var decisao = await sut.CheckAsync("policy", "subject", janela, 3);

            // Então
            decisao.Allowed.ShouldBeTrue();
            decisao.ResetAt.ShouldBeGreaterThan(DateTime.MinValue);
            _store.Values.ShouldContain(1L);
        }

        [Fact]
        public async Task Dado_ContadorAbaixoDoLimite_Quando_VerificarVariasVezes_Entao_DevePermitirAteLimite()
        {
            // Dado
            var sut = CriarGerenciador();
            var janela = TimeSpan.FromMinutes(1);

            // Quando
            var primeira = await sut.CheckAsync("policy", "subject", janela, 2);
            var segunda = await sut.CheckAsync("policy", "subject", janela, 2);

            // Então
            primeira.Allowed.ShouldBeTrue();
            segunda.Allowed.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_ContadorAcimaDoLimite_Quando_Verificar_Entao_DeveNegarComRetryAfter()
        {
            // Dado
            var sut = CriarGerenciador();
            var janela = TimeSpan.FromMinutes(5);
            await sut.CheckAsync("policy", "subject", janela, 1);

            // Quando
            var decisao = await sut.CheckAsync("policy", "subject", janela, 1);

            // Então
            decisao.Allowed.ShouldBeFalse();
            decisao.RetryAfterSeconds.ShouldBeGreaterThanOrEqualTo(0);
            decisao.ResetAt.ShouldBeGreaterThan(DateTime.MinValue);
        }

        [Fact]
        public async Task Dado_JanelasDiferentes_Quando_Verificar_Entao_DeveUsarContadoresSeparados()
        {
            // Dado
            var sut = CriarGerenciador();

            // Quando
            await sut.CheckAsync("policyA", "subject", TimeSpan.FromMinutes(1), 1);
            await sut.CheckAsync("policyB", "subject", TimeSpan.FromMinutes(1), 1);
            var segundaA = await sut.CheckAsync("policyA", "subject", TimeSpan.FromMinutes(1), 1);
            var segundaB = await sut.CheckAsync("policyB", "subject", TimeSpan.FromMinutes(1), 1);

            // Então
            segundaA.Allowed.ShouldBeFalse();
            segundaB.Allowed.ShouldBeFalse();
            _store.Count.ShouldBe(2);
        }

        [Fact]
        public async Task Dado_CancelamentoSolicitado_Quando_Verificar_Entao_DeveLancarOperationCanceled()
        {
            // Dado
            var sut = CriarGerenciador();
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Quando / Então
            await Should.ThrowAsync<OperationCanceledException>(() =>
                sut.CheckAsync("policy-cancel", "subject", TimeSpan.FromMinutes(1), 1, cts.Token));
        }
    }
}

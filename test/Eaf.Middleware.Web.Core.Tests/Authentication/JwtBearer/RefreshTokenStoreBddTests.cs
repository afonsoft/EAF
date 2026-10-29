using Abp.Runtime.Caching;
using Eaf.Middleware.Web.Authentication.JwtBearer;
using NSubstitute;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Authentication.JwtBearer
{
    /// <summary>
    /// Testes BDD para RefreshTokenStore seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class RefreshTokenStoreBddTests
    {
        private readonly ICache _cache;
        private readonly RefreshTokenStore _sut;

        public RefreshTokenStoreBddTests()
        {
            _cache = Substitute.For<ICache>();
            var cacheManager = Substitute.For<ICacheManager>();
            cacheManager.GetCache("EafRefreshTokens").Returns(_cache);

            _sut = new RefreshTokenStore(cacheManager);
        }

        [Fact]
        public async Task Dado_TokenValido_Quando_Obter_Entao_DeveConsultarCache()
        {
            // Dado
            var info = new RefreshTokenInfo { Token = "abc", UserId = 1 };
            _cache.GetOrDefaultAsync("abc").Returns(info);

            // Quando
            var resultado = await _sut.GetAsync("abc");

            // Então
            resultado.ShouldBeSameAs(info);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Dado_TokenVazio_Quando_Obter_Entao_DeveRetornarNuloSemConsultarCache(string token)
        {
            // Quando
            var resultado = await _sut.GetAsync(token);

            // Então
            resultado.ShouldBeNull();
            await _cache.DidNotReceive().GetOrDefaultAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task Dado_RefreshTokenValido_Quando_Salvar_Entao_DevePersistirComExpiracaoAbsoluta()
        {
            // Dado
            var info = new RefreshTokenInfo { Token = "xyz", ExpireDate = DateTime.UtcNow.AddDays(7) };

            // Quando
            await _sut.SetAsync(info);

            // Então
            await _cache.Received(1).SetAsync(
                "xyz",
                info,
                Arg.Is<TimeSpan?>(s => s == null),
                Arg.Is<DateTimeOffset?>(d => d.HasValue));
        }

        [Fact]
        public async Task Dado_RefreshTokenNuloOuSemToken_Quando_Salvar_Entao_NaoDevePersistir()
        {
            // Quando
            await _sut.SetAsync(null);
            await _sut.SetAsync(new RefreshTokenInfo { Token = null });

            // Então
            await _cache.DidNotReceive().SetAsync(
                Arg.Any<string>(), Arg.Any<object>(), Arg.Any<TimeSpan?>(),
                Arg.Any<DateTimeOffset?>());
        }

        [Fact]
        public async Task Dado_TokenValido_Quando_Remover_Entao_DeveRemoverDoCache()
        {
            // Quando
            await _sut.RemoveAsync("abc");

            // Então
            await _cache.Received(1).RemoveAsync("abc");
        }

        [Fact]
        public async Task Dado_TokenVazio_Quando_Remover_Entao_NaoDeveChamarCache()
        {
            // Quando
            await _sut.RemoveAsync(null);
            await _sut.RemoveAsync(string.Empty);

            // Então
            await _cache.DidNotReceive().RemoveAsync(Arg.Any<string>());
        }
    }
}

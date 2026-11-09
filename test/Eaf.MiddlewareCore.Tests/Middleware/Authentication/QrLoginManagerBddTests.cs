using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Memory;
using Eaf.Middleware.Authentication.QrLogin;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Middleware.Authentication
{
    /// <summary>
    /// Testes do QrLoginManager — ciclo de vida pending → approved → consumed,
    /// rejeição de entradas inválidas e uso único do token.
    /// </summary>
    public class QrLoginManagerBddTests
    {
        private static QrLoginManager CreateManager()
        {
            var cacheManager = Substitute.For<ICacheManager>();
            cacheManager.GetCache(QrLoginManager.CacheName)
                .Returns(new AbpMemoryCache(QrLoginManager.CacheName));

            return new QrLoginManager(cacheManager);
        }

        [Fact]
        public async Task Dado_TokenCriado_Quando_Consultado_Entao_Pendente()
        {
            var manager = CreateManager();

            var token = await manager.CreateAsync();
            var fetched = await manager.GetOrNullAsync(token.TokenId);

            fetched.ShouldNotBeNull();
            fetched.Status.ShouldBe(QrLoginStatus.Pending);
            token.TokenId.Length.ShouldBe(32);
        }

        [Fact]
        public async Task Dado_TokenPendente_Quando_Aprovado_Entao_GuardaUsuario()
        {
            var manager = CreateManager();
            var token = await manager.CreateAsync();

            var ok = await manager.ApproveAsync(token.TokenId, userId: 42, tenantId: 7);
            var fetched = await manager.GetOrNullAsync(token.TokenId);

            ok.ShouldBeTrue();
            fetched.Status.ShouldBe(QrLoginStatus.Approved);
            fetched.ApprovedUserId.ShouldBe(42);
            fetched.ApprovedTenantId.ShouldBe(7);
        }

        [Fact]
        public async Task Dado_TokenAprovado_Quando_Consumido_Entao_UsoUnico()
        {
            var manager = CreateManager();
            var token = await manager.CreateAsync();
            await manager.ApproveAsync(token.TokenId, 42, 7);

            var first = await manager.ConsumeAsync(token.TokenId);
            var second = await manager.ConsumeAsync(token.TokenId);

            first.ShouldNotBeNull();
            first.Status.ShouldBe(QrLoginStatus.Consumed);
            second.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_TokenPendente_Quando_Consumido_Entao_Nulo()
        {
            var manager = CreateManager();
            var token = await manager.CreateAsync();

            var consumed = await manager.ConsumeAsync(token.TokenId);

            consumed.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_TokenAprovado_Quando_AprovadoNovamente_Entao_Falso()
        {
            var manager = CreateManager();
            var token = await manager.CreateAsync();
            await manager.ApproveAsync(token.TokenId, 42, 7);

            var again = await manager.ApproveAsync(token.TokenId, 99, 1);

            again.ShouldBeFalse();
            var fetched = await manager.GetOrNullAsync(token.TokenId);
            fetched.ApprovedUserId.ShouldBe(42);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Dado_TokenIdInvalido_Quando_Consultado_Entao_Nulo(string tokenId)
        {
            var manager = CreateManager();

            var fetched = await manager.GetOrNullAsync(tokenId);

            fetched.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_TokenIdMuitoLongo_Quando_Consultado_Entao_Nulo()
        {
            var manager = CreateManager();

            var fetched = await manager.GetOrNullAsync(new string('a', 65));

            fetched.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_TokenInexistente_Quando_Aprovado_Entao_Falso()
        {
            var manager = CreateManager();

            var ok = await manager.ApproveAsync("nao-existe", 1, 1);

            ok.ShouldBeFalse();
        }
    }
}

using Abp;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Memory;
using Eaf.Middleware.Authentication.Sessions;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Middleware.Authentication
{
    /// <summary>
    /// Testes do SessionRegistry — registro, listagem, revogação e denylist.
    /// </summary>
    public class SessionRegistryTests
    {
        private static SessionRegistry CreateRegistry()
        {
            var cacheManager = Substitute.For<ICacheManager>();
            var sessions = new AbpMemoryCache(SessionRegistry.SessionsCacheName);
            var index = new AbpMemoryCache(SessionRegistry.IndexCacheName);
            var revoked = new AbpMemoryCache(SessionRegistry.RevokedCacheName);

            cacheManager.GetCache(SessionRegistry.SessionsCacheName).Returns(sessions);
            cacheManager.GetCache(SessionRegistry.IndexCacheName).Returns(index);
            cacheManager.GetCache(SessionRegistry.RevokedCacheName).Returns(revoked);

            return new SessionRegistry(cacheManager);
        }

        private static UserSessionInfo Sample(string sessionId, long userId = 10, int? tenantId = 1)
            => new UserSessionInfo
            {
                SessionId = sessionId,
                UserId = userId,
                TenantId = tenantId,
                RefreshToken = "rt-" + sessionId,
                IpAddress = "10.0.0.1",
                UserAgent = "agent",
                CreationTime = DateTime.UtcNow,
                LastActivityTime = DateTime.UtcNow,
                ExpireDate = DateTime.UtcNow.AddDays(7)
            };

        [Fact]
        public async Task SetAsync_RegistraEIndexaPorUsuario()
        {
            // Dado/Quando: registra duas sessões do mesmo usuário
            var registry = CreateRegistry();
            await registry.SetAsync(Sample("s1"));
            await registry.SetAsync(Sample("s2"));

            // Então: index retorna ambas, mais recente atividade primeiro
            var sessions = await registry.GetForUserAsync(new UserIdentifier(1, 10));
            sessions.Count.ShouldBe(2);
        }

        [Fact]
        public async Task GetForUserAsync_NaoRetornaSessaoRevogada()
        {
            var registry = CreateRegistry();
            await registry.SetAsync(Sample("s1"));
            await registry.SetAsync(Sample("s2"));

            await registry.RevokeAsync("s1");

            var sessions = await registry.GetForUserAsync(new UserIdentifier(1, 10));
            sessions.Count.ShouldBe(1);
            sessions[0].SessionId.ShouldBe("s2");
        }

        [Fact]
        public async Task GetForUserAsync_RemoveExpiradasDoIndice()
        {
            var registry = CreateRegistry();
            var expired = Sample("old");
            expired.ExpireDate = DateTime.UtcNow.AddMinutes(-1);
            await registry.SetAsync(expired);
            await registry.SetAsync(Sample("fresh"));

            var sessions = await registry.GetForUserAsync(new UserIdentifier(1, 10));
            sessions.Count.ShouldBe(1);
            sessions[0].SessionId.ShouldBe("fresh");
        }

        [Fact]
        public async Task RevokeAllForUserAsync_MantemSessaoCorrente()
        {
            var registry = CreateRegistry();
            await registry.SetAsync(Sample("s1"));
            await registry.SetAsync(Sample("s2"));
            await registry.SetAsync(Sample("s3"));

            await registry.RevokeAllForUserAsync(new UserIdentifier(1, 10), exceptSessionId: "s2");

            (await registry.IsRevokedAsync("s1")).ShouldBeTrue();
            (await registry.IsRevokedAsync("s2")).ShouldBeFalse();
            (await registry.IsRevokedAsync("s3")).ShouldBeTrue();

            var sessions = await registry.GetForUserAsync(new UserIdentifier(1, 10));
            sessions.Count.ShouldBe(1);
        }

        [Fact]
        public async Task RevokeAllForUserAsync_SemExcecaoRevogaTudo()
        {
            var registry = CreateRegistry();
            await registry.SetAsync(Sample("s1"));
            await registry.SetAsync(Sample("s2"));

            await registry.RevokeAllForUserAsync(new UserIdentifier(1, 10));

            (await registry.GetForUserAsync(new UserIdentifier(1, 10))).ShouldBeEmpty();
        }

        [Fact]
        public async Task TouchAsync_AtualizaLastActivityTime()
        {
            var registry = CreateRegistry();
            var s = Sample("s1");
            s.LastActivityTime = DateTime.UtcNow.AddMinutes(-5);
            await registry.SetAsync(s);

            await registry.TouchAsync("s1");

            var updated = await registry.GetAsync("s1");
            updated.LastActivityTime.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-1));
        }

        [Fact]
        public async Task TouchAsync_SessaoInexistente_NaoFalha()
        {
            var registry = CreateRegistry();
            await Should.NotThrowAsync(() => registry.TouchAsync("nao-existe"));
        }

        [Fact]
        public async Task IsRevokedAsync_NuncaRegistrada_Falso()
        {
            var registry = CreateRegistry();
            (await registry.IsRevokedAsync("desconhecida")).ShouldBeFalse();
        }

        [Fact]
        public async Task RevokeAsync_RemoveSessaoDoCache()
        {
            var registry = CreateRegistry();
            await registry.SetAsync(Sample("s1"));

            await registry.RevokeAsync("s1");

            (await registry.GetAsync("s1")).ShouldBeNull();
            (await registry.IsRevokedAsync("s1")).ShouldBeTrue();
        }

        [Fact]
        public async Task SessoesDeUsuariosDiferentes_SaoIsoladas()
        {
            var registry = CreateRegistry();
            await registry.SetAsync(Sample("s-a", userId: 10));
            await registry.SetAsync(Sample("s-b", userId: 20));

            var a = await registry.GetForUserAsync(new UserIdentifier(1, 10));
            var b = await registry.GetForUserAsync(new UserIdentifier(1, 20));

            a.ShouldHaveSingleItem().SessionId.ShouldBe("s-a");
            b.ShouldHaveSingleItem().SessionId.ShouldBe("s-b");
        }
    }
}

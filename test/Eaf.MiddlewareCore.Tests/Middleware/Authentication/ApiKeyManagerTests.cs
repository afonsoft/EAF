using Abp;
using Abp.Domain.Repositories;
using Abp.UI;
using Eaf.Middleware.Authentication.ApiKeys;
using NSubstitute;
using Shouldly;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Middleware.Authentication
{
    /// <summary>
    /// Testes do ApiKeyManager — geração, hash-only storage, validação e escopo de IP.
    /// </summary>
    public class ApiKeyManagerTests
    {
        private static ApiKeyManager CreateManager(out IRepository<ApiKey, long> repo)
        {
            repo = Substitute.For<IRepository<ApiKey, long>>();
            return new ApiKeyManager(repo);
        }

        // --- Geração de material ---

        [Fact]
        public void GeneratePlainKey_TemPrefixoEFormato()
        {
            // Dado/Quando: gera uma chave em claro
            var key = ApiKeyManager.GeneratePlainKey();

            // Então: prefixo eak_ e material base64url de 32 bytes (~43 chars)
            key.ShouldStartWith(ApiKeyConsts.KeyPrefix);
            key.Length.ShouldBeGreaterThan(40);
        }

        [Fact]
        public void GeneratePlainKey_ChavesSaoUnicas()
        {
            var a = ApiKeyManager.GeneratePlainKey();
            var b = ApiKeyManager.GeneratePlainKey();
            a.ShouldNotBe(b);
        }

        [Fact]
        public void ComputeHash_EstabeleEhHexDe64Chars()
        {
            var h1 = ApiKeyManager.ComputeHash("eak_test");
            var h2 = ApiKeyManager.ComputeHash("eak_test");
            h1.ShouldBe(h2);
            h1.Length.ShouldBe(64);
            h1.ShouldMatch("^[0-9a-f]+$");
        }

        // --- Criação ---

        [Fact]
        public async Task CreateAsync_PersisteSomenteHashENuncaOPlain()
        {
            var mgr = CreateManager(out var repo);
            var user = new UserIdentifier(1, 42);

            var (entity, plain) = await mgr.CreateAsync(user, "ci-pipeline");

            entity.KeyHash.ShouldBe(ApiKeyManager.ComputeHash(plain));
            entity.KeyHash.ShouldNotBe(plain);
            entity.KeyPrefix.ShouldBe(plain[..ApiKeyConsts.DisplayPrefixLength]);
            entity.Status.ShouldBe(ApiKeyStatus.Active);
            entity.TenantId.ShouldBe(1);
            entity.UserId.ShouldBe(42);
            await repo.Received(1).InsertAsync(Arg.Any<ApiKey>());
        }

        [Fact]
        public async Task CreateAsync_SemNome_LancaUserFriendly()
        {
            var mgr = CreateManager(out _);
            await Should.ThrowAsync<UserFriendlyException>(
                () => mgr.CreateAsync(new UserIdentifier(null, 1), "  "));
        }

        [Fact]
        public async Task CreateAsync_IpRangeInvalido_LancaUserFriendly()
        {
            var mgr = CreateManager(out _);
            await Should.ThrowAsync<UserFriendlyException>(
                () => mgr.CreateAsync(new UserIdentifier(null, 1), "k", allowedIpRanges: new[] { "not-an-ip" }));
        }

        [Fact]
        public async Task CreateAsync_ComEscopoESerializacao()
        {
            var mgr = CreateManager(out _);
            var (entity, _) = await mgr.CreateAsync(
                new UserIdentifier(2, 7), "scoped",
                permissionScopes: new[] { "Pages.Administration.Users", "Pages.Tenants" },
                allowedIpRanges: new[] { "10.0.0.0/8", "192.168.1.10" });

            entity.GetPermissionScopes().ShouldBe(new[] { "Pages.Administration.Users", "Pages.Tenants" }, ignoreOrder: true);
            entity.AllowedIpRanges.ShouldBe("10.0.0.0/8,192.168.1.10");
        }

        // --- Validação ---

        [Fact]
        public async Task ValidateAsync_ChaveValida_RetornaSucessoEMarcaLastUsed()
        {
            var mgr = CreateManager(out var repo);
            var plain = ApiKeyManager.GeneratePlainKey();
            var stored = new ApiKey { KeyHash = ApiKeyManager.ComputeHash(plain), Status = ApiKeyStatus.Active };
            repo.FirstOrDefaultAsync(Arg.Any<Expression<Func<ApiKey, bool>>>())
                .Returns(stored);

            var result = await mgr.ValidateAsync(plain, IPAddress.Parse("8.8.8.8"));

            result.Succeeded.ShouldBeTrue();
            result.ApiKey.ShouldBe(stored);
            stored.LastUsedTime.ShouldNotBeNull();
            await repo.Received(1).UpdateAsync(stored);
        }

        [Fact]
        public async Task ValidateAsync_PrefixoErrado_Falha()
        {
            var mgr = CreateManager(out _);
            var result = await mgr.ValidateAsync("bearer abc", null);
            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldBe(ApiKeyValidationError.KeyNotFound);
        }

        [Fact]
        public async Task ValidateAsync_Revogada_Falha()
        {
            var mgr = CreateManager(out var repo);
            var plain = ApiKeyManager.GeneratePlainKey();
            repo.FirstOrDefaultAsync(Arg.Any<Expression<Func<ApiKey, bool>>>())
                .Returns(new ApiKey { KeyHash = ApiKeyManager.ComputeHash(plain), Status = ApiKeyStatus.Revoked });

            var result = await mgr.ValidateAsync(plain, null);
            result.Error.ShouldBe(ApiKeyValidationError.Revoked);
        }

        [Fact]
        public async Task ValidateAsync_Expirada_Falha()
        {
            var mgr = CreateManager(out var repo);
            var plain = ApiKeyManager.GeneratePlainKey();
            repo.FirstOrDefaultAsync(Arg.Any<Expression<Func<ApiKey, bool>>>())
                .Returns(new ApiKey
                {
                    KeyHash = ApiKeyManager.ComputeHash(plain),
                    Status = ApiKeyStatus.Active,
                    ExpirationTime = DateTime.UtcNow.AddMinutes(-1)
                });

            var result = await mgr.ValidateAsync(plain, null);
            result.Error.ShouldBe(ApiKeyValidationError.Expired);
        }

        [Fact]
        public async Task ValidateAsync_IpForaDoAllowlist_Falha()
        {
            var mgr = CreateManager(out var repo);
            var plain = ApiKeyManager.GeneratePlainKey();
            repo.FirstOrDefaultAsync(Arg.Any<Expression<Func<ApiKey, bool>>>())
                .Returns(new ApiKey
                {
                    KeyHash = ApiKeyManager.ComputeHash(plain),
                    Status = ApiKeyStatus.Active,
                    AllowedIpRanges = "10.0.0.0/8"
                });

            var result = await mgr.ValidateAsync(plain, IPAddress.Parse("203.0.113.5"));
            result.Error.ShouldBe(ApiKeyValidationError.IpNotAllowed);
        }

        // --- Revogação e rotação ---

        [Fact]
        public async Task RevokeAsync_MarcaComoRevogada()
        {
            var mgr = CreateManager(out var repo);
            var key = new ApiKey { Id = 9, Status = ApiKeyStatus.Active };
            repo.FirstOrDefaultAsync(9L).Returns(key);

            await mgr.RevokeAsync(9);

            key.Status.ShouldBe(ApiKeyStatus.Revoked);
            await repo.Received(1).UpdateAsync(key);
        }

        [Fact]
        public async Task RotateAsync_RevogadaAntigaGeraNovaComMesmoEscopo()
        {
            var mgr = CreateManager(out var repo);
            var old = new ApiKey
            {
                Id = 3,
                UserId = 11,
                TenantId = 4,
                Name = "rotate-me",
                Status = ApiKeyStatus.Active,
                AllowedIpRanges = "10.0.0.0/8"
            };
            old.SetPermissionScopes(new[] { "Pages.A" });
            repo.FirstOrDefaultAsync(3L).Returns(old);

            var (entity, plain) = await mgr.RotateAsync(3);

            old.Status.ShouldBe(ApiKeyStatus.Revoked);
            entity.Id.ShouldBe(0); // nova entidade, não a antiga
            entity.Name.ShouldBe("rotate-me");
            entity.UserId.ShouldBe(11);
            entity.GetPermissionScopes().ShouldBe(new[] { "Pages.A" });
            entity.AllowedIpRanges.ShouldBe("10.0.0.0/8");
            entity.KeyHash.ShouldBe(ApiKeyManager.ComputeHash(plain));
        }

        // --- IP / CIDR matcher ---

        [Theory]
        [InlineData("10.1.2.3", "10.0.0.0/8", true)]
        [InlineData("11.0.0.1", "10.0.0.0/8", false)]
        [InlineData("192.168.1.10", "192.168.1.10", true)]
        [InlineData("192.168.1.11", "192.168.1.10", false)]
        [InlineData("172.16.0.5", "172.16.0.0/12", true)]
        [InlineData("172.32.0.5", "172.16.0.0/12", false)]
        [InlineData("203.0.113.9", "203.0.113.0/30", false)]
        [InlineData("203.0.113.2", "203.0.113.0/30", true)]
        public void IsIpInRange_Cidrs(string ip, string range, bool expected)
        {
            ApiKeyManager.IsIpInRange(IPAddress.Parse(ip), range).ShouldBe(expected);
        }

        [Fact]
        public void IsIpInRange_Ipv6FamilyMismatch_False()
        {
            ApiKeyManager.IsIpInRange(IPAddress.Parse("::1"), "10.0.0.0/8").ShouldBeFalse();
        }

        [Fact]
        public void IsIpAllowed_SemAllowlist_SempreTrue()
        {
            var mgr = CreateManager(out _);
            mgr.IsIpAllowed(new ApiKey { AllowedIpRanges = null }, IPAddress.Parse("1.2.3.4")).ShouldBeTrue();
            mgr.IsIpAllowed(new ApiKey { AllowedIpRanges = "" }, null).ShouldBeTrue();
        }

        [Fact]
        public void IsIpAllowed_ComAllowlist_IpNulo_False()
        {
            var mgr = CreateManager(out _);
            mgr.IsIpAllowed(new ApiKey { AllowedIpRanges = "10.0.0.0/8" }, null).ShouldBeFalse();
        }

        [Fact]
        public void IsInScope_SemEscopo_SempreTrue_ComEscopo_Filtra()
        {
            var mgr = CreateManager(out _);
            mgr.IsInScope(new ApiKey(), "anything").ShouldBeTrue();

            var scoped = new ApiKey();
            scoped.SetPermissionScopes(new[] { "Pages.A" });
            mgr.IsInScope(scoped, "Pages.A").ShouldBeTrue();
            mgr.IsInScope(scoped, "Pages.B").ShouldBeFalse();
        }

        // --- Entidade ---

        [Fact]
        public void IsUsable_RefleteStatusExpiracaoEDelete()
        {
            new ApiKey().IsUsable(DateTime.UtcNow).ShouldBeTrue();
            new ApiKey { Status = ApiKeyStatus.Revoked }.IsUsable(DateTime.UtcNow).ShouldBeFalse();
            new ApiKey { ExpirationTime = DateTime.UtcNow.AddDays(-1) }.IsUsable(DateTime.UtcNow).ShouldBeFalse();
            new ApiKey { IsDeleted = true }.IsUsable(DateTime.UtcNow).ShouldBeFalse();
        }
    }
}

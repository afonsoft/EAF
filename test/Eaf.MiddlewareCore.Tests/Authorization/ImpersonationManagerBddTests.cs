using Abp;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Memory;
using Abp.Runtime.Session;
using Abp.UI;
using Eaf.Middleware.Authorization.Impersonation;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Shouldly;
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization
{
    /// <summary>
    /// Testes BDD para ImpersonationManager exercitando caminhos reais de token e identidade.
    /// </summary>
    public class ImpersonationManagerBddTests
    {
        [Fact]
        public void Dado_TipoImpersonationManager_Quando_Verificar_Entao_DeveImplementarIImpersonationManager()
        {
            typeof(IImpersonationManager).IsAssignableFrom(typeof(ImpersonationManager)).ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_UsuarioAutenticado_Quando_GetImpersonationToken_Entao_DeveRetornarToken()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(1L);

            var sut = CoreManagerTestHelper.CreateImpersonationManager(abpSession);

            // Quando
            var token = await sut.GetImpersonationToken(2, 1);

            // Então
            token.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_ImpersonacaoEmCascata_Quando_GetImpersonationToken_Entao_DeveLancarExcecao()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(1L);
            abpSession.ImpersonatorUserId.Returns(1L);

            var sut = CoreManagerTestHelper.CreateImpersonationManager(abpSession);

            // Quando / Então
            Should.Throw<UserFriendlyException>(() => sut.GetImpersonationToken(2, 1).GetAwaiter().GetResult());
        }

        [Fact]
        public void Dado_TenantDiferente_Quando_GetImpersonationToken_Entao_DeveLancarExcecao()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(1L);

            var sut = CoreManagerTestHelper.CreateImpersonationManager(abpSession);

            // Quando / Então
            Should.Throw<UserFriendlyException>(() => sut.GetImpersonationToken(2, 99).GetAwaiter().GetResult());
        }

        [Fact]
        public void Dado_HostImpersonandoTenant_Quando_GetImpersonationTokenSemTenant_Entao_DeveLancarExcecao()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(1L);

            var sut = CoreManagerTestHelper.CreateImpersonationManager(abpSession);

            // Quando / Então
            Should.Throw<UserFriendlyException>(() => sut.GetImpersonationToken(2, null).GetAwaiter().GetResult());
        }

        [Fact]
        public async Task Dado_TokenValido_Quando_GetImpersonatedUserAndIdentity_Entao_DeveRetornarUsuarioEIdentidade()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(1L);

            var sut = CoreManagerTestHelper.CreateImpersonationManager(abpSession);
            var token = await sut.GetImpersonationToken(2, 1);

            // Quando
            var result = await sut.GetImpersonatedUserAndIdentity(token);

            // Então
            result.ShouldNotBeNull();
            result.User.ShouldNotBeNull();
            result.User.Id.ShouldBe(2);
            result.Identity.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_TokenBackToImpersonator_Quando_GetImpersonatedUserAndIdentity_Entao_DeveRetornarUsuarioEIdentidade()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(2L);
            abpSession.ImpersonatorUserId.Returns(1L);
            abpSession.ImpersonatorTenantId.Returns(1);

            var sut = CoreManagerTestHelper.CreateImpersonationManager(abpSession);
            var token = await sut.GetBackToImpersonatorToken();

            // Quando
            var result = await sut.GetImpersonatedUserAndIdentity(token);

            // Então
            result.ShouldNotBeNull();
            result.User.ShouldNotBeNull();
            result.User.Id.ShouldBe(1);
            result.Identity.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_TokenInvalido_Quando_GetImpersonatedUserAndIdentity_Entao_DeveLancarExcecao()
        {
            // Dado
            var sut = CoreManagerTestHelper.CreateImpersonationManager();

            // Quando / Então
            Should.Throw<UserFriendlyException>(() => sut.GetImpersonatedUserAndIdentity("invalid-token").GetAwaiter().GetResult());
        }

        [Fact]
        public async Task Dado_HostImpersonandoTenant_Quando_GetImpersonationToken_Entao_DeveRetornarToken()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            abpSession.UserId.Returns(1L);

            var sut = CoreManagerTestHelper.CreateImpersonationManager(abpSession);

            // Quando
            var token = await sut.GetImpersonationToken(2, 1);

            // Então
            token.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_SemImpersonacao_Quando_GetBackToImpersonatorToken_Entao_DeveLancarExcecao()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            abpSession.UserId.Returns(1L);

            var sut = CoreManagerTestHelper.CreateImpersonationManager(abpSession);

            // Quando / Então
            Should.Throw<UserFriendlyException>(() => sut.GetBackToImpersonatorToken().GetAwaiter().GetResult());
        }

        [Fact]
        public async Task Dado_TokenEmCacheNaoEncontradoComUserTokenValido_Quando_GetImpersonatedUserAndIdentity_Entao_DeveRetornarUsuarioEIdentidade()
        {
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(2L);

            var sut = CriarImpersonationManagerComUserToken(
                abpSession,
                CriarUserToken(
                    tenantId: 1,
                    userId: 2,
                    name: "token-from-db",
                    value: "1-1",
                    expireDate: DateTime.UtcNow.AddHours(1)),
                new User { Id = 2, TenantId = 1, UserName = "target" });

            var result = await sut.GetImpersonatedUserAndIdentity("token-from-db");

            result.ShouldNotBeNull();
            result.User.ShouldNotBeNull();
            result.User.Id.ShouldBe(2);
            result.Identity.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_TokenEmCacheNaoEncontradoComTenantDiferente_Quando_GetImpersonatedUserAndIdentity_Entao_DeveLancarExcecao()
        {
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(2);
            abpSession.UserId.Returns(2L);

            var sut = CriarImpersonationManagerComUserToken(
                abpSession,
                CriarUserToken(
                    tenantId: 1,
                    userId: 2,
                    name: "token-from-db",
                    value: "1-1",
                    expireDate: DateTime.UtcNow.AddHours(1)),
                new User { Id = 2, TenantId = 1, UserName = "target" });

            Should.Throw<UserFriendlyException>(() => sut.GetImpersonatedUserAndIdentity("token-from-db").GetAwaiter().GetResult());
        }

        [Fact]
        public async Task Dado_ErroAoSalvarTokenDeImpersonacao_Quando_GetImpersonationToken_Entao_DeveRetornarTokenMesmoAssim()
        {
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            abpSession.UserId.Returns(1L);

            var userTokenRepository = Substitute.For<IRepository<UserToken, long>>();
            userTokenRepository.When(x => x.InsertAndGetIdAsync(Arg.Any<UserToken>())).Do(_ => throw new Exception("db error"));

            var sut = CriarImpersonationManager(abpSession, userTokenRepository);

            var token = await sut.GetImpersonationToken(2, 1);

            token.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_LocalizationManagerComArgs_Quando_LocalizarComArgumentos_Entao_DeveRetornarTextoFormatado()
        {
            var source = Substitute.For<ILocalizationSource>();
            source.GetStringOrNull("Key", Arg.Any<CultureInfo>()).Returns("Hello {0}");

            var localizationManager = Substitute.For<ILocalizationManager>();
            localizationManager.GetSource("EafCore").Returns(source);

            var sut = CriarImpersonationManagerLocalizacao(localizationManager);

            var result = sut.LocalizeArgs("Key", "World");

            result.ShouldBe("Hello World");
        }

        [Fact]
        public void Dado_LocalizationManagerComCulture_Quando_LocalizarComCulture_Entao_DeveRetornarTextoLocalizado()
        {
            var source = Substitute.For<ILocalizationSource>();
            source.GetStringOrNull("Key", CultureInfo.InvariantCulture).Returns("Localized");

            var localizationManager = Substitute.For<ILocalizationManager>();
            localizationManager.GetSource("EafCore").Returns(source);

            var sut = CriarImpersonationManagerLocalizacao(localizationManager);

            var result = sut.LocalizeCulture("Key", CultureInfo.InvariantCulture);

            result.ShouldBe("Localized");
        }

        private static UserToken CriarUserToken(int? tenantId, long userId, string name, string value, DateTime expireDate)
        {
            var token = (UserToken)Activator.CreateInstance(typeof(UserToken), true)!;
            token.TenantId = tenantId;
            token.UserId = userId;
            token.Name = name;
            token.Value = value;
            token.ExpireDate = expireDate;
            return token;
        }

        private static ImpersonationManager CriarImpersonationManager(IAbpSession abpSession, IRepository<UserToken, long> userTokenRepository)
        {
            var userManager = CoreManagerTestHelper.CreateUserManager();
            var roleManager = CoreManagerTestHelper.CreateRoleManager();
            var principalFactory = CoreManagerTestHelper.CreateUserClaimsPrincipalFactory(userManager, roleManager);
            var cacheManager = Substitute.For<ICacheManager>();
            cacheManager.GetCache("AppImpersonationCache").Returns(new AbpMemoryCache("AppImpersonationCache"));
            var settingManager = Substitute.For<ISettingManager>();

            var sut = new ImpersonationManager(cacheManager, userManager, principalFactory, userTokenRepository, settingManager);
            sut.AbpSession = abpSession;
            return sut;
        }

        private static ImpersonationManager CriarImpersonationManagerComUserToken(IAbpSession abpSession, UserToken userToken, User targetUser)
        {
            var userTokenRepository = Substitute.For<IRepository<UserToken, long>>();
            userTokenRepository.FirstOrDefaultAsync(Arg.Any<Expression<Func<UserToken, bool>>>()).Returns(userToken);

            var userManager = CoreManagerTestHelper.CreateUserManager();
            userManager.FindByIdAsync(targetUser.Id.ToString()).Returns(targetUser);

            var roleManager = CoreManagerTestHelper.CreateRoleManager();
            var principalFactory = CoreManagerTestHelper.CreateUserClaimsPrincipalFactory(userManager, roleManager);

            var cacheManager = Substitute.For<ICacheManager>();
            cacheManager.GetCache("AppImpersonationCache").Returns(new AbpMemoryCache("AppImpersonationCache"));

            var settingManager = Substitute.For<ISettingManager>();

            var sut = new ImpersonationManager(cacheManager, userManager, principalFactory, userTokenRepository, settingManager);
            sut.AbpSession = abpSession;
            return sut;
        }

        private static TestableImpersonationManager CriarImpersonationManagerLocalizacao(ILocalizationManager localizationManager)
        {
            var userManager = CoreManagerTestHelper.CreateUserManager();
            var roleManager = CoreManagerTestHelper.CreateRoleManager();
            var principalFactory = CoreManagerTestHelper.CreateUserClaimsPrincipalFactory(userManager, roleManager);
            var cacheManager = Substitute.For<ICacheManager>();
            var userTokenRepository = Substitute.For<IRepository<UserToken, long>>();
            var settingManager = Substitute.For<ISettingManager>();

            var sut = new TestableImpersonationManager(cacheManager, userManager, principalFactory, userTokenRepository, settingManager);
            sut.LocalizationManager = localizationManager;
            return sut;
        }

        private class TestableImpersonationManager : ImpersonationManager
        {
            public TestableImpersonationManager(ICacheManager cacheManager, UserManager userManager, UserClaimsPrincipalFactory principalFactory, IRepository<UserToken, long> userTokenRepository, ISettingManager settingManager)
                : base(cacheManager, userManager, principalFactory, userTokenRepository, settingManager)
            {
            }

            public string LocalizeArgs(string name, params object[] args) => base.L(name, args);

            public string LocalizeCulture(string name, CultureInfo culture) => base.L(name, culture);
        }
    }
}

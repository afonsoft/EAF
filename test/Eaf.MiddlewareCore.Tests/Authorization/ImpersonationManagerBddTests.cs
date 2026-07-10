using Abp;
using Abp.Runtime.Session;
using Abp.UI;
using Eaf.Middleware.Authorization.Impersonation;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Tests.Helpers;
using NSubstitute;
using Shouldly;
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
    }
}

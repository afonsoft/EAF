using Abp.Runtime.Session;
using Eaf.Middleware.Authorization.Accounts;
using Eaf.Middleware.Authorization.Accounts.Dto;
using Eaf.Middleware.Authorization.Impersonation;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Url;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts
{
    /// <summary>
    /// Testes BDD para AccountAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class AccountAppServiceBddTests
    {
        private readonly IUserEmailer _userEmailer;
        private readonly IWebUrlService _webUrlService;
        private readonly IImpersonationManager _impersonationManager;
        private readonly AccountAppService _sut;

        public AccountAppServiceBddTests()
        {
            _userEmailer = Substitute.For<IUserEmailer>();
            _webUrlService = Substitute.For<IWebUrlService>();
            _impersonationManager = Substitute.For<IImpersonationManager>();

            _sut = new AccountAppService(_userEmailer, _webUrlService, _impersonationManager);
        }

        #region Construtor

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
            _sut.AppUrlService.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_AppUrlServiceDeveSerNullInstance()
        {
            _sut.AppUrlService.ShouldBe(NullAppUrlService.Instance);
        }

        #endregion

        #region ResolveTenantId

        [Fact]
        public async Task Dado_ParametroCVazio_Quando_ResolveTenantId_Entao_DeveRetornarTenantIdDaSessao()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(5);
            _sut.AbpSession = abpSession;

            // Quando
            var result = await _sut.ResolveTenantId(new ResolveTenantIdInput { c = "" });

            // Então
            result.ShouldBe(5);
        }

        [Fact]
        public async Task Dado_ParametroCNulo_Quando_ResolveTenantId_Entao_DeveRetornarTenantIdDaSessao()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(3);
            _sut.AbpSession = abpSession;

            // Quando
            var result = await _sut.ResolveTenantId(new ResolveTenantIdInput { c = null });

            // Então
            result.ShouldBe(3);
        }

        #endregion

        #region BackToImpersonator

        [Fact]
        public async Task Dado_UsuarioImpersonado_Quando_BackToImpersonator_Entao_DeveRetornarToken()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.ImpersonatorTenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            _impersonationManager.GetBackToImpersonatorToken().Returns("token-abc-123");

            // Quando
            var result = await _sut.BackToImpersonator();

            // Então
            result.ShouldNotBeNull();
            result.ImpersonationToken.ShouldBe("token-abc-123");
        }

        #endregion
    }
}

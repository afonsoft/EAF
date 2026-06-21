using Abp;
using Abp.Runtime.Session;
using Eaf.Middleware.Sessions;
using Eaf.Middleware.UiCustomization;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Sessions
{
    /// <summary>
    /// Testes BDD para SessionAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class SessionAppServiceBddTests
    {
        private readonly IUiThemeCustomizerFactory _uiThemeCustomizerFactory;
        private readonly SessionAppService _sut;

        public SessionAppServiceBddTests()
        {
            _uiThemeCustomizerFactory = Substitute.For<IUiThemeCustomizerFactory>();
            _sut = new SessionAppService(_uiThemeCustomizerFactory);
        }

        #region Construtor

        [Fact]
        public void Dado_UiThemeCustomizerFactory_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion

        #region UpdateUserSignInToken

        [Fact]
        public async Task Dado_UsuarioNaoLogado_Quando_UpdateUserSignInToken_Entao_DeveLancarExcecao()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns((long?)null);
            _sut.AbpSession = abpSession;

            var localizationManager = Substitute.For<Abp.Localization.ILocalizationManager>();
            _sut.LocalizationManager = localizationManager;

            // Quando / Então
            await Should.ThrowAsync<System.Exception>(() => _sut.UpdateUserSignInToken());
        }

        [Fact]
        public async Task Dado_UserIdZero_Quando_UpdateUserSignInToken_Entao_DeveLancarExcecao()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(0L);
            _sut.AbpSession = abpSession;

            var localizationManager = Substitute.For<Abp.Localization.ILocalizationManager>();
            _sut.LocalizationManager = localizationManager;

            // Quando / Então
            await Should.ThrowAsync<AbpException>(() => _sut.UpdateUserSignInToken());
        }

        #endregion
    }
}

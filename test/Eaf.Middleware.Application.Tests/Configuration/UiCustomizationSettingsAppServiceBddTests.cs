using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Configuration.Dto;
using Eaf.Middleware.UiCustomization;
using Eaf.Middleware.UiCustomization.Dto;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Configuration
{
    /// <summary>
    /// Testes BDD para UiCustomizationSettingsAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class UiCustomizationSettingsAppServiceBddTests
    {
        private readonly SettingManager _settingManager;
        private readonly IIocResolver _iocResolver;
        private readonly IUiThemeCustomizerFactory _uiThemeCustomizerFactory;
        private readonly UiCustomizationSettingsAppService _sut;

        public UiCustomizationSettingsAppServiceBddTests()
        {
            _settingManager = Substitute.For<SettingManager>(
                Substitute.For<ISettingDefinitionManager>(),
                Substitute.For<ICacheManager>(),
                Substitute.For<IMultiTenancyConfig>(),
                Substitute.For<ITenantStore>(),
                Substitute.For<ISettingEncryptionService>(),
                Substitute.For<IUnitOfWorkManager>()
            );
            _iocResolver = Substitute.For<IIocResolver>();
            _uiThemeCustomizerFactory = Substitute.For<IUiThemeCustomizerFactory>();

            _sut = new UiCustomizationSettingsAppService(
                _settingManager,
                _iocResolver,
                _uiThemeCustomizerFactory
            );
        }

        #region Construtor

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion

        #region GetUiManagementSettings

        [Fact]
        public async Task Dado_NenhumCustomizer_Quando_GetUiManagementSettings_Entao_DeveRetornarListaVazia()
        {
            // Dado
            _iocResolver.ResolveAll<IUiCustomizer>().Returns(new IUiCustomizer[0]);

            // Quando
            var result = await _sut.GetUiManagementSettings();

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Dado_CustomizersExistentes_Quando_GetUiManagementSettings_Entao_DeveRetornarSettings()
        {
            // Dado
            var customizer = Substitute.For<IUiCustomizer>();
            var uiSettings = new UiCustomizationSettingsDto
            {
                BaseSettings = new ThemeSettingsDto { Theme = "default" }
            };
            customizer.GetUiSettings().Returns(uiSettings);

            _iocResolver.ResolveAll<IUiCustomizer>().Returns(new[] { customizer });

            // Quando
            var result = await _sut.GetUiManagementSettings();

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
        }

        #endregion

        #region UpdateDefaultUiManagementSettings - Tenant

        [Fact]
        public async Task Dado_TenantLogado_Quando_UpdateDefaultUiManagementSettings_Entao_DeveAtualizarParaTenant()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            _sut.AbpSession = abpSession;

            var customizer = Substitute.For<IUiCustomizer>();
            _uiThemeCustomizerFactory.GetUiCustomizer("default").Returns(customizer);

            var settings = new ThemeSettingsDto { Theme = "default" };

            // Quando
            await _sut.UpdateDefaultUiManagementSettings(settings);

            // Então
            await customizer.Received(1).UpdateTenantUiManagementSettingsAsync(1, settings);
        }

        #endregion

        #region UpdateDefaultUiManagementSettings - Host

        [Fact]
        public async Task Dado_HostLogado_Quando_UpdateDefaultUiManagementSettings_Entao_DeveAtualizarParaAplicacao()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var customizer = Substitute.For<IUiCustomizer>();
            _uiThemeCustomizerFactory.GetUiCustomizer("default").Returns(customizer);

            var settings = new ThemeSettingsDto { Theme = "default" };

            // Quando
            await _sut.UpdateDefaultUiManagementSettings(settings);

            // Então
            await customizer.Received(1).UpdateApplicationUiManagementSettingsAsync(settings);
        }

        #endregion

        #region UpdateUiManagementSettings

        [Fact]
        public async Task Dado_UsuarioLogado_Quando_UpdateUiManagementSettings_Entao_DeveAtualizarParaUsuario()
        {
            // Dado
            var userIdentifier = new Abp.UserIdentifier(1, 42);
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            var customizer = Substitute.For<IUiCustomizer>();
            _uiThemeCustomizerFactory.GetUiCustomizer("dark").Returns(customizer);

            var settings = new ThemeSettingsDto { Theme = "dark" };

            // Quando
            await _sut.UpdateUiManagementSettings(settings);

            // Então
            await customizer.Received(1).UpdateUserUiManagementSettingsAsync(Arg.Any<Abp.UserIdentifier>(), settings);
        }

        #endregion
    }
}

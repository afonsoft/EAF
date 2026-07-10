using Abp;
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
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Configuration
{
    /// <summary>
    /// Testes BDD para UiCustomizationSettingsAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class UiCustomizationSettingsAppServiceBddTests
    {
        private static UiCustomizationSettingsAppService CreateSut(
            out IIocResolver iocResolver,
            out IUiThemeCustomizerFactory uiThemeCustomizerFactory,
            out IUiCustomizer uiCustomizer,
            out IAbpSession abpSession,
            out SettingManager settingManager)
        {
            iocResolver = Substitute.For<IIocResolver>();
            uiThemeCustomizerFactory = Substitute.For<IUiThemeCustomizerFactory>();
            uiCustomizer = Substitute.For<IUiCustomizer>();
            abpSession = Substitute.For<IAbpSession>();

            settingManager = Substitute.For<SettingManager>(new object[]
            {
                Substitute.For<ISettingDefinitionManager>(),
                Substitute.For<ICacheManager>(),
                Substitute.For<IMultiTenancyConfig>(),
                Substitute.For<ITenantStore>(),
                Substitute.For<ISettingEncryptionService>(),
                Substitute.For<IUnitOfWorkManager>()
            });

            var sut = new UiCustomizationSettingsAppService(settingManager, iocResolver, uiThemeCustomizerFactory)
            {
                AbpSession = abpSession
            };

            return sut;
        }

        [Fact]
        public async Task Dado_ThemeCadastrado_Quando_GetUiManagementSettings_Entao_DeveRetornarListaDeTemas()
        {
            // Dado
            var sut = CreateSut(out var iocResolver, out _, out var uiCustomizer, out _, out _);
            iocResolver.ResolveAll<IUiCustomizer>().Returns(new[] { uiCustomizer });
            uiCustomizer.GetUiSettings().Returns(new UiCustomizationSettingsDto
            {
                BaseSettings = new ThemeSettingsDto { Theme = "Default" }
            });

            // Quando
            var result = await sut.GetUiManagementSettings();

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].Theme.ShouldBe("Default");
        }

        [Fact]
        public async Task Dado_UsuarioLogado_Quando_UpdateUiManagementSettings_Entao_DeveAtualizarConfiguracaoDoUsuario()
        {
            // Dado
            var settings = new ThemeSettingsDto { Theme = "Default" };
            var userIdentifier = new UserIdentifier(1, 1);

            var sut = CreateSut(out _, out var uiThemeCustomizerFactory, out var uiCustomizer, out var abpSession, out var _);
            abpSession.UserId.Returns(1L);
            abpSession.TenantId.Returns(1);
            uiThemeCustomizerFactory.GetUiCustomizer("Default").Returns(uiCustomizer);

            // Quando
            await sut.UpdateUiManagementSettings(settings);

            // Então
            uiThemeCustomizerFactory.Received(1).GetUiCustomizer("Default");
            await uiCustomizer.Received(1).UpdateUserUiManagementSettingsAsync(userIdentifier, settings);
        }

        [Fact]
        public async Task Dado_TenantExistente_Quando_UpdateDefaultUiManagementSettings_Entao_DeveAtualizarConfiguracaoDoTenant()
        {
            // Dado
            var settings = new ThemeSettingsDto { Theme = "Default" };

            var sut = CreateSut(out _, out var uiThemeCustomizerFactory, out var uiCustomizer, out var abpSession, out var _);
            abpSession.TenantId.Returns(1);
            uiThemeCustomizerFactory.GetUiCustomizer("Default").Returns(uiCustomizer);

            // Quando
            await sut.UpdateDefaultUiManagementSettings(settings);

            // Então
            uiThemeCustomizerFactory.Received(1).GetUiCustomizer("Default");
            await uiCustomizer.Received(1).UpdateTenantUiManagementSettingsAsync(1, settings);
        }

        [Fact]
        public async Task Dado_HostSemTenant_Quando_UpdateDefaultUiManagementSettings_Entao_DeveAtualizarConfiguracaoAplicacao()
        {
            // Dado
            var settings = new ThemeSettingsDto { Theme = "Default" };

            var sut = CreateSut(out _, out var uiThemeCustomizerFactory, out var uiCustomizer, out var abpSession, out var _);
            abpSession.TenantId.Returns((int?)null);
            uiThemeCustomizerFactory.GetUiCustomizer("Default").Returns(uiCustomizer);

            // Quando
            await sut.UpdateDefaultUiManagementSettings(settings);

            // Então
            uiThemeCustomizerFactory.Received(1).GetUiCustomizer("Default");
            await uiCustomizer.Received(1).UpdateApplicationUiManagementSettingsAsync(settings);
        }

    }
}

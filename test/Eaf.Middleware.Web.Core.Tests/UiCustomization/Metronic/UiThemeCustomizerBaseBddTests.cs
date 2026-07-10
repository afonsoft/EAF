using Abp;
using Abp.Configuration;
using Eaf.Middleware.Web.UiCustomization.Metronic;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.UiCustomization.Metronic
{
    public class UiThemeCustomizerBaseBddTests
    {
        private sealed class TestableUiThemeCustomizerBase : UiThemeCustomizerBase
        {
            public TestableUiThemeCustomizerBase(SettingManager settingManager, string themeName)
                : base(settingManager, themeName)
            {
            }

            public new Task<string> GetSettingValueAsync(string settingName) => base.GetSettingValueAsync(settingName);
            public new Task<T> GetSettingValueAsync<T>(string settingName) where T : struct => base.GetSettingValueAsync<T>(settingName);
            public new Task<string> GetSettingValueForApplicationAsync(string settingName) => base.GetSettingValueForApplicationAsync(settingName);
            public new Task<T> GetSettingValueForApplicationAsync<T>(string settingName) where T : struct => base.GetSettingValueForApplicationAsync<T>(settingName);
            public new Task<string> GetSettingValueForTenantAsync(string settingName, int tenantId) => base.GetSettingValueForTenantAsync(settingName, tenantId);
            public new Task<T> GetSettingValueForTenantAsync<T>(string settingName, int tenantId) where T : struct => base.GetSettingValueForTenantAsync<T>(settingName, tenantId);
            public new Task ChangeSettingForApplicationAsync(string name, string value) => base.ChangeSettingForApplicationAsync(name, value);
            public new Task ChangeSettingForTenantAsync(int tenantId, string name, string value) => base.ChangeSettingForTenantAsync(tenantId, name, value);
            public new Task ChangeSettingForUserAsync(UserIdentifier user, string name, string value) => base.ChangeSettingForUserAsync(user, name, value);
        }

        [Fact]
        public void Dado_Tipo_Quando_VerificarNome_Entao_DeveSerCorreto()
        {
            typeof(UiThemeCustomizerBase).Name.ShouldBe("UiThemeCustomizerBase");
        }

        [Fact]
        public async Task Dado_ValorString_Quando_GetSettingValueAsync_Entao_DeveRetornarValorComPrefixoDoTema()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager("test");
            var sut = new TestableUiThemeCustomizerBase(settingManager, "test");

            var result = await sut.GetSettingValueAsync("Theme");

            result.ShouldBe("test");
        }

        [Fact]
        public async Task Dado_ValorBooleano_Quando_GetSettingValueAsync_Entao_DeveRetornarBooleanConvertido()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager("test");
            var sut = new TestableUiThemeCustomizerBase(settingManager, "test");

            var result = await sut.GetSettingValueAsync<bool>("Fixed");

            result.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_ValorStringApplication_Quando_GetSettingValueForApplicationAsync_Entao_DeveRetornarValor()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager("test");
            var sut = new TestableUiThemeCustomizerBase(settingManager, "test");

            var result = await sut.GetSettingValueForApplicationAsync("Theme");

            result.ShouldBe("test");
        }

        [Fact]
        public async Task Dado_ValorBooleanoApplication_Quando_GetSettingValueForApplicationAsync_Entao_DeveRetornarBooleanConvertido()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager("test");
            var sut = new TestableUiThemeCustomizerBase(settingManager, "test");

            var result = await sut.GetSettingValueForApplicationAsync<bool>("DesktopFixedHeader");

            result.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_ValorStringTenant_Quando_GetSettingValueForTenantAsync_Entao_DeveRetornarValor()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager("test");
            var sut = new TestableUiThemeCustomizerBase(settingManager, "test");

            var result = await sut.GetSettingValueForTenantAsync("Theme", 42);

            result.ShouldBe("test");
        }

        [Fact]
        public async Task Dado_ValorBooleanoTenant_Quando_GetSettingValueForTenantAsync_Entao_DeveRetornarBooleanConvertido()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager("test");
            var sut = new TestableUiThemeCustomizerBase(settingManager, "test");

            var result = await sut.GetSettingValueForTenantAsync<bool>("FixedAside", 42);

            result.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_Configuracao_Quando_ChangeSettingForApplicationAsync_Entao_DeveChamarSettingManagerComPrefixo()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager("test");
            var sut = new TestableUiThemeCustomizerBase(settingManager, "test");

            await sut.ChangeSettingForApplicationAsync("Theme", "dark");

            await settingManager.Received(1).ChangeSettingForApplicationAsync("test.Theme", "dark");
        }

        [Fact]
        public async Task Dado_Configuracao_Quando_ChangeSettingForTenantAsync_Entao_DeveChamarSettingManagerComPrefixo()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager("test");
            var sut = new TestableUiThemeCustomizerBase(settingManager, "test");

            await sut.ChangeSettingForTenantAsync(42, "Theme", "dark");

            await settingManager.Received(1).ChangeSettingForTenantAsync(42, "test.Theme", "dark");
        }

        [Fact]
        public async Task Dado_Configuracao_Quando_ChangeSettingForUserAsync_Entao_DeveChamarSettingManagerComPrefixo()
        {
            var settingManager = UiCustomizationTestHelper.CreateSettingManager("test");
            var sut = new TestableUiThemeCustomizerBase(settingManager, "test");
            var user = new UserIdentifier(42, 1);

            await sut.ChangeSettingForUserAsync(user, "Theme", "dark");

            await settingManager.Received(1).ChangeSettingForUserAsync(user, "test.Theme", "dark");
        }
    }
}

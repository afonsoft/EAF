using Abp.Configuration;
using Eaf.Middleware.Web.UiCustomization.Metronic;
using NSubstitute;
using Shouldly;
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
        }

        [Fact]
        public void Dado_Tipo_Quando_VerificarNome_Entao_DeveSerCorreto()
        {
            typeof(UiThemeCustomizerBase).Name.ShouldBe("UiThemeCustomizerBase");
        }
    }
}

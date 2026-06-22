using Abp.Configuration;
using Eaf.Middleware.UiCustomization;
using Eaf.Middleware.Web.UiCustomization;
using Eaf.Middleware.Web.UiCustomization.Metronic;
using NSubstitute;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.UiCustomization
{
    /// <summary>
    /// Testes BDD para UiThemeCustomizerFactory seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class UiThemeCustomizerFactoryBddTests
    {
        #region Instanciacao

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var settingManager = Substitute.For<ISettingManager>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            var sut = new UiThemeCustomizerFactory(settingManager, serviceProvider);

            sut.ShouldNotBeNull();
            sut.ShouldBeAssignableTo<IUiThemeCustomizerFactory>();
        }

        #endregion

        #region GetUiCustomizer - Resolucao de Tipos

        [Fact]
        public void Dado_Theme2_Quando_GetUiCustomizer_Entao_DeveResolverTipoTheme2UiCustomizer()
        {
            // Dado
            var settingManager = Substitute.For<ISettingManager>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            Type requestedType = null;
            serviceProvider.GetService(Arg.Do<Type>(t => requestedType = t)).Returns((object)null);

            var sut = new UiThemeCustomizerFactory(settingManager, serviceProvider);

            // Quando
            try { sut.GetUiCustomizer("theme2"); } catch { }

            // Entao
            requestedType.ShouldBe(typeof(Theme2UiCustomizer));
        }

        [Fact]
        public void Dado_Theme3_Quando_GetUiCustomizer_Entao_DeveResolverTipoTheme3UiCustomizer()
        {
            var settingManager = Substitute.For<ISettingManager>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            Type requestedType = null;
            serviceProvider.GetService(Arg.Do<Type>(t => requestedType = t)).Returns((object)null);

            var sut = new UiThemeCustomizerFactory(settingManager, serviceProvider);
            try { sut.GetUiCustomizer("theme3"); } catch { }
            requestedType.ShouldBe(typeof(Theme3UiCustomizer));
        }

        [Fact]
        public void Dado_Theme4_Quando_GetUiCustomizer_Entao_DeveResolverTipoTheme4UiCustomizer()
        {
            var settingManager = Substitute.For<ISettingManager>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            Type requestedType = null;
            serviceProvider.GetService(Arg.Do<Type>(t => requestedType = t)).Returns((object)null);

            var sut = new UiThemeCustomizerFactory(settingManager, serviceProvider);
            try { sut.GetUiCustomizer("theme4"); } catch { }
            requestedType.ShouldBe(typeof(Theme4UiCustomizer));
        }

        [Fact]
        public void Dado_ThemeDefault_Quando_GetUiCustomizer_Entao_DeveResolverTipoThemeDefaultUiCustomizer()
        {
            var settingManager = Substitute.For<ISettingManager>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            Type requestedType = null;
            serviceProvider.GetService(Arg.Do<Type>(t => requestedType = t)).Returns((object)null);

            var sut = new UiThemeCustomizerFactory(settingManager, serviceProvider);
            try { sut.GetUiCustomizer("default"); } catch { }
            requestedType.ShouldBe(typeof(ThemeDefaultUiCustomizer));
        }

        [Fact]
        public void Dado_ThemeDesconhecido_Quando_GetUiCustomizer_Entao_DeveResolverThemeDefault()
        {
            var settingManager = Substitute.For<ISettingManager>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            Type requestedType = null;
            serviceProvider.GetService(Arg.Do<Type>(t => requestedType = t)).Returns((object)null);

            var sut = new UiThemeCustomizerFactory(settingManager, serviceProvider);
            try { sut.GetUiCustomizer("unknown"); } catch { }
            requestedType.ShouldBe(typeof(ThemeDefaultUiCustomizer));
        }

        [Fact]
        public void Dado_Theme2CaseInsensitive_Quando_GetUiCustomizer_Entao_DeveResolverTheme2()
        {
            var settingManager = Substitute.For<ISettingManager>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            Type requestedType = null;
            serviceProvider.GetService(Arg.Do<Type>(t => requestedType = t)).Returns((object)null);

            var sut = new UiThemeCustomizerFactory(settingManager, serviceProvider);
            try { sut.GetUiCustomizer("Theme2"); } catch { }
            requestedType.ShouldBe(typeof(Theme2UiCustomizer));
        }

        #endregion

        #region GetCurrentUiCustomizer

        [Fact]
        public async Task Dado_SettingComTheme2_Quando_GetCurrentUiCustomizer_Entao_DeveResolverTheme2()
        {
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueAsync(Arg.Any<string>()).Returns("theme2");
            var serviceProvider = Substitute.For<IServiceProvider>();
            Type requestedType = null;
            serviceProvider.GetService(Arg.Do<Type>(t => requestedType = t)).Returns((object)null);

            var sut = new UiThemeCustomizerFactory(settingManager, serviceProvider);
            try { await sut.GetCurrentUiCustomizer(); } catch { }
            requestedType.ShouldBe(typeof(Theme2UiCustomizer));
        }

        #endregion
    }
}

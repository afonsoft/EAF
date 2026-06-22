using Eaf.Middleware.Core.Authentication.External;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Authorization.External
{
    /// <summary>
    /// Testes BDD para ExternalAuthConfiguration seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class ExternalAuthConfigurationBddTests
    {
        #region Instanciacao

        [Fact]
        public void Dado_ExternalAuthConfiguration_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var config = new ExternalAuthConfiguration();
            config.ShouldNotBeNull();
            config.ShouldBeAssignableTo<IExternalAuthConfiguration>();
        }

        [Fact]
        public void Dado_ExternalAuthConfiguration_Quando_CriarInstancia_Entao_ListaDeveSerVazia()
        {
            var config = new ExternalAuthConfiguration();
            config.ExternalLoginInfoProviders.ShouldNotBeNull();
            config.ExternalLoginInfoProviders.Count.ShouldBe(0);
        }

        #endregion

        #region ExternalLoginInfoProviders

        [Fact]
        public void Dado_ExternalAuthConfiguration_Quando_AdicionarProvider_Entao_DeveConterProvider()
        {
            // Dado
            var config = new ExternalAuthConfiguration();
            var provider = Substitute.For<IExternalLoginInfoProvider>();
            provider.Name.Returns("Google");

            // Quando
            config.ExternalLoginInfoProviders.Add(provider);

            // Entao
            config.ExternalLoginInfoProviders.Count.ShouldBe(1);
            config.ExternalLoginInfoProviders[0].Name.ShouldBe("Google");
        }

        [Fact]
        public void Dado_ExternalAuthConfiguration_Quando_AdicionarMultiplosProviders_Entao_DeveConterTodos()
        {
            // Dado
            var config = new ExternalAuthConfiguration();
            var google = Substitute.For<IExternalLoginInfoProvider>();
            google.Name.Returns("Google");
            var microsoft = Substitute.For<IExternalLoginInfoProvider>();
            microsoft.Name.Returns("Microsoft");

            // Quando
            config.ExternalLoginInfoProviders.Add(google);
            config.ExternalLoginInfoProviders.Add(microsoft);

            // Entao
            config.ExternalLoginInfoProviders.Count.ShouldBe(2);
        }

        #endregion
    }
}

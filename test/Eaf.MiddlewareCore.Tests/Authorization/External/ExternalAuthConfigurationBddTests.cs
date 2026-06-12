using Eaf.Middleware.Core.Authentication.External;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.External
{
    /// <summary>
    /// Testes BDD para ExternalAuthConfiguration seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ExternalAuthConfigurationBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_ProvidersDeveSerListaVazia()
        {
            // Dado & Quando
            var config = new ExternalAuthConfiguration();

            // Então
            config.ExternalLoginInfoProviders.ShouldNotBeNull();
            config.ExternalLoginInfoProviders.Count.ShouldBe(0);
        }
    }
}

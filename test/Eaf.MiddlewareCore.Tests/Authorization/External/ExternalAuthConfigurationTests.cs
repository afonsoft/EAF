using Eaf.Middleware.Core.Authentication.External;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Authorization.External
{
    public class ExternalAuthConfigurationTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_ListaDeveSerVazia()
        {
            var config = new ExternalAuthConfiguration();
            config.ExternalLoginInfoProviders.ShouldNotBeNull();
            config.ExternalLoginInfoProviders.Count.ShouldBe(0);
        }
    }
}

using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Models.TokenAuth
{
    public class ExternalLoginProviderInfoModelBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ExternalLoginProviderInfoModel();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirName_Entao_DeveArmazenar()
        {
            var sut = new ExternalLoginProviderInfoModel();
            sut.Name = "Google";
            sut.Name.ShouldBe("Google");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirClientId_Entao_DeveArmazenar()
        {
            var sut = new ExternalLoginProviderInfoModel();
            sut.ClientId = "client_123";
            sut.ClientId.ShouldBe("client_123");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirAdditionalParams_Entao_DeveArmazenar()
        {
            var sut = new ExternalLoginProviderInfoModel();
            sut.AdditionalParams = new Dictionary<string, string> { { "key", "value" } };
            sut.AdditionalParams.ShouldContainKey("key");
        }
    }
}

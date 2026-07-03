using Eaf.Middleware.WebHooks.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.WebHooks
{
    public class GetAllAvailableWebhooksOutputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetAllAvailableWebhooksOutput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirDisplayName_Entao_DeveArmazenar()
        {
            var sut = new GetAllAvailableWebhooksOutput();
            sut.DisplayName = "test_value";
            sut.DisplayName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirName_Entao_DeveArmazenar()
        {
            var sut = new GetAllAvailableWebhooksOutput();
            sut.Name = "test_value";
            sut.Name.ShouldBe("test_value");
        }
    }
}

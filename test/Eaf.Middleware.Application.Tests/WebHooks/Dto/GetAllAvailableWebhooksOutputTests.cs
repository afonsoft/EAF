using Eaf.Middleware.WebHooks.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.WebHooks.Dto
{
    public class GetAllAvailableWebhooksOutputTests
    {
        [Fact]
        public void Dado_GetAllAvailableWebhooksOutput_Quando_Criado_Entao_PropriedadesDevemSerNulas()
        {
            var output = new GetAllAvailableWebhooksOutput();

            output.Description.ShouldBeNull();
            output.DisplayName.ShouldBeNull();
            output.Name.ShouldBeNull();
        }

        [Fact]
        public void Dado_GetAllAvailableWebhooksOutput_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var output = new GetAllAvailableWebhooksOutput
            {
                Description = "Triggered when a new user is created",
                DisplayName = "User Created",
                Name = "App.UserCreated"
            };

            output.Description.ShouldBe("Triggered when a new user is created");
            output.DisplayName.ShouldBe("User Created");
            output.Name.ShouldBe("App.UserCreated");
        }
    }
}

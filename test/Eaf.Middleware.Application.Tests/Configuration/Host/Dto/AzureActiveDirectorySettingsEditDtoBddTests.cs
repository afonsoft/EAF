using Eaf.Middleware.Configuration.Host.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration.Host
{
    public class AzureActiveDirectorySettingsEditDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new AzureActiveDirectorySettingsEditDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirClientSecret_Entao_DeveArmazenar()
        {
            var sut = new AzureActiveDirectorySettingsEditDto();
            sut.ClientSecret = "test_value";
            sut.ClientSecret.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsEnabled_Entao_DeveArmazenar()
        {
            var sut = new AzureActiveDirectorySettingsEditDto();
            sut.IsEnabled = true;
            sut.IsEnabled.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsModuleEnabled_Entao_DeveArmazenar()
        {
            var sut = new AzureActiveDirectorySettingsEditDto();
            sut.IsModuleEnabled = true;
            sut.IsModuleEnabled.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTenant_Entao_DeveArmazenar()
        {
            var sut = new AzureActiveDirectorySettingsEditDto();
            sut.Tenant = "test_value";
            sut.Tenant.ShouldBe("test_value");
        }
    }
}

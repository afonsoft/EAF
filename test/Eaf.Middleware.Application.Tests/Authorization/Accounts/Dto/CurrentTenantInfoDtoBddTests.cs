using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Accounts
{
    public class CurrentTenantInfoDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new CurrentTenantInfoDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirName_Entao_DeveArmazenar()
        {
            var sut = new CurrentTenantInfoDto();
            sut.Name = "test_value";
            sut.Name.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTenancyName_Entao_DeveArmazenar()
        {
            var sut = new CurrentTenantInfoDto();
            sut.TenancyName = "test_value";
            sut.TenancyName.ShouldBe("test_value");
        }
    }
}

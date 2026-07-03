using Eaf.Middleware.MultiTenancy.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.MultiTenancy
{
    public class TenantListDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new TenantListDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsActive_Entao_DeveArmazenar()
        {
            var sut = new TenantListDto();
            sut.IsActive = true;
            sut.IsActive.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirName_Entao_DeveArmazenar()
        {
            var sut = new TenantListDto();
            sut.Name = "test_value";
            sut.Name.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTenancyName_Entao_DeveArmazenar()
        {
            var sut = new TenantListDto();
            sut.TenancyName = "test_value";
            sut.TenancyName.ShouldBe("test_value");
        }
    }
}

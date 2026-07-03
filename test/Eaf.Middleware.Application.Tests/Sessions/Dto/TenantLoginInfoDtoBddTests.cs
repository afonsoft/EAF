using Eaf.Middleware.Sessions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Sessions
{
    public class TenantLoginInfoDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new TenantLoginInfoDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirCreationTime_Entao_DeveArmazenar()
        {
            var sut = new TenantLoginInfoDto();
            var dt = System.DateTime.UtcNow; sut.CreationTime = dt;
            sut.CreationTime.ShouldBe(dt);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirName_Entao_DeveArmazenar()
        {
            var sut = new TenantLoginInfoDto();
            sut.Name = "test_value";
            sut.Name.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTenancyName_Entao_DeveArmazenar()
        {
            var sut = new TenantLoginInfoDto();
            sut.TenancyName = "test_value";
            sut.TenancyName.ShouldBe("test_value");
        }
    }
}

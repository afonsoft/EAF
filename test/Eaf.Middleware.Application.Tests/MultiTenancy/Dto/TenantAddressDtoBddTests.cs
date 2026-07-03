using Eaf.Middleware.MultiTenancy.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.MultiTenancy
{
    public class TenantAddressDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new TenantAddressDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirZipCode_Entao_DeveArmazenar()
        {
            var sut = new TenantAddressDto();
            sut.ZipCode = "test_value";
            sut.ZipCode.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirStreet_Entao_DeveArmazenar()
        {
            var sut = new TenantAddressDto();
            sut.Street = "test_value";
            sut.Street.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirNeighborhood_Entao_DeveArmazenar()
        {
            var sut = new TenantAddressDto();
            sut.Neighborhood = "test_value";
            sut.Neighborhood.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirCity_Entao_DeveArmazenar()
        {
            var sut = new TenantAddressDto();
            sut.City = "test_value";
            sut.City.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirState_Entao_DeveArmazenar()
        {
            var sut = new TenantAddressDto();
            sut.State = "test_value";
            sut.State.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirComplement_Entao_DeveArmazenar()
        {
            var sut = new TenantAddressDto();
            sut.Complement = "test_value";
            sut.Complement.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirObservation_Entao_DeveArmazenar()
        {
            var sut = new TenantAddressDto();
            sut.Observation = "test_value";
            sut.Observation.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirEmail_Entao_DeveArmazenar()
        {
            var sut = new TenantAddressDto();
            sut.Email = "test_value";
            sut.Email.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirDocument_Entao_DeveArmazenar()
        {
            var sut = new TenantAddressDto();
            sut.Document = "test_value";
            sut.Document.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTenantId_Entao_DeveArmazenar()
        {
            var sut = new TenantAddressDto();
            sut.TenantId = 42;
            sut.TenantId.ShouldBe(42);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirExtensionData_Entao_DeveArmazenar()
        {
            var sut = new TenantAddressDto();
            sut.ExtensionData = "test_value";
            sut.ExtensionData.ShouldBe("test_value");
        }
    }
}

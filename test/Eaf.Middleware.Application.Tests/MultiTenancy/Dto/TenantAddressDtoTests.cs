using Eaf.Middleware.MultiTenancy.Dto;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Application.Tests.MultiTenancy.Dto
{
    public class TenantAddressDtoTests
    {
        [Fact]
        public void Dado_TenantAddressDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new TenantAddressDto();

            dto.ZipCode.ShouldBeNull();
            dto.Street.ShouldBeNull();
            dto.Neighborhood.ShouldBeNull();
            dto.City.ShouldBeNull();
            dto.State.ShouldBeNull();
            dto.Complement.ShouldBeNull();
            dto.Observation.ShouldBeNull();
            dto.Email.ShouldBeNull();
            dto.Document.ShouldBeNull();
            dto.TenantId.ShouldBe(0);
            dto.ExtensionData.ShouldBeNull();
        }

        [Fact]
        public void Dado_TenantAddressDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var dto = new TenantAddressDto
            {
                ZipCode = "01001-000",
                Street = "Praça da Sé",
                Neighborhood = "Sé",
                City = "São Paulo",
                State = "SP",
                Complement = "lado ímpar",
                Observation = "Centro",
                Email = "tenant@test.com",
                Document = "12345678000195",
                TenantId = 1
            };

            dto.ZipCode.ShouldBe("01001-000");
            dto.Street.ShouldBe("Praça da Sé");
            dto.Neighborhood.ShouldBe("Sé");
            dto.City.ShouldBe("São Paulo");
            dto.State.ShouldBe("SP");
            dto.Complement.ShouldBe("lado ímpar");
            dto.Observation.ShouldBe("Centro");
            dto.Email.ShouldBe("tenant@test.com");
            dto.Document.ShouldBe("12345678000195");
            dto.TenantId.ShouldBe(1);
        }

        [Theory]
        [InlineData(nameof(TenantAddressDto.ZipCode))]
        [InlineData(nameof(TenantAddressDto.Street))]
        [InlineData(nameof(TenantAddressDto.Neighborhood))]
        [InlineData(nameof(TenantAddressDto.City))]
        [InlineData(nameof(TenantAddressDto.State))]
        public void Dado_TenantAddressDto_Quando_Verificado_Entao_PropriedadeDeveConterRequiredAttribute(string propertyName)
        {
            var prop = typeof(TenantAddressDto).GetProperty(propertyName);
            var attr = prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();
            attr.ShouldNotBeNull();
        }
    }
}

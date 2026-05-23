using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts.Dto
{
    public class CurrentTenantInfoDtoTests
    {
        [Fact]
        public void Dado_CurrentTenantInfoDto_Quando_Criado_Entao_PropriedadesDevemSerNulas()
        {
            var dto = new CurrentTenantInfoDto();
            dto.Name.ShouldBeNull();
            dto.TenancyName.ShouldBeNull();
        }

        [Fact]
        public void Dado_CurrentTenantInfoDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var dto = new CurrentTenantInfoDto
            {
                Name = "Test Tenant",
                TenancyName = "TestTenant"
            };

            dto.Name.ShouldBe("Test Tenant");
            dto.TenancyName.ShouldBe("TestTenant");
        }
    }
}

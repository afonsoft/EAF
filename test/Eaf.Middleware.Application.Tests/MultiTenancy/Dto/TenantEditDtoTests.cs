using Eaf.Middleware.MultiTenancy.Dto;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Application.Tests.MultiTenancy.Dto
{
    public class TenantEditDtoTests
    {
        [Fact]
        public void Dado_TenantEditDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new TenantEditDto();

            dto.IsActive.ShouldBeFalse();
            dto.Name.ShouldBeNull();
            dto.TenancyName.ShouldBeNull();
        }

        [Fact]
        public void Dado_TenantEditDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var dto = new TenantEditDto
            {
                IsActive = true,
                Name = "Test Tenant",
                TenancyName = "TestTenant"
            };

            dto.IsActive.ShouldBeTrue();
            dto.Name.ShouldBe("Test Tenant");
            dto.TenancyName.ShouldBe("TestTenant");
        }

        [Fact]
        public void Dado_TenantEditDto_Quando_Verificado_Entao_NameDeveConterRequiredAttribute()
        {
            var prop = typeof(TenantEditDto).GetProperty(nameof(TenantEditDto.Name));
            var attr = prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();
            attr.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_TenantEditDto_Quando_Verificado_Entao_TenancyNameDeveConterRequiredAttribute()
        {
            var prop = typeof(TenantEditDto).GetProperty(nameof(TenantEditDto.TenancyName));
            var attr = prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();
            attr.ShouldNotBeNull();
        }
    }
}

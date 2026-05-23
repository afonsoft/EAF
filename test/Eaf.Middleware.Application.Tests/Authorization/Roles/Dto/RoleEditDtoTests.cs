using Eaf.Middleware.Authorization.Roles.Dto;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Roles.Dto
{
    public class RoleEditDtoTests
    {
        [Fact]
        public void Dado_RoleEditDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new RoleEditDto();

            dto.DisplayName.ShouldBeNull();
            dto.Id.ShouldBeNull();
            dto.IsDefault.ShouldBeFalse();
        }

        [Fact]
        public void Dado_RoleEditDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var dto = new RoleEditDto
            {
                DisplayName = "Admin",
                Id = 1,
                IsDefault = true
            };

            dto.DisplayName.ShouldBe("Admin");
            dto.Id.ShouldBe(1);
            dto.IsDefault.ShouldBeTrue();
        }

        [Fact]
        public void Dado_RoleEditDto_Quando_Verificado_Entao_DisplayNameDeveConterRequiredAttribute()
        {
            var prop = typeof(RoleEditDto).GetProperty(nameof(RoleEditDto.DisplayName));
            var attr = prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();
            attr.ShouldNotBeNull();
        }
    }
}

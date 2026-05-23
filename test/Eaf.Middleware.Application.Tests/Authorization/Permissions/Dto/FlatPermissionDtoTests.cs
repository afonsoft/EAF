using Eaf.Middleware.Authorization.Permissions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Permissions.Dto
{
    public class FlatPermissionDtoTests
    {
        [Fact]
        public void Dado_FlatPermissionDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new FlatPermissionDto();

            dto.Description.ShouldBe("");
            dto.DisplayName.ShouldBeNull();
            dto.IsGrantedByDefault.ShouldBeFalse();
            dto.Name.ShouldBeNull();
            dto.ParentName.ShouldBeNull();
        }

        [Fact]
        public void Dado_FlatPermissionDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var dto = new FlatPermissionDto
            {
                Description = "Admin permission",
                DisplayName = "Administration",
                IsGrantedByDefault = true,
                Name = "Pages.Admin",
                ParentName = "Pages"
            };

            dto.Description.ShouldBe("Admin permission");
            dto.DisplayName.ShouldBe("Administration");
            dto.IsGrantedByDefault.ShouldBeTrue();
            dto.Name.ShouldBe("Pages.Admin");
            dto.ParentName.ShouldBe("Pages");
        }
    }
}

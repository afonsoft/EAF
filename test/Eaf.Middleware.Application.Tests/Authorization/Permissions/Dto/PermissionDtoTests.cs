using Eaf.Middleware.Authorization.Permissions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Permissions.Dto
{
    public class PermissionDtoTests
    {
        [Fact]
        public void Dado_FlatPermissionDto_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var dto = new FlatPermissionDto
            {
                Name = "Pages.Administration.Users",
                DisplayName = "User Management",
                Description = "Manage users",
                IsGrantedByDefault = false,
                ParentName = "Pages.Administration"
            };

            dto.Name.ShouldBe("Pages.Administration.Users");
            dto.DisplayName.ShouldBe("User Management");
            dto.Description.ShouldBe("Manage users");
            dto.IsGrantedByDefault.ShouldBeFalse();
            dto.ParentName.ShouldBe("Pages.Administration");
        }

        [Fact]
        public void Dado_FlatPermissionDto_Quando_ValoresPadrao_Entao_DeveSerCorreto()
        {
            var dto = new FlatPermissionDto();
            dto.Description.ShouldBe("");
            dto.ParentName.ShouldBeNull();
            dto.IsGrantedByDefault.ShouldBeFalse();
        }
    }
}

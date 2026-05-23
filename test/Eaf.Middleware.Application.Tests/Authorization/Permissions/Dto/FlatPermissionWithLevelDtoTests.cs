using Eaf.Middleware.Authorization.Permissions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Permissions.Dto
{
    public class FlatPermissionWithLevelDtoTests
    {
        [Fact]
        public void Dado_FlatPermissionWithLevelDto_Quando_Criado_Entao_LevelDeveSerZero()
        {
            var dto = new FlatPermissionWithLevelDto();
            dto.Level.ShouldBe(0);
        }

        [Fact]
        public void Dado_FlatPermissionWithLevelDto_Quando_AtribuirLevel_Entao_DeveRetornarValor()
        {
            var dto = new FlatPermissionWithLevelDto { Level = 3 };
            dto.Level.ShouldBe(3);
        }

        [Fact]
        public void Dado_FlatPermissionWithLevelDto_Quando_Verificado_Entao_DeveHerdarFlatPermissionDto()
        {
            var dto = new FlatPermissionWithLevelDto
            {
                Name = "Pages.Admin",
                Level = 2
            };

            dto.ShouldBeAssignableTo<FlatPermissionDto>();
            dto.Name.ShouldBe("Pages.Admin");
            dto.Level.ShouldBe(2);
        }
    }
}

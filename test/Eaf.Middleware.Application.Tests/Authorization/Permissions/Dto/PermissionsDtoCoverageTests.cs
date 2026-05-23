using Eaf.Middleware.Authorization.Permissions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Permissions.Dto
{
    public class PermissionsDtoCoverageTests
    {
        [Fact]
        public void FlatPermissionDto_Defaults()
        {
            var dto = new FlatPermissionDto();
            dto.Description.ShouldBe("");
            dto.ParentName.ShouldBeNull();
            dto.IsGrantedByDefault.ShouldBeFalse();
        }

        [Fact]
        public void FlatPermissionDto_ShouldSetAll()
        {
            var dto = new FlatPermissionDto
            {
                Description = "desc",
                DisplayName = "dn",
                IsGrantedByDefault = true,
                Name = "n",
                ParentName = "p"
            };
            dto.Description.ShouldBe("desc");
            dto.DisplayName.ShouldBe("dn");
            dto.IsGrantedByDefault.ShouldBeTrue();
            dto.Name.ShouldBe("n");
            dto.ParentName.ShouldBe("p");
        }

        [Fact]
        public void FlatPermissionWithLevelDto_ShouldInheritAndSetLevel()
        {
            var dto = new FlatPermissionWithLevelDto { Name = "n", Level = 2 };
            dto.Name.ShouldBe("n");
            dto.Level.ShouldBe(2);
            dto.ShouldBeAssignableTo<FlatPermissionDto>();
        }
    }
}

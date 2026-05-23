using Eaf.Middleware.Authorization.Permissions.Dto;
using Eaf.Middleware.Authorization.Roles.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Roles.Dto
{
    public class RolesDtoCoverageTests
    {
        [Fact]
        public void CreateOrUpdateRoleInput_ShouldSet()
        {
            var dto = new CreateOrUpdateRoleInput
            {
                GrantedPermissionNames = new List<string> { "A" },
                Role = new RoleEditDto { DisplayName = "r" }
            };
            dto.GrantedPermissionNames.ShouldContain("A");
            dto.Role.DisplayName.ShouldBe("r");
        }

        [Fact]
        public void GetRoleForEditOutput_ShouldSet()
        {
            var dto = new GetRoleForEditOutput
            {
                GrantedPermissionNames = new List<string> { "a" },
                Permissions = new List<FlatPermissionDto> { new() { Name = "p" } },
                Role = new RoleEditDto()
            };
            dto.GrantedPermissionNames.Count.ShouldBe(1);
            dto.Permissions.Count.ShouldBe(1);
            dto.Role.ShouldNotBeNull();
        }

        [Fact]
        public void GetRolesInput_NormalizeEmptySorting_ShouldDefault()
        {
            var dto = new GetRolesInput();
            dto.Filter.ShouldBe("");
            dto.Normalize();
            dto.Sorting.ShouldBe("Name");
        }

        [Fact]
        public void GetRolesInput_NormalizeWithExistingSorting_ShouldKeep()
        {
            var dto = new GetRolesInput { Sorting = "DisplayName" };
            dto.Normalize();
            dto.Sorting.ShouldBe("DisplayName");
        }

        [Fact]
        public void RoleEditDto_ShouldSet()
        {
            var dto = new RoleEditDto { DisplayName = "Admin", Id = 1, IsDefault = true };
            dto.DisplayName.ShouldBe("Admin");
            dto.Id.ShouldBe(1);
            dto.IsDefault.ShouldBeTrue();
        }

        [Fact]
        public void RoleListDto_LastModificationDate_WhenLastModificationNull_UsesCreationTime()
        {
            var dto = new RoleListDto
            {
                DisplayName = "Admin",
                IsDefault = true,
                IsStatic = false,
                Name = "Admin",
                CreationTime = new DateTime(2020, 1, 1),
                LastModificationTime = null
            };
            dto.LastModificationDate.ShouldBe(new DateTime(2020, 1, 1));
        }

        [Fact]
        public void RoleListDto_LastModificationDate_UsesLastModificationWhenSet()
        {
            var last = new DateTime(2022, 5, 5);
            var dto = new RoleListDto
            {
                Name = "n",
                CreationTime = new DateTime(2020, 1, 1),
                LastModificationTime = last
            };
            dto.LastModificationDate.ShouldBe(last);
        }
    }
}

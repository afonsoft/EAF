using Eaf.Middleware.Authorization.Roles.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Roles.Dto
{
    public class RoleDtoTests
    {
        [Fact]
        public void Dado_RoleListDto_Quando_SemLastModificationTime_Entao_LastModificationDateDeveRetornarCreationTime()
        {
            var creationTime = new DateTime(2024, 1, 15, 10, 0, 0);
            var dto = new RoleListDto
            {
                CreationTime = creationTime,
                LastModificationTime = null
            };

            dto.LastModificationDate.ShouldBe(creationTime);
        }

        [Fact]
        public void Dado_RoleListDto_Quando_ComLastModificationTime_Entao_LastModificationDateDeveRetornarLastModificationTime()
        {
            var creationTime = new DateTime(2024, 1, 15, 10, 0, 0);
            var modificationTime = new DateTime(2024, 2, 20, 14, 30, 0);
            var dto = new RoleListDto
            {
                CreationTime = creationTime,
                LastModificationTime = modificationTime
            };

            dto.LastModificationDate.ShouldBe(modificationTime);
        }

        [Fact]
        public void Dado_RoleListDto_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var dto = new RoleListDto
            {
                Id = 1,
                Name = "Admin",
                DisplayName = "Administrator",
                IsStatic = true,
                IsDefault = false
            };

            dto.Id.ShouldBe(1);
            dto.Name.ShouldBe("Admin");
            dto.DisplayName.ShouldBe("Administrator");
            dto.IsStatic.ShouldBeTrue();
            dto.IsDefault.ShouldBeFalse();
        }

        [Fact]
        public void Dado_RoleEditDto_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var dto = new RoleEditDto
            {
                Id = 5,
                DisplayName = "Manager",
                IsDefault = true
            };

            dto.Id.ShouldBe(5);
            dto.DisplayName.ShouldBe("Manager");
            dto.IsDefault.ShouldBeTrue();
        }

        [Fact]
        public void Dado_RoleEditDto_Quando_IdNull_Entao_DevePermitir()
        {
            var dto = new RoleEditDto { DisplayName = "NewRole" };
            dto.Id.ShouldBeNull();
        }
    }
}

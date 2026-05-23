using Eaf.Middleware.Authorization.Roles.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Roles.Dto
{
    public class RoleListDtoTests
    {
        [Fact]
        public void Dado_RoleListDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new RoleListDto();

            dto.DisplayName.ShouldBeNull();
            dto.IsDefault.ShouldBeFalse();
            dto.IsStatic.ShouldBeFalse();
            dto.Name.ShouldBeNull();
        }

        [Fact]
        public void Dado_RoleListDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var dto = new RoleListDto
            {
                DisplayName = "Administrator",
                IsDefault = true,
                IsStatic = true,
                Name = "Admin"
            };

            dto.DisplayName.ShouldBe("Administrator");
            dto.IsDefault.ShouldBeTrue();
            dto.IsStatic.ShouldBeTrue();
            dto.Name.ShouldBe("Admin");
        }

        [Fact]
        public void Dado_RoleListDto_Quando_SemLastModification_Entao_LastModificationDateDeveSerCreationTime()
        {
            var creationTime = new DateTime(2024, 1, 1, 10, 0, 0);
            var dto = new RoleListDto();
            dto.CreationTime = creationTime;

            dto.LastModificationDate.ShouldBe(creationTime);
        }

        [Fact]
        public void Dado_RoleListDto_Quando_ComLastModification_Entao_LastModificationDateDeveSerLastModificationTime()
        {
            var creationTime = new DateTime(2024, 1, 1, 10, 0, 0);
            var lastModTime = new DateTime(2024, 6, 15, 14, 30, 0);
            var dto = new RoleListDto();
            dto.CreationTime = creationTime;
            dto.LastModificationTime = lastModTime;

            dto.LastModificationDate.ShouldBe(lastModTime);
        }
    }
}

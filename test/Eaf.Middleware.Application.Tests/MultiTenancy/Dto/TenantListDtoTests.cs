using Eaf.Middleware.MultiTenancy.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.MultiTenancy.Dto
{
    public class TenantListDtoTests
    {
        [Fact]
        public void Dado_TenantListDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new TenantListDto();

            dto.IsActive.ShouldBeFalse();
            dto.Name.ShouldBeNull();
            dto.TenancyName.ShouldBeNull();
        }

        [Fact]
        public void Dado_TenantListDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var dto = new TenantListDto
            {
                IsActive = true,
                Name = "Default Tenant",
                TenancyName = "Default"
            };

            dto.IsActive.ShouldBeTrue();
            dto.Name.ShouldBe("Default Tenant");
            dto.TenancyName.ShouldBe("Default");
        }

        [Fact]
        public void Dado_TenantListDto_Quando_SemLastModification_Entao_LastModificationDateDeveSerCreationTime()
        {
            var creationTime = new DateTime(2024, 3, 1, 8, 0, 0);
            var dto = new TenantListDto();
            dto.CreationTime = creationTime;

            dto.LastModificationDate.ShouldBe(creationTime);
        }

        [Fact]
        public void Dado_TenantListDto_Quando_ComLastModification_Entao_LastModificationDateDeveSerLastModificationTime()
        {
            var creationTime = new DateTime(2024, 3, 1, 8, 0, 0);
            var lastModTime = new DateTime(2024, 9, 20, 16, 45, 0);
            var dto = new TenantListDto();
            dto.CreationTime = creationTime;
            dto.LastModificationTime = lastModTime;

            dto.LastModificationDate.ShouldBe(lastModTime);
        }
    }
}

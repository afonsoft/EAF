using Abp.Events.Bus.Entities;
using Eaf.Middleware.Auditing.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Auditing.Dto
{
    public class EntityChangeDtoTests
    {
        [Fact]
        public void Dado_EntityChangeDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new EntityChangeDto();

            dto.ChangeTime.ShouldBe(default(DateTime));
            dto.ChangeType.ShouldBe(default(EntityChangeType));
            dto.EntityChangeSetId.ShouldBe(0L);
            dto.EntityEntry.ShouldBeNull();
            dto.EntityId.ShouldBeNull();
            dto.EntityTypeFullName.ShouldBeNull();
            dto.TenantId.ShouldBeNull();
        }

        [Fact]
        public void Dado_EntityChangeDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var now = DateTime.UtcNow;
            var dto = new EntityChangeDto
            {
                ChangeTime = now,
                ChangeType = EntityChangeType.Updated,
                EntityChangeSetId = 42L,
                EntityEntry = new object(),
                EntityId = "123",
                EntityTypeFullName = "Eaf.Middleware.Entity",
                TenantId = 5
            };

            dto.ChangeTime.ShouldBe(now);
            dto.ChangeType.ShouldBe(EntityChangeType.Updated);
            dto.EntityChangeSetId.ShouldBe(42L);
            dto.EntityEntry.ShouldNotBeNull();
            dto.EntityId.ShouldBe("123");
            dto.EntityTypeFullName.ShouldBe("Eaf.Middleware.Entity");
            dto.TenantId.ShouldBe(5);
        }
    }
}

using Abp.Events.Bus.Entities;
using Eaf.Middleware.Auditing.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Auditing.Dto
{
    public class EntityChangeListDtoBddTests
    {
        [Fact]
        public void Dado_EntityChangeListDto_Quando_DefinirPropriedades_Entao_DevePersistir()
        {
            var now = DateTime.UtcNow;
            var dto = new EntityChangeListDto
            {
                Id = 42,
                ChangeTime = now,
                ChangeType = EntityChangeType.Created,
                EntityChangeSetId = 100,
                EntityTypeFullName = "Eaf.Middleware.MultiTenancy.Tenant",
                UserId = 1,
                UserName = "admin"
            };

            dto.Id.ShouldBe(42);
            dto.ChangeTime.ShouldBe(now);
            dto.ChangeType.ShouldBe(EntityChangeType.Created);
            dto.EntityChangeSetId.ShouldBe(100);
            dto.EntityTypeFullName.ShouldBe("Eaf.Middleware.MultiTenancy.Tenant");
            dto.UserId.ShouldBe(1);
            dto.UserName.ShouldBe("admin");
        }

        [Theory]
        [InlineData(EntityChangeType.Created, "Created")]
        [InlineData(EntityChangeType.Updated, "Updated")]
        [InlineData(EntityChangeType.Deleted, "Deleted")]
        public void Dado_EntityChangeListDto_Quando_ChangeTypeName_Entao_DeveRetornarNomeDoEnum(
            EntityChangeType changeType, string expectedName)
        {
            var dto = new EntityChangeListDto { ChangeType = changeType };

            dto.ChangeTypeName.ShouldBe(expectedName);
        }

        [Fact]
        public void Dado_EntityChangeListDto_SemUserId_Quando_Verificar_Entao_UserIdDeveSerNull()
        {
            var dto = new EntityChangeListDto();

            dto.UserId.ShouldBeNull();
        }
    }
}

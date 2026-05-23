using Eaf.Middleware.Auditing.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Auditing.Dto
{
    public class EntityPropertyChangeDtoTests
    {
        [Fact]
        public void Dado_EntityPropertyChangeDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new EntityPropertyChangeDto();

            dto.EntityChangeId.ShouldBe(0L);
            dto.NewValue.ShouldBeNull();
            dto.OriginalValue.ShouldBeNull();
            dto.PropertyName.ShouldBeNull();
            dto.PropertyTypeFullName.ShouldBeNull();
            dto.TenantId.ShouldBeNull();
        }

        [Fact]
        public void Dado_EntityPropertyChangeDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var dto = new EntityPropertyChangeDto
            {
                EntityChangeId = 10L,
                NewValue = "new",
                OriginalValue = "old",
                PropertyName = "Name",
                PropertyTypeFullName = "System.String",
                TenantId = 3
            };

            dto.EntityChangeId.ShouldBe(10L);
            dto.NewValue.ShouldBe("new");
            dto.OriginalValue.ShouldBe("old");
            dto.PropertyName.ShouldBe("Name");
            dto.PropertyTypeFullName.ShouldBe("System.String");
            dto.TenantId.ShouldBe(3);
        }
    }
}

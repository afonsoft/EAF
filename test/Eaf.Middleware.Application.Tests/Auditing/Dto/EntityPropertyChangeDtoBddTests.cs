using Eaf.Middleware.Auditing.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Auditing
{
    public class EntityPropertyChangeDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new EntityPropertyChangeDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirEntityChangeId_Entao_DeveArmazenar()
        {
            var sut = new EntityPropertyChangeDto();
            sut.EntityChangeId = 100L;
            sut.EntityChangeId.ShouldBe(100L);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirNewValue_Entao_DeveArmazenar()
        {
            var sut = new EntityPropertyChangeDto();
            sut.NewValue = "test_value";
            sut.NewValue.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirOriginalValue_Entao_DeveArmazenar()
        {
            var sut = new EntityPropertyChangeDto();
            sut.OriginalValue = "test_value";
            sut.OriginalValue.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirPropertyName_Entao_DeveArmazenar()
        {
            var sut = new EntityPropertyChangeDto();
            sut.PropertyName = "test_value";
            sut.PropertyName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirPropertyTypeFullName_Entao_DeveArmazenar()
        {
            var sut = new EntityPropertyChangeDto();
            sut.PropertyTypeFullName = "test_value";
            sut.PropertyTypeFullName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTenantId_Entao_DeveArmazenar()
        {
            var sut = new EntityPropertyChangeDto();
            sut.TenantId = 42;
            sut.TenantId.ShouldBe(42);
        }
    }
}

using Eaf.Middleware.Auditing.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Auditing
{
    public class GetEntityChangeInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetEntityChangeInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirEndDate_Entao_DeveArmazenar()
        {
            var sut = new GetEntityChangeInput();
            var dt = System.DateTime.UtcNow;
            sut.EndDate = dt;
            sut.EndDate.ShouldBe(dt);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirEntityTypeFullName_Entao_DeveArmazenar()
        {
            var sut = new GetEntityChangeInput();
            sut.EntityTypeFullName = "test_value";
            sut.EntityTypeFullName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirStartDate_Entao_DeveArmazenar()
        {
            var sut = new GetEntityChangeInput();
            var dt = System.DateTime.UtcNow;
            sut.StartDate = dt;
            sut.StartDate.ShouldBe(dt);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUserName_Entao_DeveArmazenar()
        {
            var sut = new GetEntityChangeInput();
            sut.UserName = "test_value";
            sut.UserName.ShouldBe("test_value");
        }
    }
}

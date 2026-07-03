using Eaf.Middleware.Auditing;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Auditing
{
    public class EntityChangeAndUserBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new EntityChangeAndUser();
            sut.ShouldNotBeNull();
        }
    }
}

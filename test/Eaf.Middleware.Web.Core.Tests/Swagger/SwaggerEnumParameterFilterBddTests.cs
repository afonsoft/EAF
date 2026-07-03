using Eaf.Middleware.Web.Swagger;
using Shouldly;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Swagger
{
    public class SwaggerEnumParameterFilterBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new SwaggerEnumParameterFilter();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_VerificarInterface_Entao_DeveImplementarIParameterFilter()
        {
            var sut = new SwaggerEnumParameterFilter();
            sut.ShouldBeAssignableTo<IParameterFilter>();
        }
    }
}

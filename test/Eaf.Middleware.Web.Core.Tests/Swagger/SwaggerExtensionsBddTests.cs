using Eaf.Middleware.Web.Swagger;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Swagger
{
    public class SwaggerExtensionsBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(SwaggerExtensions).IsAbstract.ShouldBeTrue();
            typeof(SwaggerExtensions).IsSealed.ShouldBeTrue();
        }
    }
}

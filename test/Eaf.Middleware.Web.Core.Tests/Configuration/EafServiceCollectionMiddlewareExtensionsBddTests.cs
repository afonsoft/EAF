using Eaf.Middleware.Web.Startup;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Configuration
{
    public class EafServiceCollectionMiddlewareExtensionsBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(EafServiceCollectionMiddlewareExtensions).IsAbstract.ShouldBeTrue();
            typeof(EafServiceCollectionMiddlewareExtensions).IsSealed.ShouldBeTrue();
        }
    }
}

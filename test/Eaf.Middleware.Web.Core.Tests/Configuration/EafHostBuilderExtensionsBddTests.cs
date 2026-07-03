using Eaf.Middleware.Web.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Configuration
{
    public class EafHostBuilderExtensionsBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(EafHostBuilderExtensions).IsAbstract.ShouldBeTrue();
            typeof(EafHostBuilderExtensions).IsSealed.ShouldBeTrue();
        }
    }
}

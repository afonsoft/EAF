using Eaf.Middleware.Web.Serilog;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Serilog
{
    public class SerilogEafHostBuilderExtensionsBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(SerilogEafHostBuilderExtensions).IsAbstract.ShouldBeTrue();
            typeof(SerilogEafHostBuilderExtensions).IsSealed.ShouldBeTrue();
        }
    }
}

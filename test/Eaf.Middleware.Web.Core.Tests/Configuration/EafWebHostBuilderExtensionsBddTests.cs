using Eaf.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Configuration
{
    public class EafWebHostBuilderExtensionsBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(EafWebHostBuilderExtensions).IsAbstract.ShouldBeTrue();
            typeof(EafWebHostBuilderExtensions).IsSealed.ShouldBeTrue();
        }
    }
}

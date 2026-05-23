using Eaf.Middleware.Auditing;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Auditing
{
    public class NamespaceStripperTests
    {
        private readonly NamespaceStripper _stripper;

        public NamespaceStripperTests()
        {
            _stripper = new NamespaceStripper();
        }

        [Fact]
        public void Dado_StringVazia_Quando_StripNameSpace_Entao_DeveRetornarStringVazia()
        {
            _stripper.StripNameSpace("").ShouldBe("");
        }

        [Fact]
        public void Dado_StringNull_Quando_StripNameSpace_Entao_DeveRetornarNull()
        {
            _stripper.StripNameSpace(null).ShouldBeNull();
        }

        [Fact]
        public void Dado_StringSemPonto_Quando_StripNameSpace_Entao_DeveRetornarMesmaString()
        {
            _stripper.StripNameSpace("MyService").ShouldBe("MyService");
        }

        [Fact]
        public void Dado_StringComNamespace_Quando_StripNameSpace_Entao_DeveRetornarApenasNomeClasse()
        {
            _stripper.StripNameSpace("Eaf.Middleware.Services.MyService")
                .ShouldBe("MyService");
        }

        [Fact]
        public void Dado_StringComUmPonto_Quando_StripNameSpace_Entao_DeveRetornarTextoPosUltimoPonto()
        {
            _stripper.StripNameSpace("Services.MyService")
                .ShouldBe("MyService");
        }
    }
}

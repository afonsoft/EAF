using Eaf.Middleware.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application
{
    public class FileDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new FileDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirFileName_Entao_DeveArmazenar()
        {
            var sut = new FileDto();
            sut.FileName = "test_value";
            sut.FileName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirFileToken_Entao_DeveArmazenar()
        {
            var sut = new FileDto();
            sut.FileToken = "test_value";
            sut.FileToken.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirFileType_Entao_DeveArmazenar()
        {
            var sut = new FileDto();
            sut.FileType = "test_value";
            sut.FileType.ShouldBe("test_value");
        }
    }
}

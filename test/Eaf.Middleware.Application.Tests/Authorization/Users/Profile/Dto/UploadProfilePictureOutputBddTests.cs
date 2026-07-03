using Eaf.Middleware.Authorization.Users.Profile.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Users.Profile
{
    public class UploadProfilePictureOutputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new UploadProfilePictureOutput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirFileName_Entao_DeveArmazenar()
        {
            var sut = new UploadProfilePictureOutput();
            sut.FileName = "test_value";
            sut.FileName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirFileToken_Entao_DeveArmazenar()
        {
            var sut = new UploadProfilePictureOutput();
            sut.FileToken = "test_value";
            sut.FileToken.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirFileType_Entao_DeveArmazenar()
        {
            var sut = new UploadProfilePictureOutput();
            sut.FileType = "test_value";
            sut.FileType.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirHeight_Entao_DeveArmazenar()
        {
            var sut = new UploadProfilePictureOutput();
            sut.Height = 42;
            sut.Height.ShouldBe(42);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirWidth_Entao_DeveArmazenar()
        {
            var sut = new UploadProfilePictureOutput();
            sut.Width = 42;
            sut.Width.ShouldBe(42);
        }
    }
}

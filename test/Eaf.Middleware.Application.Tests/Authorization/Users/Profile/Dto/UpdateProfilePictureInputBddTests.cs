using Eaf.Middleware.Authorization.Users.Profile.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Users.Profile
{
    public class UpdateProfilePictureInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new UpdateProfilePictureInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirHeight_Entao_DeveArmazenar()
        {
            var sut = new UpdateProfilePictureInput();
            sut.Height = 42;
            sut.Height.ShouldBe(42);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirWidth_Entao_DeveArmazenar()
        {
            var sut = new UpdateProfilePictureInput();
            sut.Width = 42;
            sut.Width.ShouldBe(42);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirX_Entao_DeveArmazenar()
        {
            var sut = new UpdateProfilePictureInput();
            sut.X = 42;
            sut.X.ShouldBe(42);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirY_Entao_DeveArmazenar()
        {
            var sut = new UpdateProfilePictureInput();
            sut.Y = 42;
            sut.Y.ShouldBe(42);
        }
    }
}

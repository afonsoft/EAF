using Eaf.Middleware.Sessions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Sessions
{
    public class UpdateUserSignInTokenOutputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new UpdateUserSignInTokenOutput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirEncodedUserId_Entao_DeveArmazenar()
        {
            var sut = new UpdateUserSignInTokenOutput();
            sut.EncodedUserId = "test_value";
            sut.EncodedUserId.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSignInToken_Entao_DeveArmazenar()
        {
            var sut = new UpdateUserSignInTokenOutput();
            sut.SignInToken = "test_value";
            sut.SignInToken.ShouldBe("test_value");
        }
    }
}

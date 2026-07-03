using Eaf.Middleware.Authorization.Users.Profile.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Users.Profile
{
    public class GetProfilePictureOutputBddTests
    {
        [Fact]
        public void Dado_ProfilePicture_Quando_CriarComParametro_Entao_DeveArmazenar()
        {
            var sut = new GetProfilePictureOutput("base64_picture_data");
            sut.ProfilePicture.ShouldBe("base64_picture_data");
        }

        [Fact]
        public void Dado_ProfilePictureNulo_Quando_CriarComNull_Entao_DevePermitir()
        {
            var sut = new GetProfilePictureOutput(null);
            sut.ProfilePicture.ShouldBeNull();
        }
    }
}

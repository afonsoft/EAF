using Abp.Web.Models;
using Eaf.Middleware.Authorization.Users.Profile.Dto;
using Eaf.Middleware.Security;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Profile.Dto
{
    public class ProfileDtoBddExtendedTests
    {
        [Fact]
        public void Dado_UpdateProfilePictureInput_Quando_DefinirPropriedades_Entao_DevePersistir()
        {
            var input = new UpdateProfilePictureInput
            {
                FileToken = "token-abc-123",
                X = 10,
                Y = 20,
                Width = 200,
                Height = 200
            };

            input.FileToken.ShouldBe("token-abc-123");
            input.X.ShouldBe(10);
            input.Y.ShouldBe(20);
            input.Width.ShouldBe(200);
            input.Height.ShouldBe(200);
        }

        [Fact]
        public void Dado_UploadProfilePictureOutput_Quando_CriarSemParametros_Entao_DeveInicializarVazio()
        {
            var output = new UploadProfilePictureOutput();

            output.FileName.ShouldBeNull();
            output.FileToken.ShouldBeNull();
            output.FileType.ShouldBeNull();
        }

        [Fact]
        public void Dado_UploadProfilePictureOutput_Quando_CriarComErrorInfo_Entao_DeveColocarMensagemDeErro()
        {
            var error = new ErrorInfo(400, "Arquivo inválido")
            {
                Details = "Formato não suportado"
            };

            var output = new UploadProfilePictureOutput(error);

            output.Code.ShouldBe(400);
            output.Message.ShouldBe("Arquivo inválido");
            output.Details.ShouldBe("Formato não suportado");
        }

        [Fact]
        public void Dado_UploadProfilePictureOutput_Quando_DefinirPropriedades_Entao_DevePersistir()
        {
            var output = new UploadProfilePictureOutput
            {
                FileName = "foto.jpg",
                FileToken = "upload-token-xyz",
                FileType = "image/jpeg",
                Width = 640,
                Height = 480
            };

            output.FileName.ShouldBe("foto.jpg");
            output.FileToken.ShouldBe("upload-token-xyz");
            output.FileType.ShouldBe("image/jpeg");
            output.Width.ShouldBe(640);
            output.Height.ShouldBe(480);
        }

        [Fact]
        public void Dado_GetProfilePictureOutput_Quando_CriarComPicture_Entao_DeveDefinirProfilePicture()
        {
            var output = new GetProfilePictureOutput("base64-image-data");

            output.ProfilePicture.ShouldBe("base64-image-data");
        }

        [Fact]
        public void Dado_GetPasswordComplexitySettingOutput_Quando_DefinirSetting_Entao_DevePersistir()
        {
            var output = new GetPasswordComplexitySettingOutput
            {
                Setting = new PasswordComplexitySetting
                {
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireUppercase = true,
                    RequireNonAlphanumeric = true,
                    RequiredLength = 8
                }
            };

            output.Setting.ShouldNotBeNull();
            output.Setting.RequireDigit.ShouldBeTrue();
            output.Setting.RequiredLength.ShouldBe(8);
        }
    }
}

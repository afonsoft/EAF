using Abp.Web.Models;
using Eaf.Middleware.Authorization.Users.Profile.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.Users.Profile.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de Perfil seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ProfileDtoTests
    {
        #region ChangePasswordInput

        [Fact]
        public void Dado_ChangePasswordInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var input = new ChangePasswordInput
            {
                CurrentPassword = "OldP@ss",
                NewPassword = "NewP@ss123"
            };

            // Então
            input.CurrentPassword.ShouldBe("OldP@ss");
            input.NewPassword.ShouldBe("NewP@ss123");
        }

        #endregion

        #region CurrentUserProfileEditDto

        [Fact]
        public void Dado_CurrentUserProfileEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var dto = new CurrentUserProfileEditDto
            {
                Name = "João",
                Surname = "Silva",
                EmailAddress = "joao@acme.com",
                UserName = "joao.silva",
                Timezone = "America/Sao_Paulo"
            };

            // Então
            dto.Name.ShouldBe("João");
            dto.Surname.ShouldBe("Silva");
            dto.EmailAddress.ShouldBe("joao@acme.com");
            dto.UserName.ShouldBe("joao.silva");
            dto.Timezone.ShouldBe("America/Sao_Paulo");
        }

        #endregion

        #region GetPasswordComplexitySettingOutput

        [Fact]
        public void Dado_GetPasswordComplexitySettingOutput_Quando_DefinirSetting_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var output = new GetPasswordComplexitySettingOutput
            {
                Setting = new Eaf.Middleware.Security.PasswordComplexitySetting
                {
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireUppercase = true,
                    RequireNonAlphanumeric = true,
                    RequiredLength = 8
                }
            };

            // Então
            output.Setting.ShouldNotBeNull();
            output.Setting.RequireDigit.ShouldBeTrue();
            output.Setting.RequiredLength.ShouldBe(8);
        }

        #endregion

        #region GetProfilePictureOutput

        [Fact]
        public void Dado_GetProfilePictureOutput_Quando_CriarComBase64_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var output = new GetProfilePictureOutput("base64data==");

            // Então
            output.ProfilePicture.ShouldBe("base64data==");
        }

        #endregion

        #region UpdateProfilePictureInput

        [Fact]
        public void Dado_UpdateProfilePictureInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var input = new UpdateProfilePictureInput
            {
                FileToken = "token-abc",
                X = 10,
                Y = 20,
                Width = 200,
                Height = 200
            };

            // Então
            input.FileToken.ShouldBe("token-abc");
            input.X.ShouldBe(10);
            input.Y.ShouldBe(20);
            input.Width.ShouldBe(200);
            input.Height.ShouldBe(200);
        }

        #endregion

        #region UploadProfilePictureOutput

        [Fact]
        public void Dado_UploadProfilePictureOutputPadrao_Quando_Criar_Entao_DeveInicializar()
        {
            // Dado & Quando
            var output = new UploadProfilePictureOutput();

            // Então
            output.FileName.ShouldBeNull();
            output.FileToken.ShouldBeNull();
        }

        [Fact]
        public void Dado_UploadProfilePictureOutput_Quando_CriarComErro_Entao_DeveMapearErro()
        {
            // Dado
            var error = new ErrorInfo(500, "Arquivo inválido")
            {
                Details = "O arquivo excede o tamanho máximo"
            };

            // Quando
            var output = new UploadProfilePictureOutput(error);

            // Então
            output.Code.ShouldBe(500);
            output.Message.ShouldBe("Arquivo inválido");
            output.Details.ShouldBe("O arquivo excede o tamanho máximo");
        }

        [Fact]
        public void Dado_UploadProfilePictureOutput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var output = new UploadProfilePictureOutput
            {
                FileName = "foto.jpg",
                FileToken = "token-xyz",
                FileType = "image/jpeg",
                Width = 300,
                Height = 400
            };

            // Então
            output.FileName.ShouldBe("foto.jpg");
            output.FileToken.ShouldBe("token-xyz");
            output.FileType.ShouldBe("image/jpeg");
            output.Width.ShouldBe(300);
            output.Height.ShouldBe(400);
        }

        #endregion
    }
}

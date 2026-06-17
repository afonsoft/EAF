using Eaf.Middleware.Authorization.Users;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.Users
{
    /// <summary>
    /// Testes BDD para User seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class UserTests
    {
        [Fact]
        public void Dado_NovoUsuario_Quando_Criar_Entao_IsLockoutEnabledDeveSerTrue()
        {
            // Dado & Quando
            var user = new User();

            // Então
            user.IsLockoutEnabled.ShouldBeTrue();
        }

        [Fact]
        public void Dado_Usuario_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado
            var user = new User();
            var pictureId = System.Guid.NewGuid();

            // Quando
            user.ProfilePictureId = pictureId;
            user.ShouldChangePasswordOnNextLogin = true;
            user.ExternalAuthProviderformation = "Google";
            user.SignInToken = "token-123";

            // Então
            user.ProfilePictureId.ShouldBe(pictureId);
            user.ShouldChangePasswordOnNextLogin.ShouldBeTrue();
            user.ExternalAuthProviderformation.ShouldBe("Google");
            user.SignInToken.ShouldBe("token-123");
        }

        [Fact]
        public void Dado_MetodoFactory_Quando_CriarSenhaAleatoria_Entao_DeveTer16Caracteres()
        {
            // Quando
            var password = User.CreateRandomPassword();

            // Então
            password.ShouldNotBeNull();
            password.Length.ShouldBe(16);
        }

        [Fact]
        public void Dado_MetodoFactory_Quando_CriarAdminTenant_Entao_DeveConfigurarCorretamente()
        {
            // Dado
            var tenantId = 1;
            var email = "admin@acme.com";

            // Quando
            var user = User.CreateTenantAdminUser(tenantId, email);

            // Então
            user.TenantId.ShouldBe(tenantId);
            user.EmailAddress.ShouldBe(email);
            user.UserName.ShouldBe("admin");
            user.Name.ShouldBe("admin");
            user.Surname.ShouldBe("admin");
        }

        [Fact]
        public void Dado_Usuario_Quando_SetNewPasswordResetCode_Entao_DeveGerarCodigo10Chars()
        {
            // Dado
            var user = new User();

            // Quando
            user.SetNewPasswordResetCode();

            // Então
            user.PasswordResetCode.ShouldNotBeNull();
            user.PasswordResetCode.Length.ShouldBe(10);
            user.PasswordResetCode.ShouldBe(user.PasswordResetCode.ToUpperInvariant());
        }

        [Fact]
        public void Dado_Usuario_Quando_SetSignInToken_Entao_DeveGerarTokenEExpiracao()
        {
            // Dado
            var user = new User();

            // Quando
            user.SetSignInToken();

            // Então
            user.SignInToken.ShouldNotBeNullOrEmpty();
            user.SignInTokenExpireTimeUtc.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Usuario_Quando_SetSignInTokenComSegundos_Entao_DeveUsarSegundosCustom()
        {
            // Dado
            var user = new User();

            // Quando
            user.SetSignInToken(60);

            // Então
            user.SignInToken.ShouldNotBeNullOrEmpty();
            user.SignInTokenExpireTimeUtc.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_UsuarioBloqueado_Quando_Unlock_Entao_DeveZerarContadorELimparData()
        {
            // Dado
            var user = new User();
            user.AccessFailedCount = 5;
            user.LockoutEndDateUtc = System.DateTime.UtcNow.AddHours(1);

            // Quando
            user.Unlock();

            // Então
            user.AccessFailedCount.ShouldBe(0);
            user.LockoutEndDateUtc.ShouldBeNull();
        }

        [Fact]
        public void Dado_MetodoFactory_Quando_CriarMultiplasSenhas_Entao_DevemSerDiferentes()
        {
            // Quando
            var password1 = User.CreateRandomPassword();
            var password2 = User.CreateRandomPassword();

            // Então
            password1.ShouldNotBe(password2);
        }
    }
}

using Eaf.Middleware.Authorization.Users;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.Users
{
    public class UserBddTests
    {
        [Fact]
        public void Dado_User_Quando_CriarNovo_Entao_IsLockoutEnabledDeveSerTrue()
        {
            var user = new User();

            user.IsLockoutEnabled.ShouldBeTrue();
        }

        [Fact]
        public void Dado_User_Quando_CreateRandomPassword_Entao_DeveTer16Caracteres()
        {
            var password = User.CreateRandomPassword();

            password.ShouldNotBeNullOrEmpty();
            password.Length.ShouldBe(16);
        }

        [Fact]
        public void Dado_User_Quando_CreateRandomPassword_Entao_DeveSerUnicoACadaChamada()
        {
            var p1 = User.CreateRandomPassword();
            var p2 = User.CreateRandomPassword();

            p1.ShouldNotBe(p2);
        }

        [Fact]
        public void Dado_TenantId_Quando_CreateTenantAdminUser_Entao_DeveDefinirPropriedadesCorretas()
        {
            var user = User.CreateTenantAdminUser(42, "admin@acme.com");

            user.TenantId.ShouldBe(42);
            user.EmailAddress.ShouldBe("admin@acme.com");
            user.UserName.ShouldBe("admin");
            user.Name.ShouldBe("admin");
            user.Surname.ShouldBe("admin");
            user.NormalizedUserName.ShouldBe("ADMIN");
            user.NormalizedEmailAddress.ShouldBe("ADMIN@ACME.COM");
        }

        [Fact]
        public void Dado_User_Quando_SetNewPasswordResetCode_Entao_DeveGerarCodigoDe10Chars()
        {
            var user = new User();

            user.SetNewPasswordResetCode();

            user.PasswordResetCode.ShouldNotBeNullOrEmpty();
            user.PasswordResetCode.Length.ShouldBe(10);
            user.PasswordResetCode.ShouldBe(user.PasswordResetCode.ToUpperInvariant());
        }

        [Fact]
        public void Dado_User_Quando_SetSignInToken_Entao_DeveDefinirTokenEExpiracao()
        {
            var user = new User();

            user.SetSignInToken();

            user.SignInToken.ShouldNotBeNullOrEmpty();
            user.SignInTokenExpireTimeUtc.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_User_Quando_SetSignInTokenComSegundos_Entao_DeveUsarSegundosCustom()
        {
            var user = new User();

            user.SetSignInToken(60);

            user.SignInToken.ShouldNotBeNullOrEmpty();
            user.SignInTokenExpireTimeUtc.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_UserBloqueado_Quando_Unlock_Entao_DeveZerarContadorELimparLockout()
        {
            var user = new User
            {
                AccessFailedCount = 5,
                LockoutEndDateUtc = System.DateTime.UtcNow.AddHours(1)
            };

            user.Unlock();

            user.AccessFailedCount.ShouldBe(0);
            user.LockoutEndDateUtc.ShouldBeNull();
        }

        [Fact]
        public void Dado_User_Quando_DefinirPropriedades_Entao_DevePersistir()
        {
            var pictureId = System.Guid.NewGuid();
            var user = new User
            {
                ProfilePictureId = pictureId,
                ShouldChangePasswordOnNextLogin = true,
                ExternalAuthProviderformation = "google",
                SignInToken = "abc-123"
            };

            user.ProfilePictureId.ShouldBe(pictureId);
            user.ShouldChangePasswordOnNextLogin.ShouldBeTrue();
            user.ExternalAuthProviderformation.ShouldBe("google");
            user.SignInToken.ShouldBe("abc-123");
        }
    }
}

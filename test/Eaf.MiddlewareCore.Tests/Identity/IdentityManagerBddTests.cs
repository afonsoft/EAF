using Eaf.Middleware.Identity;
using Shouldly;
using Xunit;

namespace Eaf.Middleware
{
    /// <summary>
    /// Testes BDD para os wrappers de identity em Eaf.Middleware.Core.
    /// </summary>
    public class IdentityManagerBddTests
    {
        [Fact]
        public void Dado_Dependencias_Quando_CriarLogInManager_Entao_DeveSerInstanciado()
        {
            var userManager = IdentityHelper.CreateUserManager();
            var roleManager = IdentityHelper.CreateRoleManager();

            var logInManager = IdentityHelper.CreateLogInManager(userManager, roleManager);

            logInManager.ShouldNotBeNull();
            logInManager.ShouldBeOfType<LogInManager>();
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarSignInManager_Entao_DeveSerInstanciado()
        {
            var userManager = IdentityHelper.CreateUserManager();
            var roleManager = IdentityHelper.CreateRoleManager();

            var signInManager = IdentityHelper.CreateSignInManager(userManager, roleManager);

            signInManager.ShouldNotBeNull();
            signInManager.ShouldBeOfType<SignInManager>();
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarSecurityStampValidator_Entao_DeveSerInstanciado()
        {
            var userManager = IdentityHelper.CreateUserManager();
            var roleManager = IdentityHelper.CreateRoleManager();
            var signInManager = IdentityHelper.CreateSignInManager(userManager, roleManager);

            var validator = IdentityHelper.CreateSecurityStampValidator(signInManager);

            validator.ShouldNotBeNull();
            validator.ShouldBeOfType<SecurityStampValidator>();
        }
    }
}

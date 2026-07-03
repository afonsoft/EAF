using Eaf.Middleware.MultiTenancy.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.MultiTenancy
{
    public class CreateTenantInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new CreateTenantInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirAdminPassword_Entao_DeveArmazenar()
        {
            var sut = new CreateTenantInput();
            sut.AdminPassword = "test_value";
            sut.AdminPassword.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsActive_Entao_DeveArmazenar()
        {
            var sut = new CreateTenantInput();
            sut.IsActive = true;
            sut.IsActive.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirName_Entao_DeveArmazenar()
        {
            var sut = new CreateTenantInput();
            sut.Name = "test_value";
            sut.Name.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSendActivationEmail_Entao_DeveArmazenar()
        {
            var sut = new CreateTenantInput();
            sut.SendActivationEmail = true;
            sut.SendActivationEmail.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirShouldChangePasswordOnNextLogin_Entao_DeveArmazenar()
        {
            var sut = new CreateTenantInput();
            sut.ShouldChangePasswordOnNextLogin = true;
            sut.ShouldChangePasswordOnNextLogin.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTenancyName_Entao_DeveArmazenar()
        {
            var sut = new CreateTenantInput();
            sut.TenancyName = "test_value";
            sut.TenancyName.ShouldBe("test_value");
        }
    }
}

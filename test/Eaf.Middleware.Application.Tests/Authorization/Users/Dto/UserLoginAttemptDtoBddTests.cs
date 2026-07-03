using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Users
{
    public class UserLoginAttemptDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new UserLoginAttemptDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirClientIpAddress_Entao_DeveArmazenar()
        {
            var sut = new UserLoginAttemptDto();
            sut.ClientIpAddress = "test_value";
            sut.ClientIpAddress.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirClientName_Entao_DeveArmazenar()
        {
            var sut = new UserLoginAttemptDto();
            sut.ClientName = "test_value";
            sut.ClientName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirCreationTime_Entao_DeveArmazenar()
        {
            var sut = new UserLoginAttemptDto();
            var dt = System.DateTime.UtcNow; sut.CreationTime = dt;
            sut.CreationTime.ShouldBe(dt);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirResult_Entao_DeveArmazenar()
        {
            var sut = new UserLoginAttemptDto();
            sut.Result = "test_value";
            sut.Result.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTenancyName_Entao_DeveArmazenar()
        {
            var sut = new UserLoginAttemptDto();
            sut.TenancyName = "test_value";
            sut.TenancyName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUserNameOrEmail_Entao_DeveArmazenar()
        {
            var sut = new UserLoginAttemptDto();
            sut.UserNameOrEmail = "test_value";
            sut.UserNameOrEmail.ShouldBe("test_value");
        }
    }
}

using Eaf.Middleware.Configuration.Host.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration.Host
{
    public class UserLockOutSettingsEditDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new UserLockOutSettingsEditDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsEnabled_Entao_DeveArmazenar()
        {
            var sut = new UserLockOutSettingsEditDto();
            sut.IsEnabled = true;
            sut.IsEnabled.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirMaxFailedAccessAttemptsBeforeLockout_Entao_DeveArmazenar()
        {
            var sut = new UserLockOutSettingsEditDto();
            sut.MaxFailedAccessAttemptsBeforeLockout = 42;
            sut.MaxFailedAccessAttemptsBeforeLockout.ShouldBe(42);
        }
    }
}

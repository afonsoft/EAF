using Eaf.Middleware.Configuration.Host.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration.Host
{
    public class LdapSettingsEditDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new LdapSettingsEditDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsEnabled_Entao_DeveArmazenar()
        {
            var sut = new LdapSettingsEditDto();
            sut.IsEnabled = true;
            sut.IsEnabled.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsModuleEnabled_Entao_DeveArmazenar()
        {
            var sut = new LdapSettingsEditDto();
            sut.IsModuleEnabled = true;
            sut.IsModuleEnabled.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirPassword_Entao_DeveArmazenar()
        {
            var sut = new LdapSettingsEditDto();
            sut.Password = "test_value";
            sut.Password.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUserName_Entao_DeveArmazenar()
        {
            var sut = new LdapSettingsEditDto();
            sut.UserName = "test_value";
            sut.UserName.ShouldBe("test_value");
        }
    }
}

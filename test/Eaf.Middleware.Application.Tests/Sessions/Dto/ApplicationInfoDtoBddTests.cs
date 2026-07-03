using Eaf.Middleware.Sessions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Sessions
{
    public class ApplicationInfoDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ApplicationInfoDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirCurrencySign_Entao_DeveArmazenar()
        {
            var sut = new ApplicationInfoDto();
            sut.CurrencySign = "test_value";
            sut.CurrencySign.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirReleaseDate_Entao_DeveArmazenar()
        {
            var sut = new ApplicationInfoDto();
            var dt = System.DateTime.UtcNow; sut.ReleaseDate = dt;
            sut.ReleaseDate.ShouldBe(dt);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTwoFactorCodeExpireSeconds_Entao_DeveArmazenar()
        {
            var sut = new ApplicationInfoDto();
            sut.TwoFactorCodeExpireSeconds = 3.14;
            sut.TwoFactorCodeExpireSeconds.ShouldBe(3.14);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirVersion_Entao_DeveArmazenar()
        {
            var sut = new ApplicationInfoDto();
            sut.Version = "test_value";
            sut.Version.ShouldBe("test_value");
        }
    }
}

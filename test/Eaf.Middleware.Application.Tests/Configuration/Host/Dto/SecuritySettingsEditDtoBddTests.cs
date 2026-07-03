using Eaf.Middleware.Configuration.Host.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration.Host
{
    public class SecuritySettingsEditDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new SecuritySettingsEditDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUseDefaultPasswordComplexitySettings_Entao_DeveArmazenar()
        {
            var sut = new SecuritySettingsEditDto();
            sut.UseDefaultPasswordComplexitySettings = true;
            sut.UseDefaultPasswordComplexitySettings.ShouldBe(true);
        }
    }
}

using Eaf.Middleware.Configuration.Host.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration.Host
{
    public class ExpiredEntityLogDeleterSettingsEditDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ExpiredEntityLogDeleterSettingsEditDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirEnabled_Entao_DeveArmazenar()
        {
            var sut = new ExpiredEntityLogDeleterSettingsEditDto();
            sut.Enabled = true;
            sut.Enabled.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirDeletedQuantity_Entao_DeveArmazenar()
        {
            var sut = new ExpiredEntityLogDeleterSettingsEditDto();
            sut.DeletedQuantity = 42;
            sut.DeletedQuantity.ShouldBe(42);
        }
    }
}

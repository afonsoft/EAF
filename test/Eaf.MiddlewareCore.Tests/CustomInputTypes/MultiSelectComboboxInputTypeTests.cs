using Eaf.Middleware.Core.CustomInputTypes;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.CustomInputTypes
{
    public class MultiSelectComboboxInputTypeTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveSerCriada()
        {
            var inputType = new MultiSelectComboboxInputType();
            inputType.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_MultiSelectComboboxInputType_Quando_VerificarNome_Entao_DeveSerMULTISELECTCOMBOBOX()
        {
            var inputType = new MultiSelectComboboxInputType();
            inputType.Name.ShouldBe("MULTISELECTCOMBOBOX");
        }
    }
}

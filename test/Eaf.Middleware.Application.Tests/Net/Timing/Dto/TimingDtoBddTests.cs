using Abp.Configuration;
using Eaf.Middleware.Timing.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Net.Timing.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de Timing seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class TimingDtoBddTests
    {
        [Fact]
        public void Dado_GetTimezonesInput_Quando_CriarPadrao_Entao_ScopeDeveSerApplication()
        {
            var input = new GetTimezonesInput();
            input.DefaultTimezoneScope.ShouldBe(SettingScopes.Application);
        }

        [Fact]
        public void Dado_GetTimezoneComboboxItemsInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new GetTimezoneComboboxItemsInput
            {
                DefaultTimezoneScope = SettingScopes.Tenant,
                SelectedTimezoneId = "America/Sao_Paulo"
            };

            input.DefaultTimezoneScope.ShouldBe(SettingScopes.Tenant);
            input.SelectedTimezoneId.ShouldBe("America/Sao_Paulo");
        }
    }
}

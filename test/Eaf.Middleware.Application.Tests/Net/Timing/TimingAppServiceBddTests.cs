using Abp.Application.Services.Dto;
using Abp.Configuration;
using Eaf.Middleware.Timing;
using Eaf.Middleware.Timing.Dto;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Net.Timing
{
    /// <summary>
    /// Testes BDD para TimingAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class TimingAppServiceBddTests
    {
        private readonly ITimeZoneService _timeZoneService;
        private readonly TimingAppService _sut;

        public TimingAppServiceBddTests()
        {
            _timeZoneService = Substitute.For<ITimeZoneService>();
            _sut = new TimingAppService(_timeZoneService);
        }

        #region Construtor

        [Fact]
        public void Dado_TimeZoneService_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            // Dado / Quando
            var sut = new TimingAppService(_timeZoneService);

            // Então
            sut.ShouldNotBeNull();
        }

        #endregion

        #region GetTimezoneComboboxItemsInput

        [Fact]
        public void Dado_GetTimezoneComboboxItemsInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado / Quando
            var input = new GetTimezoneComboboxItemsInput
            {
                DefaultTimezoneScope = SettingScopes.Application,
                SelectedTimezoneId = "America/Sao_Paulo"
            };

            // Então
            input.DefaultTimezoneScope.ShouldBe(SettingScopes.Application);
            input.SelectedTimezoneId.ShouldBe("America/Sao_Paulo");
        }

        #endregion

        #region GetTimezonesInput

        [Fact]
        public void Dado_GetTimezonesInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado / Quando
            var input = new GetTimezonesInput
            {
                DefaultTimezoneScope = SettingScopes.Tenant
            };

            // Então
            input.DefaultTimezoneScope.ShouldBe(SettingScopes.Tenant);
        }

        #endregion
    }
}

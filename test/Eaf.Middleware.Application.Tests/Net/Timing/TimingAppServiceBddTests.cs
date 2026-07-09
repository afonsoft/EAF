using Abp.Application.Services.Dto;
using Abp.Configuration;
using Abp.Runtime.Session;
using Eaf.Middleware.Timing;
using Eaf.Middleware.Timing.Dto;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IAbpSession _abpSession;
        private readonly TimingAppService _sut;

        public TimingAppServiceBddTests()
        {
            _timeZoneService = Substitute.For<ITimeZoneService>();
            _abpSession = Substitute.For<IAbpSession>();
            _abpSession.TenantId.Returns(1);

            _sut = new TimingAppService(_timeZoneService)
            {
                AbpSession = _abpSession
            };
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

        #region GetTimezones

        [Fact]
        public async Task Dado_TimeZonesDisponiveis_Quando_GetTimezones_Entao_DeveRetornarListaComDefaultNoInicio()
        {
            // Dado
            _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Application, 1).Returns("E. South America Standard Time");
            _timeZoneService.GetWindowsTimezones().Returns(new List<NameValueDto>
            {
                new NameValueDto("Brasília", "E. South America Standard Time"),
                new NameValueDto("New York", "Eastern Standard Time")
            });

            // Quando
            var result = await _sut.GetTimezones(new GetTimezonesInput { DefaultTimezoneScope = SettingScopes.Application });

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(3);
            result.Items[0].Name.ShouldContain("E. South America Standard Time");
            result.Items[0].Value.ShouldBe(string.Empty);
        }

        #endregion

        #region GetTimezoneComboboxItems

        [Fact]
        public async Task Dado_TimeZoneSelecionadoExistente_Quando_GetTimezoneComboboxItems_Entao_DeveMarcarComoSelecionado()
        {
            // Dado
            _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Application, 1).Returns("E. South America Standard Time");
            _timeZoneService.GetWindowsTimezones().Returns(new List<NameValueDto>
            {
                new NameValueDto("Brasília", "E. South America Standard Time"),
                new NameValueDto("New York", "Eastern Standard Time")
            });

            var input = new GetTimezoneComboboxItemsInput
            {
                DefaultTimezoneScope = SettingScopes.Application,
                SelectedTimezoneId = "E. South America Standard Time"
            };

            // Quando
            var result = await _sut.GetTimezoneComboboxItems(input);

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(3);
            result.First(x => x.Value == "E. South America Standard Time").IsSelected.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_TimeZoneSelecionadoInexistente_Quando_GetTimezoneComboboxItems_Entao_NaoDeveMarcarNenhumItem()
        {
            // Dado
            _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Application, 1).Returns("E. South America Standard Time");
            _timeZoneService.GetWindowsTimezones().Returns(new List<NameValueDto>
            {
                new NameValueDto("Brasília", "E. South America Standard Time"),
                new NameValueDto("New York", "Eastern Standard Time")
            });

            var input = new GetTimezoneComboboxItemsInput
            {
                DefaultTimezoneScope = SettingScopes.Application,
                SelectedTimezoneId = "Pacific Standard Time"
            };

            // Quando
            var result = await _sut.GetTimezoneComboboxItems(input);

            // Então
            result.ShouldNotBeNull();
            result.All(x => !x.IsSelected).ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_SemTimeZoneSelecionado_Quando_GetTimezoneComboboxItems_Entao_DeveRetornarListaCompleta()
        {
            // Dado
            _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Application, 1).Returns("E. South America Standard Time");
            _timeZoneService.GetWindowsTimezones().Returns(new List<NameValueDto>
            {
                new NameValueDto("Brasília", "E. South America Standard Time")
            });

            var input = new GetTimezoneComboboxItemsInput
            {
                DefaultTimezoneScope = SettingScopes.Application,
                SelectedTimezoneId = string.Empty
            };

            // Quando
            var result = await _sut.GetTimezoneComboboxItems(input);

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
        }

        #endregion
    }
}

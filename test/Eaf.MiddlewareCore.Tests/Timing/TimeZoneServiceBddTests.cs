using Abp.Application.Services.Dto;
using Abp.Configuration;
using Abp.Timing;
using Eaf.Middleware.Timing;
using NSubstitute;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Timing
{
    /// <summary>
    /// Testes BDD para TimeZoneService seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class TimeZoneServiceBddTests
    {
        private readonly ISettingManager _settingManager;
        private readonly ISettingDefinitionManager _settingDefinitionManager;
        private readonly TimeZoneService _sut;

        public TimeZoneServiceBddTests()
        {
            _settingManager = Substitute.For<ISettingManager>();
            _settingDefinitionManager = Substitute.For<ISettingDefinitionManager>();
            _sut = new TimeZoneService(_settingManager, _settingDefinitionManager);
        }

        #region FindTimeZoneById

        [Fact]
        public void Dado_TimezoneIdValido_Quando_FindTimeZoneById_Entao_DeveRetornarTimeZoneInfo()
        {
            // Quando
            var result = _sut.FindTimeZoneById("UTC");

            // Entao
            result.ShouldNotBeNull();
            result.Id.ShouldContain("UTC");
        }

        [Fact]
        public void Dado_TimezoneWindowsId_Quando_FindTimeZoneById_Entao_DeveRetornarTimeZoneInfo()
        {
            // Quando
            var result = _sut.FindTimeZoneById("E. South America Standard Time");

            // Entao
            result.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_TimezoneIdInvalido_Quando_FindTimeZoneById_Entao_DeveLancarExcecao()
        {
            // Quando/Entao
            Should.Throw<Exception>(() => _sut.FindTimeZoneById("Invalid/TimeZone/Id/123"));
        }

        #endregion

        #region GetDefaultTimezoneAsync

        [Fact]
        public async Task Dado_ScopeUser_ComTenantId_Quando_GetDefaultTimezoneAsync_Entao_DeveRetornarTimezoneDoTenant()
        {
            // Dado
            _settingManager.GetSettingValueForTenantAsync(TimingSettingNames.TimeZone, 1)
                .Returns("E. South America Standard Time");

            // Quando
            var result = await _sut.GetDefaultTimezoneAsync(SettingScopes.User, 1);

            // Entao
            result.ShouldBe("E. South America Standard Time");
        }

        [Fact]
        public async Task Dado_ScopeUser_SemTenantId_Quando_GetDefaultTimezoneAsync_Entao_DeveRetornarTimezoneApplication()
        {
            // Dado
            _settingManager.GetSettingValueForApplicationAsync(TimingSettingNames.TimeZone)
                .Returns("UTC");

            // Quando
            var result = await _sut.GetDefaultTimezoneAsync(SettingScopes.User, null);

            // Entao
            result.ShouldBe("UTC");
        }

        [Fact]
        public async Task Dado_ScopeTenant_Quando_GetDefaultTimezoneAsync_Entao_DeveRetornarTimezoneApplication()
        {
            // Dado
            _settingManager.GetSettingValueForApplicationAsync(TimingSettingNames.TimeZone)
                .Returns("Pacific Standard Time");

            // Quando
            var result = await _sut.GetDefaultTimezoneAsync(SettingScopes.Tenant, null);

            // Entao
            result.ShouldBe("Pacific Standard Time");
        }

        [Fact]
        public async Task Dado_ScopeApplication_Quando_GetDefaultTimezoneAsync_Entao_DeveRetornarValorPadrao()
        {
            // Dado
            var settingDef = new SettingDefinition(TimingSettingNames.TimeZone, "UTC");
            _settingDefinitionManager.GetSettingDefinition(TimingSettingNames.TimeZone)
                .Returns(settingDef);

            // Quando
            var result = await _sut.GetDefaultTimezoneAsync(SettingScopes.Application, null);

            // Entao
            result.ShouldBe("UTC");
        }

        [Fact]
        public async Task Dado_ScopeDesconhecido_Quando_GetDefaultTimezoneAsync_Entao_DeveLancarExcecao()
        {
            // Quando/Entao
            await Should.ThrowAsync<Exception>(
                () => _sut.GetDefaultTimezoneAsync((SettingScopes)999, null)
            );
        }

        #endregion

        #region GetWindowsTimezones

        [Fact]
        public void Dado_TimeZoneService_Quando_GetWindowsTimezones_Entao_DeveRetornarListaNaoVazia()
        {
            // Quando
            var result = _sut.GetWindowsTimezones();

            // Entao
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Dado_TimeZoneService_Quando_GetWindowsTimezones_Entao_DeveRetornarNameValueDto()
        {
            // Quando
            var result = _sut.GetWindowsTimezones();

            // Entao
            result.ShouldAllBe(r => r.Name != null && r.Value != null);
        }

        [Fact]
        public void Dado_TimeZoneService_Quando_GetWindowsTimezones_Entao_DeveEstarOrdenado()
        {
            // Quando
            var result = _sut.GetWindowsTimezones();

            // Entao
            var values = result.Select(r => r.Value).ToList();
            values.ShouldBe(values.OrderBy(v => v).ToList());
        }

        #endregion

        #region Instanciacao

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
            _sut.ShouldBeAssignableTo<ITimeZoneService>();
        }

        #endregion
    }
}

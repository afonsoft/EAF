using Abp.Configuration;
using Abp.Zero.Configuration;
using Eaf.Middleware.Security;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Security
{
    /// <summary>
    /// Testes BDD para PasswordComplexitySettingStore seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class PasswordComplexitySettingStoreBddTests
    {
        private readonly ISettingManager _settingManager;
        private readonly PasswordComplexitySettingStore _sut;

        public PasswordComplexitySettingStoreBddTests()
        {
            _settingManager = Substitute.For<ISettingManager>();
            _sut = new PasswordComplexitySettingStore(_settingManager);
        }

        #region GetSettingsAsync

        [Fact]
        public async Task Dado_ConfiguracoesComplexas_Quando_GetSettingsAsync_Entao_DeveRetornarTodasConfiguracoes()
        {
            // Dado
            _settingManager.GetSettingValueAsync(
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit)
                .Returns("True");
            _settingManager.GetSettingValueAsync(
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase)
                .Returns("True");
            _settingManager.GetSettingValueAsync(
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric)
                .Returns("True");
            _settingManager.GetSettingValueAsync(
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase)
                .Returns("True");
            _settingManager.GetSettingValueAsync(
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength)
                .Returns("8");

            // Quando
            var result = await _sut.GetSettingsAsync();

            // Entao
            result.ShouldNotBeNull();
            result.RequireDigit.ShouldBeTrue();
            result.RequireLowercase.ShouldBeTrue();
            result.RequireNonAlphanumeric.ShouldBeTrue();
            result.RequireUppercase.ShouldBeTrue();
            result.RequiredLength.ShouldBe(8);
        }

        [Fact]
        public async Task Dado_ConfiguracoesMinimasDesabilitadas_Quando_GetSettingsAsync_Entao_DeveRetornarFalso()
        {
            // Dado
            _settingManager.GetSettingValueAsync(
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit)
                .Returns("False");
            _settingManager.GetSettingValueAsync(
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase)
                .Returns("False");
            _settingManager.GetSettingValueAsync(
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric)
                .Returns("False");
            _settingManager.GetSettingValueAsync(
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase)
                .Returns("False");
            _settingManager.GetSettingValueAsync(
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength)
                .Returns("4");

            // Quando
            var result = await _sut.GetSettingsAsync();

            // Entao
            result.RequireDigit.ShouldBeFalse();
            result.RequireLowercase.ShouldBeFalse();
            result.RequireNonAlphanumeric.ShouldBeFalse();
            result.RequireUppercase.ShouldBeFalse();
            result.RequiredLength.ShouldBe(4);
        }

        #endregion

        #region Instanciacao

        [Fact]
        public void Dado_SettingManager_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
            _sut.ShouldBeAssignableTo<IPasswordComplexitySettingStore>();
        }

        #endregion
    }
}

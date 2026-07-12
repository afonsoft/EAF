using Abp.Configuration;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.AzureActiveDirectory.Tests.Configuration
{
    /// <summary>
    /// Testes BDD para AzureActiveDirectorySettings.
    /// </summary>
    public class AzureActiveDirectorySettingsBddTests
    {
        private readonly ISettingManager _settingManager;
        private readonly AzureActiveDirectorySettings _sut;

        public AzureActiveDirectorySettingsBddTests()
        {
            _settingManager = Substitute.For<ISettingManager>();
            _sut = new AzureActiveDirectorySettings(_settingManager);
        }

        [Fact]
        public async Task Dado_SettingManagerRetornandoTrue_Quando_GetIsEnabled_Entao_DeveRetornarTrue()
        {
            // Dado
            _settingManager.GetSettingValueForApplicationAsync(AzureActiveDirectorySettingNames.IsEnabled)
                .Returns(Task.FromResult("true"));

            // Quando
            var result = await _sut.GetIsEnabled();

            // Então
            result.ShouldBeTrue();
            await _settingManager.Received(1).GetSettingValueForApplicationAsync(AzureActiveDirectorySettingNames.IsEnabled);
        }

        [Fact]
        public async Task Dado_SettingManagerRetornandoFalse_Quando_GetIsEnabled_Entao_DeveRetornarFalse()
        {
            // Dado
            _settingManager.GetSettingValueForApplicationAsync(AzureActiveDirectorySettingNames.IsEnabled)
                .Returns(Task.FromResult("false"));

            // Quando
            var result = await _sut.GetIsEnabled();

            // Então
            result.ShouldBeFalse();
        }
    }
}

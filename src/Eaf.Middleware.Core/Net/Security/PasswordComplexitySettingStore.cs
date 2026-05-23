using Abp.Configuration;
using Abp.Dependency;
using Abp.Zero.Configuration;
using Eaf.Middleware.Configuration;
using System.Threading.Tasks;

namespace Eaf.Middleware.Security
{
    /// <summary>
    /// Representa a classe PasswordComplexitySettingStore.
    /// </summary>
    public class PasswordComplexitySettingStore : IPasswordComplexitySettingStore, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// PasswordComplexitySettingStore.
        /// </summary>
        /// <param name="settingManager">Parâmetro settingManager.</param>
        /// <returns>Resultado da operação.</returns>
        public PasswordComplexitySettingStore(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        /// <summary>
        /// GetSettingsAsync.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public async Task<PasswordComplexitySetting> GetSettingsAsync()
        {
            return new PasswordComplexitySetting
            {
                RequireDigit = await _settingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit),
                RequireLowercase = await _settingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase),
                RequireNonAlphanumeric = await _settingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric),
                RequireUppercase = await _settingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase),
                RequiredLength = await _settingManager.GetSettingValueAsync<int>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength)
            };
        }
    }
}
using Abp;
using Abp.Dependency;
using Eaf.Middleware.Configuration.Dto;
using Eaf.Middleware.UiCustomization.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.UiCustomization
{
    /// <summary>
    /// Representa a interface IUiCustomizer.
    /// </summary>
    public interface IUiCustomizer : ISingletonDependency
    {
        Task<ThemeSettingsDto> GetHostUiManagementSettings();

        Task<ThemeSettingsDto> GetTenantUiCustomizationSettings(int tenantId);

        Task<UiCustomizationSettingsDto> GetUiSettings();

        Task UpdateApplicationUiManagementSettingsAsync(ThemeSettingsDto settings);

        Task UpdateTenantUiManagementSettingsAsync(int tenantId, ThemeSettingsDto settings);

        Task UpdateUserUiManagementSettingsAsync(UserIdentifier user, ThemeSettingsDto settings);
    }
}
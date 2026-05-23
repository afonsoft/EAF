using System.Threading.Tasks;

namespace Eaf.Middleware.Security
{
    /// <summary>
    /// Representa a interface IPasswordComplexitySettingStore.
    /// </summary>
    public interface IPasswordComplexitySettingStore
    {
        Task<PasswordComplexitySetting> GetSettingsAsync();
    }
}
using System.Threading.Tasks;

namespace Eaf.Middleware.AzureActiveDirectory.Configuration
{
    /// <summary>
    /// Used to obtain current values of AzureActiveDirectory settings. This abstraction allows to
    /// define a different source for settings than SettingManager (see default implementation: <see cref="AzureActiveDirectorySettings"/>).
    /// </summary>
    public interface IAzureActiveDirectorySettings
    {
        Task<string> GetClientId();

        Task<string> GetClientSecret();

        Task<bool> GetIsEnabled();

        Task<string> GetTenant();
    }
}
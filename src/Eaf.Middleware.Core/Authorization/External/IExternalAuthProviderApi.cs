using System.Threading.Tasks;

namespace Eaf.Middleware.Core.Authentication.External
{
    /// <summary>
    /// Representa a interface IExternalAuthProviderApi.
    /// </summary>
    public interface IExternalAuthProviderApi
    {
        ExternalLoginProviderInfo ProviderInfo { get; }

        Task<ExternalAuthUserInfo> GetUserInfo(string accessCode);

        void Initialize(ExternalLoginProviderInfo providerInfo);

        Task<bool> IsValidUser(string userId, string accessCode);
    }
}
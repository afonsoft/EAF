using System.Threading.Tasks;

namespace Eaf.Middleware.Core.Authentication.External
{
    /// <summary>
    /// Representa a interface IExternalAuthManager.
    /// </summary>
    public interface IExternalAuthManager
    {
        Task<ExternalAuthUserInfo> GetUserInfo(string provider, string accessCode);

        Task<bool> IsValidUser(string provider, string providerKey, string providerAccessCode);
    }
}
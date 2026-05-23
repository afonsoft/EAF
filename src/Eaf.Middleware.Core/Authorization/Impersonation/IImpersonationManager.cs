using Abp.Domain.Services;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authorization.Impersonation
{
    /// <summary>
    /// Representa a interface IImpersonationManager.
    /// </summary>
    public interface IImpersonationManager : IDomainService
    {
        Task<string> GetBackToImpersonatorToken();

        Task<UserAndIdentity> GetImpersonatedUserAndIdentity(string impersonationToken);

        Task<string> GetImpersonationToken(long userId, int? tenantId);
    }
}
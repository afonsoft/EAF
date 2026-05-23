using Abp.Application.Services;
using Eaf.Middleware.Authorization.Accounts.Dto;
using Eaf.Middleware.MultiTenancy.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authorization.Accounts
{
    /// <summary>
    /// Representa a interface IAccountAppService.
    /// </summary>
    public interface IAccountAppService : IApplicationService
    {
        Task ActivateEmail(ActivateEmailInput input);

        Task<ImpersonateOutput> BackToImpersonator();

        Task<List<TenantListDto>> GetAllTenants();

        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<ResetPasswordOutput> ResetPassword(ResetPasswordInput input);

        Task<int?> ResolveTenantId(ResolveTenantIdInput input);

        Task SendEmailActivationLink(SendEmailActivationLinkInput input);

        Task SendPasswordResetCode(SendPasswordResetCodeInput input);
    }
}
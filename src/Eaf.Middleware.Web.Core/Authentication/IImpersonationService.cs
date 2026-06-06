using Eaf.Middleware.Web.Models.TokenAuth;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Authentication
{
    /// <summary>
    /// Serviço de impersonação de usuários e tenants.
    /// </summary>
    public interface IImpersonationService
    {
        /// <summary>
        /// Inicia impersonação de um usuário específico.
        /// </summary>
        /// <param name="input">Dados do usuário a impersonar.</param>
        /// <returns>Resultado com token impersonado.</returns>
        Task<ImpersonatedAuthenticateResultModel> ImpersonateUserAsync(ImpersonateModel input);

        /// <summary>
        /// Inicia impersonação de um tenant específico.
        /// </summary>
        /// <param name="tenantId">ID do tenant a impersonar.</param>
        /// <returns>Resultado com token impersonado.</returns>
        Task<ImpersonatedAuthenticateResultModel> ImpersonateTenantAsync(int tenantId);

        /// <summary>
        /// Volta à identidade original após impersonação.
        /// </summary>
        /// <returns>Resultado com token do impersonador original.</returns>
        Task<ImpersonatedAuthenticateResultModel> BackToImpersonatorAsync();
    }
}

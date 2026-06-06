using Eaf.Middleware.Web.Models.TokenAuth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Authentication
{
    /// <summary>
    /// Serviço de autenticação externa (Google, Microsoft, AuthZero).
    /// </summary>
    public interface IExternalAuthenticationService
    {
        /// <summary>
        /// Autentica usando provedor externo.
        /// </summary>
        /// <param name="model">Modelo com dados do provedor externo.</param>
        /// <returns>Resultado da autenticação externa.</returns>
        Task<ExternalAuthenticateResultModel> ExternalAuthenticateAsync(ExternalAuthenticateModel model);

        /// <summary>
        /// Obtém provedores externos configurados para o tenant atual.
        /// </summary>
        /// <returns>Lista de provedores externos disponíveis.</returns>
        Task<List<ExternalLoginProviderInfoModel>> GetExternalAuthenticationProvidersAsync();
    }
}

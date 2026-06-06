using Eaf.Middleware.Web.Models.TokenAuth;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Authentication
{
    /// <summary>
    /// Serviço de autenticação local (login, token, refresh).
    /// </summary>
    public interface ITokenAuthenticationService
    {
        /// <summary>
        /// Autentica um usuário local e gera tokens JWT.
        /// </summary>
        /// <param name="model">Modelo de autenticação com credenciais.</param>
        /// <returns>Resultado da autenticação com tokens.</returns>
        Task<AuthenticateResultModel> AuthenticateAsync(AuthenticateModel model);

        /// <summary>
        /// Gera um access token JWT para o conjunto de claims fornecido.
        /// </summary>
        /// <param name="claims">Claims para incluir no token.</param>
        /// <param name="expiration">Tempo de expiração do token.</param>
        /// <returns>Token JWT serializado.</returns>
        Task<string> CreateAccessTokenAsync(IEnumerable<Claim> claims, TimeSpan expiration);
    }
}

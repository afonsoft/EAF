using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Authentication.JwtBearer
{
    /// <summary>
    /// Armazenamento abstrato de refresh tokens para rotação segura de tokens JWT.
    /// </summary>
    public interface IRefreshTokenStore
    {
        /// <summary>Recupera as informações de um refresh token.</summary>
        /// <param name="token">Valor do refresh token.</param>
        Task<RefreshTokenInfo> GetAsync(string token);

        /// <summary>Armazena um refresh token com expiração absoluta.</summary>
        /// <param name="refreshToken">Informações do refresh token.</param>
        Task SetAsync(RefreshTokenInfo refreshToken);

        /// <summary>Remove um refresh token do armazenamento.</summary>
        /// <param name="token">Valor do refresh token.</param>
        Task RemoveAsync(string token);
    }
}

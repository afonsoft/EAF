using System;

namespace Eaf.Middleware.Web.Authentication.JwtBearer
{
    /// <summary>
    /// Informações do refresh token armazenadas de forma segura (cookie HttpOnly).
    /// </summary>
    [Serializable]
    public class RefreshTokenInfo
    {
        /// <summary>Token do refresh.</summary>
        public string Token { get; set; }

        /// <summary>Identificador do usuário.</summary>
        public long UserId { get; set; }

        /// <summary>Identificador do tenant, se aplicável.</summary>
        public int? TenantId { get; set; }

        /// <summary>Security stamp do usuário no momento da emissão.</summary>
        public string SecurityStamp { get; set; }

        /// <summary>Data de expiração UTC do refresh token.</summary>
        public DateTime ExpireDate { get; set; }
    }
}

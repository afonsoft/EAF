using System;

namespace Eaf.Middleware.Authentication.Sessions
{
    /// <summary>
    /// Registro de uma sessão interativa do usuário (login com refresh token).
    /// Armazenado em cache distribuído; expira junto do refresh token.
    /// </summary>
    [Serializable]
    public class UserSessionInfo
    {
        /// <summary>Id único da sessão (claim <c>eaf_session_id</c> no JWT).</summary>
        public string SessionId { get; set; }

        /// <summary>Id do usuário dono da sessão.</summary>
        public long UserId { get; set; }

        /// <summary>Tenant da sessão, se aplicável.</summary>
        public int? TenantId { get; set; }

        /// <summary>Refresh token atualmente vinculado à sessão.</summary>
        public string RefreshToken { get; set; }

        /// <summary>IP do cliente no login (truncado/normalizado).</summary>
        public string IpAddress { get; set; }

        /// <summary>User agent do cliente no login (truncado a 256 chars).</summary>
        public string UserAgent { get; set; }

        /// <summary>Criação UTC.</summary>
        public DateTime CreationTime { get; set; }

        /// <summary>Última atividade observada UTC (granularidade ~30s).</summary>
        public DateTime LastActivityTime { get; set; }

        /// <summary>Expiração absoluta UTC (igual à do refresh token).</summary>
        public DateTime ExpireDate { get; set; }
    }
}

namespace Eaf.Middleware.Authentication.Sessions
{
    /// <summary>
    /// Constantes de sessões interativas.
    /// </summary>
    public static class SessionConsts
    {
        /// <summary>Claim do JWT que identifica a sessão interativa.</summary>
        public const string SessionIdClaim = "eaf_session_id";

        /// <summary>Intervalo mínimo entre gravações de LastActivityTime no cache.</summary>
        public static readonly System.TimeSpan ActivityThrottle = System.TimeSpan.FromSeconds(30);
    }
}

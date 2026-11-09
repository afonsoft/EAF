namespace Eaf.Middleware.Authentication.ApiKeys
{
    /// <summary>
    /// Status de uma API key.
    /// </summary>
    public enum ApiKeyStatus : byte
    {
        /// <summary>
        /// Chave ativa e utilizável para autenticação.
        /// </summary>
        Active = 0,

        /// <summary>
        /// Chave revogada; autenticação é sempre rejeitada.
        /// </summary>
        Revoked = 1
    }
}

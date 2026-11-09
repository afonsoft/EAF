namespace Eaf.Middleware.Authentication.ApiKeys
{
    /// <summary>
    /// Resultado da validação de uma API key apresentada em uma requisição.
    /// </summary>
    public class ApiKeyValidationResult
    {
        /// <summary>
        /// A chave encontrada e validada (null quando a autenticação falhou).
        /// </summary>
        public ApiKey ApiKey { get; }

        /// <summary>
        /// Motivo da falha (null quando sucesso).
        /// </summary>
        public ApiKeyValidationError? Error { get; }

        /// <summary>
        /// Indica sucesso na validação.
        /// </summary>
        public bool Succeeded => ApiKey != null && Error == null;

        private ApiKeyValidationResult(ApiKey apiKey, ApiKeyValidationError? error)
        {
            ApiKey = apiKey;
            Error = error;
        }

        /// <summary>
        /// Cria um resultado de sucesso.
        /// </summary>
        public static ApiKeyValidationResult Success(ApiKey apiKey) => new ApiKeyValidationResult(apiKey, null);

        /// <summary>
        /// Cria um resultado de falha.
        /// </summary>
        public static ApiKeyValidationResult Fail(ApiKeyValidationError error, ApiKey apiKey = null) => new ApiKeyValidationResult(apiKey, error);
    }

    /// <summary>
    /// Motivos de falha na validação de uma API key.
    /// </summary>
    public enum ApiKeyValidationError
    {
        /// <summary>
        /// Funcionalidade desabilitada por configuração.
        /// </summary>
        FeatureDisabled,

        /// <summary>
        /// Chave ausente, malformada ou inexistente.
        /// </summary>
        KeyNotFound,

        /// <summary>
        /// Chave revogada.
        /// </summary>
        Revoked,

        /// <summary>
        /// Chave expirada.
        /// </summary>
        Expired,

        /// <summary>
        /// IP de origem fora da allowlist da chave.
        /// </summary>
        IpNotAllowed,

        /// <summary>
        /// Usuário dono da chave inexistente ou inativo.
        /// </summary>
        UserUnavailable
    }
}

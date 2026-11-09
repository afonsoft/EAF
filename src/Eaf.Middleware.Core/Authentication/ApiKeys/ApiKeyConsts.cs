namespace Eaf.Middleware.Authentication.ApiKeys
{
    /// <summary>
    /// Constantes para autenticação por API key (header <c>X-API-Key</c>).
    /// </summary>
    public static class ApiKeyConsts
    {
        /// <summary>
        /// Header HTTP que transporta a API key.
        /// </summary>
        public const string HeaderName = "X-API-Key";

        /// <summary>
        /// Nome do esquema de autenticação registrado no pipeline ASP.NET Core.
        /// </summary>
        public const string SchemeName = "EafApiKey";

        /// <summary>
        /// Prefixo legível de toda chave gerada pelo EAF.
        /// </summary>
        public const string KeyPrefix = "eak_";

        /// <summary>
        /// Quantidade de bytes aleatórios do material secreto da chave.
        /// </summary>
        public const int KeyBytesLength = 32;

        /// <summary>
        /// Tamanho do prefixo exibido para identificação da chave na UI.
        /// </summary>
        public const int DisplayPrefixLength = 12;

        /// <summary>
        /// Claim que marca um principal autenticado via API key (valor = id da chave).
        /// </summary>
        public const string ApiKeyIdClaim = "eaf_api_key_id";

        /// <summary>
        /// Claim que carrega o escopo de permissões da chave (separado por vírgula).
        /// </summary>
        public const string PermissionScopeClaim = "eaf_api_key_scopes";
    }
}

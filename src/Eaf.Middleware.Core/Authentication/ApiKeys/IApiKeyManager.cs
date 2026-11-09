using Abp;
using Abp.Domain.Services;
using System.Net;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authentication.ApiKeys
{
    /// <summary>
    /// Serviço de domínio para emissão, validação e gerenciamento de API keys.
    /// </summary>
    public interface IApiKeyManager : IDomainService
    {
        /// <summary>
        /// Gera uma nova chave para o usuário informado e persiste somente o hash.
        /// Retorna a entidade criada e o material em claro (exibido uma única vez).
        /// </summary>
        /// <param name="user">Dono da chave.</param>
        /// <param name="name">Nome amigável.</param>
        /// <param name="permissionScopes">Escopo de permissões (null/vazio = irrestrito).</param>
        /// <param name="allowedIpRanges">IPs/CIDR permitidos (null/vazio = qualquer IP).</param>
        /// <param name="expirationTime">Expiração UTC (null = sem expiração).</param>
        Task<(ApiKey Entity, string PlainKey)> CreateAsync(
            UserIdentifier user,
            string name,
            string[] permissionScopes = null,
            string[] allowedIpRanges = null,
            System.DateTime? expirationTime = null);

        /// <summary>
        /// Valida uma chave apresentada em uma requisição.
        /// </summary>
        /// <param name="plainKey">Material completo da chave (nunca logado).</param>
        /// <param name="remoteIp">IP de origem da requisição.</param>
        Task<ApiKeyValidationResult> ValidateAsync(string plainKey, IPAddress remoteIp);

        /// <summary>
        /// Revoga uma chave; efeito imediato.
        /// </summary>
        Task RevokeAsync(long apiKeyId);

        /// <summary>
        /// Rotaciona uma chave: revoga a atual e emite uma nova com as mesmas restrições.
        /// </summary>
        Task<(ApiKey Entity, string PlainKey)> RotateAsync(long apiKeyId);

        /// <summary>
        /// Verifica se um IP está dentro da allowlist da chave (ou a allowlist está vazia).
        /// </summary>
        bool IsIpAllowed(ApiKey apiKey, IPAddress remoteIp);

        /// <summary>
        /// Verifica se a chave cobre a permissão informada (ou é irrestrita).
        /// </summary>
        bool IsInScope(ApiKey apiKey, string permissionName);
    }
}

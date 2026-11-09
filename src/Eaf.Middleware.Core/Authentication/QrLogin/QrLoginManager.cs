using Abp.Domain.Services;
using Abp.Runtime.Caching;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authentication.QrLogin
{
    /// <summary>Estados do token de QR login.</summary>
    public enum QrLoginStatus
    {
        /// <summary>Aguardando aprovação por um dispositivo autenticado.</summary>
        Pending = 0,

        /// <summary>Aprovado; aguardando consumo pelo browser que exibiu o QR.</summary>
        Approved = 1,

        /// <summary>Token já consumido (uso único).</summary>
        Consumed = 2,
    }

    /// <summary>
    /// Token de QR login mantido apenas em cache (TTL curto, single-use).
    /// </summary>
    [Serializable]
    public class QrLoginToken
    {
        /// <summary>Id público do token (conteúdo do QR).</summary>
        public string TokenId { get; set; }

        /// <summary>Status atual.</summary>
        public QrLoginStatus Status { get; set; }

        /// <summary>Usuário que aprovou (quando <see cref="QrLoginStatus.Approved"/>).</summary>
        public long? ApprovedUserId { get; set; }

        /// <summary>Tenant do usuário aprovador.</summary>
        public int? ApprovedTenantId { get; set; }
    }

    /// <summary>
    /// Gerencia o ciclo de vida de tokens de QR login em cache distribuído.
    /// </summary>
    public interface IQrLoginManager
    {
        /// <summary>Cria um token pendente com TTL curto.</summary>
        Task<QrLoginToken> CreateAsync();

        /// <summary>Retorna o token ou <c>null</c> quando expirado/inexistente.</summary>
        Task<QrLoginToken> GetOrNullAsync(string tokenId);

        /// <summary>Marca como aprovado; <c>false</c> se não estiver pendente.</summary>
        Task<bool> ApproveAsync(string tokenId, long userId, int? tenantId);

        /// <summary>Consome um token aprovado (single-use); <c>null</c> se não aprovável.</summary>
        Task<QrLoginToken> ConsumeAsync(string tokenId);
    }

    /// <summary>
    /// Implementação de <see cref="IQrLoginManager"/> sobre <see cref="ICacheManager"/>.
    /// </summary>
    public class QrLoginManager : DomainService, IQrLoginManager
    {
        /// <summary>Nome do cache de tokens de QR login.</summary>
        public const string CacheName = "EafQrLoginTokens";

        /// <summary>TTL do token em segundos (2 minutos).</summary>
        public const int TokenTimeToLiveSeconds = 120;

        private readonly ITypedCache<string, QrLoginToken> _cache;

        /// <summary>
        /// QrLoginManager.
        /// </summary>
        public QrLoginManager(ICacheManager cacheManager)
        {
            _cache = cacheManager.GetCache<string, QrLoginToken>(CacheName);
        }

        /// <inheritdoc />
        public virtual async Task<QrLoginToken> CreateAsync()
        {
            var token = new QrLoginToken
            {
                TokenId = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant(),
                Status = QrLoginStatus.Pending,
            };

            await _cache.SetAsync(
                token.TokenId,
                token,
                absoluteExpireTime: DateTimeOffset.UtcNow.AddSeconds(TokenTimeToLiveSeconds));

            return token;
        }

        /// <inheritdoc />
        public virtual async Task<QrLoginToken> GetOrNullAsync(string tokenId)
        {
            if (string.IsNullOrWhiteSpace(tokenId) || tokenId.Length > 64)
            {
                return null;
            }

            return await _cache.GetOrDefaultAsync(tokenId);
        }

        /// <inheritdoc />
        public virtual async Task<bool> ApproveAsync(string tokenId, long userId, int? tenantId)
        {
            var token = await GetOrNullAsync(tokenId);
            if (token == null || token.Status != QrLoginStatus.Pending)
            {
                return false;
            }

            token.Status = QrLoginStatus.Approved;
            token.ApprovedUserId = userId;
            token.ApprovedTenantId = tenantId;

            await _cache.SetAsync(
                tokenId,
                token,
                absoluteExpireTime: DateTimeOffset.UtcNow.AddSeconds(TokenTimeToLiveSeconds));
            return true;
        }

        /// <inheritdoc />
        public virtual async Task<QrLoginToken> ConsumeAsync(string tokenId)
        {
            var token = await GetOrNullAsync(tokenId);
            if (token == null || token.Status != QrLoginStatus.Approved)
            {
                return null;
            }

            token.Status = QrLoginStatus.Consumed;
            await _cache.SetAsync(
                tokenId,
                token,
                absoluteExpireTime: DateTimeOffset.UtcNow.AddSeconds(TokenTimeToLiveSeconds));
            return token;
        }
    }
}

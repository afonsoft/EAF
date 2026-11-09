using Abp;
using Abp.Domain.Services;
using Abp.Runtime.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authentication.Sessions
{
    /// <summary>
    /// Registro consultável de sessões ativas (uma sessão = um refresh token vivo).
    /// Duas estruturas de cache: sessão por id e índice sessões-por-usuário, mais uma
    /// denylist de sessões revogadas para invalidar access tokens ainda válidos.
    /// </summary>
    public interface ISessionRegistry
    {
        /// <summary>Registra/atualiza uma sessão.</summary>
        Task SetAsync(UserSessionInfo session);

        /// <summary>Versão síncrona de <see cref="GetAsync"/> para caminhos não-async (ex.: validação JWT).</summary>
        UserSessionInfo Get(string sessionId);

        /// <summary>Versão síncrona de <see cref="IsRevokedAsync"/>.</summary>
        bool IsRevoked(string sessionId);

        /// <summary>Versão síncrona de <see cref="TouchAsync"/>.</summary>
        void Touch(string sessionId);

        /// <summary>Busca uma sessão pelo id.</summary>
        Task<UserSessionInfo> GetAsync(string sessionId);

        /// <summary>Lista as sessões ativas do usuário.</summary>
        Task<List<UserSessionInfo>> GetForUserAsync(UserIdentifier user);

        /// <summary>Atualiza LastActivityTime (idempotente; caller controla o throttle).</summary>
        Task TouchAsync(string sessionId);

        /// <summary>Revoga uma sessão específica (entra na denylist).</summary>
        Task RevokeAsync(string sessionId);

        /// <summary>Revoga todas as sessões do usuário, exceto opcionalmente a corrente.</summary>
        Task RevokeAllForUserAsync(UserIdentifier user, string exceptSessionId = null);

        /// <summary>Verdadeiro se a sessão foi revogada (denylist).</summary>
        Task<bool> IsRevokedAsync(string sessionId);
    }

    /// <summary>
    /// Implementação de <see cref="ISessionRegistry"/> sobre <see cref="ICacheManager"/>.
    /// </summary>
    public class SessionRegistry : DomainService, ISessionRegistry
    {
        /// <summary>Nome do cache de sessões por id.</summary>
        public const string SessionsCacheName = "EafUserSessions";

        /// <summary>Nome do cache do índice sessões-por-usuário.</summary>
        public const string IndexCacheName = "EafUserSessionIndex";

        /// <summary>Nome da denylist de sessões revogadas.</summary>
        public const string RevokedCacheName = "EafRevokedSessions";

        private static readonly TimeSpan RevokedTtl = TimeSpan.FromDays(30);

        private readonly ITypedCache<string, UserSessionInfo> _sessions;
        private readonly ITypedCache<string, List<string>> _index;
        private readonly ITypedCache<string, DateTime> _revoked;
        private readonly ICacheManager _cacheManager;

        /// <summary>
        /// SessionRegistry.
        /// </summary>
        public SessionRegistry(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            _sessions = cacheManager.GetCache<string, UserSessionInfo>(SessionsCacheName);
            _index = cacheManager.GetCache<string, List<string>>(IndexCacheName);
            _revoked = cacheManager.GetCache<string, DateTime>(RevokedCacheName);
        }

        /// <inheritdoc />
        public async Task SetAsync(UserSessionInfo session)
        {
            if (session == null || string.IsNullOrEmpty(session.SessionId))
            {
                return;
            }

            await _sessions.SetAsync(session.SessionId, session,
                absoluteExpireTime: new DateTimeOffset(session.ExpireDate));
            await AddToIndexAsync(session);
        }

        /// <inheritdoc />
        public Task<UserSessionInfo> GetAsync(string sessionId)
        {
            return string.IsNullOrEmpty(sessionId)
                ? Task.FromResult<UserSessionInfo>(null)
                : _sessions.GetOrDefaultAsync(sessionId);
        }

        /// <inheritdoc />
        public async Task<List<UserSessionInfo>> GetForUserAsync(UserIdentifier user)
        {
            var ids = (await _index.GetOrDefaultAsync(IndexKey(user))) ?? new List<string>();
            var result = new List<UserSessionInfo>();
            var stale = new List<string>();
            foreach (var id in ids)
            {
                var session = await _sessions.GetOrDefaultAsync(id);
                if (session == null || session.ExpireDate < DateTime.UtcNow)
                {
                    stale.Add(id);
                }
                else if (!(await IsRevokedAsync(id)))
                {
                    result.Add(session);
                }
            }

            if (stale.Count > 0)
            {
                ids = ids.Except(stale).ToList();
                await _index.SetAsync(IndexKey(user), ids);
            }

            return result.OrderByDescending(s => s.LastActivityTime).ToList();
        }

        /// <inheritdoc />
        public async Task TouchAsync(string sessionId)
        {
            var session = await _sessions.GetOrDefaultAsync(sessionId);
            if (session == null)
            {
                return;
            }

            session.LastActivityTime = DateTime.UtcNow;
            await _sessions.SetAsync(session.SessionId, session,
                absoluteExpireTime: new DateTimeOffset(session.ExpireDate));
        }

        /// <inheritdoc />
        public async Task RevokeAsync(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return;
            }

            await _revoked.SetAsync(sessionId, DateTime.UtcNow, absoluteExpireTime: DateTimeOffset.UtcNow.Add(RevokedTtl));
            await _sessions.RemoveAsync(sessionId);
        }

        /// <inheritdoc />
        public async Task RevokeAllForUserAsync(UserIdentifier user, string exceptSessionId = null)
        {
            var ids = (await _index.GetOrDefaultAsync(IndexKey(user))) ?? new List<string>();
            foreach (var id in ids.Where(i => i != exceptSessionId).ToList())
            {
                await RevokeAsync(id);
            }

            var remaining = exceptSessionId == null
                ? new List<string>()
                : new List<string> { exceptSessionId };
            await _index.SetAsync(IndexKey(user), remaining);
        }

        /// <inheritdoc />
        public async Task<bool> IsRevokedAsync(string sessionId)
        {
            return !string.IsNullOrEmpty(sessionId)
                && (await _revoked.GetOrDefaultAsync(sessionId)) != default;
        }

        /// <inheritdoc />
        public UserSessionInfo Get(string sessionId)
        {
            return string.IsNullOrEmpty(sessionId) ? null : _sessions.GetOrDefault(sessionId);
        }

        /// <inheritdoc />
        public bool IsRevoked(string sessionId)
        {
            return !string.IsNullOrEmpty(sessionId)
                && _revoked.GetOrDefault(sessionId) != default;
        }

        /// <inheritdoc />
        public void Touch(string sessionId)
        {
            var session = _sessions.GetOrDefault(sessionId);
            if (session == null)
            {
                return;
            }

            session.LastActivityTime = DateTime.UtcNow;
            _sessions.Set(session.SessionId, session,
                absoluteExpireTime: new DateTimeOffset(session.ExpireDate));
        }

        private static string IndexKey(UserIdentifier user) => $"{user.TenantId}:{user.UserId}";

        private async Task AddToIndexAsync(UserSessionInfo session)
        {
            var key = $"{session.TenantId}:{session.UserId}";
            var ids = (await _index.GetOrDefaultAsync(key)) ?? new List<string>();
            if (!ids.Contains(session.SessionId))
            {
                ids.Add(session.SessionId);
                await _index.SetAsync(key, ids);
            }
        }
    }
}

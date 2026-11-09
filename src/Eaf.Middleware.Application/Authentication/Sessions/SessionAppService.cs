using Abp;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Runtime.Session;
using Abp.UI;
using Eaf.Middleware.Authentication.Sessions;
using Eaf.Middleware.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authentication.Sessions
{
    /// <summary>
    /// Sessões ativas do usuário autenticado (listar/revogar).
    /// </summary>
    public interface ISessionAppService : IApplicationService
    {
        /// <summary>Lista as sessões ativas do usuário corrente.</summary>
        Task<List<UserSessionDto>> GetMySessions();

        /// <summary>Revoga uma sessão do usuário corrente.</summary>
        Task RevokeMySession(RevokeSessionInput input);

        /// <summary>Revoga todas as sessões do usuário; opcionalmente preserva a corrente.</summary>
        Task RevokeAllMySessions(RevokeAllSessionsInput input);
    }

    /// <summary>
    /// Sessões de usuários do tenant (admin).
    /// </summary>
    public interface IAdminSessionAppService : IApplicationService
    {
        /// <summary>Lista as sessões ativas de um usuário do tenant.</summary>
        Task<List<UserSessionDto>> GetUserSessions(GetUserSessionsInput input);

        /// <summary>Revoga uma sessão específica.</summary>
        Task RevokeSession(RevokeSessionInput input);

        /// <summary>Revoga todas as sessões de um usuário.</summary>
        Task RevokeAllUserSessions(RevokeAllUserSessionsInput input);
    }

    /// <summary>DTO de sessão exposta ao client (sem refresh token).</summary>
    public class UserSessionDto
    {
        /// <summary>Id opaco da sessão (necessário para revogação).</summary>
        public string SessionId { get; set; }

        /// <summary>IP do cliente no login.</summary>
        public string IpAddress { get; set; }

        /// <summary>User agent do cliente.</summary>
        public string UserAgent { get; set; }

        /// <summary>Criação UTC.</summary>
        public DateTime CreationTime { get; set; }

        /// <summary>Última atividade UTC.</summary>
        public DateTime LastActivityTime { get; set; }

        /// <summary>Verdadeiro para a sessão que originou a chamada.</summary>
        public bool IsCurrent { get; set; }
    }

    /// <summary>Input de revogação de uma sessão.</summary>
    public class RevokeSessionInput
    {
        /// <summary>Id da sessão.</summary>
        public string SessionId { get; set; }
    }

    /// <summary>Input de revogação em massa das próprias sessões.</summary>
    public class RevokeAllSessionsInput
    {
        /// <summary>Preservar a sessão corrente.</summary>
        public bool KeepCurrentSession { get; set; }
    }

    /// <summary>Input de listagem de sessões de um usuário (admin).</summary>
    public class GetUserSessionsInput
    {
        /// <summary>Id do usuário alvo.</summary>
        public long UserId { get; set; }

        /// <summary>Tenant do usuário alvo (null = host).</summary>
        public int? TenantId { get; set; }
    }

    /// <summary>Input de revogação em massa por usuário (admin).</summary>
    public class RevokeAllUserSessionsInput
    {
        /// <summary>Id do usuário alvo.</summary>
        public long UserId { get; set; }

        /// <summary>Tenant do usuário alvo.</summary>
        public int? TenantId { get; set; }
    }

    /// <summary>
    /// Implementação de <see cref="ISessionAppService"/>.
    /// </summary>
    [AbpAuthorize]
    public class SessionAppService : MiddlewareAppServiceBase, ISessionAppService
    {
        private readonly ISessionRegistry _sessionRegistry;
        private readonly IPrincipalAccessor _principalAccessor;

        /// <summary>
        /// SessionAppService.
        /// </summary>
        public SessionAppService(ISessionRegistry sessionRegistry, IPrincipalAccessor principalAccessor)
        {
            _sessionRegistry = sessionRegistry;
            _principalAccessor = principalAccessor;
        }

        /// <inheritdoc />
        public async Task<List<UserSessionDto>> GetMySessions()
        {
            var sessions = await _sessionRegistry.GetForUserAsync(AbpSession.ToUserIdentifier());
            var current = CurrentSessionId();
            return sessions.Select(s => ToDto(s, s.SessionId == current)).ToList();
        }

        /// <inheritdoc />
        public async Task RevokeMySession(RevokeSessionInput input)
        {
            if (string.IsNullOrEmpty(input?.SessionId))
            {
                throw new UserFriendlyException(L("InvalidRequest"));
            }

            var sessions = await _sessionRegistry.GetForUserAsync(AbpSession.ToUserIdentifier());
            if (sessions.All(s => s.SessionId != input.SessionId))
            {
                // Não pertence ao usuário — responde como "não existe" (não revela ids alheios).
                throw new UserFriendlyException(L("SessionNotFound"));
            }

            await _sessionRegistry.RevokeAsync(input.SessionId);
        }

        /// <inheritdoc />
        public async Task RevokeAllMySessions(RevokeAllSessionsInput input)
        {
            await _sessionRegistry.RevokeAllForUserAsync(
                AbpSession.ToUserIdentifier(),
                input?.KeepCurrentSession == true ? CurrentSessionId() : null);
        }

        private string CurrentSessionId()
        {
            return _principalAccessor?.Principal?.Claims
                .FirstOrDefault(c => c.Type == SessionConsts.SessionIdClaim)?.Value;
        }

        private static UserSessionDto ToDto(UserSessionInfo s, bool isCurrent) => new UserSessionDto
        {
            SessionId = s.SessionId,
            IpAddress = s.IpAddress,
            UserAgent = s.UserAgent,
            CreationTime = s.CreationTime,
            LastActivityTime = s.LastActivityTime,
            IsCurrent = isCurrent
        };
    }

    /// <summary>
    /// Implementação de <see cref="IAdminSessionAppService"/>.
    /// </summary>
    [AbpAuthorize(MiddlewarePermissions.Pages_Administration_UserSessions)]
    public class AdminSessionAppService : MiddlewareAppServiceBase, IAdminSessionAppService
    {
        private readonly ISessionRegistry _sessionRegistry;

        /// <summary>
        /// AdminSessionAppService.
        /// </summary>
        public AdminSessionAppService(ISessionRegistry sessionRegistry)
        {
            _sessionRegistry = sessionRegistry;
        }

        /// <inheritdoc />
        public async Task<List<UserSessionDto>> GetUserSessions(GetUserSessionsInput input)
        {
            var sessions = await _sessionRegistry.GetForUserAsync(
                new UserIdentifier(input.TenantId, input.UserId));
            return sessions.Select(s => new UserSessionDto
            {
                SessionId = s.SessionId,
                IpAddress = s.IpAddress,
                UserAgent = s.UserAgent,
                CreationTime = s.CreationTime,
                LastActivityTime = s.LastActivityTime,
                IsCurrent = false
            }).ToList();
        }

        /// <inheritdoc />
        public Task RevokeSession(RevokeSessionInput input)
        {
            if (string.IsNullOrEmpty(input?.SessionId))
            {
                throw new UserFriendlyException(L("InvalidRequest"));
            }
            return _sessionRegistry.RevokeAsync(input.SessionId);
        }

        /// <inheritdoc />
        public Task RevokeAllUserSessions(RevokeAllUserSessionsInput input)
        {
            return _sessionRegistry.RevokeAllForUserAsync(
                new UserIdentifier(input.TenantId, input.UserId));
        }
    }
}

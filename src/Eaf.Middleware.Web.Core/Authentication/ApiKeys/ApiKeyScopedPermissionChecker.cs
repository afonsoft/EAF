using Abp;
using Abp.Authorization;
using Abp.Runtime.Session;
using Eaf.Middleware.Authentication.ApiKeys;
using Eaf.Middleware.Authorization.Users;
using System;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Authentication.ApiKeys
{
    /// <summary>
    /// <see cref="IPermissionChecker"/> que restringe as permissões ao escopo declarado na API key
    /// quando a requisição foi autenticada pelo esquema <see cref="ApiKeyConsts.SchemeName"/>.
    /// Para qualquer outra autenticação (ou chave sem escopo), delega ao comportamento padrão do ABP.
    /// </summary>
    public class ApiKeyScopedPermissionChecker : Eaf.Middleware.Authorization.PermissionChecker
    {
        private readonly IPrincipalAccessor _principalAccessor;

        /// <summary>
        /// ApiKeyScopedPermissionChecker.
        /// </summary>
        public ApiKeyScopedPermissionChecker(UserManager userManager, IPrincipalAccessor principalAccessor)
            : base(userManager)
        {
            _principalAccessor = principalAccessor;
        }

        /// <inheritdoc />
        public override Task<bool> IsGrantedAsync(string permissionName)
        {
            var scope = GetApiKeyScope();
            if (scope != null)
            {
                return Task.FromResult(scope.Contains(permissionName));
            }
            return base.IsGrantedAsync(permissionName);
        }

        /// <inheritdoc />
        public override Task<bool> IsGrantedAsync(long userId, string permissionName)
        {
            var scope = GetApiKeyScope();
            if (scope != null)
            {
                return Task.FromResult(scope.Contains(permissionName));
            }
            return base.IsGrantedAsync(userId, permissionName);
        }

        /// <inheritdoc />
        public override Task<bool> IsGrantedAsync(UserIdentifier user, string permissionName)
        {
            var scope = GetApiKeyScope();
            if (scope != null)
            {
                return Task.FromResult(scope.Contains(permissionName));
            }
            return base.IsGrantedAsync(user, permissionName);
        }

        /// <summary>
        /// Retorna o escopo da API key do principal atual, ou null quando a requisição
        /// não é de API key com escopo restrito.
        /// </summary>
        private string[] GetApiKeyScope()
        {
            var principal = _principalAccessor?.Principal;
            if (principal?.Identity?.IsAuthenticated != true
                || !string.Equals(principal.Identity.AuthenticationType, ApiKeyConsts.SchemeName, StringComparison.Ordinal))
            {
                return null;
            }

            var scopeClaim = principal.FindFirst(ApiKeyConsts.PermissionScopeClaim)?.Value;
            if (string.IsNullOrWhiteSpace(scopeClaim))
            {
                // API key sem escopo explícito: usa as permissões completas do usuário.
                return null;
            }

            return scopeClaim.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }
    }
}

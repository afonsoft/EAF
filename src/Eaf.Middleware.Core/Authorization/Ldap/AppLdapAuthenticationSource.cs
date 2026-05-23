using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Ldap.Authentication;
using Eaf.Middleware.Ldap.Configuration;
using Eaf.Middleware.MultiTenancy;

namespace Eaf.Middleware.Authorization.Ldap
{
    /// <summary>
    /// Representa a classe AppLdapAuthenticationSource.
    /// </summary>
    public class AppLdapAuthenticationSource : LdapAuthenticationSource<Tenant, User>
    {
        /// <summary>
        /// AppLdapAuthenticationSource.
        /// </summary>
        /// <param name="settings">Parâmetro settings.</param>
        /// <param name="ldapModuleConfig">Parâmetro ldapModuleConfig.</param>
        /// <returns>Resultado da operação.</returns>
        public AppLdapAuthenticationSource(ILdapSettings settings, IEafMiddlewareLdapModuleConfig ldapModuleConfig)
            : base(settings, ldapModuleConfig)
        {
        }
    }
}
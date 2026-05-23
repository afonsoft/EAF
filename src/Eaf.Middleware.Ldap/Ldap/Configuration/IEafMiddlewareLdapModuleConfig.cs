using System;

namespace Eaf.Middleware.Ldap.Configuration
{
    /// <summary>
    /// Representa a interface IEafMiddlewareLdapModuleConfig.
    /// </summary>
    public interface IEafMiddlewareLdapModuleConfig
    {
        Type AuthenticationSourceType { get; }
        bool IsEnabled { get; }

        bool UseNovellProvider { get; set; }

        void Enable(Type authenticationSourceType);
    }
}
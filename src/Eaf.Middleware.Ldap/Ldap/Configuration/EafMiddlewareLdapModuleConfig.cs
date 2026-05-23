using Abp.Zero.Configuration;
using System;

namespace Eaf.Middleware.Ldap.Configuration
{
    /// <summary>
    /// Representa a classe EafMiddlewareLdapModuleConfig.
    /// </summary>
    public class EafMiddlewareLdapModuleConfig : IEafMiddlewareLdapModuleConfig
    {
        private readonly IAbpZeroConfig _middlewareConfig;

        /// <summary>
        /// EafMiddlewareLdapModuleConfig.
        /// </summary>
        /// <param name="middlewareConfig">Parâmetro middlewareConfig.</param>
        /// <returns>Resultado da operação.</returns>
        public EafMiddlewareLdapModuleConfig(IAbpZeroConfig middlewareConfig)
        {
            _middlewareConfig = middlewareConfig;
        }

        /// <summary>
        /// Obtém ou define AuthenticationSourceType.
        /// </summary>
        public Type AuthenticationSourceType { get; private set; }
        /// <summary>
        /// Obtém ou define IsEnabled.
        /// </summary>
        public bool IsEnabled { get; private set; }

        /// <summary>
        /// User a <see cref="Novell.Directory.Ldap"/> also a <see cref="System.DirectoryServices"/>, in OS not Windows this is true default.
        /// </summary>
        public bool UseNovellProvider { get; set; }

        /// <summary>
        /// Enable.
        /// </summary>
        /// <param name="authenticationSourceType">Parâmetro authenticationSourceType.</param>
        public void Enable(Type authenticationSourceType)
        {
            AuthenticationSourceType = authenticationSourceType;
            IsEnabled = true;
            UseNovellProvider = !OperatingSystem.IsWindows();

            _middlewareConfig.UserManagement.ExternalAuthenticationSources.Add(authenticationSourceType);
        }
    }
}
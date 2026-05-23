using Abp.Zero.Configuration;
using System;

namespace Eaf.Middleware.AzureActiveDirectory.Configuration
{
    /// <summary>
    /// Representa a classe EafMiddlewareAzureActiveDirectoryModuleConfig.
    /// </summary>
    public class EafMiddlewareAzureActiveDirectoryModuleConfig : IEafMiddlewareAzureActiveDirectoryModuleConfig
    {
        private readonly IAbpZeroConfig _middlewareConfig;

        /// <summary>
        /// EafMiddlewareAzureActiveDirectoryModuleConfig.
        /// </summary>
        /// <param name="middlewareConfig">Parâmetro middlewareConfig.</param>
        /// <returns>Resultado da operação.</returns>
        public EafMiddlewareAzureActiveDirectoryModuleConfig(IAbpZeroConfig middlewareConfig)
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
        /// Enable.
        /// </summary>
        /// <param name="authenticationSourceType">Parâmetro authenticationSourceType.</param>
        public void Enable(Type authenticationSourceType)
        {
            AuthenticationSourceType = authenticationSourceType;
            IsEnabled = true;

            _middlewareConfig.UserManagement.ExternalAuthenticationSources.Add(authenticationSourceType);
        }
    }
}
using System;

namespace Eaf.Middleware.AzureActiveDirectory.Configuration
{
    /// <summary>
    /// Representa a interface IEafMiddlewareAzureActiveDirectoryModuleConfig.
    /// </summary>
    public interface IEafMiddlewareAzureActiveDirectoryModuleConfig
    {
        Type AuthenticationSourceType { get; }
        bool IsEnabled { get; }

        void Enable(Type authenticationSourceType);
    }
}
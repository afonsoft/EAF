using System.Collections.Generic;

namespace Eaf.Middleware.Core.Authentication.External
{
    /// <summary>
    /// Representa a interface IExternalAuthConfiguration.
    /// </summary>
    public interface IExternalAuthConfiguration
    {
        List<IExternalLoginInfoProvider> ExternalLoginInfoProviders { get; }
    }
}
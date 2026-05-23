using Microsoft.Extensions.Configuration;

namespace Eaf.Middleware.Configuration
{
    /// <summary>
    /// Representa a interface IAppConfigurationAccessor.
    /// </summary>
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}
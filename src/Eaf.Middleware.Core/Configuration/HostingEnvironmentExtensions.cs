using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Eaf.Middleware.Configuration
{
    /// <summary>
    /// Representa a classe HostingEnvironmentExtensions.
    /// </summary>
    public static class HostingEnvironmentExtensions
    {
        /// <summary>
        /// GetAppConfiguration
        /// </summary>
        /// <param name="env">IWebHostEnvironment</param>
        /// <returns>IConfigurationRoot</returns>
        public static IConfigurationRoot GetAppConfiguration(this IWebHostEnvironment env)
        {
            return AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName, env.IsDevelopment());
        }

        /// <summary>
        /// GetAppConfiguration
        /// </summary>
        /// <param name="env">IHostEnvironment</param>
        /// <returns>IConfigurationRoot</returns>
        public static IConfigurationRoot GetAppConfiguration(this IHostEnvironment env)
        {
            return AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName, env.IsDevelopment());
        }
    }
}
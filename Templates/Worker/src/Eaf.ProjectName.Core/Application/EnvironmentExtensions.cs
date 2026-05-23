using Eaf.ProjectName.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Eaf.ProjectName.Application
{
    public static class EnvironmentExtensions
    {
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
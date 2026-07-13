using Abp;
using Abp.Configuration.Startup;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Eaf.Middleware.Web.Configuration
{
    /// <summary>
    /// Representa a classe EafStartupConfigurationExtensions.
    /// </summary>
    public static class EafStartupConfigurationExtensions
    {
        /// <summary>
        /// SetConfiguration.
        /// </summary>
        /// <param name="configuration">Parâmetro configuration.</param>
        /// <param name="sections">Parâmetro sections.</param>
        public static void SetConfiguration(this IAbpStartupConfiguration configuration, IEnumerable<IConfigurationSection> sections)
        {
            foreach (var section in sections.Where(s => s != null && s.Exists() && s.GetChildren().Any()))
            {
                configuration.Set(section.Key, GetChildren(section));
            }
        }

        /// <summary>
        /// SetConfiguration.
        /// </summary>
        /// <param name="configuration">Parâmetro configuration.</param>
        /// <param name="section">Parâmetro section.</param>
        public static void SetConfiguration(this IAbpStartupConfiguration configuration, IConfigurationSection section)
        {
            if (section != null && section.Exists() && section.GetChildren().Any())
                configuration.Set(section.Key, GetChildren(section));
        }

        private static Dictionary<string, object> GetChildren(IConfigurationSection section)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            if (section != null && section.Exists() && section.GetChildren().Any())
            {
                foreach (var children in section.GetChildren().AsEnumerable())
                {
                    string key = children.Key.Replace($"{section.Key}:", "");

                    if (dic.ContainsKey(key))
                        throw new AbpException($"DUPLICATE KEY - This key {key} already exists in appsettings.json");

                    if (children.GetChildren().Any())
                        dic.Add(key, GetChildren(children));
                    else
                        dic.Add(key, (object)children.Value);
                }
            }

            return dic;
        }
    }
}
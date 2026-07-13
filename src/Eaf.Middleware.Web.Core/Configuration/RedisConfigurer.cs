using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eaf.Middleware.Web.Startup
{
    /// <summary>
    /// Representa a classe RedisConfigurer.
    /// </summary>
    public static class RedisConfigurer
    {
        /// <summary>
        /// Configure.
        /// </summary>
        /// <param name="services">Parâmetro services.</param>
        /// <param name="configuration">Parâmetro configuration.</param>
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            if (((configuration["RedisCache:IsRedisEnabled"] != null && bool.Parse(configuration["RedisCache:IsRedisEnabled"]))
               || configuration["RedisCache:IsEnabled"] != null && bool.Parse(configuration["RedisCache:IsEnabled"]))
               && configuration["RedisCache:ConnectionString"] != null)
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration["RedisCache:ConnectionString"];
                });
            }
        }
    }
}
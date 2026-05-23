using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eaf.Configuration
{
    /// <summary>
    /// Representa a classe SqlServerCacheConfigurer.
    /// </summary>
    public static class SqlServerCacheConfigurer
    {
        /// <summary>
        /// Configure.
        /// </summary>
        /// <param name="services">Parâmetro services.</param>
        /// <param name="configuration">Parâmetro configuration.</param>
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            if ((configuration["SqlServer:IsSqlEnabled"] != null && bool.Parse(configuration["SqlServer:IsSqlEnabled"]))
               || configuration["SqlServerCache:IsEnabled"] != null && bool.Parse(configuration["SqlServerCache:IsEnabled"]))
            {
                services.AddDistributedSqlServerCache(options =>
                {
                    options.ConnectionString = configuration["SqlServerCache:ConnectionString"] ?? configuration.GetConnectionString("Default");
                    options.SchemaName = configuration["SqlServerCache:SchemaName"] ?? null;
                    options.TableName = configuration["SqlServerCache:TableName"] ?? "EafCache";
                });
            }
        }
    }
}
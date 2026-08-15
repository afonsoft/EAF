using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eaf.SignalR.Configuration
{
    /// <summary>
    /// Extensões para configuração do Eaf.SignalR no <see cref="IServiceCollection"/>.
    /// </summary>
    public static class EafSignalRServiceCollectionExtensions
    {
        /// <summary>
        /// Registra o SignalR com as configurações do EAF, incluindo Redis backplane quando habilitado.
        /// </summary>
        /// <param name="services">Coleção de serviços.</param>
        /// <param name="configuration">Configuração da aplicação.</param>
        /// <returns>A mesma <see cref="IServiceCollection"/> para encadeamento.</returns>
        public static IServiceCollection AddEafSignalR(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("EafSignalR");
            var options = section.Get<EafSignalROptions>() ?? new EafSignalROptions();

            var detailedErrors = options.UseDetailedErrors ?? !IsProductionEnvironment();
            var handshakeTimeout = GetTimeSpan(options.HandshakeTimeoutSeconds, TimeSpan.FromSeconds(30));
            var keepAliveInterval = GetTimeSpan(options.KeepAliveIntervalSeconds, TimeSpan.FromSeconds(30));
            var clientTimeoutInterval = GetTimeSpan(options.ClientTimeoutIntervalSeconds, TimeSpan.FromSeconds(60));

            var signalRBuilder = services.AddSignalR(signalROptions =>
            {
                signalROptions.EnableDetailedErrors = detailedErrors;
                signalROptions.HandshakeTimeout = handshakeTimeout;
                signalROptions.KeepAliveInterval = keepAliveInterval;
                signalROptions.ClientTimeoutInterval = clientTimeoutInterval;
            });

            if (options.UseRedisBackplane)
            {
                var connectionString = GetRedisConnectionString(options, configuration);

                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    signalRBuilder.AddStackExchangeRedis(connectionString, redisOptions =>
                    {
                        if (options.RedisDatabase.HasValue)
                            redisOptions.Configuration.DefaultDatabase = options.RedisDatabase.Value;
                    });
                }
            }

            services.Configure<EafSignalROptions>(section);

            return services;
        }

        private static string GetRedisConnectionString(EafSignalROptions options, IConfiguration configuration)
        {
            if (!string.IsNullOrWhiteSpace(options.RedisConnectionString))
                return options.RedisConnectionString;

            return configuration["RedisCache:ConnectionString"] ?? string.Empty;
        }

        private static TimeSpan GetTimeSpan(int seconds, TimeSpan defaultValue)
        {
            return seconds > 0 ? TimeSpan.FromSeconds(seconds) : defaultValue;
        }

        private static bool IsProductionEnvironment()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                ?? "Production";

            return !env.Equals("Development", StringComparison.OrdinalIgnoreCase);
        }
    }
}

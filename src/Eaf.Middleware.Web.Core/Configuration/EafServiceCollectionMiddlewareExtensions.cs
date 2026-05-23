using Abp.AspNetCore.Webhook;
using Eaf.Configuration;
using Eaf.Middleware.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO.Compression;

namespace Eaf.Middleware.Web.Startup
{
    /// <summary>
    /// Representa a classe EafServiceCollectionMiddlewareExtensions.
    /// </summary>
    public static class EafServiceCollectionMiddlewareExtensions
    {
        /// <summary>
        /// IdentityRegistrar, AuthConfigurer, HangFireConfigurer, RedisConfigurer, HealthChecks
        /// </summary>
        public static void AddEafConfigurer(this IServiceCollection services, IConfiguration configuration)
        {
            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, configuration);
            HangFireConfigurer.Configure(services, configuration);
            RedisConfigurer.Configure(services, configuration);
            SqlServerCacheConfigurer.Configure(services, configuration);

            services.AddHttpClient();
            services.AddHttpClient(AspNetCoreWebhookSender.WebhookSenderHttpClientName);

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.HandshakeTimeout = TimeSpan.FromSeconds(30);
                options.KeepAliveInterval = TimeSpan.FromSeconds(30);
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
            });

            services.AddMemoryCache();
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = false;
            });

            // Cookie configuration for HTTP to support cookies with SameSite=None
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.Providers.Add<BrotliCompressionProvider>();
                options.EnableForHttps = false;
            });

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });
        }
    }
}
using Abp.AspNetCore.Webhook;
using Eaf.Configuration;
using Eaf.Middleware.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO.Compression;
using System.Linq;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

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

            var isProduction = !IsDevelopmentEnvironment();

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = !isProduction;
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
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = isProduction ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
                options.Secure = isProduction ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
            });

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.Providers.Add<BrotliCompressionProvider>();
                options.EnableForHttps = false;
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    "application/json",
                    "application/javascript",
                    "text/css",
                    "text/html",
                    "text/json",
                    "text/plain",
                    "text/xml"
                });
            });

            var compressionLevel = isProduction ? CompressionLevel.Optimal : CompressionLevel.Fastest;

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = compressionLevel;
            });
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = compressionLevel;
            });

            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter(policyName: "EafGlobal", opt =>
                {
                    opt.PermitLimit = 100;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 2;
                });

                options.AddSlidingWindowLimiter(policyName: "EafAuth", opt =>
                {
                    opt.PermitLimit = 10;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.SegmentsPerWindow = 1;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });

                options.OnRejected = (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    return new ValueTask();
                };
            });
        }

        private static bool IsDevelopmentEnvironment()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                ?? "Production";

            return env.Equals("Development", StringComparison.OrdinalIgnoreCase);
        }
    }
}
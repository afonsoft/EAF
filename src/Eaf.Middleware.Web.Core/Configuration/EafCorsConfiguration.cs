using Abp.Extensions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Eaf.Middleware.Web.Startup
{
    /// <summary>
    /// Registers a safe CORS policy that reflects the caller origin instead of emitting
    /// <c>Access-Control-Allow-Origin: *</c> (which browsers reject when credentials are sent).
    /// The policy also allows all headers sent by the Angular <c>EafHttpInterceptor</c>.
    /// </summary>
    public static class EafCorsConfiguration
    {
        /// <summary>
        /// Adds a named EAF CORS policy that supports wildcard subdomains, credentials and all
    /// headers used by the Angular <c>EafHttpInterceptor</c>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">Application configuration used to read <c>App:CorsOrigins</c>.</param>
        /// <param name="isDevelopment">Whether the current hosting environment is Development.</param>
        /// <param name="policyName">Name of the CORS policy to register.</param>
        /// <returns>The same <see cref="IServiceCollection"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown in non-Development environments when <c>App:CorsOrigins</c> is empty or <c>*</c>.
        /// </exception>
        public static IServiceCollection AddEafCors(
            this IServiceCollection services,
            IConfiguration configuration,
            bool isDevelopment,
            string policyName = "EafDefaultCors")
        {
            var corsOrigins = configuration["App:CorsOrigins"];
            var allowedOrigins = SplitOrigins(corsOrigins);

            if (!isDevelopment && (string.IsNullOrWhiteSpace(corsOrigins) || corsOrigins == "*"))
            {
                throw new InvalidOperationException("App:CorsOrigins must be configured with explicit origins in production.");
            }

            services.AddCors(options =>
            {
                options.AddPolicy(policyName, builder =>
                {
                    if (isDevelopment && corsOrigins == "*")
                    {
                        builder.SetIsOriginAllowed(_ => true);
                    }
                    else if (allowedOrigins.Length == 0)
                    {
                        builder.SetIsOriginAllowed(_ => true);
                    }
                    else
                    {
                        builder.SetIsOriginAllowed(origin => IsOriginAllowed(origin, allowedOrigins));
                    }

                    builder
                        .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                        .WithHeaders(
                            "Authorization",
                            "Content-Type",
                            "Accept",
                            "Accept-Language",
                            "Cache-Control",
                            "Expires",
                            "Pragma",
                            "X-Requested-With",
                            "X-Correlation-ID",
                            "Abp-TenantId",
                            "Abp.Localization.CultureName",
                            ".AspNetCore.Culture",
                            "X-XSRF-TOKEN"
                        )
                        .WithExposedHeaders(
                            "X-RateLimit-Limit",
                            "X-RateLimit-Remaining",
                            "X-RateLimit-Reset",
                            "Retry-After"
                        )
                        .AllowCredentials()
                        .SetPreflightMaxAge(TimeSpan.FromSeconds(600));
                });
            });

            return services;
        }

        private static string[] SplitOrigins(string corsOrigins)
        {
            if (string.IsNullOrWhiteSpace(corsOrigins) || corsOrigins == "*")
                return Array.Empty<string>();

            return corsOrigins
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(o => o.Trim().RemovePostFix("/"))
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .ToArray();
        }

        /// <summary>
        /// Supports exact origins and wildcard subdomains such as <c>https://*.example.com</c>.
        /// The wildcard requires at least one non-empty subdomain.
        /// </summary>
        private static bool IsOriginAllowed(string origin, string[] allowedOrigins)
        {
            if (string.IsNullOrWhiteSpace(origin))
                return false;

            foreach (var allowed in allowedOrigins)
            {
                if (string.Equals(allowed, origin, StringComparison.OrdinalIgnoreCase))
                    return true;

                if (allowed.Contains("*"))
                {
                    var escaped = Regex.Escape(allowed).Replace("\\*", "[^./]+");
                    var pattern = "^" + escaped + "$";
                    if (Regex.IsMatch(origin, pattern, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1)))
                        return true;
                }
            }

            return false;
        }
    }
}
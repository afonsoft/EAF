using Abp.Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Representa a classe EafHealthCheckApplicationBuilderExtensions.
    /// </summary>
    public static class EafHealthCheckApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds a middleware that provides health check status. /health
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="options">A Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions used to configure the middleware.</param>
        /// <returns>A reference to the <paramref name="app"/> after the operation has completed.</returns>
        /// <remarks>
        /// <para>
        /// The health check middleware will use default settings from <see cref="IOptions{HealthCheckOptions}"/>.
        /// </para>
        /// </remarks>
        public static IApplicationBuilder UseEafHealthChecks(this IApplicationBuilder app, HealthCheckOptions options = null)
        {
            LogHelper.Logger.DebugFormat("HealthChecksEnpoint {0}", "/health");
            options ??= new HealthCheckOptions();
            return app.UseHealthChecks("/health", options);
        }
    }
}
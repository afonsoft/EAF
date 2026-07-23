using Eaf.Middleware.Web.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Eaf.Middleware.Web.Startup
{
    public static class SecurityHeadersApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds security headers middleware to the application pipeline.
        /// </summary>
        public static IApplicationBuilder UseEafSecurityHeaders(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}

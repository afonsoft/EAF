using Eaf.Middleware.Web.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Eaf.Middleware.Web.Startup
{
    /// <summary>
    /// Extension methods for registering <see cref="EafPublicErrorMiddleware"/>.
    /// </summary>
    public static class PublicErrorApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds <see cref="EafPublicErrorMiddleware"/> to the request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>The same <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseEafPublicErrorMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<EafPublicErrorMiddleware>();
        }
    }
}
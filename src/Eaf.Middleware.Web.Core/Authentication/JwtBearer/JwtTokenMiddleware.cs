using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Authentication.JwtBearer
{
    /// <summary>
    /// Representa a classe JwtTokenMiddleware.
    /// </summary>
    public static class JwtTokenMiddleware
    {
        /// <summary>
        /// Adiciona o middleware de autenticação JWT ao pipeline.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="schema">Schema de autenticação.</param>
        /// <returns>O application builder configurado.</returns>
        public static IApplicationBuilder UseJwtTokenMiddleware(this IApplicationBuilder app, string schema = "Bearer")
        {
            return UseExtensions.Use(app, (Func<HttpContext, Func<Task>, Task>)(async (ctx, next) =>
            {
                IIdentity identity = ctx.User?.Identity;
                if (identity == null || !identity.IsAuthenticated)
                {
                    AuthenticateResult authenticateResult = await AuthenticationHttpContextExtensions.AuthenticateAsync(ctx, schema);
                    if (authenticateResult.Succeeded && authenticateResult.Principal != null)
                        ctx.User = authenticateResult.Principal;
                }
                await next();
            }));
        }
    }
}
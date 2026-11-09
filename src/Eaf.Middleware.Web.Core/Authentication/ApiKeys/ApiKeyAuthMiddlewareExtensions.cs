using Eaf.Middleware.Authentication.ApiKeys;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Authentication.ApiKeys
{
    /// <summary>
    /// Middleware que autentica requisições com o header <c>X-API-Key</c> pelo esquema de API key,
    /// no mesmo padrão de <c>UseJwtTokenMiddleware</c>.
    /// Deve ser registrado após <c>UseAuthentication</c>.
    /// </summary>
    public static class ApiKeyAuthMiddlewareExtensions
    {
        /// <summary>
        /// Tenta autenticar via API key quando o header está presente e o usuário ainda não está autenticado.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="schema">Schema de autenticação.</param>
        /// <returns>O application builder configurado.</returns>
        public static IApplicationBuilder UseApiKeyTokenMiddleware(this IApplicationBuilder app, string schema = ApiKeyConsts.SchemeName)
        {
            return app.Use(async (ctx, next) =>
            {
                if ((ctx.User?.Identity == null || !ctx.User.Identity.IsAuthenticated)
                    && ctx.Request.Headers.ContainsKey(ApiKeyConsts.HeaderName))
                {
                    var authenticateResult = await ctx.AuthenticateAsync(schema);
                    if (authenticateResult.Succeeded && authenticateResult.Principal != null)
                    {
                        ctx.User = authenticateResult.Principal;
                    }
                    else if (authenticateResult.Failure != null)
                    {
                        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await ctx.Response.WriteAsJsonAsync(new { error = authenticateResult.Failure.Message });
                        return;
                    }
                }
                await next();
            });
        }
    }
}

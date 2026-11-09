using Abp.Dependency;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Eaf.RateLimiting
{
    /// <summary>
    /// Middleware que aplica regras de <see cref="EafRateLimitingOptions"/> por IP ou usuário autenticado.
    /// </summary>
    public class EafRateLimitingMiddleware : IMiddleware
    {
        private readonly EafRateLimitingOptions _options;
        private readonly IRateLimitingStore _store;
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// Cria o middleware de rate limiting.
        /// </summary>
        public EafRateLimitingMiddleware(EafRateLimitingOptions options, IRateLimitingStore store, IIocResolver iocResolver)
        {
            _options = options;
            _store = store;
            _iocResolver = iocResolver;
        }

        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var path = context.Request.Path.Value ?? "/";
            var rule = FindRule(path);

            if (rule != null)
            {
                var key = ResolveKey(context);
                var result = await _store.HitAsync(key + ":" + rule.Endpoint, rule.GetWindow(), rule.Limit);

                context.Response.Headers["X-RateLimit-Limit"] = result.Limit.ToString();
                context.Response.Headers["X-RateLimit-Remaining"] = Math.Max(0, result.Limit - result.Count).ToString();

                if (!result.Allowed)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.Response.Headers["Retry-After"] = ((int)(result.ResetsAt - DateTimeOffset.UtcNow).TotalSeconds).ToString();
                    await context.Response.WriteAsJsonAsync(new { error = "Rate limit exceeded." });
                    return;
                }
            }

            await next(context);
        }

        private string ResolveKey(HttpContext context)
        {
            if (_options.LimitByUser && _iocResolver.Resolve<Abp.Runtime.Session.IAbpSession>().UserId.HasValue)
            {
                return "u:" + _iocResolver.Resolve<Abp.Runtime.Session.IAbpSession>().UserId.Value;
            }

            return "ip:" + (context.Connection.RemoteIpAddress?.ToString() ?? "unknown");
        }

        private EafRateLimitRule FindRule(string path)
        {
            foreach (var rule in _options.Rules)
            {
                if (rule.Endpoint == "*" ||
                    (rule.Endpoint.EndsWith("/*") && path.StartsWith(rule.Endpoint[..^1], StringComparison.OrdinalIgnoreCase)) ||
                    string.Equals(rule.Endpoint, path, StringComparison.OrdinalIgnoreCase))
                {
                    return rule;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Extensões para registrar o middleware de rate limiting do EAF no pipeline.
    /// </summary>
    public static class EafRateLimitingMiddlewareExtensions
    {
        /// <summary>
        /// Adiciona o middleware Eaf.RateLimiting ao pipeline (após autenticação, antes dos controllers).
        /// </summary>
        public static IApplicationBuilder UseEafRateLimiting(this IApplicationBuilder app)
        {
            return app.UseMiddleware<EafRateLimitingMiddleware>();
        }
    }
}

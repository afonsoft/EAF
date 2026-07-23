using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var response = context.Response;

            AddHeaderIfNotExists(response, "X-Content-Type-Options", "nosniff");
            AddHeaderIfNotExists(response, "X-Frame-Options", "DENY");
            AddHeaderIfNotExists(response, "X-XSS-Protection", "1; mode=block");
            AddHeaderIfNotExists(response, "Referrer-Policy", "strict-origin-when-cross-origin");
            AddHeaderIfNotExists(response, "Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");

            if (context.Request.IsHttps)
            {
                AddHeaderIfNotExists(response, "Strict-Transport-Security", "max-age=31536000; includeSubDomains");
            }

            await _next(context);
        }

        private static void AddHeaderIfNotExists(HttpResponse response, string key, string value)
        {
            if (response != null && !response.Headers.ContainsKey(key))
            {
                response.Headers.Append(key, value);
            }
        }
    }
}

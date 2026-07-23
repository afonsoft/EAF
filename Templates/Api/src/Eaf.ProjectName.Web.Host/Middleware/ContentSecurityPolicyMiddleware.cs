using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Eaf.ProjectName.Web.Middleware
{
    public class ContentSecurityPolicyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _contentSecurityPolicy;

        public ContentSecurityPolicyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _contentSecurityPolicy = BuildContentSecurityPolicy(configuration);
        }

        public async Task Invoke(HttpContext httpContext)
        {
            AddHeaderIfNotExists(httpContext, "Content-Security-Policy", _contentSecurityPolicy);
            AddHeaderIfNotExists(httpContext, "X-Content-Security-Policy", _contentSecurityPolicy);

            await _next.Invoke(httpContext);
        }

        private static void AddHeaderIfNotExists(HttpContext context, string key, string value)
        {
            if (context?.Response != null && !context.Response.Headers.ContainsKey(key))
            {
                context.Response.Headers.Append(key, value);
            }
        }

        private static string BuildContentSecurityPolicy(IConfiguration configuration)
        {
            var clientRootAddress = configuration["App:ClientRootAddress"];
            var reportUri = configuration["App:CspReportUri"];

            var defaultSrc = "default-src 'self'";
            var scriptSrc = "script-src 'self'";
            var styleSrc = "style-src 'self'";
            var imgSrc = "img-src 'self' data:";
            var fontSrc = "font-src 'self'";
            var connectSrc = "connect-src 'self'";

            if (!string.IsNullOrWhiteSpace(clientRootAddress))
            {
                var origin = new Uri(clientRootAddress).GetLeftPart(UriPartial.Authority);
                defaultSrc += " " + origin;
                scriptSrc += " " + origin;
                styleSrc += " " + origin;
                imgSrc += " " + origin;
                fontSrc += " " + origin;
                connectSrc += " " + origin;
            }

            var csp = $"{defaultSrc}; {scriptSrc}; {styleSrc}; {imgSrc}; {fontSrc}; {connectSrc}; object-src 'none'; frame-ancestors 'self'; base-uri 'self'; form-action 'self'";

            if (!string.IsNullOrWhiteSpace(reportUri))
            {
                csp += $"; report-uri {reportUri}";
            }

            return csp;
        }
    }
}

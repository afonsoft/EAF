using Abp.Authorization;
using Abp.Runtime.Validation;
using Abp.UI;
using Eaf.Middleware.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Middleware
{
    /// <summary>
    /// Middleware that catches unhandled exceptions and returns a <see cref="PublicErrorContract"/>
    /// as JSON, avoiding generic 500 HTML pages for SDK/API consumers.
    /// </summary>
    public class EafPublicErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<EafPublicErrorMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EafPublicErrorMiddleware"/>.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">Logger instance.</param>
        public EafPublicErrorMiddleware(RequestDelegate next, ILogger<EafPublicErrorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware, mapping any unhandled exception to a public error response.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (context.RequestAborted.IsCancellationRequested)
                    throw;

                _logger.LogError(
                    ex,
                    "Unhandled exception caught by {MiddlewareName}. CorrelationId: {CorrelationId}",
                    nameof(EafPublicErrorMiddleware),
                    context.TraceIdentifier);

                var error = MapToPublicError(ex, context.TraceIdentifier);
                context.Response.StatusCode = GetStatusCode(ex);
                context.Response.ContentType = "application/json";

                if (!context.Response.HasStarted)
                {
                    await context.Response.WriteAsJsonAsync(error);
                }
            }
        }

        /// <summary>
        /// Maps a .NET exception to a stable <see cref="PublicErrorContract"/>.
        /// </summary>
        private static PublicErrorContract MapToPublicError(Exception ex, string correlationId)
        {
            return ex switch
            {
                UserFriendlyException userFriendly => new PublicErrorContract
                {
                    Code = EafErrorCodes.ValidationFailed,
                    Message = userFriendly.Message,
                    Retryable = false,
                    CorrelationId = correlationId
                },
                AbpValidationException => new PublicErrorContract
                {
                    Code = EafErrorCodes.ValidationFailed,
                    Message = "Invalid request. Please check your input and try again.",
                    Retryable = false,
                    CorrelationId = correlationId
                },
                ArgumentException or ArgumentNullException or FormatException => new PublicErrorContract
                {
                    Code = EafErrorCodes.ValidationFailed,
                    Message = "Invalid request. Please check your input and try again.",
                    Retryable = false,
                    CorrelationId = correlationId
                },
                AbpAuthorizationException => new PublicErrorContract
                {
                    Code = EafErrorCodes.NotAuthorized,
                    Message = "You are not authorized to perform this operation.",
                    Retryable = false,
                    CorrelationId = correlationId
                },
                InvalidOperationException => new PublicErrorContract
                {
                    Code = EafErrorCodes.TemporarilyUnavailable,
                    Message = "The requested operation could not be completed. Please try again later.",
                    Retryable = true,
                    CorrelationId = correlationId
                },
                TimeoutException => new PublicErrorContract
                {
                    Code = EafErrorCodes.TemporarilyUnavailable,
                    Message = "The operation timed out. Please try again later.",
                    Retryable = true,
                    CorrelationId = correlationId
                },
                _ => new PublicErrorContract
                {
                    Code = EafErrorCodes.TemporarilyUnavailable,
                    Message = "An unexpected error occurred. Please try again later.",
                    Retryable = true,
                    CorrelationId = correlationId
                }
            };
        }

        /// <summary>
        /// Determines the HTTP status code for a given exception type.
        /// </summary>
        private static int GetStatusCode(Exception ex)
        {
            return ex switch
            {
                UserFriendlyException => StatusCodes.Status400BadRequest,
                AbpValidationException => StatusCodes.Status400BadRequest,
                ArgumentException or ArgumentNullException or FormatException => StatusCodes.Status400BadRequest,
                AbpAuthorizationException => StatusCodes.Status403Forbidden,
                InvalidOperationException => StatusCodes.Status500InternalServerError,
                TimeoutException => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };
        }
    }
}
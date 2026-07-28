using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Runtime.Validation;
using Abp.UI;
using Eaf.Middleware;
using Eaf.Middleware.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Filters
{
    /// <summary>
    /// MVC exception filter that maps common ABP exceptions to a
    /// <see cref="PublicErrorContract"/> response before the default ABP exception filter.
    /// </summary>
    public class EafExceptionFilter : IExceptionFilter, IAsyncExceptionFilter, IOrderedFilter
    {
        /// <summary>
        /// Exception filters execute in reverse order, so a higher value runs before
        /// the default ABP exception filter (order 0).
        /// </summary>
        public int Order => 1000;

        /// <summary>
        /// Called when an exception occurs in an MVC action.
        /// </summary>
        /// <param name="context">The exception context.</param>
        public void OnException(ExceptionContext context)
        {
            if (context?.Exception == null)
                return;

            var result = MapException(context);
            if (result == null)
                return;

            context.Result = result;
            context.ExceptionHandled = true;
        }

        /// <summary>
        /// Called when an exception occurs in an asynchronous MVC action.
        /// </summary>
        /// <param name="context">The exception context.</param>
        public Task OnExceptionAsync(ExceptionContext context)
        {
            OnException(context);
            return Task.CompletedTask;
        }

        private static ObjectResult MapException(ExceptionContext context)
        {
            var ex = context.Exception;
            var correlationId = context.HttpContext.TraceIdentifier;

            return ex switch
            {
                UserFriendlyException userFriendly => CreateResult(
                    StatusCodes.Status400BadRequest,
                    EafErrorCodes.ValidationFailed,
                    userFriendly.Message,
                    correlationId),

                AbpValidationException or ArgumentException or ArgumentNullException or FormatException => CreateResult(
                    StatusCodes.Status400BadRequest,
                    EafErrorCodes.ValidationFailed,
                    "Invalid request. Please check your input and try again.",
                    correlationId),

                AbpAuthorizationException => CreateResult(
                    StatusCodes.Status403Forbidden,
                    EafErrorCodes.NotAuthorized,
                    "You are not authorized to perform this operation.",
                    correlationId),

                EntityNotFoundException => CreateResult(
                    StatusCodes.Status404NotFound,
                    EafErrorCodes.ValidationFailed,
                    "The requested resource was not found.",
                    correlationId),

                _ => null
            };
        }

        private static ObjectResult CreateResult(int statusCode, string code, string message, string correlationId)
        {
            return new ObjectResult(new PublicErrorContract
            {
                Code = code,
                Message = message,
                Retryable = false,
                CorrelationId = correlationId
            })
            {
                StatusCode = statusCode,
                DeclaredType = typeof(PublicErrorContract)
            };
        }
    }
}

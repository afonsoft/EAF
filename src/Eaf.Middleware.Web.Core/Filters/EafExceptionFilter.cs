using Abp.UI;
using Eaf.Middleware;
using Eaf.Middleware.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Filters
{
    /// <summary>
    /// MVC exception filter that maps <see cref="UserFriendlyException"/> to a
    /// <see cref="PublicErrorContract"/> 400 response before the default ABP exception filter.
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
            if (context?.Exception is not UserFriendlyException userFriendly)
                return;

            context.Result = CreateResult(context, userFriendly);
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

        private static ObjectResult CreateResult(ExceptionContext context, UserFriendlyException userFriendly)
        {
            return new ObjectResult(new PublicErrorContract
            {
                Code = EafErrorCodes.ValidationFailed,
                Message = userFriendly.Message,
                Retryable = false,
                CorrelationId = context.HttpContext.TraceIdentifier
            })
            {
                StatusCode = StatusCodes.Status400BadRequest,
                DeclaredType = typeof(PublicErrorContract)
            };
        }
    }
}

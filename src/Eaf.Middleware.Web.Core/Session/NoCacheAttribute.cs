using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Eaf.Middleware.Web.Session
{
    /// <summary>
    /// Representa a classe NoCacheAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class NoCacheAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// OnResultExecuting.
        /// </summary>
        /// <param name="context">Parâmetro context.</param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.HttpContext.Response.Headers["Expires"] = "-1";
            context.HttpContext.Response.Headers["Pragma"] = "no-cache";
            base.OnResultExecuting(context);
        }
    }
}
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
        /// <param name="filterContext">Parâmetro filterContext.</param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            filterContext.HttpContext.Response.Headers["Expires"] = "-1";
            filterContext.HttpContext.Response.Headers["Pragma"] = "no-cache";
            base.OnResultExecuting(filterContext);
        }
    }
}
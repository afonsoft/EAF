using Abp.Application.Services;
using Microsoft.AspNetCore.Antiforgery;

namespace Eaf.Middleware.Web.Controllers
{
    /// <summary>
    /// Representa a classe AntiForgeryController.
    /// </summary>
    public class AntiForgeryController : MiddlewareControllerBase, IApplicationService
    {
        private readonly IAntiforgery _antiforgery;

        /// <summary>
        /// AntiForgeryController.
        /// </summary>
        /// <param name="antiforgery">Parâmetro antiforgery.</param>
        /// <returns>Resultado da operação.</returns>
        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        /// <summary>
        /// GetToken.
        /// </summary>
        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
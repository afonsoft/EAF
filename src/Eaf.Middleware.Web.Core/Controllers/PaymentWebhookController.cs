using Abp.Authorization;
using Eaf.Middleware.Payments;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Controllers
{
    /// <summary>
    /// Controller responsável por receber webhooks de gateways de pagamento.
    /// </summary>
    [Route("api/payment/webhook")]
    [AbpAllowAnonymous]
    public class PaymentWebhookController : MiddlewareControllerBase
    {
        private readonly IPaymentManager _paymentManager;

        /// <summary>
        /// PaymentWebhookController.
        /// </summary>
        public PaymentWebhookController(IPaymentManager paymentManager)
        {
            _paymentManager = paymentManager;
        }

        /// <summary>
        /// Recebe um webhook assíncrono do gateway de pagamento informado.
        /// </summary>
        /// <param name="gateway">Nome do gateway (ex: Stripe).</param>
        [HttpPost("{gateway}")]
        public virtual async Task<IActionResult> Handle(string gateway)
        {
            using var reader = new StreamReader(HttpContext.Request.Body);
            var json = await reader.ReadToEndAsync();

            var signature = Request.Headers["Stripe-Signature"].FirstOrDefault()
                ?? Request.Headers[$"X-{gateway}-Signature"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(signature))
            {
                return BadRequest("Missing signature header.");
            }

            await _paymentManager.ProcessWebhookAsync(gateway, json, signature);
            return Ok();
        }
    }
}

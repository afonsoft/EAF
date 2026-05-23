using Eaf.Middleware.Dto;

namespace Eaf.Middleware.WebHooks.Dto
{
    /// <summary>
    /// Representa a classe GetAllSendAttemptsInput.
    /// </summary>
    public class GetAllSendAttemptsInput : PagedInputDto
    {
        /// <summary>
        /// Obtém ou define SubscriptionId.
        /// </summary>
        public string SubscriptionId { get; set; }
    }
}
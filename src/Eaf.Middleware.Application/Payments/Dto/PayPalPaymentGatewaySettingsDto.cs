namespace Eaf.Middleware.Payments.Dto
{
    /// <summary>
    /// Configurações do gateway PayPal.
    /// </summary>
    public class PayPalPaymentGatewaySettingsDto
    {
        /// <summary>
        /// Client ID da aplicação PayPal.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Client Secret da aplicação PayPal.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// ID do webhook PayPal.
        /// </summary>
        public string WebhookId { get; set; }
    }
}

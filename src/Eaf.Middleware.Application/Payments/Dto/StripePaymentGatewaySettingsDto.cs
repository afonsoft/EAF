namespace Eaf.Middleware.Payments.Dto
{
    /// <summary>
    /// Configurações do gateway Stripe.
    /// </summary>
    public class StripePaymentGatewaySettingsDto
    {
        /// <summary>
        /// Chave secreta da API Stripe.
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Chave pública Stripe.
        /// </summary>
        public string PublishableKey { get; set; }

        /// <summary>
        /// Segredo do webhook Stripe.
        /// </summary>
        public string WebhookSecret { get; set; }
    }
}

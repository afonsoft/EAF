namespace Eaf.Middleware.Payments.Dto
{
    /// <summary>
    /// DTO para configuração dos gateways de pagamento.
    /// </summary>
    public class PaymentGatewaySettingsDto
    {
        /// <summary>
        /// Nome do gateway padrão.
        /// </summary>
        public string DefaultGateway { get; set; }

        /// <summary>
        /// Configurações do Stripe.
        /// </summary>
        public StripePaymentGatewaySettingsDto Stripe { get; set; }

        /// <summary>
        /// Configurações do PayPal.
        /// </summary>
        public PayPalPaymentGatewaySettingsDto PayPal { get; set; }

        /// <summary>
        /// Configurações do MercadoPago.
        /// </summary>
        public MercadoPagoPaymentGatewaySettingsDto MercadoPago { get; set; }

        /// <summary>
        /// Configurações do PagSeguro.
        /// </summary>
        public PagSeguroPaymentGatewaySettingsDto PagSeguro { get; set; }
    }
}

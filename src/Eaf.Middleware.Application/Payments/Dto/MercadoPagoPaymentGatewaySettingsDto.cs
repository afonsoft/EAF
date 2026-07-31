namespace Eaf.Middleware.Payments.Dto
{
    /// <summary>
    /// Configurações do gateway MercadoPago.
    /// </summary>
    public class MercadoPagoPaymentGatewaySettingsDto
    {
        /// <summary>
        /// Access Token da conta MercadoPago.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Public Key da conta MercadoPago.
        /// </summary>
        public string PublicKey { get; set; }
    }
}

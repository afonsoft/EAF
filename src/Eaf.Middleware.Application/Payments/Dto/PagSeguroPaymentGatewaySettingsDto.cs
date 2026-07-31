namespace Eaf.Middleware.Payments.Dto
{
    /// <summary>
    /// Configurações do gateway PagSeguro.
    /// </summary>
    public class PagSeguroPaymentGatewaySettingsDto
    {
        /// <summary>
        /// Token de conta PagSeguro.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// E-mail da conta PagSeguro.
        /// </summary>
        public string Email { get; set; }
    }
}

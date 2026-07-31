namespace Eaf.Middleware.Payments.Dto
{
    /// <summary>
    /// DTO para representar um gateway de pagamento disponível.
    /// </summary>
    public class PaymentGatewayDto
    {
        /// <summary>
        /// Nome técnico do gateway.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Nome amigável do gateway.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Indica se o gateway possui as configurações mínimas preenchidas.
        /// </summary>
        public bool IsConfigured { get; set; }

        /// <summary>
        /// Indica se é o gateway padrão.
        /// </summary>
        public bool IsDefault { get; set; }
    }
}

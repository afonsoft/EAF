namespace Eaf.Middleware.Payments.Dto
{
    /// <summary>
    /// Resultado do processamento de um pagamento.
    /// </summary>
    public class PaymentResultDto
    {
        /// <summary>
        /// Identificador externo do pagamento.
        /// </summary>
        public string ExternalPaymentId { get; set; }

        /// <summary>
        /// Gateway utilizado.
        /// </summary>
        public string Gateway { get; set; }

        /// <summary>
        /// Indica se o pagamento foi confirmado.
        /// </summary>
        public bool IsSuccess { get; set; }
    }
}

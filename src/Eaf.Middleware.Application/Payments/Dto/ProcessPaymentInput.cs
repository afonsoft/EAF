namespace Eaf.Middleware.Payments.Dto
{
    /// <summary>
    /// Entrada para processar o retorno de um pagamento.
    /// </summary>
    public class ProcessPaymentInput
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
        /// Resposta ou payload do gateway.
        /// </summary>
        public string GatewayResponse { get; set; }

        /// <summary>
        /// Indica se o pagamento foi confirmado.
        /// </summary>
        public bool IsSuccess { get; set; }
    }
}

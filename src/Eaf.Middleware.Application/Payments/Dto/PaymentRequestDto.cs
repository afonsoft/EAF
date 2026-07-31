namespace Eaf.Middleware.Payments.Dto
{
    /// <summary>
    /// Resultado da criação de uma solicitação de pagamento.
    /// </summary>
    public class PaymentRequestDto
    {
        /// <summary>
        /// Identificador do pagamento gerado.
        /// </summary>
        public string PaymentId { get; set; }

        /// <summary>
        /// Gateway utilizado.
        /// </summary>
        public string Gateway { get; set; }

        /// <summary>
        /// URL de checkout.
        /// </summary>
        public string CheckoutUrl { get; set; }

        /// <summary>
        /// Indica se a solicitação foi criada com sucesso.
        /// </summary>
        public bool IsSuccess { get; set; }
    }
}

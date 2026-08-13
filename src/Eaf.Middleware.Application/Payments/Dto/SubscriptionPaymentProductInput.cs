namespace Eaf.Middleware.Payments.Dto
{
    /// <summary>
    /// Entrada para um produto/linha de pagamento de assinatura.
    /// </summary>
    public class SubscriptionPaymentProductInput
    {
        /// <summary>
        /// Descrição do produto.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Quantidade.
        /// </summary>
        public int Count { get; set; } = 1;

        /// <summary>
        /// Valor unitário.
        /// </summary>
        public decimal Amount { get; set; }
    }
}

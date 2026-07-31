namespace Eaf.Middleware.Payments
{
    /// <summary>
    /// Resolvedor de gateways de pagamento.
    /// </summary>
    public interface IPaymentGatewayResolver
    {
        /// <summary>
        /// Obtém o gateway pelo nome.
        /// </summary>
        /// <param name="gatewayName">Nome do gateway.</param>
        /// <returns>Gateway de pagamento.</returns>
        IPaymentGateway Resolve(string gatewayName);
    }
}

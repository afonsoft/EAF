using Abp.Dependency;
using System.Collections.Generic;
using System.Linq;

namespace Eaf.Middleware.Payments
{
    /// <summary>
    /// Resolvedor de gateways de pagamento baseado em nome.
    /// </summary>
    public class PaymentGatewayResolver : IPaymentGatewayResolver, ITransientDependency
    {
        private readonly IEnumerable<IPaymentGateway> _gateways;
        private readonly NullPaymentGateway _nullPaymentGateway;

        /// <summary>
        /// PaymentGatewayResolver.
        /// </summary>
        public PaymentGatewayResolver(IEnumerable<IPaymentGateway> gateways, NullPaymentGateway nullPaymentGateway)
        {
            _gateways = gateways;
            _nullPaymentGateway = nullPaymentGateway;
        }

        /// <summary>
        /// Resolve o gateway pelo nome (case-insensitive). Retorna NullPaymentGateway se não encontrado.
        /// </summary>
        public IPaymentGateway Resolve(string gatewayName)
        {
            if (string.IsNullOrWhiteSpace(gatewayName))
            {
                return _nullPaymentGateway;
            }

            return _gateways.FirstOrDefault(g => g.GetType().Name.StartsWith(gatewayName, System.StringComparison.OrdinalIgnoreCase))
                ?? _nullPaymentGateway;
        }
    }
}

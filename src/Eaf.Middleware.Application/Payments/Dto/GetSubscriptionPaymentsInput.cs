using Abp.Application.Services.Dto;

namespace Eaf.Middleware.Payments.Dto
{
    /// <summary>
    /// Entrada para listar pagamentos de assinatura.
    /// </summary>
    public class GetSubscriptionPaymentsInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Filtro por gateway ou status.
        /// </summary>
        public string Filter { get; set; }
    }
}

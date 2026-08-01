using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Authorization.Accounts.Dto
{
    /// <summary>
    /// Representa a entrada para aprovação ou rejeição de uma solicitação de ingresso.
    /// </summary>
    public class ApproveTenantJoinRequestInput
    {
        /// <summary>
        /// Id da solicitação.
        /// </summary>
        [Required]
        public long RequestId { get; set; }

        /// <summary>
        /// Indica se a solicitação foi aprovada.
        /// </summary>
        [Required]
        public bool IsApproved { get; set; }
    }
}

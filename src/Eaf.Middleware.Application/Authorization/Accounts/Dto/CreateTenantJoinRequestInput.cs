using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Authorization.Accounts.Dto
{
    /// <summary>
    /// Representa a entrada para criação de uma solicitação de ingresso em tenant.
    /// </summary>
    public class CreateTenantJoinRequestInput
    {
        /// <summary>
        /// Id do tenant solicitado.
        /// </summary>
        [Required]
        public int TenantId { get; set; }

        /// <summary>
        /// Mensagem opcional do solicitante.
        /// </summary>
        [StringLength(512)]
        public string Message { get; set; }
    }
}

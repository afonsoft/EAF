using Abp.Application.Services.Dto;

namespace Eaf.Middleware.MassNotifications.Dto
{
    /// <summary>
    /// Entrada para listar notificações em massa.
    /// </summary>
    public class GetMassNotificationsInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// Filtro por assunto ou mensagem.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Status da notificação.
        /// </summary>
        public string Status { get; set; }
    }
}

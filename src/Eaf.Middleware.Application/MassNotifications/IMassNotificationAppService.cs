using Abp.Application.Services.Dto;
using Eaf.Middleware.MassNotifications.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.MassNotifications
{
    /// <summary>
    /// Serviço de aplicação para notificações em massa.
    /// </summary>
    public interface IMassNotificationAppService
    {
        /// <summary>
        /// Obtém as notificações em massa paginadas.
        /// </summary>
        Task<PagedResultDto<MassNotificationDto>> GetAllAsync(GetMassNotificationsInput input);

        /// <summary>
        /// Cria uma nova notificação em massa e agenda o envio.
        /// </summary>
        Task<MassNotificationDto> CreateAsync(CreateMassNotificationInput input);

        /// <summary>
        /// Cancela uma notificação em massa.
        /// </summary>
        Task CancelAsync(EntityDto<long> input);
    }
}

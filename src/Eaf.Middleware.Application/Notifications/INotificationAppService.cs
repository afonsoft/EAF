using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Eaf.Middleware.Notifications.Dto;
using System;
using System.Threading.Tasks;

namespace Eaf.Middleware.Notifications
{
    /// <summary>
    /// Representa a interface INotificationAppService.
    /// </summary>
    public interface INotificationAppService : IApplicationService
    {
        Task DeleteNotification(EntityDto<Guid> input);

        Task<GetNotificationSettingsOutput> GetNotificationSettings();

        Task<GetNotificationsOutput> GetUserNotifications(GetUserNotificationsInput input);

        Task SetAllNotificationsAsRead();

        Task SetNotificationAsRead(EntityDto<Guid> input);

        Task UpdateNotificationSettings(UpdateNotificationSettingsInput input);
    }
}
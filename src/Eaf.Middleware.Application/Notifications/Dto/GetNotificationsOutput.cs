using Abp.Application.Services.Dto;
using Abp.Notifications;
using System.Collections.Generic;

namespace Eaf.Middleware.Notifications.Dto
{
    /// <summary>
    /// Representa a classe GetNotificationsOutput.
    /// </summary>
    public class GetNotificationsOutput : PagedResultDto<UserNotification>
    {
        /// <summary>
        /// GetNotificationsOutput.
        /// </summary>
        /// <param name="totalCount">Parâmetro totalCount.</param>
        /// <param name="unreadCount">Parâmetro unreadCount.</param>
        /// <param name="notifications">Parâmetro notifications.</param>
        /// <returns>Resultado da operação.</returns>
        public GetNotificationsOutput(int totalCount, int unreadCount, List<UserNotification> notifications)
            : base(totalCount, notifications)
        {
            UnreadCount = unreadCount;
        }

        /// <summary>
        /// Obtém ou define UnreadCount.
        /// </summary>
        public int UnreadCount { get; set; }
    }
}
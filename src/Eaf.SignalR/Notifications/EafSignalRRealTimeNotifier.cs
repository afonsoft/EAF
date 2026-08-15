using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Notifications;
using Abp.RealTime;
using Castle.Core.Logging;
using Eaf.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Eaf.SignalR.Notifications
{
    /// <summary>
    /// Notificador em tempo real baseado em SignalR.
    /// </summary>
    public class EafSignalRRealTimeNotifier : IRealTimeNotifier, ITransientDependency
    {
        private readonly IOnlineClientManager _onlineClientManager;
        private readonly IHubContext<EafCommonHub> _hubContext;

        /// <summary>
        /// Indica se o notificador deve ser usado apenas quando explicitamente solicitado como destino.
        /// </summary>
        public bool UseOnlyIfRequestedAsTarget => false;

        /// <summary>
        /// Logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// EafSignalRRealTimeNotifier.
        /// </summary>
        /// <param name="onlineClientManager">Gerenciador de clientes online.</param>
        /// <param name="hubContext">Contexto do hub EafCommonHub.</param>
        public EafSignalRRealTimeNotifier(IOnlineClientManager onlineClientManager, IHubContext<EafCommonHub> hubContext)
        {
            _onlineClientManager = onlineClientManager;
            _hubContext = hubContext;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Envia notificações para os clientes conectados.
        /// </summary>
        /// <param name="userNotifications">Notificações a serem enviadas.</param>
        public async Task SendNotificationsAsync(UserNotification[] userNotifications)
        {
            foreach (var userNotification in userNotifications)
            {
                try
                {
                    var onlineClients = await _onlineClientManager.GetAllByUserIdAsync(userNotification);
                    var connectionIds = onlineClients.Select(client => client.ConnectionId).ToList();

                    if (connectionIds.Count > 0)
                    {
                        await _hubContext.Clients.Clients(connectionIds).SendAsync("getNotification", userNotification);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn($"Could not send notification to user: {userNotification.UserId}");
                    Logger.Warn(ex.ToString(), ex);
                }
            }
        }
    }
}

using Abp.AspNetCore.SignalR.Hubs;
using Abp.RealTime;

namespace Eaf.SignalR.Hubs
{
    /// <summary>
    /// Hub SignalR comum do EAF, mapeado em <c>/signalr</c>.
    /// </summary>
    public class EafCommonHub : AbpCommonHub
    {
        /// <summary>
        /// EafCommonHub.
        /// </summary>
        /// <param name="onlineClientManager">Gerenciador de clientes online.</param>
        /// <param name="clientInfoProvider">Provedor de informações do cliente.</param>
        public EafCommonHub(IOnlineClientManager onlineClientManager, IOnlineClientInfoProvider clientInfoProvider)
            : base(onlineClientManager, clientInfoProvider)
        {
        }
    }
}

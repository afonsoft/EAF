using System.Threading.Tasks;
using Abp.Notifications;
using Abp.RealTime;
using Eaf.SignalR.Notifications;
using Microsoft.AspNetCore.SignalR;
using Shouldly;
using Xunit;

namespace Eaf.SignalR.Tests.Notifications
{
    public class EafSignalRRealTimeNotifierTests : EafSignalRTestBase
    {
        private readonly IOnlineClientManager _onlineClientManager;

        public EafSignalRRealTimeNotifierTests()
        {
            _onlineClientManager = Resolve<IOnlineClientManager>();
        }

        [Fact]
        public async Task Dado_UsuarioComClienteConectado_Quando_EnviarNotificacao_Entao_DeveChamarGetNotification()
        {
            var clientProxy = new FakeSingleClientProxy();
            var hubContext = new FakeHubContext(new FakeHubClients(clientProxy));

            await _onlineClientManager.AddAsync(new OnlineClient("conn1", "127.0.0.1", null, 1));

            var notifier = new EafSignalRRealTimeNotifier(_onlineClientManager, hubContext);
            await notifier.SendNotificationsAsync(new[]
            {
                new UserNotification
                {
                    TenantId = null,
                    UserId = 1,
                    Notification = new TenantNotification { NotificationName = "Test" }
                }
            });

            clientProxy.SendCoreAsyncCalls.Count.ShouldBe(1);
            clientProxy.SendCoreAsyncCalls[0].Method.ShouldBe("getNotification");
            clientProxy.SendCoreAsyncCalls[0].Args.Length.ShouldBe(1);
            clientProxy.SendCoreAsyncCalls[0].Args[0].ShouldBeOfType<UserNotification>();
        }

        [Fact]
        public async Task Dado_UsuarioSemClienteConectado_Quando_EnviarNotificacao_Entao_NaoDeveChamarSendCoreAsync()
        {
            var clientProxy = new FakeSingleClientProxy();
            var hubContext = new FakeHubContext(new FakeHubClients(clientProxy));

            var notifier = new EafSignalRRealTimeNotifier(_onlineClientManager, hubContext);
            await notifier.SendNotificationsAsync(new[]
            {
                new UserNotification
                {
                    TenantId = null,
                    UserId = 999,
                    Notification = new TenantNotification { NotificationName = "Test" }
                }
            });

            clientProxy.SendCoreAsyncCalls.Count.ShouldBe(0);
            notifier.UseOnlyIfRequestedAsTarget.ShouldBeFalse();
        }
    }
}

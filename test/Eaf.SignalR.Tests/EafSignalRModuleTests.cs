using System.Threading.Tasks;
using Abp.Notifications;
using Abp.RealTime;
using Eaf.SignalR.Hubs;
using Eaf.SignalR.Notifications;
using Shouldly;
using Xunit;

namespace Eaf.SignalR.Tests
{
    public class EafSignalRModuleTests : EafSignalRTestBase
    {
        private readonly IOnlineClientManager _onlineClientManager;

        public EafSignalRModuleTests()
        {
            _onlineClientManager = Resolve<IOnlineClientManager>();
        }

        [Fact]
        public void Dado_ModuloInicializado_Quando_ExecutarCicloDeVida_Entao_ServicosRealTimeDevemEstarRegistrados()
        {
            Resolve<IOnlineClientManager>().ShouldNotBeNull();
            Resolve<IOnlineClientStore>().ShouldNotBeNull();
            Resolve<IOnlineClientManager<TestChannel>>().ShouldNotBeNull();
            Resolve<Eaf.SignalR.RealTime.IOnlineClientStore<TestChannel>>().ShouldNotBeNull();
            Resolve<IRealTimeNotifier>().ShouldBeOfType<EafSignalRRealTimeNotifier>();
            Resolve<EafCommonHub>().ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ClienteConectado_Quando_BuscarPorUsuario_Entao_DeveRetornarCliente()
        {
            var client = new OnlineClient("conn1", "127.0.0.1", null, 1);
            await _onlineClientManager.AddAsync(client);

            var clients = await _onlineClientManager.GetAllByUserIdAsync(new Abp.UserIdentifier(null, 1));

            clients.Count.ShouldBe(1);
            clients[0].ConnectionId.ShouldBe("conn1");
        }

        private class TestChannel
        {
        }
    }
}

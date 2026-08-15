using System.Threading.Tasks;
using Abp.RealTime;
using Eaf.SignalR.RealTime;
using Shouldly;
using Xunit;

namespace Eaf.SignalR.Tests.RealTime
{
    public class EafOnlineClientManagerTests
    {
        [Fact]
        public async Task Dado_EafOnlineClientManager_Quando_AdicionarCliente_Entao_DeveRetornarPorUsuario()
        {
            var store = new EafInMemoryOnlineClientStore();
            var manager = new EafOnlineClientManager(store);

            var client = new OnlineClient("conn1", "127.0.0.1", null, 1);
            await manager.AddAsync(client);

            var clients = await manager.GetAllByUserIdAsync(new Abp.UserIdentifier(null, 1));
            clients.Count.ShouldBe(1);
            clients[0].ConnectionId.ShouldBe("conn1");
        }

        [Fact]
        public async Task Dado_EafOnlineClientManagerGenerico_Quando_AdicionarCliente_Entao_DeveRetornarPorUsuario()
        {
            var store = new EafInMemoryOnlineClientStore<TestChannel>();
            var manager = new EafOnlineClientManager<TestChannel>(store);

            var client = new OnlineClient("conn2", "127.0.0.1", 1, 2);
            await manager.AddAsync(client);

            var clients = await manager.GetAllByUserIdAsync(new Abp.UserIdentifier(1, 2));
            clients.Count.ShouldBe(1);
            clients[0].ConnectionId.ShouldBe("conn2");
        }

        private class TestChannel
        {
        }
    }
}

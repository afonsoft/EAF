using Abp.RealTime;
using Eaf.SignalR.RealTime;
using Shouldly;
using Xunit;

namespace Eaf.SignalR.Tests.RealTime
{
    public class EafInMemoryOnlineClientStoreTests
    {
        [Fact]
        public void Dado_EafInMemoryOnlineClientStore_Quando_Criado_Entao_DeveSerInstanciaValida()
        {
            var store = new EafInMemoryOnlineClientStore();
            store.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_EafInMemoryOnlineClientStoreGenerico_Quando_Criado_Entao_DeveImplementarIOnlineClientStore()
        {
            var store = new EafInMemoryOnlineClientStore<TestChannel>();
            store.ShouldBeAssignableTo<Eaf.SignalR.RealTime.IOnlineClientStore<TestChannel>>();
            store.ShouldBeAssignableTo<Abp.RealTime.IOnlineClientStore>();
        }

        [Fact]
        public void Dado_EafInMemoryOnlineClientStoreGenerico_Quando_Criado_Entao_DeveSerInstanciaValida()
        {
            var store = new EafInMemoryOnlineClientStore<TestChannel>();
            store.ShouldNotBeNull();
        }

        private class TestChannel
        {
        }
    }
}

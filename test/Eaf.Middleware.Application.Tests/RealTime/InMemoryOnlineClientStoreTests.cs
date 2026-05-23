using Eaf.Middleware.RealTime;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.RealTime
{
    public class InMemoryOnlineClientStoreTests
    {
        [Fact]
        public void Dado_InMemoryOnlineClientStore_Quando_Criado_Entao_DeveSerInstanciaValida()
        {
            var store = new InMemoryOnlineClientStore<object>();
            store.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_InMemoryOnlineClientStore_Quando_Verificado_Entao_DeveImplementarIOnlineClientStore()
        {
            var store = new InMemoryOnlineClientStore<object>();
            store.ShouldBeAssignableTo<IOnlineClientStore<object>>();
        }
    }
}

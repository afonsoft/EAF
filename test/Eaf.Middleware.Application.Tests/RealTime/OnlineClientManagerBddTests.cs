using Abp.RealTime;
using Eaf.Middleware.Chat;
using Eaf.Middleware.RealTime;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.RealTime
{
    public class OnlineClientManagerBddTests
    {
        [Fact]
        public void Dado_Store_Quando_CriarComStore_Entao_DeveInicializarCorretamente()
        {
            var store = Substitute.For<IOnlineClientStore<ChatChannel>>();
            var sut = new OnlineClientManager<ChatChannel>(store);
            sut.ShouldNotBeNull();
        }
    }
}

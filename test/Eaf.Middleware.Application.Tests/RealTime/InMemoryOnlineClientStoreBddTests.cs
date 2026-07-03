using Eaf.Middleware.Chat;
using Eaf.Middleware.RealTime;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.RealTime
{
    public class InMemoryOnlineClientStoreBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_CriarComChatChannel_Entao_DeveInicializarCorretamente()
        {
            var sut = new InMemoryOnlineClientStore<ChatChannel>();
            sut.ShouldNotBeNull();
        }
    }
}

using Eaf.Middleware.Chat;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Chat
{
    public class ChatChannelTests
    {
        [Fact]
        public void Dado_ChatChannel_Quando_Instanciar_Entao_DeveCriarComSucesso()
        {
            var channel = new ChatChannel();
            channel.ShouldNotBeNull();
        }
    }
}

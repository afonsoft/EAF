using Abp.ObjectMapping;
using Abp.RealTime;
using Eaf.AspNetCore.SignalR.Chat;
using Eaf.Middleware.Chat;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.SignalR.Chat
{
    public class SignalRChatCommunicatorBddTests
    {
        [Fact]
        public void Dado_Dependencias_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var objectMapper = Substitute.For<IObjectMapper>();
            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            var chatHub = Substitute.For<IHubContext<ChatHub>>();

            var sut = new SignalRChatCommunicator(objectMapper, onlineClientManager, chatHub);
            sut.ShouldNotBeNull();
        }
    }
}

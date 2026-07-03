using Eaf.AspNetCore.SignalR.Chat;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.SignalR.Chat
{
    public class ChatHubBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarNome_Entao_DeveSerCorreto()
        {
            typeof(ChatHub).Name.ShouldBe("ChatHub");
        }

        [Fact]
        public void Dado_Tipo_Quando_VerificarHeranca_Entao_DeveHerdarDeOnlineClientHubBase()
        {
            typeof(ChatHub).BaseType.Name.ShouldBe("OnlineClientHubBase");
        }
    }
}

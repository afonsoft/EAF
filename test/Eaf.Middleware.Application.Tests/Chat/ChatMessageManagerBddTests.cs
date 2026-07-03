using Eaf.Middleware.Chat;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Chat
{
    public class ChatMessageManagerBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarNome_Entao_DeveSerCorreto()
        {
            typeof(ChatMessageManager).Name.ShouldBe("ChatMessageManager");
        }

        [Fact]
        public void Dado_Tipo_Quando_VerificarInterface_Entao_DeveImplementarIChatMessageManager()
        {
            typeof(ChatMessageManager).GetInterface(nameof(IChatMessageManager)).ShouldNotBeNull();
        }
    }
}

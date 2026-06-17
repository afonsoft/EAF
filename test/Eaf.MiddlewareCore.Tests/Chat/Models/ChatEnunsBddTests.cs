using Eaf.Middleware.Chat;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Chat.Models
{
    /// <summary>
    /// Testes BDD para enums de Chat seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ChatEnunsBddTests
    {
        [Fact]
        public void Dado_ChatMessageReadState_Quando_Verificar_Entao_DevemTerValoresCorretos()
        {
            ((int)ChatMessageReadState.Unread).ShouldBe(1);
            ((int)ChatMessageReadState.Read).ShouldBe(2);
        }

        [Fact]
        public void Dado_ChatSide_Quando_Verificar_Entao_DevemTerValoresCorretos()
        {
            ((int)ChatSide.Sender).ShouldBe(1);
            ((int)ChatSide.Receiver).ShouldBe(2);
        }
    }
}

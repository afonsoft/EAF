using Eaf.Middleware.Chat;
using Eaf.Middleware.Storage;
using Eaf.Middleware.Web.Controllers;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Controllers
{
    public class ChatControllerBddTests
    {
        [Fact]
        public void Dado_Dependencias_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var binaryObjectManager = Substitute.For<IBinaryObjectManager>();
            var chatMessageManager = Substitute.For<IChatMessageManager>();
            var sut = new ChatController(binaryObjectManager, chatMessageManager);
            sut.ShouldNotBeNull();
        }
    }
}

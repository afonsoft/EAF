using Eaf.Middleware.Chat;
using Eaf.Middleware.Storage;
using Eaf.Middleware.Web.Controllers;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.Controllers
{
    /// <summary>
    /// Testes BDD para ChatControllerBase seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class ChatControllerBaseBddTests
    {
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IChatMessageManager _chatMessageManager;

        public ChatControllerBaseBddTests()
        {
            _binaryObjectManager = Substitute.For<IBinaryObjectManager>();
            _chatMessageManager = Substitute.For<IChatMessageManager>();
        }

        #region Instanciacao

        [Fact]
        public void Dado_Dependencias_Quando_CriarChatController_Entao_DeveInicializarCorretamente()
        {
            var sut = new ChatController(_binaryObjectManager, _chatMessageManager);
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_BinaryObjectManager_Quando_CriarChatController_Entao_DeveHerdarDeMiddlewareControllerBase()
        {
            var sut = new ChatController(_binaryObjectManager, _chatMessageManager);
            sut.ShouldBeAssignableTo<MiddlewareControllerBase>();
        }

        #endregion
    }
}

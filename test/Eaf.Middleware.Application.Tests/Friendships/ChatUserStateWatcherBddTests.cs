using Abp.RealTime;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Cache;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Friendships
{
    public class ChatUserStateWatcherBddTests
    {
        [Fact]
        public void Dado_Dependencias_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var chatCommunicator = Substitute.For<IChatCommunicator>();
            var userFriendsCache = Substitute.For<IUserFriendsCache>();
            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();

            var sut = new ChatUserStateWatcher(chatCommunicator, userFriendsCache, onlineClientManager);
            sut.ShouldNotBeNull();
        }
    }
}

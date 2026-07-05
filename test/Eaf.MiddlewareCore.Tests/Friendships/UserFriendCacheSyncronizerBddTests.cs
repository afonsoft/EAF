using System;
using Abp;
using Abp.Events.Bus.Entities;
using Abp.ObjectMapping;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships.Cache;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Friendships
{
    /// <summary>
    /// Testes BDD para UserFriendCacheSyncronizer seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class UserFriendCacheSyncronizerBddTests
    {
        private readonly IUserFriendsCache _userFriendsCache = Substitute.For<IUserFriendsCache>();
        private readonly IObjectMapper _objectMapper = Substitute.For<IObjectMapper>();

        private UserFriendCacheSyncronizer CriarSut()
        {
            return new UserFriendCacheSyncronizer(_userFriendsCache, _objectMapper);
        }

        private static ChatMessage CriarChatMessage(ChatMessageReadState readState)
        {
            return new ChatMessage(
                new UserIdentifier(1, 10),
                new UserIdentifier(1, 20),
                ChatSide.Sender,
                "olá",
                readState,
                Guid.NewGuid(),
                ChatMessageReadState.Unread);
        }

        [Fact]
        public void Dado_MensagemNaoLida_Quando_HandleEventChatMessageCriada_Entao_DeveIncrementarContadorNaoLidas()
        {
            var sut = CriarSut();
            var eventData = new EntityCreatedEventData<ChatMessage>(CriarChatMessage(ChatMessageReadState.Unread));

            sut.HandleEvent(eventData);

            _userFriendsCache.Received(1).IncreaseUnreadMessageCount(
                Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>(), 1);
        }

        [Fact]
        public void Dado_MensagemLida_Quando_HandleEventChatMessageCriada_Entao_NaoDeveIncrementarContador()
        {
            var sut = CriarSut();
            var eventData = new EntityCreatedEventData<ChatMessage>(CriarChatMessage(ChatMessageReadState.Read));

            sut.HandleEvent(eventData);

            _userFriendsCache.DidNotReceive().IncreaseUnreadMessageCount(
                Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>(), Arg.Any<int>());
        }
    }
}

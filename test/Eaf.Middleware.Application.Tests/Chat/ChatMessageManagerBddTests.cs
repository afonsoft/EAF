using Abp;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.RealTime;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Cache;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Chat
{
    public class ChatMessageManagerBddTests
    {
        private static ChatMessageManager CriarChatMessageManager(
            IRepository<ChatMessage, long>? chatMessageRepository,
            UserManager? userManager)
        {
            var friendshipManager = Substitute.For<IFriendshipManager>();
            var chatCommunicator = Substitute.For<IChatCommunicator>();
            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            if (userManager == null) userManager = ManagerTestHelper.CreateUserManager();
            if (chatMessageRepository == null) chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            var tenantCache = Substitute.For<ITenantCache>();
            var userFriendsCache = Substitute.For<IUserFriendsCache>();
            var userEmailer = Substitute.For<IUserEmailer>();
            var chatFeatureChecker = Substitute.For<IChatFeatureChecker>();

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            return new ChatMessageManager(
                friendshipManager,
                chatCommunicator,
                onlineClientManager,
                userManager,
                tenantCache,
                userFriendsCache,
                userEmailer,
                chatMessageRepository,
                chatFeatureChecker,
                unitOfWorkManager);
        }

        private static ChatMessage CriarMensagem(long id, long userId, long targetUserId, Guid sharedMessageId, ChatMessageReadState readState = ChatMessageReadState.Unread)
        {
            var message = new ChatMessage(
                new UserIdentifier(null, userId),
                new UserIdentifier(null, targetUserId),
                ChatSide.Sender,
                "Hello",
                readState,
                sharedMessageId,
                ChatMessageReadState.Read);
            message.Id = id;
            return message;
        }

        [Fact]
        public async Task Dado_MensagemExistente_Quando_FindMessageAsync_Entao_DeveRetornarMensagem()
        {
            // Dado
            var expected = CriarMensagem(1, 10, 20, Guid.NewGuid());
            var repository = Substitute.For<IRepository<ChatMessage, long>>();
            repository.FirstOrDefaultAsync(Arg.Any<Expression<Func<ChatMessage, bool>>>())
                .Returns(expected);

            var sut = CriarChatMessageManager(repository, null);

            // Quando
            var result = await sut.FindMessageAsync(1, 10);

            // Então
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
        }

        [Fact]
        public void Dado_MensagensNaoLidas_Quando_GetUnreadMessageCount_Entao_DeveRetornarQuantidade()
        {
            // Dado
            var repository = Substitute.For<IRepository<ChatMessage, long>>();
            repository.Count(Arg.Any<Expression<Func<ChatMessage, bool>>>()).Returns(3);

            var sut = CriarChatMessageManager(repository, null);

            // Quando
            var result = sut.GetUnreadMessageCount(
                new UserIdentifier(null, 10),
                new UserIdentifier(null, 20));

            // Então
            result.ShouldBe(3);
        }

        [Fact]
        public void Dado_NovaMensagem_Quando_Save_Entao_DeveRetornarIdInserido()
        {
            // Dado
            var message = CriarMensagem(0, 10, 20, Guid.NewGuid());
            var repository = Substitute.For<IRepository<ChatMessage, long>>();
            repository.InsertAndGetId(Arg.Any<ChatMessage>()).Returns(99L);

            var sut = CriarChatMessageManager(repository, null);

            // Quando
            var result = sut.Save(message);

            // Então
            result.ShouldBe(99L);
        }

        [Fact]
        public void Dado_MensagensComMaisDeTresDias_Quando_Delete_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var sharedMessageId = Guid.NewGuid();
            var message = CriarMensagem(1, 10, 20, sharedMessageId);
            message.CreationTime = DateTime.Now.AddDays(-5);

            var repository = Substitute.For<IRepository<ChatMessage, long>>();
            var messages = new List<ChatMessage> { message }.AsAsyncQueryable();
            repository.GetAll().Returns(messages);
            repository.Delete(Arg.Any<Expression<Func<ChatMessage, bool>>>());

            var sut = CriarChatMessageManager(repository, null);

            // Quando/Então
            Should.Throw<Abp.UI.UserFriendlyException>(() => sut.Delete(sharedMessageId));
        }
    }
}

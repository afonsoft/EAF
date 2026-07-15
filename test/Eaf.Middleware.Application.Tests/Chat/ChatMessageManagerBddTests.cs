using Abp;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.MultiTenancy;
using Abp.RealTime;
using Abp.UI;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Cache;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Chat
{
    public class ChatMessageManagerBddTests
    {
        private static TestableChatMessageManager CriarChatMessageManager(
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

            return new TestableChatMessageManager(
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

        [Fact]
        public void Dado_MensagensRecentes_Quando_Delete_Entao_DeveRemoverMensagens()
        {
            // Dado
            var sharedMessageId = Guid.NewGuid();
            var message = CriarMensagem(1, 10, 20, sharedMessageId);
            message.CreationTime = DateTime.Now;

            var repository = Substitute.For<IRepository<ChatMessage, long>>();
            var messages = new List<ChatMessage> { message }.AsAsyncQueryable();
            repository.GetAll().Returns(messages);
            repository.Delete(Arg.Any<Expression<Func<ChatMessage, bool>>>());

            var sut = CriarChatMessageManager(repository, null);

            // Quando
            sut.Delete(sharedMessageId);

            // Então
            repository.Received(1).Delete(Arg.Any<Expression<Func<ChatMessage, bool>>>());
        }

        [Fact]
        public void Dado_NenhumaMensagem_Quando_Delete_Entao_DeveChamarDeleteComListaVazia()
        {
            // Dado
            var sharedMessageId = Guid.NewGuid();

            var repository = Substitute.For<IRepository<ChatMessage, long>>();
            repository.GetAll().Returns(new List<ChatMessage>().AsAsyncQueryable());

            var sut = CriarChatMessageManager(repository, null);

            // Quando
            sut.Delete(sharedMessageId);

            // Então - ainda chama Delete, mas com ids vazios
            repository.Received(1).Delete(Arg.Any<Expression<Func<ChatMessage, bool>>>());
        }

        [Fact]
        public async Task Dado_UsuariosAtivos_Quando_SendMessageToGroupAsync_Entao_DeveSalvarMensagensParaSenderEReceivers()
        {
            // Dado
            var sender = new UserIdentifier(null, 10);
            var receiverGroup = new UserIdentifier(null, 20);

            var user = new User { Id = 20, UserName = "receiver", IsActive = true };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.Users.Returns(new List<User> { user }.AsAsyncQueryable());

            var repository = Substitute.For<IRepository<ChatMessage, long>>();
            repository.InsertAndGetId(Arg.Any<ChatMessage>()).Returns(1L);

            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>())
                .Returns(Task.FromResult<IReadOnlyList<IOnlineClient>>(new List<IOnlineClient>()));

            var chatCommunicator = Substitute.For<IChatCommunicator>();
            chatCommunicator.SendMessageToClient(Arg.Any<IReadOnlyList<IOnlineClient>>(), Arg.Any<ChatMessage>())
                .Returns(Task.CompletedTask);

            var chatFeatureChecker = Substitute.For<IChatFeatureChecker>();
            chatFeatureChecker.CheckChatFeatures(Arg.Any<int?>(), Arg.Any<int?>());

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var sut = new ChatMessageManager(
                Substitute.For<IFriendshipManager>(),
                chatCommunicator,
                onlineClientManager,
                userManager,
                Substitute.For<ITenantCache>(),
                Substitute.For<IUserFriendsCache>(),
                Substitute.For<IUserEmailer>(),
                repository,
                chatFeatureChecker,
                unitOfWorkManager);

            // Quando
            await sut.SendMessageToGroupAsync(sender, receiverGroup, "Hello group");

            // Então
            repository.Received(2).InsertAndGetId(Arg.Any<ChatMessage>());
            await chatCommunicator.Received(1).SendMessageToClient(Arg.Any<IReadOnlyList<IOnlineClient>>(), Arg.Any<ChatMessage>());
        }

        [Fact]
        public async Task Dado_UsuariosAmigos_Quando_SendMessageAsync_Entao_DeveSalvarMensagensParaAmbos()
        {
            // Dado
            var sender = new UserIdentifier(1, 10);
            var receiver = new UserIdentifier(1, 20);
            var senderUser = new User { Id = 10, UserName = "sender", TenantId = 1 };
            var receiverUser = new User { Id = 20, UserName = "receiver", TenantId = 1 };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });
            userManager.GetUserAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });

            var friendshipManager = Substitute.For<IFriendshipManager>();
            friendshipManager.GetFriendshipOrNullAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>()).Returns((Friendship?)null);

            var tenantCache = Substitute.For<ITenantCache>();
            tenantCache.Get(1).Returns(new TenantCacheItem { TenancyName = "acme" });
            tenantCache.GetAsync(1).Returns(Task.FromResult(new TenantCacheItem { TenancyName = "acme" }));

            var chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            chatMessageRepository.InsertAndGetId(Arg.Any<ChatMessage>()).Returns(1L);
            chatMessageRepository.Count(Arg.Any<Expression<Func<ChatMessage, bool>>>()).Returns(1);

            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>()).Returns(new List<IOnlineClient>());

            var chatCommunicator = Substitute.For<IChatCommunicator>();
            chatCommunicator.SendMessageToClient(Arg.Any<IReadOnlyList<IOnlineClient>>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var userEmailer = Substitute.For<IUserEmailer>();
            userEmailer.TryToSendChatMessageMail(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var sut = new ChatMessageManager(
                friendshipManager,
                chatCommunicator,
                onlineClientManager,
                userManager,
                tenantCache,
                Substitute.For<IUserFriendsCache>(),
                userEmailer,
                chatMessageRepository,
                Substitute.For<IChatFeatureChecker>(),
                unitOfWorkManager);

            // Quando
            await sut.SendMessageAsync(sender, receiver, "Hello", "acme", "sender", null);

            // Então
            chatMessageRepository.Received(2).InsertAndGetId(Arg.Any<ChatMessage>());
            await friendshipManager.Received(2).CreateFriendshipAsync(Arg.Any<Friendship>());
            await userEmailer.Received(1).TryToSendChatMessageMail(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ChatMessage>());
        }

        [Fact]
        public async Task Dado_UsuariosAmigosSemTenant_Quando_SendMessageAsync_Entao_DeveCriarAmizadeSemTenancyName()
        {
            // Dado
            var sender = new UserIdentifier(null, 10);
            var receiver = new UserIdentifier(null, 20);
            var senderUser = new User { Id = 10, UserName = "sender" };
            var receiverUser = new User { Id = 20, UserName = "receiver" };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });
            userManager.GetUserAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });

            var friendshipManager = Substitute.For<IFriendshipManager>();
            friendshipManager.GetFriendshipOrNullAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>()).Returns((Friendship?)null);

            var chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            chatMessageRepository.InsertAndGetId(Arg.Any<ChatMessage>()).Returns(1L);
            chatMessageRepository.Count(Arg.Any<Expression<Func<ChatMessage, bool>>>()).Returns(1);

            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>()).Returns(new List<IOnlineClient>());

            var chatCommunicator = Substitute.For<IChatCommunicator>();
            chatCommunicator.SendMessageToClient(Arg.Any<IReadOnlyList<IOnlineClient>>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var userEmailer = Substitute.For<IUserEmailer>();
            userEmailer.TryToSendChatMessageMail(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var sut = new ChatMessageManager(
                friendshipManager,
                chatCommunicator,
                onlineClientManager,
                userManager,
                Substitute.For<ITenantCache>(),
                Substitute.For<IUserFriendsCache>(),
                userEmailer,
                chatMessageRepository,
                Substitute.For<IChatFeatureChecker>(),
                unitOfWorkManager);

            // Quando
            await sut.SendMessageAsync(sender, receiver, "Hello", null, "sender", null);

            // Então
            chatMessageRepository.Received(2).InsertAndGetId(Arg.Any<ChatMessage>());
            await friendshipManager.Received(2).CreateFriendshipAsync(Arg.Any<Friendship>());
        }

        [Fact]
        public async Task Dado_UsuarioBloqueado_Quando_SendMessageAsync_Entao_DeveLancarExcecaoDeBloqueio()
        {
            // Dado
            var sender = new UserIdentifier(1, 10);
            var receiver = new UserIdentifier(1, 20);
            var receiverUser = new User { Id = 20, UserName = "receiver", TenantId = 1 };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(receiverUser);

            var friendshipManager = Substitute.For<IFriendshipManager>();
            var blocked = new Friendship(sender, receiver, "acme", "receiver", null, FriendshipState.Blocked);
            friendshipManager.GetFriendshipOrNullAsync(sender, receiver).Returns(blocked);

            var chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var sut = new ChatMessageManager(
                friendshipManager,
                Substitute.For<IChatCommunicator>(),
                Substitute.For<IOnlineClientManager<ChatChannel>>(),
                userManager,
                Substitute.For<ITenantCache>(),
                Substitute.For<IUserFriendsCache>(),
                Substitute.For<IUserEmailer>(),
                chatMessageRepository,
                Substitute.For<IChatFeatureChecker>(),
                unitOfWorkManager);

            // Quando / Então
            var exception = await Should.ThrowAsync<UserFriendlyException>(async () =>
                await sut.SendMessageAsync(sender, receiver, "Hello", "acme", "sender", null));
            exception.Message.ShouldContain("UserIsBlocked");
        }

        [Fact]
        public async Task Dado_AmigoComInformacoesDesatualizadas_Quando_SendMessageAsync_Entao_DeveAtualizarDadosDoRemetente()
        {
            // Dado
            var sender = new UserIdentifier(1, 10);
            var receiver = new UserIdentifier(1, 20);
            var senderUser = new User { Id = 10, UserName = "sender", TenantId = 1, ProfilePictureId = Guid.NewGuid() };
            var receiverUser = new User { Id = 20, UserName = "receiver", TenantId = 1 };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });
            userManager.GetUserAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });

            var friendshipManager = Substitute.For<IFriendshipManager>();
            friendshipManager.GetFriendshipOrNullAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>()).Returns((Friendship?)null);

            var tenantCache = Substitute.For<ITenantCache>();
            tenantCache.Get(1).Returns(new TenantCacheItem { TenancyName = "acme" });
            tenantCache.GetAsync(1).Returns(Task.FromResult(new TenantCacheItem { TenancyName = "acme" }));

            var chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            chatMessageRepository.InsertAndGetId(Arg.Any<ChatMessage>()).Returns(1L);

            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>()).Returns(new List<IOnlineClient>());

            var chatCommunicator = Substitute.For<IChatCommunicator>();
            chatCommunicator.SendMessageToClient(Arg.Any<IReadOnlyList<IOnlineClient>>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var userFriendsCache = Substitute.For<IUserFriendsCache>();
            var cacheItem = new UserWithFriendsCacheItem
            {
                Friends = new List<FriendCacheItem>
                {
                    new FriendCacheItem { FriendTenantId = 1, FriendUserId = 10, FriendTenancyName = "old", FriendUserName = "old", FriendProfilePictureId = null }
                }
            };
            userFriendsCache.GetCacheItemOrNull(receiver).Returns(cacheItem);

            var receiverFriendship = new Friendship(receiver, sender, "acme", "old", null, FriendshipState.Accepted);
            friendshipManager.GetFriendshipOrNullAsync(receiver, sender).Returns(receiverFriendship);

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var sut = new ChatMessageManager(
                friendshipManager,
                chatCommunicator,
                onlineClientManager,
                userManager,
                tenantCache,
                userFriendsCache,
                Substitute.For<IUserEmailer>(),
                chatMessageRepository,
                Substitute.For<IChatFeatureChecker>(),
                unitOfWorkManager);

            // Quando
            await sut.SendMessageAsync(sender, receiver, "Hello", "acme", "sender", senderUser.ProfilePictureId);

            // Então
            receiverFriendship.FriendTenancyName.ShouldBe("acme");
            receiverFriendship.FriendUserName.ShouldBe("sender");
            receiverFriendship.FriendProfilePictureId.ShouldBe(senderUser.ProfilePictureId);
            await friendshipManager.Received(1).UpdateFriendshipAsync(receiverFriendship);
        }

        [Fact]
        public async Task Dado_AmigoNaoEncontradoNoCache_Quando_SendMessageAsync_Entao_NaoDeveAtualizarAmizade()
        {
            // Dado
            var sender = new UserIdentifier(1, 10);
            var receiver = new UserIdentifier(1, 20);
            var senderUser = new User { Id = 10, UserName = "sender", TenantId = 1 };
            var receiverUser = new User { Id = 20, UserName = "receiver", TenantId = 1 };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });
            userManager.GetUserAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });

            var friendshipManager = Substitute.For<IFriendshipManager>();
            friendshipManager.GetFriendshipOrNullAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>()).Returns((Friendship?)null);

            var tenantCache = Substitute.For<ITenantCache>();
            tenantCache.Get(1).Returns(new TenantCacheItem { TenancyName = "acme" });
            tenantCache.GetAsync(1).Returns(Task.FromResult(new TenantCacheItem { TenancyName = "acme" }));

            var chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            chatMessageRepository.InsertAndGetId(Arg.Any<ChatMessage>()).Returns(1L);
            chatMessageRepository.Count(Arg.Any<System.Linq.Expressions.Expression<Func<ChatMessage, bool>>>()).Returns(1);

            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>()).Returns(new List<IOnlineClient>());

            var chatCommunicator = Substitute.For<IChatCommunicator>();
            chatCommunicator.SendMessageToClient(Arg.Any<IReadOnlyList<IOnlineClient>>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var userFriendsCache = Substitute.For<IUserFriendsCache>();
            userFriendsCache.GetCacheItemOrNull(receiver).Returns((UserWithFriendsCacheItem?)null);

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var sut = new ChatMessageManager(
                friendshipManager,
                chatCommunicator,
                onlineClientManager,
                userManager,
                tenantCache,
                userFriendsCache,
                Substitute.For<IUserEmailer>(),
                chatMessageRepository,
                Substitute.For<IChatFeatureChecker>(),
                unitOfWorkManager);

            // Quando
            await sut.SendMessageAsync(sender, receiver, "Hello", "acme", "sender", senderUser.ProfilePictureId);

            // Então
            await friendshipManager.DidNotReceive().UpdateFriendshipAsync(Arg.Any<Friendship>());
        }

        [Fact]
        public async Task Dado_ReceptorBloqueouRemetente_Quando_SendMessageAsync_Entao_DeveSalvarApenasMensagemDoRemetente()
        {
            // Dado
            var sender = new UserIdentifier(1, 10);
            var receiver = new UserIdentifier(1, 20);
            var senderUser = new User { Id = 10, UserName = "sender", TenantId = 1 };
            var receiverUser = new User { Id = 20, UserName = "receiver", TenantId = 1 };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(receiverUser);
            userManager.GetUserAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });

            var friendshipManager = Substitute.For<IFriendshipManager>();
            friendshipManager.GetFriendshipOrNullAsync(sender, receiver).Returns((Friendship?)null);
            friendshipManager.GetFriendshipOrNullAsync(receiver, sender).Returns(new Friendship(receiver, sender, "acme", "sender", null, FriendshipState.Blocked));

            var tenantCache = Substitute.For<ITenantCache>();
            tenantCache.Get(1).Returns(new TenantCacheItem { TenancyName = "acme" });
            tenantCache.GetAsync(1).Returns(Task.FromResult(new TenantCacheItem { TenancyName = "acme" }));

            var chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            chatMessageRepository.InsertAndGetId(Arg.Any<ChatMessage>()).Returns(1L);

            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>()).Returns(new List<IOnlineClient>());

            var chatCommunicator = Substitute.For<IChatCommunicator>();
            chatCommunicator.SendMessageToClient(Arg.Any<IReadOnlyList<IOnlineClient>>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var userEmailer = Substitute.For<IUserEmailer>();
            userEmailer.TryToSendChatMessageMail(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var sut = new ChatMessageManager(
                friendshipManager,
                chatCommunicator,
                onlineClientManager,
                userManager,
                tenantCache,
                Substitute.For<IUserFriendsCache>(),
                userEmailer,
                chatMessageRepository,
                Substitute.For<IChatFeatureChecker>(),
                unitOfWorkManager);

            // Quando
            await sut.SendMessageAsync(sender, receiver, "Hello", "acme", "sender", null);

            // Então
            chatMessageRepository.Received(1).InsertAndGetId(Arg.Any<ChatMessage>());
            await userEmailer.DidNotReceive().TryToSendChatMessageMail(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ChatMessage>());
        }

        [Fact]
        public async Task Dado_AmizadeJaExistente_Quando_SendMessageAsync_Entao_DeveSalvarMensagensSemCriarAmizade()
        {
            // Dado
            var sender = new UserIdentifier(1, 10);
            var receiver = new UserIdentifier(1, 20);
            var senderUser = new User { Id = 10, UserName = "sender", TenantId = 1 };
            var receiverUser = new User { Id = 20, UserName = "receiver", TenantId = 1 };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(receiverUser);
            userManager.GetUserAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });

            var friendshipManager = Substitute.For<IFriendshipManager>();
            friendshipManager.GetFriendshipOrNullAsync(sender, receiver).Returns(new Friendship(sender, receiver, "acme", "receiver", null, FriendshipState.Accepted));
            friendshipManager.GetFriendshipOrNullAsync(receiver, sender).Returns(new Friendship(receiver, sender, "acme", "sender", null, FriendshipState.Accepted));

            var tenantCache = Substitute.For<ITenantCache>();
            tenantCache.Get(1).Returns(new TenantCacheItem { TenancyName = "acme" });
            tenantCache.GetAsync(1).Returns(Task.FromResult(new TenantCacheItem { TenancyName = "acme" }));

            var chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            chatMessageRepository.InsertAndGetId(Arg.Any<ChatMessage>()).Returns(1L);
            chatMessageRepository.Count(Arg.Any<System.Linq.Expressions.Expression<Func<ChatMessage, bool>>>()).Returns(1);

            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>()).Returns(new List<IOnlineClient>());

            var chatCommunicator = Substitute.For<IChatCommunicator>();
            chatCommunicator.SendMessageToClient(Arg.Any<IReadOnlyList<IOnlineClient>>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var userEmailer = Substitute.For<IUserEmailer>();
            userEmailer.TryToSendChatMessageMail(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var sut = new ChatMessageManager(
                friendshipManager,
                chatCommunicator,
                onlineClientManager,
                userManager,
                tenantCache,
                Substitute.For<IUserFriendsCache>(),
                userEmailer,
                chatMessageRepository,
                Substitute.For<IChatFeatureChecker>(),
                unitOfWorkManager);

            // Quando
            await sut.SendMessageAsync(sender, receiver, "Hello", "acme", "sender", null);

            // Então
            chatMessageRepository.Received(2).InsertAndGetId(Arg.Any<ChatMessage>());
            await friendshipManager.DidNotReceive().CreateFriendshipAsync(Arg.Any<Friendship>());
            await userEmailer.Received(1).TryToSendChatMessageMail(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ChatMessage>());
        }

        [Fact]
        public async Task Dado_AmigoComInformacoesAtualizadas_Quando_SendMessageAsync_Entao_NaoDeveAtualizarAmizade()
        {
            // Dado
            var sender = new UserIdentifier(1, 10);
            var receiver = new UserIdentifier(1, 20);
            var senderUser = new User { Id = 10, UserName = "sender", TenantId = 1, ProfilePictureId = Guid.NewGuid() };
            var receiverUser = new User { Id = 20, UserName = "receiver", TenantId = 1 };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });
            userManager.GetUserAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });

            var friendshipManager = Substitute.For<IFriendshipManager>();
            friendshipManager.GetFriendshipOrNullAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>()).Returns((Friendship?)null);

            var tenantCache = Substitute.For<ITenantCache>();
            tenantCache.Get(1).Returns(new TenantCacheItem { TenancyName = "acme" });
            tenantCache.GetAsync(1).Returns(Task.FromResult(new TenantCacheItem { TenancyName = "acme" }));

            var chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            chatMessageRepository.InsertAndGetId(Arg.Any<ChatMessage>()).Returns(1L);

            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>()).Returns(new List<IOnlineClient>());

            var chatCommunicator = Substitute.For<IChatCommunicator>();
            chatCommunicator.SendMessageToClient(Arg.Any<IReadOnlyList<IOnlineClient>>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var userFriendsCache = Substitute.For<IUserFriendsCache>();
            var cacheItem = new UserWithFriendsCacheItem
            {
                Friends = new List<FriendCacheItem>
                {
                    new FriendCacheItem
                    {
                        FriendTenantId = 1,
                        FriendUserId = 10,
                        FriendTenancyName = "acme",
                        FriendUserName = "sender",
                        FriendProfilePictureId = senderUser.ProfilePictureId
                    }
                }
            };
            userFriendsCache.GetCacheItemOrNull(receiver).Returns(cacheItem);

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var sut = new ChatMessageManager(
                friendshipManager,
                chatCommunicator,
                onlineClientManager,
                userManager,
                tenantCache,
                userFriendsCache,
                Substitute.For<IUserEmailer>(),
                chatMessageRepository,
                Substitute.For<IChatFeatureChecker>(),
                unitOfWorkManager);

            // Quando
            await sut.SendMessageAsync(sender, receiver, "Hello", "acme", "sender", senderUser.ProfilePictureId);

            // Então
            await friendshipManager.DidNotReceive().UpdateFriendshipAsync(Arg.Any<Friendship>());
        }

        [Fact]
        public async Task Dado_AmigoNoCacheSemFriendship_Quando_SendMessageAsync_Entao_NaoDeveAtualizarAmizade()
        {
            // Dado
            var sender = new UserIdentifier(1, 10);
            var receiver = new UserIdentifier(1, 20);
            var senderUser = new User { Id = 10, UserName = "sender", TenantId = 1, ProfilePictureId = Guid.NewGuid() };
            var receiverUser = new User { Id = 20, UserName = "receiver", TenantId = 1 };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });
            userManager.GetUserAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });

            var friendshipManager = Substitute.For<IFriendshipManager>();
            friendshipManager.GetFriendshipOrNullAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>()).Returns((Friendship?)null);
            friendshipManager.GetFriendshipOrNullAsync(receiver, sender).Returns((Friendship?)null);

            var tenantCache = Substitute.For<ITenantCache>();
            tenantCache.Get(1).Returns(new TenantCacheItem { TenancyName = "acme" });
            tenantCache.GetAsync(1).Returns(Task.FromResult(new TenantCacheItem { TenancyName = "acme" }));

            var chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            chatMessageRepository.InsertAndGetId(Arg.Any<ChatMessage>()).Returns(1L);

            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>()).Returns(new List<IOnlineClient>());

            var chatCommunicator = Substitute.For<IChatCommunicator>();
            chatCommunicator.SendMessageToClient(Arg.Any<IReadOnlyList<IOnlineClient>>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var userFriendsCache = Substitute.For<IUserFriendsCache>();
            var cacheItem = new UserWithFriendsCacheItem
            {
                Friends = new List<FriendCacheItem>
                {
                    new FriendCacheItem
                    {
                        FriendTenantId = 1,
                        FriendUserId = 10,
                        FriendTenancyName = "old",
                        FriendUserName = "old",
                        FriendProfilePictureId = null
                    }
                }
            };
            userFriendsCache.GetCacheItemOrNull(receiver).Returns(cacheItem);

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var sut = new ChatMessageManager(
                friendshipManager,
                chatCommunicator,
                onlineClientManager,
                userManager,
                tenantCache,
                userFriendsCache,
                Substitute.For<IUserEmailer>(),
                chatMessageRepository,
                Substitute.For<IChatFeatureChecker>(),
                unitOfWorkManager);

            // Quando
            await sut.SendMessageAsync(sender, receiver, "Hello", "acme", "sender", senderUser.ProfilePictureId);

            // Então
            await friendshipManager.DidNotReceive().UpdateFriendshipAsync(Arg.Any<Friendship>());
        }

        [Fact]
        public async Task Dado_ReceptorOfflineSemUnicaMensagemNaoLida_Quando_SendMessageAsync_Entao_NaoDeveEnviarEmail()
        {
            // Dado
            var sender = new UserIdentifier(1, 10);
            var receiver = new UserIdentifier(1, 20);
            var senderUser = new User { Id = 10, UserName = "sender", TenantId = 1 };
            var receiverUser = new User { Id = 20, UserName = "receiver", TenantId = 1 };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(receiverUser);
            userManager.GetUserAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });

            var friendshipManager = Substitute.For<IFriendshipManager>();
            friendshipManager.GetFriendshipOrNullAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>()).Returns((Friendship?)null);

            var tenantCache = Substitute.For<ITenantCache>();
            tenantCache.Get(1).Returns(new TenantCacheItem { TenancyName = "acme" });
            tenantCache.GetAsync(1).Returns(Task.FromResult(new TenantCacheItem { TenancyName = "acme" }));

            var chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            chatMessageRepository.InsertAndGetId(Arg.Any<ChatMessage>()).Returns(1L);
            chatMessageRepository.Count(Arg.Any<System.Linq.Expressions.Expression<Func<ChatMessage, bool>>>()).Returns(2);

            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>()).Returns(new List<IOnlineClient>());

            var chatCommunicator = Substitute.For<IChatCommunicator>();
            chatCommunicator.SendMessageToClient(Arg.Any<IReadOnlyList<IOnlineClient>>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var userEmailer = Substitute.For<IUserEmailer>();
            userEmailer.TryToSendChatMessageMail(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var sut = new ChatMessageManager(
                friendshipManager,
                chatCommunicator,
                onlineClientManager,
                userManager,
                tenantCache,
                Substitute.For<IUserFriendsCache>(),
                userEmailer,
                chatMessageRepository,
                Substitute.For<IChatFeatureChecker>(),
                unitOfWorkManager);

            // Quando
            await sut.SendMessageAsync(sender, receiver, "Hello", "acme", "sender", null);

            // Então
            await userEmailer.DidNotReceive().TryToSendChatMessageMail(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<ChatMessage>());
        }

        [Fact]
        public async Task Dado_UsuariosAtivosEClientesOnline_Quando_SendMessageToGroupAsync_Entao_DeveEnviarMensagemParaTodos()
        {
            // Dado
            var sender = new UserIdentifier(null, 10);
            var receiverGroup = new UserIdentifier(null, 20);

            var senderUser = new User { Id = 10, UserName = "sender", IsActive = true };
            var receiverUser = new User { Id = 20, UserName = "receiver", IsActive = true };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.Users.Returns(new List<User> { senderUser, receiverUser }.AsAsyncQueryable());

            var chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            chatMessageRepository.InsertAndGetId(Arg.Any<ChatMessage>()).Returns(1L);

            var onlineClient = Substitute.For<IOnlineClient>();
            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>())
                .Returns(ci =>
                {
                    var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                    return Task.FromResult<IReadOnlyList<IOnlineClient>>(userId == receiverUser.Id ? new List<IOnlineClient> { onlineClient } : new List<IOnlineClient>());
                });

            var chatCommunicator = Substitute.For<IChatCommunicator>();
            chatCommunicator.SendMessageToClient(Arg.Any<IReadOnlyList<IOnlineClient>>(), Arg.Any<ChatMessage>())
                .Returns(Task.CompletedTask);

            var chatFeatureChecker = Substitute.For<IChatFeatureChecker>();
            chatFeatureChecker.CheckChatFeatures(Arg.Any<int?>(), Arg.Any<int?>());

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var sut = new ChatMessageManager(
                Substitute.For<IFriendshipManager>(),
                chatCommunicator,
                onlineClientManager,
                userManager,
                Substitute.For<ITenantCache>(),
                Substitute.For<IUserFriendsCache>(),
                Substitute.For<IUserEmailer>(),
                chatMessageRepository,
                chatFeatureChecker,
                unitOfWorkManager);

            // Quando
            await sut.SendMessageToGroupAsync(sender, receiverGroup, "Hello group");

            // Então
            chatMessageRepository.Received(2).InsertAndGetId(Arg.Any<ChatMessage>());
            await chatCommunicator.Received(2).SendMessageToClient(Arg.Any<IReadOnlyList<IOnlineClient>>(), Arg.Any<ChatMessage>());
        }

        [Fact]
        public async Task Dado_UsuarioDestinoInexistente_Quando_SendMessageAsync_Entao_DeveLancarExcecaoUsuarioNaoEncontrado()
        {
            var sender = new UserIdentifier(1, 10);
            var receiver = new UserIdentifier(1, 20);

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns((User?)null);

            var repository = Substitute.For<IRepository<ChatMessage, long>>();
            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var sut = new ChatMessageManager(
                Substitute.For<IFriendshipManager>(),
                Substitute.For<IChatCommunicator>(),
                Substitute.For<IOnlineClientManager<ChatChannel>>(),
                userManager,
                Substitute.For<ITenantCache>(),
                Substitute.For<IUserFriendsCache>(),
                Substitute.For<IUserEmailer>(),
                repository,
                Substitute.For<IChatFeatureChecker>(),
                unitOfWorkManager);

            var exception = await Should.ThrowAsync<UserFriendlyException>(() =>
                sut.SendMessageAsync(sender, receiver, "Hello", "acme", "sender", null));
            exception.Message.ShouldContain("TargetUserNotFoundProbablyDeleted");
        }

        [Fact]
        public async Task Dado_UsuariosAmigosEClienteOnline_Quando_SendMessageAsync_Entao_DeveEnviarMensagemParaCliente()
        {
            var sender = new UserIdentifier(1, 10);
            var receiver = new UserIdentifier(1, 20);
            var senderUser = new User { Id = 10, UserName = "sender", TenantId = 1 };
            var receiverUser = new User { Id = 20, UserName = "receiver", TenantId = 1 };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });
            userManager.GetUserAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });

            var friendshipManager = Substitute.For<IFriendshipManager>();
            friendshipManager.GetFriendshipOrNullAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>()).Returns((Friendship?)null);

            var tenantCache = Substitute.For<ITenantCache>();
            tenantCache.Get(1).Returns(new TenantCacheItem { TenancyName = "acme" });
            tenantCache.GetAsync(1).Returns(Task.FromResult(new TenantCacheItem { TenancyName = "acme" }));

            var chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            chatMessageRepository.InsertAndGetId(Arg.Any<ChatMessage>()).Returns(1L);

            var onlineClient = Substitute.For<IOnlineClient>();
            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>()).Returns(Task.FromResult<IReadOnlyList<IOnlineClient>>(new List<IOnlineClient> { onlineClient }));

            var chatCommunicator = Substitute.For<IChatCommunicator>();
            chatCommunicator.SendMessageToClient(Arg.Any<IReadOnlyList<IOnlineClient>>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var sut = new ChatMessageManager(
                friendshipManager,
                chatCommunicator,
                onlineClientManager,
                userManager,
                tenantCache,
                Substitute.For<IUserFriendsCache>(),
                Substitute.For<IUserEmailer>(),
                chatMessageRepository,
                Substitute.For<IChatFeatureChecker>(),
                unitOfWorkManager);

            await sut.SendMessageAsync(sender, receiver, "Hello", "acme", "sender", null);

            await chatCommunicator.Received(2).SendMessageToClient(
                Arg.Is<IReadOnlyList<IOnlineClient>>(list => list.Count == 1),
                Arg.Any<ChatMessage>());
        }

        [Fact]
        public async Task Dado_UsuarioLogadoEAmigoNaoPossuiInformacaoDesatualizada_Quando_SendMessageAsync_Entao_DeveRetornarSemAtualizarAmizade()
        {
            var sender = new UserIdentifier(1, 10);
            var receiver = new UserIdentifier(1, 20);
            var senderUser = new User { Id = 10, UserName = "sender", TenantId = 1 };
            var receiverUser = new User { Id = 20, UserName = "receiver", TenantId = 1 };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });
            userManager.GetUserAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });

            var friendshipManager = Substitute.For<IFriendshipManager>();
            friendshipManager.GetFriendshipOrNullAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>()).Returns((Friendship?)null);

            var tenantCache = Substitute.For<ITenantCache>();
            tenantCache.Get(1).Returns(new TenantCacheItem { TenancyName = "acme" });
            tenantCache.GetAsync(1).Returns(Task.FromResult(new TenantCacheItem { TenancyName = "acme" }));

            var chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            chatMessageRepository.InsertAndGetId(Arg.Any<ChatMessage>()).Returns(1L);

            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>()).Returns(new List<IOnlineClient>());

            var chatCommunicator = Substitute.For<IChatCommunicator>();
            chatCommunicator.SendMessageToClient(Arg.Any<IReadOnlyList<IOnlineClient>>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var userFriendsCache = Substitute.For<IUserFriendsCache>();
            var cacheItem = new UserWithFriendsCacheItem
            {
                Friends = new List<FriendCacheItem>
                {
                    new FriendCacheItem { FriendTenantId = sender.TenantId, FriendUserId = sender.UserId, FriendTenancyName = "acme", FriendUserName = "sender", FriendProfilePictureId = null }
                }
            };
            userFriendsCache.GetCacheItemOrNull(receiver).Returns(cacheItem);

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var sut = new ChatMessageManager(
                friendshipManager,
                chatCommunicator,
                onlineClientManager,
                userManager,
                tenantCache,
                userFriendsCache,
                Substitute.For<IUserEmailer>(),
                chatMessageRepository,
                Substitute.For<IChatFeatureChecker>(),
                unitOfWorkManager);

            await sut.SendMessageAsync(sender, receiver, "Hello", "acme", "sender", null);

            await friendshipManager.DidNotReceive().UpdateFriendshipAsync(Arg.Any<Friendship>());
        }

        [Theory]
        [InlineData("old", "sender", null, "acme", "sender", null)]
        [InlineData("acme", "old", null, "acme", "sender", null)]
        [InlineData("acme", "sender", "00000000-0000-0000-0000-000000000001", "acme", "sender", null)]
        public async Task Dado_AmigoComUmaInformacaoDesatualizada_Quando_SendMessageAsync_Entao_DeveAtualizarAmizade(
            string friendTenancyName, string friendUserName, string? friendProfilePictureId,
            string senderTenancyName, string senderUserName, string? senderProfilePictureId)
        {
            var sender = new UserIdentifier(1, 10);
            var receiver = new UserIdentifier(1, 20);
            var senderUser = new User { Id = 10, UserName = "sender", TenantId = 1 };
            var receiverUser = new User { Id = 20, UserName = "receiver", TenantId = 1 };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });
            userManager.GetUserAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });

            var friendshipManager = Substitute.For<IFriendshipManager>();
            friendshipManager.GetFriendshipOrNullAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>()).Returns((Friendship?)null);

            var tenantCache = Substitute.For<ITenantCache>();
            tenantCache.Get(1).Returns(new TenantCacheItem { TenancyName = "acme" });
            tenantCache.GetAsync(1).Returns(Task.FromResult(new TenantCacheItem { TenancyName = "acme" }));

            var chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            chatMessageRepository.InsertAndGetId(Arg.Any<ChatMessage>()).Returns(1L);

            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>()).Returns(new List<IOnlineClient>());

            var chatCommunicator = Substitute.For<IChatCommunicator>();
            chatCommunicator.SendMessageToClient(Arg.Any<IReadOnlyList<IOnlineClient>>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var userFriendsCache = Substitute.For<IUserFriendsCache>();
            Guid? pictureId = string.IsNullOrEmpty(friendProfilePictureId) ? null : Guid.Parse(friendProfilePictureId);
            var cacheItem = new UserWithFriendsCacheItem
            {
                Friends = new List<FriendCacheItem>
                {
                    new FriendCacheItem { FriendTenantId = sender.TenantId, FriendUserId = sender.UserId, FriendTenancyName = friendTenancyName, FriendUserName = friendUserName, FriendProfilePictureId = pictureId }
                }
            };
            userFriendsCache.GetCacheItemOrNull(receiver).Returns(cacheItem);

            var receiverFriendship = new Friendship(receiver, sender, friendTenancyName, friendUserName, pictureId, FriendshipState.Accepted);
            friendshipManager.GetFriendshipOrNullAsync(receiver, sender).Returns(receiverFriendship);

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var sut = new ChatMessageManager(
                friendshipManager,
                chatCommunicator,
                onlineClientManager,
                userManager,
                tenantCache,
                userFriendsCache,
                Substitute.For<IUserEmailer>(),
                chatMessageRepository,
                Substitute.For<IChatFeatureChecker>(),
                unitOfWorkManager);

            Guid? senderPictureId = string.IsNullOrEmpty(senderProfilePictureId) ? null : Guid.Parse(senderProfilePictureId);
            await sut.SendMessageAsync(sender, receiver, "Hello", senderTenancyName, senderUserName, senderPictureId);

            await friendshipManager.Received(1).UpdateFriendshipAsync(receiverFriendship);
        }

        [Fact]
        public async Task Dado_UsuarioLogadoComDestinoSemTenant_Quando_SendMessageAsync_Entao_DeveCriarAmizadeSemTenancyName()
        {
            var sender = new UserIdentifier(1, 10);
            var receiver = new UserIdentifier(null, 20);
            var senderUser = new User { Id = 10, UserName = "sender", TenantId = 1 };
            var receiverUser = new User { Id = 20, UserName = "receiver", TenantId = 1 };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(receiverUser);
            userManager.GetUserAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });

            var friendshipManager = Substitute.For<IFriendshipManager>();
            friendshipManager.GetFriendshipOrNullAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>()).Returns((Friendship?)null);

            var tenantCache = Substitute.For<ITenantCache>();
            tenantCache.Get(1).Returns(new TenantCacheItem { TenancyName = "acme" });
            tenantCache.GetAsync(1).Returns(Task.FromResult(new TenantCacheItem { TenancyName = "acme" }));

            var chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            chatMessageRepository.InsertAndGetId(Arg.Any<ChatMessage>()).Returns(1L);

            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>()).Returns(new List<IOnlineClient>());

            var chatCommunicator = Substitute.For<IChatCommunicator>();
            chatCommunicator.SendMessageToClient(Arg.Any<IReadOnlyList<IOnlineClient>>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var sut = new TestableChatMessageManager(
                friendshipManager,
                chatCommunicator,
                onlineClientManager,
                userManager,
                tenantCache,
                Substitute.For<IUserFriendsCache>(),
                Substitute.For<IUserEmailer>(),
                chatMessageRepository,
                Substitute.For<IChatFeatureChecker>(),
                unitOfWorkManager);

            await sut.SendMessageAsync(sender, receiver, "Hello", "acme", "sender", null);

            await friendshipManager.Received(2).CreateFriendshipAsync(Arg.Any<Friendship>());
        }

        [Fact]
        public async Task Dado_AmizadeAtualizadaNaoEncontrada_Quando_SendMessageAsync_Entao_NaoDeveAtualizarAmizade()
        {
            var sender = new UserIdentifier(1, 10);
            var receiver = new UserIdentifier(1, 20);
            var senderUser = new User { Id = 10, UserName = "sender", TenantId = 1, ProfilePictureId = Guid.NewGuid() };
            var receiverUser = new User { Id = 20, UserName = "receiver", TenantId = 1 };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserOrNullAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });
            userManager.GetUserAsync(Arg.Any<UserIdentifier>()).Returns(ci =>
            {
                var userId = ci.ArgAt<UserIdentifier>(0).UserId;
                return Task.FromResult(userId == senderUser.Id ? senderUser : receiverUser);
            });

            var friendshipManager = Substitute.For<IFriendshipManager>();
            friendshipManager.GetFriendshipOrNullAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>()).Returns((Friendship?)null);

            var tenantCache = Substitute.For<ITenantCache>();
            tenantCache.Get(1).Returns(new TenantCacheItem { TenancyName = "acme" });
            tenantCache.GetAsync(1).Returns(Task.FromResult(new TenantCacheItem { TenancyName = "acme" }));

            var chatMessageRepository = Substitute.For<IRepository<ChatMessage, long>>();
            chatMessageRepository.InsertAndGetId(Arg.Any<ChatMessage>()).Returns(1L);

            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            onlineClientManager.GetAllByUserIdAsync(Arg.Any<UserIdentifier>()).Returns(new List<IOnlineClient>());

            var chatCommunicator = Substitute.For<IChatCommunicator>();
            chatCommunicator.SendMessageToClient(Arg.Any<IReadOnlyList<IOnlineClient>>(), Arg.Any<ChatMessage>()).Returns(Task.CompletedTask);

            var userFriendsCache = Substitute.For<IUserFriendsCache>();
            var cacheItem = new UserWithFriendsCacheItem
            {
                Friends = new List<FriendCacheItem>
                {
                    new FriendCacheItem { FriendTenantId = 1, FriendUserId = 10, FriendTenancyName = "old", FriendUserName = "old", FriendProfilePictureId = null }
                }
            };
            userFriendsCache.GetCacheItemOrNull(receiver).Returns(cacheItem);

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var sut = new TestableChatMessageManager(
                friendshipManager,
                chatCommunicator,
                onlineClientManager,
                userManager,
                tenantCache,
                userFriendsCache,
                Substitute.For<IUserEmailer>(),
                chatMessageRepository,
                Substitute.For<IChatFeatureChecker>(),
                unitOfWorkManager);

            await sut.SendMessageAsync(sender, receiver, "Hello", "acme", "sender", senderUser.ProfilePictureId);

            await friendshipManager.DidNotReceive().UpdateFriendshipAsync(Arg.Any<Friendship>());
        }

        [Fact]
        public void Dado_LocalizationManagerComSource_Quando_LComArgs_Entao_DeveFormatar()
        {
            var localizationManager = Substitute.For<ILocalizationManager>();
            var source = Substitute.For<ILocalizationSource>();
            source.GetStringOrNull("Welcome", Arg.Any<CultureInfo>()).Returns("Olá, {0}!");
            localizationManager.GetSource("EafCore").Returns(source);

            var sut = CriarChatMessageManager(null, null);
            sut.LocalizationManager = localizationManager;

            var result = sut.InvokeL("Welcome", "João");
            result.ShouldBe("Olá, João!");
        }

        [Fact]
        public void Dado_LocalizationManagerComSource_Quando_LComCultura_Entao_DeveRetornarTraducao()
        {
            var ptBR = new CultureInfo("pt-BR");
            var localizationManager = Substitute.For<ILocalizationManager>();
            var source = Substitute.For<ILocalizationSource>();
            source.GetStringOrNull("Save", ptBR).Returns("Salvar");
            localizationManager.GetSource("EafCore").Returns(source);

            var sut = CriarChatMessageManager(null, null);
            sut.LocalizationManager = localizationManager;

            var result = sut.InvokeL("Save", ptBR);
            result.ShouldBe("Salvar");
        }

        [Fact]
        public void Dado_LocalizationManagerComSource_Quando_LComCulturaEArgs_Entao_DeveFormatar()
        {
            var ptBR = new CultureInfo("pt-BR");
            var localizationManager = Substitute.For<ILocalizationManager>();
            var source = Substitute.For<ILocalizationSource>();
            source.GetStringOrNull("Count", ptBR).Returns("{0} itens");
            localizationManager.GetSource("EafCore").Returns(source);

            var sut = CriarChatMessageManager(null, null);
            sut.LocalizationManager = localizationManager;

            var result = sut.InvokeL("Count", ptBR, 5);
            result.ShouldBe("5 itens");
        }

        private class TestableChatMessageManager : ChatMessageManager
        {
            public TestableChatMessageManager(
                IFriendshipManager friendshipManager,
                IChatCommunicator chatCommunicator,
                IOnlineClientManager<ChatChannel> onlineClientManager,
                UserManager userManager,
                ITenantCache tenantCache,
                IUserFriendsCache userFriendsCache,
                IUserEmailer userEmailer,
                IRepository<ChatMessage, long> chatMessageRepository,
                IChatFeatureChecker chatFeatureChecker,
                IUnitOfWorkManager unitOfWorkManager)
                : base(friendshipManager, chatCommunicator, onlineClientManager, userManager, tenantCache, userFriendsCache, userEmailer, chatMessageRepository, chatFeatureChecker, unitOfWorkManager)
            {
            }

            public string InvokeL(string name) => base.L(name);
            public string InvokeL(string name, params object[] args) => base.L(name, args);
            public string InvokeL(string name, CultureInfo culture) => base.L(name, culture);
            public string InvokeL(string name, CultureInfo culture, params object[] args) => base.L(name, culture, args);
        }
    }
}

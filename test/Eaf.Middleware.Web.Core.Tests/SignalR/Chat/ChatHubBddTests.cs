using Abp;
using Abp.AspNetCore.SignalR;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.RealTime;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.UI;
using Castle.Core.Logging;
using Castle.Windsor;
using Eaf.AspNetCore.SignalR.Chat;
using Eaf.Middleware.Chat;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using Shouldly;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
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
            typeof(ChatHub).BaseType.ShouldBe(typeof(OnlineClientHubBase));
        }

        [Fact]
        public void Dado_ChatHub_Quando_InvocarRegister_Entao_DeveLogarConexao()
        {
            var chatHub = CriarChatHub();
            var context = Substitute.For<HubCallerContext>();
            context.ConnectionId.Returns("conn-123");
            chatHub.Context = context;

            chatHub.Register();

            context.Received(1).ConnectionId.ShouldNotBeNull();
            context.ConnectionId.ShouldBe("conn-123");
        }

        [Fact]
        public async Task Dado_MensagemNaoEncontrada_Quando_DeleteMessage_Entao_DeveRetornarMensagemDeNaoEncontrado()
        {
            var chatHub = CriarChatHub();
            var context = CriarContextoComUsuario(1, 1);
            chatHub.Context = context;

            var result = await chatHub.DeleteMessage(1);

            result.ShouldContain("Could not find chat message 1");
            result.ShouldContain("1@1");
        }

        [Fact]
        public async Task Dado_UsuarioDestinoValido_Quando_SendMessage_Entao_DeveEnviarMensagemERetornarVazio()
        {
            var chatHub = CriarChatHub();
            var context = CriarContextoComUsuario(1, 1);
            chatHub.Context = context;

            var input = new SendChatMessageInput
            {
                UserId = 2,
                TenantId = 1,
                Message = "Hello",
                UserName = "user2",
                TenancyName = "tenant1"
            };

            var result = await chatHub.SendMessage(input);

            result.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task Dado_GrupoDestinoValido_Quando_SendMessage_Entao_DeveEnviarMensagemParaGrupoERetornarVazio()
        {
            var chatHub = CriarChatHub();
            var context = CriarContextoComUsuario(1, 1);
            chatHub.Context = context;

            var input = new SendChatMessageInput
            {
                GroupId = 5,
                TenantId = 1,
                Message = "Hello group"
            };

            var result = await chatHub.SendMessage(input);

            result.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task Dado_MensagemComSharedMessageId_Quando_DeleteMessage_Entao_DeveDeletarERetornarVazio()
        {
            var (chatHub, chatMessageManager, _) = CriarChatHubCompleto();
            var context = CriarContextoComUsuario(1, 1);
            chatHub.Context = context;

            var message = new ChatMessage(
                new UserIdentifier(1, 1),
                new UserIdentifier(1, 2),
                ChatSide.Sender,
                "mensagem",
                ChatMessageReadState.Unread,
                Guid.NewGuid(),
                ChatMessageReadState.Unread);

            chatMessageManager.FindMessageAsync(10, 1).Returns(Task.FromResult<ChatMessage?>(message));

            var result = await chatHub.DeleteMessage(10);

            result.ShouldBe(string.Empty);
            chatMessageManager.Received(1).Delete(message.SharedMessageId!.Value);
        }

        [Fact]
        public async Task Dado_SendMessageComUserFriendlyException_Quando_Enviar_Entao_DeveRetornarMensagemDeErro()
        {
            var (chatHub, chatMessageManager, _) = CriarChatHubCompleto();
            chatMessageManager.SendMessageAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>(),
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Guid?>())
                .Returns(Task.FromException(new UserFriendlyException("Usuário offline")));

            var context = CriarContextoComUsuario(1, 1);
            chatHub.Context = context;

            var input = new SendChatMessageInput
            {
                UserId = 2,
                TenantId = 1,
                Message = "Hello",
                UserName = "user2",
                TenancyName = "tenant1"
            };

            var result = await chatHub.SendMessage(input);

            result.ShouldBe("Usuário offline");
        }

        [Fact]
        public async Task Dado_SendMessageComExceptionGenerica_Quando_Enviar_Entao_DeveRetornarInternalServerError()
        {
            var (chatHub, chatMessageManager, _) = CriarChatHubCompleto();
            chatMessageManager.SendMessageAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>(),
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Guid?>())
                .Returns(Task.FromException(new Exception("falha")));

            var context = CriarContextoComUsuario(1, 1);
            chatHub.Context = context;

            var input = new SendChatMessageInput
            {
                UserId = 2,
                TenantId = 1,
                Message = "Hello",
                UserName = "user2",
                TenancyName = "tenant1"
            };

            var result = await chatHub.SendMessage(input);

            result.ShouldBe("InternalServerError");
        }

        [Fact]
        public async Task Dado_DestinoInvalido_Quando_SendMessage_Entao_DeveRetornarInternalServerError()
        {
            var chatHub = CriarChatHub();
            var context = CriarContextoComUsuario(1, 1);
            chatHub.Context = context;

            var input = new SendChatMessageInput
            {
                TenantId = 1,
                Message = "No destination"
            };

            var result = await chatHub.SendMessage(input);

            result.ShouldBe("InternalServerError");
        }

        [Fact]
        public async Task Dado_DeleteMessageComAbpException_Quando_Deletar_Entao_DeveRetornarMensagemDeErro()
        {
            var (chatHub, chatMessageManager, _) = CriarChatHubCompleto();
            chatMessageManager.FindMessageAsync(10, 1).Returns(Task.FromException<ChatMessage?>(new AbpException("Erro abp")));

            var context = CriarContextoComUsuario(1, 1);
            chatHub.Context = context;

            var result = await chatHub.DeleteMessage(10);

            result.ShouldBe("Erro abp");
        }

        [Fact]
        public async Task Dado_DeleteMessageComExceptionGenerica_Quando_Deletar_Entao_DeveRetornarInternalServerError()
        {
            var (chatHub, chatMessageManager, _) = CriarChatHubCompleto();
            chatMessageManager.FindMessageAsync(10, 1).Returns(Task.FromException<ChatMessage?>(new Exception("falha")));

            var context = CriarContextoComUsuario(1, 1);
            chatHub.Context = context;

            var result = await chatHub.DeleteMessage(10);

            result.ShouldBe("InternalServerError");
        }

        [Fact]
        public async Task Dado_SendMessageParaGrupoComUserFriendlyException_Quando_Enviar_Entao_DeveRetornarMensagemDeErro()
        {
            var (chatHub, chatMessageManager, _) = CriarChatHubCompleto();
            chatMessageManager.SendMessageToGroupAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>(), Arg.Any<string>())
                .Returns(Task.FromException(new UserFriendlyException("Grupo offline")));

            var context = CriarContextoComUsuario(1, 1);
            chatHub.Context = context;

            var input = new SendChatMessageInput
            {
                GroupId = 5,
                TenantId = 1,
                Message = "Hello group"
            };

            var result = await chatHub.SendMessage(input);

            result.ShouldBe("Grupo offline");
        }

        [Fact]
        public async Task Dado_SendMessageParaGrupoComExceptionGenerica_Quando_Enviar_Entao_DeveRetornarInternalServerError()
        {
            var (chatHub, chatMessageManager, _) = CriarChatHubCompleto();
            chatMessageManager.SendMessageToGroupAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>(), Arg.Any<string>())
                .Returns(Task.FromException(new Exception("falha")));

            var context = CriarContextoComUsuario(1, 1);
            chatHub.Context = context;

            var input = new SendChatMessageInput
            {
                GroupId = 5,
                TenantId = 1,
                Message = "Hello group"
            };

            var result = await chatHub.SendMessage(input);

            result.ShouldBe("InternalServerError");
        }

        [Fact]
        public async Task Dado_MensagemEncontradaSemSharedMessageId_Quando_DeleteMessage_Entao_DeveRetornarMensagemDeNaoEncontrado()
        {
            var (chatHub, chatMessageManager, _) = CriarChatHubCompleto();
            var context = CriarContextoComUsuario(1, 1);
            chatHub.Context = context;

            var message = new ChatMessage(
                new UserIdentifier(1, 1),
                new UserIdentifier(1, 2),
                ChatSide.Sender,
                "mensagem",
                ChatMessageReadState.Unread,
                Guid.Empty,
                ChatMessageReadState.Unread);
            message.SharedMessageId = null;

            chatMessageManager.FindMessageAsync(10, 1).Returns(Task.FromResult<ChatMessage?>(message));

            var result = await chatHub.DeleteMessage(10);

            result.ShouldContain("Could not find chat message 10");
        }

        [Fact]
        public async Task Dado_UserIdZero_Quando_SendMessage_Entao_DeveRetornarInternalServerError()
        {
            var chatHub = CriarChatHub();
            var context = CriarContextoComUsuario(1, 1);
            chatHub.Context = context;

            var input = new SendChatMessageInput
            {
                UserId = 0,
                TenantId = 1,
                Message = "Hello"
            };

            var result = await chatHub.SendMessage(input);

            result.ShouldBe("InternalServerError");
        }

        [Fact]
        public async Task Dado_GroupIdZero_Quando_SendMessage_Entao_DeveRetornarInternalServerError()
        {
            var chatHub = CriarChatHub();
            var context = CriarContextoComUsuario(1, 1);
            chatHub.Context = context;

            var input = new SendChatMessageInput
            {
                GroupId = 0,
                TenantId = 1,
                Message = "Hello group"
            };

            var result = await chatHub.SendMessage(input);

            result.ShouldBe("InternalServerError");
        }

        [Fact]
        public void Dado_ChatHub_Quando_Dispose_Entao_DeveLiberarViaWindsorContainer()
        {
            var (chatHub, _, windsorContainer) = CriarChatHubCompleto();

            chatHub.Dispose();

            windsorContainer.Received(1).Release(chatHub);
        }

        [Fact]
        public void Dado_ChatHub_Quando_DisposeDuasVezes_Entao_DeveLiberarApenasUmaVez()
        {
            var (chatHub, _, windsorContainer) = CriarChatHubCompleto();

            chatHub.Dispose();
            chatHub.Dispose();

            windsorContainer.Received(1).Release(chatHub);
        }

        private static ChatHub CriarChatHub()
        {
            return CriarChatHubCompleto().Hub;
        }

        private static (ChatHub Hub, IChatMessageManager ChatMessageManager, IWindsorContainer WindsorContainer) CriarChatHubCompleto()
        {
            var chatMessageManager = Substitute.For<IChatMessageManager>();
            chatMessageManager.FindMessageAsync(Arg.Any<int>(), Arg.Any<long>())
                .Returns(Task.FromResult<ChatMessage?>(null));
            chatMessageManager.SendMessageAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>(),
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Guid?>())
                .Returns(Task.CompletedTask);
            chatMessageManager.SendMessageToGroupAsync(Arg.Any<UserIdentifier>(), Arg.Any<UserIdentifier>(),
                Arg.Any<string>())
                .Returns(Task.CompletedTask);

            var localizationSource = Substitute.For<ILocalizationSource>();
            localizationSource.GetString("InternalServerError").Returns("InternalServerError");

            var localizationManager = Substitute.For<ILocalizationManager>();
            localizationManager.GetSource("Eaf").Returns(localizationSource);

            var onlineClientManager = Substitute.For<IOnlineClientManager<ChatChannel>>();
            var clientInfoProvider = Substitute.For<IOnlineClientInfoProvider>();
            var windsorContainer = Substitute.For<IWindsorContainer>();

            var hub = new ChatHub(
                chatMessageManager,
                localizationManager,
                windsorContainer,
                onlineClientManager,
                clientInfoProvider);

            return (hub, chatMessageManager, windsorContainer);
        }

        private static HubCallerContext CriarContextoComUsuario(long userId, int tenantId)
        {
            var context = Substitute.For<HubCallerContext>();
            context.ConnectionId.Returns("conn-123");
            context.User.Returns(new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(AbpClaimTypes.UserId, userId.ToString()),
                new Claim(AbpClaimTypes.TenantId, tenantId.ToString())
            })));
            return context;
        }
    }
}

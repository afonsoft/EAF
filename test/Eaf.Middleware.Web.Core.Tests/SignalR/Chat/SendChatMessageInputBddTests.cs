using Eaf.AspNetCore.SignalR.Chat;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.SignalR.Chat
{
    /// <summary>
    /// Testes BDD para SendChatMessageInput seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class SendChatMessageInputBddTests
    {
        #region Propriedades

        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirMessage_Entao_DeveArmazenar()
        {
            var sut = new SendChatMessageInput { Message = "Hello World" };
            sut.Message.ShouldBe("Hello World");
        }

        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirProfilePictureId_Entao_DeveArmazenar()
        {
            var id = Guid.NewGuid();
            var sut = new SendChatMessageInput { ProfilePictureId = id };
            sut.ProfilePictureId.ShouldBe(id);
        }

        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirProfilePictureIdNull_Entao_DeveSerNull()
        {
            var sut = new SendChatMessageInput { ProfilePictureId = null };
            sut.ProfilePictureId.ShouldBeNull();
        }

        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirTenancyName_Entao_DeveArmazenar()
        {
            var sut = new SendChatMessageInput { TenancyName = "TestTenant" };
            sut.TenancyName.ShouldBe("TestTenant");
        }

        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirTenantId_Entao_DeveArmazenar()
        {
            var sut = new SendChatMessageInput { TenantId = 42 };
            sut.TenantId.ShouldBe(42);
        }

        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirTenantIdNull_Entao_DeveSerNull()
        {
            var sut = new SendChatMessageInput { TenantId = null };
            sut.TenantId.ShouldBeNull();
        }

        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirUserId_Entao_DeveArmazenar()
        {
            var sut = new SendChatMessageInput { UserId = 123L };
            sut.UserId.ShouldBe(123L);
        }

        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirUserIdNull_Entao_DeveSerNull()
        {
            var sut = new SendChatMessageInput { UserId = null };
            sut.UserId.ShouldBeNull();
        }

        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirUserName_Entao_DeveArmazenar()
        {
            var sut = new SendChatMessageInput { UserName = "john.doe" };
            sut.UserName.ShouldBe("john.doe");
        }

        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirGroupId_Entao_DeveArmazenar()
        {
            var sut = new SendChatMessageInput { GroupId = 999L };
            sut.GroupId.ShouldBe(999L);
        }

        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirGroupIdNull_Entao_DeveSerNull()
        {
            var sut = new SendChatMessageInput { GroupId = null };
            sut.GroupId.ShouldBeNull();
        }

        #endregion

        #region Instanciacao

        [Fact]
        public void Dado_Padrao_Quando_CriarInstancia_Entao_PropriedadesDevemSerDefault()
        {
            var sut = new SendChatMessageInput();
            sut.Message.ShouldBeNull();
            sut.ProfilePictureId.ShouldBeNull();
            sut.TenancyName.ShouldBeNull();
            sut.TenantId.ShouldBeNull();
            sut.UserId.ShouldBeNull();
            sut.UserName.ShouldBeNull();
            sut.GroupId.ShouldBeNull();
        }

        #endregion
    }
}

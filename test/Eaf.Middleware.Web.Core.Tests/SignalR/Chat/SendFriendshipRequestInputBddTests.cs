using Eaf.AspNetCore.SignalR.Chat;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.SignalR.Chat
{
    /// <summary>
    /// Testes BDD para SendFriendshipRequestInput seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class SendFriendshipRequestInputBddTests
    {
        #region Propriedades

        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirTenantId_Entao_DeveArmazenar()
        {
            var sut = new SendFriendshipRequestInput { TenantId = 10 };
            sut.TenantId.ShouldBe(10);
        }

        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirTenantIdNull_Entao_DeveSerNull()
        {
            var sut = new SendFriendshipRequestInput { TenantId = null };
            sut.TenantId.ShouldBeNull();
        }

        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirUserId_Entao_DeveArmazenar()
        {
            var sut = new SendFriendshipRequestInput { UserId = 42L };
            sut.UserId.ShouldBe(42L);
        }

        #endregion

        #region Instanciacao

        [Fact]
        public void Dado_Padrao_Quando_CriarInstancia_Entao_PropriedadesDevemSerDefault()
        {
            var sut = new SendFriendshipRequestInput();
            sut.TenantId.ShouldBeNull();
            sut.UserId.ShouldBe(0L);
        }

        #endregion
    }
}

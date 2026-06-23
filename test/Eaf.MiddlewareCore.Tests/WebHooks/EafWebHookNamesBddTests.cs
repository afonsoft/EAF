using Eaf.Middleware;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebHooks
{
    /// <summary>
    /// Testes BDD para EafWebHookNames seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class EafWebHookNamesBddTests
    {
        [Fact]
        public void Dado_NewUserRegistered_Quando_Verificar_Entao_DeveTerValorCorreto()
        {
            EafWebHookNames.NewUserRegistered.ShouldBe("WebHook.NewUserRegistered");
        }

        [Fact]
        public void Dado_NewUserRegistered_Quando_Verificar_Entao_DeveSerConstante()
        {
            EafWebHookNames.NewUserRegistered.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void Dado_NewUserRegistered_Quando_Verificar_Entao_DeveConterPrefixoWebHook()
        {
            EafWebHookNames.NewUserRegistered.ShouldStartWith("WebHook.");
        }
    }
}

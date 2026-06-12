using Eaf.WebHooks;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.WebHooks
{
    /// <summary>
    /// Testes BDD para EafWebHookNames seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class EafWebHookNamesBddTests
    {
        [Fact]
        public void Dado_NewUserRegistered_Quando_Verificar_Entao_DeveTerValorCorreto()
        {
            EafWebHookNames.NewUserRegistered.ShouldBe("WebHook.NewUserRegistered");
        }
    }
}

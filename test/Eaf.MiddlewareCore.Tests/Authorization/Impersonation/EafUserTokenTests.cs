using Eaf.Middleware.Authorization.Impersonation;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Authorization.Impersonation
{
    public class EafUserTokenTests
    {
        [Fact]
        public void Dado_ConstrutorPadrao_Quando_Criar_Entao_DeveCriarInstancia()
        {
            var token = new EafUserToken();
            token.ShouldNotBeNull();
        }
    }
}

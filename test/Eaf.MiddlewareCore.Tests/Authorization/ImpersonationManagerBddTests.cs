using Abp.Domain.Services;
using Eaf.Middleware.Authorization.Impersonation;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization
{
    /// <summary>
    /// Testes BDD para ImpersonationManager seguindo o padrão Dado/Quando/Então.
    /// A construção real depende de UserManager (construtor complexo não mockável),
    /// portanto validam-se características de tipo e contrato.
    /// </summary>
    public class ImpersonationManagerBddTests
    {
        [Fact]
        public void Dado_TipoImpersonationManager_Quando_Verificar_Entao_DeveImplementarIImpersonationManager()
        {
            typeof(IImpersonationManager).IsAssignableFrom(typeof(ImpersonationManager)).ShouldBeTrue();
        }

        [Fact]
        public void Dado_TipoImpersonationManager_Quando_Verificar_Entao_DeveHerdarDomainService()
        {
            typeof(DomainService).IsAssignableFrom(typeof(ImpersonationManager)).ShouldBeTrue();
        }
    }
}

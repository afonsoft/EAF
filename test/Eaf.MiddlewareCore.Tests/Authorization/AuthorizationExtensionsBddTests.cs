using Abp;
using Abp.Runtime.Session;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization
{
    /// <summary>
    /// Testes BDD para AuthorizationExtensions seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class AuthorizationExtensionsBddTests
    {
        [Fact]
        public void Dado_ClasseAuthorizationExtensions_Quando_VerificarTipo_Entao_DeveSerEstatica()
        {
            var tipo = typeof(AuthorizationExtensions);
            (tipo.IsAbstract && tipo.IsSealed).ShouldBeTrue();
        }

        [Fact]
        public void Dado_SessaoSemUserId_Quando_GetExternalTokenInformation_Entao_DeveLancarAbpException()
        {
            var session = Substitute.For<IAbpSession>();
            session.UserId.Returns((long?)null);

            Should.Throw<AbpException>(() => session.GetExternalTokenInformation());
        }
    }
}

using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Models.TokenAuth
{
    public class ProviderModelBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ProviderModel();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUsernameOrEmailAddress_Entao_DeveArmazenar()
        {
            var sut = new ProviderModel();
            sut.UsernameOrEmailAddress = "user@example.com";
            sut.UsernameOrEmailAddress.ShouldBe("user@example.com");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirAuthenticationSource_Entao_DeveArmazenar()
        {
            var sut = new ProviderModel();
            sut.AuthenticationSource = "LDAP";
            sut.AuthenticationSource.ShouldBe("LDAP");
        }

        [Fact]
        public void Dado_NovaInstanciaTenantModal_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new TenantModal();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_TenantModal_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var sut = new TenantModal();
            sut.Name = "Tenant1";
            sut.TenancyName = "tenant1";
            sut.Id = 1;
            sut.Name.ShouldBe("Tenant1");
            sut.TenancyName.ShouldBe("tenant1");
            sut.Id.ShouldBe(1);
        }
    }
}

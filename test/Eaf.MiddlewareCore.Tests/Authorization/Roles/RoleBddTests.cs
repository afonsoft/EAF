using Eaf.Middleware.Authorization.Roles;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.Roles
{
    /// <summary>
    /// Testes BDD para Role seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class RoleBddTests
    {
        [Fact]
        public void Dado_ConstrutorPadrao_Quando_Criar_Entao_DeveInicializarSemErro()
        {
            var role = new Role();
            role.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ConstrutorComTenantEDisplayName_Quando_Criar_Entao_DeveDefinirPropriedades()
        {
            var role = new Role(1, "Administrador");
            role.TenantId.ShouldBe(1);
            role.DisplayName.ShouldBe("Administrador");
        }

        [Fact]
        public void Dado_ConstrutorComTenantNameEDisplayName_Quando_Criar_Entao_DeveDefinirPropriedades()
        {
            var role = new Role(1, "Admin", "Administrador");
            role.TenantId.ShouldBe(1);
            role.Name.ShouldBe("Admin");
            role.DisplayName.ShouldBe("Administrador");
        }

        [Fact]
        public void Dado_ConstrutorComTenantNull_Quando_Criar_Entao_TenantDeveSerNull()
        {
            var role = new Role(null, "Host Admin");
            role.TenantId.ShouldBeNull();
            role.DisplayName.ShouldBe("Host Admin");
        }
    }
}

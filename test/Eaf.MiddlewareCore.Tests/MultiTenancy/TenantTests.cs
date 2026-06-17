using Eaf.Middleware.MultiTenancy;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.MultiTenancy
{
    /// <summary>
    /// Testes BDD para Tenant seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class TenantTests
    {
        [Fact]
        public void Dado_NomeETenancyName_Quando_CriarTenant_Entao_DeveInicializarCorretamente()
        {
            // Dado & Quando
            var tenant = new Tenant("acme", "Acme Corp");

            // Então
            tenant.TenancyName.ShouldBe("acme");
            tenant.Name.ShouldBe("Acme Corp");
        }

        [Fact]
        public void Dado_Tenant_Quando_DefinirAddresses_Entao_DeveArmazenar()
        {
            // Dado
            var tenant = new Tenant("test", "Test Tenant");

            // Quando
            tenant.Addresses = new System.Collections.Generic.List<TenantAddress>
            {
                new TenantAddress { City = "São Paulo", State = "SP", Street = "Rua A", Neighborhood = "Centro", ZipCode = "01001" }
            };

            // Então
            tenant.Addresses.ShouldNotBeNull();
            tenant.Addresses.Count.ShouldBe(1);
        }
    }
}

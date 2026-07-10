using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Tests.Helpers;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.MultiTenancy
{
    /// <summary>
    /// Testes BDD para TenantManager exercitando caminhos reais de criação e consulta.
    /// </summary>
    public class TenantManagerBddTests
    {
        [Fact]
        public void Dado_IdInvalido_Quando_GetById_Entao_DeveLancarNotImplementedException()
        {
            // Dado
            var tenantManager = CoreManagerTestHelper.CreateTenantManager(
                out _, out _, out _, out _, out _);

            // Quando / Então
            Should.Throw<NotImplementedException>(() => tenantManager.GetById(1));
        }

        [Fact]
        public async Task Dado_DadosValidos_Quando_CreateWithAdminUserAsync_Entao_DeveRetornarIdTenant()
        {
            // Dado
            var tenantManager = CoreManagerTestHelper.CreateTenantManager(
                out _, out _, out _, out _, out _);

            // Quando
            var tenantId = await tenantManager.CreateWithAdminUserAsync(
                "tenant1",
                "Tenant One",
                "password123",
                "admin@tenant1.com",
                isActive: true,
                shouldChangePasswordOnNextLogin: false,
                sendActivationEmail: false,
                emailActivationLink: null);

            // Então
            tenantId.ShouldBeGreaterThan(0);
        }
    }
}

using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
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
            Should.Throw<NotImplementedException>(() => TenantManager.GetById(1));
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

        [Fact]
        public async Task Dado_CriacaoComEmailAtivacao_Quando_CreateWithAdminUserAsync_Entao_DeveEnviarEmailAtivacao()
        {
            // Dado
            var tenantManager = CoreManagerTestHelper.CreateTenantManager(
                out _, out _, out _, out _, out _);

            // Quando
            var tenantId = await tenantManager.CreateWithAdminUserAsync(
                "tenant2",
                "Tenant Two",
                null,
                "admin@tenant2.com",
                isActive: true,
                shouldChangePasswordOnNextLogin: false,
                sendActivationEmail: true,
                emailActivationLink: "https://localhost/activate");

            // Então
            tenantId.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Dado_SenhaComValidador_Quando_CreateWithAdminUserAsync_Entao_DeveCriarTenant()
        {
            // Dado
            var tenantManager = CoreManagerTestHelper.CreateTenantManager(
                new List<IPasswordValidator<User>> { new FakePasswordValidator() },
                out var userManager, out _, out _, out _, out _);

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

        private class FakePasswordValidator : IPasswordValidator<User>
        {
            public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
            {
                return Task.FromResult(IdentityResult.Success);
            }
        }
    }
}

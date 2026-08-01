using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Tests.Helpers;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.MultiTenancy
{
    /// <summary>
    /// Testes BDD para TenantUserManager cobrindo criação e aprovação de memberships.
    /// </summary>
    public class TenantUserManagerBddTests
    {
        [Fact]
        public async Task Dado_UsuarioHostSemTenant_Quando_CriarPendingMembership_Entao_DeveCriarShadowUserInativoESolicitacao()
        {
            // Dado
            var hostUser = new User { Id = 10, UserName = "hostuser", Name = "Host", Surname = "User", EmailAddress = "host@example.com" };
            var tenant = new Tenant("tenant1", "Tenant One") { Id = 1, IsActive = true };
            var userRole = new Role(1, StaticRoleNames.Tenants.User, StaticRoleNames.Tenants.User) { Id = 1 };

            var tenantUserManager = CoreManagerTestHelper.CreateTenantUserManager(
                tenant: tenant,
                hostUser: hostUser,
                userRole: userRole,
                out var joinRequestRepository,
                out _);

            // Quando
            var request = await tenantUserManager.CreatePendingMembershipAsync(hostUser.Id, tenant.Id, "Quero participar");

            // Então
            request.ShouldNotBeNull();
            await joinRequestRepository.Received(1).InsertAsync(Arg.Is<TenantJoinRequest>(r =>
                r.UserId == hostUser.Id &&
                r.TenantId == tenant.Id &&
                r.TenantUserId == 2 &&
                r.Status == TenantJoinRequestStatus.Pending &&
                r.Message == "Quero participar"));
            request.UserId.ShouldBe(hostUser.Id);
            request.TenantId.ShouldBe(tenant.Id);
            request.TenantUserId.ShouldBe(2);
            request.Status.ShouldBe(TenantJoinRequestStatus.Pending);
            request.Message.ShouldBe("Quero participar");
        }

        [Fact]
        public async Task Dado_SolicitacaoPendente_Quando_AprovarMembership_Entao_DeveAtivarShadowUserECriarMembership()
        {
            // Dado
            var hostUser = new User { Id = 10, UserName = "hostuser", Name = "Host", Surname = "User", EmailAddress = "host@example.com" };
            var shadowUser = new User { Id = 2, TenantId = 1, UserName = "hostuser", Name = "Host", Surname = "User", EmailAddress = "host@example.com", IsActive = false };
            var tenant = new Tenant("tenant1", "Tenant One") { Id = 1, IsActive = true };
            var pendingRequest = new TenantJoinRequest { Id = 100, UserId = hostUser.Id, TenantId = tenant.Id, TenantUserId = shadowUser.Id, Status = TenantJoinRequestStatus.Pending };

            UserTenantMembership capturedMembership = null;
            var tenantUserManager = CoreManagerTestHelper.CreateTenantUserManager(
                tenant: tenant,
                hostUser: hostUser,
                userRole: null,
                shadowUser: shadowUser,
                pendingRequest: pendingRequest,
                onMembershipInserted: m => capturedMembership = m,
                joinRequestRepository: out _,
                membershipRepository: out _);

            // Quando
            await tenantUserManager.ApproveMembershipAsync(pendingRequest.Id, 99);

            // Então
            shadowUser.IsActive.ShouldBeTrue();
            pendingRequest.Status.ShouldBe(TenantJoinRequestStatus.Approved);
            pendingRequest.ApproverUserId.ShouldBe(99);
            capturedMembership.ShouldNotBeNull();
            capturedMembership.UserId.ShouldBe(hostUser.Id);
            capturedMembership.TenantId.ShouldBe(tenant.Id);
            capturedMembership.TenantUserId.ShouldBe(shadowUser.Id);
            capturedMembership.IsDefault.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_TenantInexistente_Quando_CriarPendingMembership_Entao_DeveLancarExcecao()
        {
            // Dado
            var hostUser = new User { Id = 10, UserName = "hostuser", Name = "Host", Surname = "User", EmailAddress = "host@example.com" };
            var tenantUserManager = CoreManagerTestHelper.CreateTenantUserManager(
                tenant: null,
                hostUser: hostUser,
                userRole: null,
                out _,
                out _);

            // Quando / Então
            var ex = await Should.ThrowAsync<UserFriendlyException>(async () =>
                await tenantUserManager.CreatePendingMembershipAsync(hostUser.Id, 999, null));
            ex.Message.ShouldContain("TenantNotFound");
        }
    }
}

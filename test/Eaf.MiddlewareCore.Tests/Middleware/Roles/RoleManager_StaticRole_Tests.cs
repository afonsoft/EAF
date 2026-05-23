using Abp.Configuration.Startup;
using Abp.Domain.Uow;
using Eaf.Middleware.Configuration;
using Eaf.MiddlewareCore.SampleApp.Core;
using Abp.MultiTenancy;
using Shouldly;
using System.Threading.Tasks;
using Xunit;
using Abp.Zero.Configuration;

namespace Eaf.Middleware.Roles
{
    public class RoleManager_StaticRole_Tests : EafMiddlewareTestBase
    {
        private readonly IRoleManagementConfig _roleManagementConfig;
        private readonly RoleManager _roleManager;
        private readonly TenantManager _tenantManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public RoleManager_StaticRole_Tests()
        {
            _tenantManager = Resolve<TenantManager>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            _roleManagementConfig = Resolve<IRoleManagementConfig>();
            _roleManager = Resolve<RoleManager>();
        }

        [Fact]
        public async Task Static_Roles_GrantAllPermissionsByDefault_False_Test()
        {
            LocalIocManager.Resolve<IMultiTenancyConfig>().IsEnabled = true;

            await CreateTestStaticRoles();

            var tenant = await GetTenantAsync("Tenant1");
            AbpSession.TenantId = tenant.Id;

            var adminRole = await _roleManager.GetRoleByNameAsync("admin");

            //Default granted permissions
            (await _roleManager.IsGrantedAsync(adminRole.Id, "Permission1")).ShouldBe(false);
            (await _roleManager.IsGrantedAsync(adminRole.Id, "Permission2")).ShouldBe(false);
        }

        private async Task CreateTestStaticRoles()
        {
            _roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    "admin",
                    MultiTenancySides.Host,
                    grantAllPermissionsByDefault: true)
            );

            _roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    "admin",
                    MultiTenancySides.Tenant)
            );

            using (var uow = _unitOfWorkManager.Begin())
            {
                var tenant = new Tenant("Tenant1", "Tenant1");
                await _tenantManager.CreateAsync(tenant);
                await _unitOfWorkManager.Current.SaveChangesAsync();

                using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                {
                    AbpSession.TenantId = tenant.Id;

                    await _roleManager.CreateStaticRoles(tenant.Id);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }

                await uow.CompleteAsync();
            }
        }
    }
}
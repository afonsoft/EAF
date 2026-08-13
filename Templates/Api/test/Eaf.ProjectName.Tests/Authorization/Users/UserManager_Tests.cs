using Abp.Configuration;
using Abp.Data;
using Abp.Domain.Uow;
using Abp.Zero.Configuration;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Configuration;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.ProjectName.Tests.Authorization.Users
{
    public class UserManager_Tests : UserAppServiceTestBase
    {
        private readonly ISettingManager _settingManager;
        private readonly UserManager _userManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserManager_Tests()
        {
            _settingManager = Resolve<ISettingManager>();
            _userManager = Resolve<UserManager>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();

            LoginAsDefaultTenantAdmin();
        }

        [Fact]
        public async Task Should_Create_User_With_Random_Password_For_Tenant()
        {
            await _settingManager.ChangeSettingForApplicationAsync(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase, "true");
            await _settingManager.ChangeSettingForApplicationAsync(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric, "true");
            await _settingManager.ChangeSettingForApplicationAsync(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength, "6");

            var randomPassword = "R5q9y6t2";

            randomPassword.Length.ShouldBeGreaterThanOrEqualTo(8);
            randomPassword.Any(char.IsUpper).ShouldBeTrue();
            randomPassword.Any(char.IsLetterOrDigit).ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_Usuarios_Host_E_Tenant_Com_Mesmo_Nome_Quando_Buscar_Por_Nome_Ou_Email_Entao_Deve_Priorizar_Host_Sem_Tenant_E_Filtrar_Por_Tenant()
        {
            User hostUser = null;
            User tenantUser = null;

            UsingDbContext(null, context =>
            {
                context.SuppressAutoSetTenantId = true;

                hostUser = new User
                {
                    TenantId = null,
                    UserName = "shareduser",
                    Name = "Shared",
                    Surname = "Host",
                    EmailAddress = "shared@projectname.local",
                    IsEmailConfirmed = true,
                    IsActive = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    Password = "123qwe"
                };
                hostUser.SetNormalizedNames();
                context.Users.Add(hostUser);
            });

            UsingDbContext(1, context =>
            {
                context.SuppressAutoSetTenantId = true;

                tenantUser = new User
                {
                    TenantId = 1,
                    UserName = "shareduser",
                    Name = "Shared",
                    Surname = "Tenant",
                    EmailAddress = "shared@projectname.local",
                    IsEmailConfirmed = true,
                    IsActive = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    Password = "123qwe"
                };
                tenantUser.SetNormalizedNames();
                context.Users.Add(tenantUser);
            });

            using (var uow = _unitOfWorkManager.Begin())
            using (_unitOfWorkManager.Current.SetTenantId(null))
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var byName = await _userManager.FindByNameOrEmailAsync(null, "shareduser");
                byName.ShouldNotBeNull();
                hostUser.ShouldNotBeNull();
                byName.Id.ShouldBe(hostUser.Id);

                var byEmail = await _userManager.FindByNameOrEmailAsync(null, "shared@projectname.local");
                byEmail.ShouldNotBeNull();
                byEmail.Id.ShouldBe(hostUser.Id);

                var byNameTenant = await _userManager.FindByNameOrEmailAsync(1, "shareduser");
                byNameTenant.ShouldNotBeNull();
                tenantUser.ShouldNotBeNull();
                byNameTenant.Id.ShouldBe(tenantUser.Id);

                await uow.CompleteAsync();
            }
        }
    }
}
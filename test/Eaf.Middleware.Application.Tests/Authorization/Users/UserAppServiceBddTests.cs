using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Notifications;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Webhooks;
using Abp.Zero.Configuration;
using Eaf.Middleware.Authorization.AzureActiveDirectory;
using Eaf.Middleware.Authorization.Ldap;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Eaf.Middleware.Ldap.Configuration;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Authorization.Users.Exporting;
using Eaf.Middleware.Url;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users
{
    /// <summary>
    /// Testes BDD para UserAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class UserAppServiceBddTests
    {
        private readonly UserAppService _sut;

        public UserAppServiceBddTests()
        {
            var roleStore = Substitute.For<RoleStore>(
                Substitute.For<IUnitOfWorkManager>(),
                Substitute.For<IRepository<Role>>(),
                Substitute.For<IRepository<RolePermissionSetting, long>>()
            );

            var roleManager = Substitute.For<RoleManager>(
                roleStore,
                new List<IRoleValidator<Role>>(),
                Substitute.For<ILookupNormalizer>(),
                new IdentityErrorDescriber(),
                Substitute.For<ILogger<RoleManager>>(),
                Substitute.For<IPermissionManager>(),
                Substitute.For<IRoleManagementConfig>(),
                Substitute.For<ICacheManager>(),
                Substitute.For<IUnitOfWorkManager>(),
                Substitute.For<ILocalizationManager>(),
                Substitute.For<IRepository<OrganizationUnit, long>>(),
                Substitute.For<IRepository<OrganizationUnitRole, long>>()
            );

            var cacheManager = Substitute.For<ICacheManager>();
            cacheManager.GetCache(Arg.Any<string>()).Returns(Substitute.For<ICache>());

            _sut = new UserAppService(
                roleManager,
                Substitute.For<IUserEmailer>(),
                Substitute.For<IUserListExcelExporter>(),
                Substitute.For<INotificationSubscriptionManager>(),
                Substitute.For<IRepository<Abp.Authorization.Users.UserRole, long>>(),
                new List<IPasswordValidator<User>>(),
                Substitute.For<IPasswordHasher<User>>(),
                Substitute.For<AppAzureActiveDirectoryAuthenticationSource>(
                    Substitute.For<IAzureActiveDirectorySettings>(),
                    Substitute.For<IEafMiddlewareAzureActiveDirectoryModuleConfig>()
                ),
                Substitute.For<AppLdapAuthenticationSource>(
                    Substitute.For<ILdapSettings>(),
                    Substitute.For<IEafMiddlewareLdapModuleConfig>()
                ),
                Substitute.For<INotificationPublisher>(),
                Substitute.For<IWebhookPublisher>(),
                cacheManager
            );
        }

        #region Construtor

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
            _sut.AppUrlService.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_AppUrlServiceDeveSerNullInstance()
        {
            _sut.AppUrlService.ShouldBe(NullAppUrlService.Instance);
        }

        #endregion

        #region DeleteUser

        [Fact]
        public async Task Dado_UsuarioTentandoDeletarProprioId_Quando_DeleteUser_Entao_DeveLancarExcecao()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(() =>
                _sut.DeleteUser(new Abp.Application.Services.Dto.EntityDto<long>(42)));
        }

        #endregion

        #region Injecao de Propriedade

        [Fact]
        public void Dado_AppUrlServiceCustom_Quando_Atribuir_Entao_DeveSubstituirPadrao()
        {
            var customService = Substitute.For<IAppUrlService>();
            _sut.AppUrlService = customService;
            _sut.AppUrlService.ShouldBe(customService);
        }

        #endregion
    }
}

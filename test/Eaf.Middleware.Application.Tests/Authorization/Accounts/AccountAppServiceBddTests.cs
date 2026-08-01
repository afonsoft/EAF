using Abp;
using Abp.Application.Editions;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.UI;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Configuration;
using static Eaf.Middleware.Application.Tests.Helpers.ManagerTestHelper;
using Eaf.Middleware.Authorization.Accounts;
using Eaf.Middleware.Authorization.Accounts.Dto;
using Eaf.Middleware.Authorization.Impersonation;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.MultiTenancy.Dto;
using Eaf.Middleware.Url;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts
{
    /// <summary>
    /// Testes BDD para AccountAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class AccountAppServiceBddTests
    {
        private readonly IUserEmailer _userEmailer;
        private readonly IWebUrlService _webUrlService;
        private readonly IImpersonationManager _impersonationManager;
        private readonly RoleManager _roleManager;
        private readonly AccountAppService _sut;

        public AccountAppServiceBddTests()
        {
            _userEmailer = Substitute.For<IUserEmailer>();
            _webUrlService = Substitute.For<IWebUrlService>();
            _impersonationManager = Substitute.For<IImpersonationManager>();
            _roleManager = ManagerTestHelper.CreateRoleManager();
            var editionManager = ManagerTestHelper.CreateEditionManager();
            var membershipRepository = Substitute.For<IRepository<UserTenantMembership, long>>();
            var tenantUserManager = Substitute.For<ITenantUserManager>();

            _sut = new AccountAppService(_userEmailer, _webUrlService, _impersonationManager, _roleManager, editionManager, membershipRepository, tenantUserManager);
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

        #region ResolveTenantId

        [Fact]
        public async Task Dado_ParametroCVazio_Quando_ResolveTenantId_Entao_DeveRetornarTenantIdDaSessao()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(5);
            _sut.AbpSession = abpSession;

            // Quando
            var result = await _sut.ResolveTenantId(new ResolveTenantIdInput { c = "" });

            // Então
            result.ShouldBe(5);
        }

        [Fact]
        public async Task Dado_ParametroCNulo_Quando_ResolveTenantId_Entao_DeveRetornarTenantIdDaSessao()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(3);
            _sut.AbpSession = abpSession;

            // Quando
            var result = await _sut.ResolveTenantId(new ResolveTenantIdInput { c = null });

            // Então
            result.ShouldBe(3);
        }

        [Fact]
        public async Task Dado_ParametroCComTenantId_Quando_ResolveTenantId_Entao_DeveRetornarTenantIdDecriptado()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(3);
            _sut.AbpSession = abpSession;

            var encrypted = SimpleStringCipher.Instance.Encrypt("tenantId=5");

            // Quando
            var result = await _sut.ResolveTenantId(new ResolveTenantIdInput { c = encrypted });

            // Então
            result.ShouldBe(5);
        }

        [Fact]
        public async Task Dado_ParametroCSemTenantId_Quando_ResolveTenantId_Entao_DeveRetornarNulo()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(3);
            _sut.AbpSession = abpSession;

            var encrypted = SimpleStringCipher.Instance.Encrypt("other=value");

            // Quando
            var result = await _sut.ResolveTenantId(new ResolveTenantIdInput { c = encrypted });

            // Então
            result.ShouldBeNull();
        }

        #endregion

        #region BackToImpersonator

        [Fact]
        public async Task Dado_UsuarioImpersonado_Quando_BackToImpersonator_Entao_DeveRetornarToken()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.ImpersonatorTenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            _impersonationManager.GetBackToImpersonatorToken().Returns("token-abc-123");

            // Quando
            var result = await _sut.BackToImpersonator();

            // Então
            result.ShouldNotBeNull();
            result.ImpersonationToken.ShouldBe("token-abc-123");
        }

        #endregion

        #region GetAllTenants

        [Fact]
        public async Task Dado_TenantsAtivos_Quando_GetAllTenants_Entao_DeveRetornarListaMapeada()
        {
            // Dado
            var tenantManager = ManagerTestHelper.CreateTenantManager();
            var tenant = new Tenant("tenant1", "Tenant One") { Id = 1, IsActive = true };
            tenantManager.Tenants.Returns(new List<Tenant> { tenant }.AsAsyncQueryable());

            _sut.TenantManager = tenantManager;
            _sut.ObjectMapper = CreateObjectMapper();

            // Quando
            var result = await _sut.GetAllTenants();

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
        }

        #endregion

        #region IsTenantAvailable

        [Fact]
        public async Task Dado_TenantAtivo_Quando_IsTenantAvailable_Entao_DeveRetornarDisponivel()
        {
            // Dado
            var tenantManager = ManagerTestHelper.CreateTenantManager();
            var tenant = new Tenant("tenant1", "Tenant One") { Id = 1, IsActive = true };
            tenantManager.FindByTenancyNameAsync("tenant1").Returns(tenant);

            _webUrlService.GetServerRootAddress("tenant1").Returns("https://tenant1.example.com");
            _sut.TenantManager = tenantManager;

            // Quando
            var result = await _sut.IsTenantAvailable(new IsTenantAvailableInput { TenancyName = "tenant1" });

            // Então
            result.ShouldNotBeNull();
            result.State.ShouldBe(TenantAvailabilityState.Available);
            result.TenantId.ShouldBe(1);
            result.ServerRootAddress.ShouldBe("https://tenant1.example.com");
        }

        [Fact]
        public async Task Dado_TenantInativo_Quando_IsTenantAvailable_Entao_DeveRetornarInativo()
        {
            // Dado
            var tenantManager = ManagerTestHelper.CreateTenantManager();
            var tenant = new Tenant("tenant1", "Tenant One") { Id = 1, IsActive = false };
            tenantManager.FindByTenancyNameAsync("tenant1").Returns(tenant);

            _sut.TenantManager = tenantManager;

            // Quando
            var result = await _sut.IsTenantAvailable(new IsTenantAvailableInput { TenancyName = "tenant1" });

            // Então
            result.State.ShouldBe(TenantAvailabilityState.InActive);
        }

        [Fact]
        public async Task Dado_TenantInexistente_Quando_IsTenantAvailable_Entao_DeveRetornarNaoEncontrado()
        {
            // Dado
            var tenantManager = ManagerTestHelper.CreateTenantManager();
            tenantManager.FindByTenancyNameAsync("tenant1").Returns((Tenant)null!);

            _sut.TenantManager = tenantManager;

            // Quando
            var result = await _sut.IsTenantAvailable(new IsTenantAvailableInput { TenancyName = "tenant1" });

            // Então
            result.State.ShouldBe(TenantAvailabilityState.NotFound);
        }

        #endregion

        #region Impersonate

        [Fact]
        public async Task Dado_UsuarioETenant_Quando_Impersonate_Entao_DeveRetornarTokenETenancyName()
        {
            // Dado
            var tenantManager = ManagerTestHelper.CreateTenantManager();
            var tenant = new Tenant("tenant1", "Tenant One") { Id = 1, IsActive = true };
            tenantManager.GetByIdAsync(1).Returns(tenant);
            tenantManager.FindByIdAsync(1).Returns(tenant);

            _impersonationManager.GetImpersonationToken(10, 1).Returns("token-xyz");
            _sut.TenantManager = tenantManager;

            // Quando
            var result = await _sut.Impersonate(new ImpersonateInput { UserId = 10, TenantId = 1 });

            // Então
            result.ShouldNotBeNull();
            result.ImpersonationToken.ShouldBe("token-xyz");
            result.TenancyName.ShouldBe("tenant1");
        }

        [Fact]
        public async Task Dado_TenantInexistente_Quando_Impersonate_Entao_DeveLancarExcecao()
        {
            // Dado
            var tenantManager = ManagerTestHelper.CreateTenantManager();
            tenantManager.FindByIdAsync(1).Returns((Tenant)null!);

            _sut.TenantManager = tenantManager;
            _sut.LocalizationManager = Abp.Localization.NullLocalizationManager.Instance;

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(async () =>
                await _sut.Impersonate(new ImpersonateInput { UserId = 10, TenantId = 1 }));
        }

        [Fact]
        public async Task Dado_TenantInativo_Quando_Impersonate_Entao_DeveLancarExcecao()
        {
            // Dado
            var tenantManager = ManagerTestHelper.CreateTenantManager();
            var tenant = new Tenant("tenant1", "Tenant One") { Id = 1, IsActive = false };
            tenantManager.FindByIdAsync(1).Returns(tenant);

            _sut.TenantManager = tenantManager;
            _sut.LocalizationManager = Abp.Localization.NullLocalizationManager.Instance;

            // Quando / Então
            var ex = await Should.ThrowAsync<UserFriendlyException>(async () =>
                await _sut.Impersonate(new ImpersonateInput { UserId = 10, TenantId = 1 }));
            ex.Message.ShouldContain("TenantIdIsNotActive");
        }

        #endregion

        #region ActivateEmail

        [Fact]
        public async Task Dado_CodigoValido_Quando_ActivateEmail_Entao_DeveConfirmarEmail()
        {
            // Dado
            var user = new User { Id = 1, EmailAddress = "test@example.com", EmailConfirmationCode = "123" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns(user);
            userManager.UpdateAsync(user).Returns(IdentityResult.Success);

            _sut.UserManager = userManager;

            // Quando
            await _sut.ActivateEmail(new ActivateEmailInput { UserId = 1, ConfirmationCode = "123" });

            // Então
            user.IsEmailConfirmed.ShouldBeTrue();
            user.EmailConfirmationCode.ShouldBeNull();
            await userManager.Received(1).UpdateAsync(user);
        }

        [Fact]
        public async Task Dado_CodigoInvalido_Quando_ActivateEmail_Entao_DeveLancarExcecao()
        {
            // Dado
            var user = new User { Id = 1, EmailAddress = "test@example.com", EmailConfirmationCode = "123" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns(user);

            _sut.UserManager = userManager;
            _sut.LocalizationManager = Substitute.For<Abp.Localization.ILocalizationManager>();

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(async () =>
                await _sut.ActivateEmail(new ActivateEmailInput { UserId = 1, ConfirmationCode = "999" }));
        }

        [Fact]
        public async Task Dado_UsuarioNaoEncontrado_Quando_ActivateEmail_Entao_DeveLancarExcecao()
        {
            // Dado
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns((User?)null);

            _sut.UserManager = userManager;
            _sut.LocalizationManager = Substitute.For<Abp.Localization.ILocalizationManager>();

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(async () =>
                await _sut.ActivateEmail(new ActivateEmailInput { UserId = 1, ConfirmationCode = "123" }));
        }

        #endregion

        #region ResetPassword

        [Fact]
        public async Task Dado_CodigoValido_Quando_ResetPassword_Entao_DeveRedefinirSenha()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin", PasswordResetCode = "456", IsActive = true };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns(user);
            userManager.InitializeOptionsAsync(Arg.Any<int?>()).Returns(Task.CompletedTask);
            userManager.ChangePasswordAsync(user, Arg.Any<string>()).Returns(IdentityResult.Success);
            userManager.UpdateAsync(user).Returns(IdentityResult.Success);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;

            // Quando
            var result = await _sut.ResetPassword(new ResetPasswordInput { UserId = 1, ResetCode = "456", Password = "NewPass123!" });

            // Então
            result.ShouldNotBeNull();
            result.CanLogin.ShouldBeTrue();
            result.UserName.ShouldBe("admin");
            user.PasswordResetCode.ShouldBeNull();
            user.IsEmailConfirmed.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_CodigoInvalido_Quando_ResetPassword_Entao_DeveLancarExcecao()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin", PasswordResetCode = "456", IsActive = true };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns(user);

            _sut.UserManager = userManager;
            _sut.LocalizationManager = Substitute.For<Abp.Localization.ILocalizationManager>();

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(async () =>
                await _sut.ResetPassword(new ResetPasswordInput { UserId = 1, ResetCode = "999", Password = "NewPass123!" }));
        }

        [Fact]
        public async Task Dado_UsuarioNaoEncontrado_Quando_ResetPassword_Entao_DeveLancarExcecao()
        {
            // Dado
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns((User?)null);

            _sut.UserManager = userManager;
            _sut.LocalizationManager = Substitute.For<Abp.Localization.ILocalizationManager>();

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(async () =>
                await _sut.ResetPassword(new ResetPasswordInput { UserId = 1, ResetCode = "456", Password = "NewPass123!" }));
        }

        [Fact]
        public async Task Dado_UsuarioInativo_Quando_ResetPassword_Entao_DeveRetornarCanLoginFalse()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin", PasswordResetCode = "456", IsActive = false };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns(user);
            userManager.InitializeOptionsAsync(Arg.Any<int?>()).Returns(Task.CompletedTask);
            userManager.ChangePasswordAsync(user, Arg.Any<string>()).Returns(IdentityResult.Success);
            userManager.UpdateAsync(user).Returns(IdentityResult.Success);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;

            // Quando
            var result = await _sut.ResetPassword(new ResetPasswordInput { UserId = 1, ResetCode = "456", Password = "NewPass123!" });

            // Então
            result.ShouldNotBeNull();
            result.CanLogin.ShouldBeFalse();
        }

        #endregion

        #region SendEmailActivationLink

        [Fact]
        public async Task Dado_EmailValido_Quando_SendEmailActivationLink_Entao_DeveChamarUserEmailer()
        {
            // Dado
            var user = new User { Id = 1, EmailAddress = "test@example.com" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByEmailAsync("test@example.com").Returns(user);

            var appUrlService = Substitute.For<IAppUrlService>();
            appUrlService.CreateEmailActivationUrlFormat(Arg.Any<int?>()).Returns("https://example.com/activate");

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);

            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;
            _sut.AppUrlService = appUrlService;

            // Quando
            await _sut.SendEmailActivationLink(new SendEmailActivationLinkInput { EmailAddress = "test@example.com" });

            // Então
            await _userEmailer.Received(1).SendEmailActivationLinkAsync(user, "https://example.com/activate");
        }

        [Fact]
        public async Task Dado_EmailInvalido_Quando_SendEmailActivationLink_Entao_DeveLancarExcecao()
        {
            // Dado
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByEmailAsync("missing@example.com").Returns((User?)null);

            _sut.UserManager = userManager;
            _sut.LocalizationManager = Substitute.For<Abp.Localization.ILocalizationManager>();

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(async () =>
                await _sut.SendEmailActivationLink(new SendEmailActivationLinkInput { EmailAddress = "missing@example.com" }));
        }

        [Fact]
        public async Task Dado_UsuarioComTenant_Quando_SendEmailActivationLink_Entao_DeveChamarUserEmailer()
        {
            // Dado
            var user = new User { Id = 1, EmailAddress = "test@example.com", TenantId = 1 };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByEmailAsync("test@example.com").Returns(user);

            var appUrlService = Substitute.For<IAppUrlService>();
            appUrlService.CreateEmailActivationUrlFormat(Arg.Any<int?>()).Returns("https://example.com/activate?userId={userId}&tenantId={tenantId}&confirmationCode={confirmationCode}");

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);

            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;
            _sut.AppUrlService = appUrlService;

            // Quando
            await _sut.SendEmailActivationLink(new SendEmailActivationLinkInput { EmailAddress = "test@example.com" });

            // Então
            await _userEmailer.Received(1).SendEmailActivationLinkAsync(user, Arg.Is<string>(s => s.Contains("tenantId=")));
        }

        #endregion

        #region SendPasswordResetCode

        [Fact]
        public async Task Dado_EmailValido_Quando_SendPasswordResetCode_Entao_DeveChamarUserEmailer()
        {
            // Dado
            var user = new User { Id = 1, EmailAddress = "test@example.com" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByEmailAsync("test@example.com").Returns(user);

            var appUrlService = Substitute.For<IAppUrlService>();
            appUrlService.CreatePasswordResetUrlFormat(Arg.Any<int?>()).Returns("https://example.com/reset");

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);

            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;
            _sut.AppUrlService = appUrlService;

            // Quando
            await _sut.SendPasswordResetCode(new SendPasswordResetCodeInput { EmailAddress = "test@example.com" });

            // Então
            await _userEmailer.Received(1).SendPasswordResetLinkAsync(user, "https://example.com/reset");
        }

        [Fact]
        public async Task Dado_EmailInvalido_Quando_SendPasswordResetCode_Entao_DeveLancarExcecao()
        {
            // Dado
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByEmailAsync("missing@example.com").Returns((User?)null);

            _sut.UserManager = userManager;
            _sut.LocalizationManager = Substitute.For<Abp.Localization.ILocalizationManager>();

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(async () =>
                await _sut.SendPasswordResetCode(new SendPasswordResetCodeInput { EmailAddress = "missing@example.com" }));
        }

        [Fact]
        public async Task Dado_UsuarioComAuthenticationSource_Quando_SendPasswordResetCode_Entao_DeveChamarUserEmailer()
        {
            // Dado
            var user = new User { Id = 1, EmailAddress = "test@example.com", TenantId = 1, AuthenticationSource = "Google" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByEmailAsync("test@example.com").Returns(user);

            var appUrlService = Substitute.For<IAppUrlService>();
            appUrlService.CreatePasswordResetUrlFormat(Arg.Any<int?>()).Returns("https://example.com/reset?userId={userId}&resetCode={resetCode}&tenantId={tenantId}&authenticationSource={authenticationSource}");

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);

            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;
            _sut.AppUrlService = appUrlService;

            // Quando
            await _sut.SendPasswordResetCode(new SendPasswordResetCodeInput { EmailAddress = "test@example.com" });

            // Então
            await _userEmailer.Received(1).SendPasswordResetLinkAsync(user, Arg.Is<string>(s => s.Contains("authenticationSource")));
        }

        #endregion

        #region Register

        [Fact]
        public async Task Dado_SelfRegistrationDesabilitado_Quando_Register_Entao_DeveLancarExcecao()
        {
            // Dado
            ConfigurarSetting(AppSettings.TenantManagement.AllowSelfRegistration, false);

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(async () =>
                await _sut.Register(new RegisterInput
                {
                    TenantSelectionMode = TenantSelectionMode.DefaultTenant,
                    Name = "Joao",
                    Surname = "Silva",
                    UserName = "joaosilva",
                    EmailAddress = "joao@example.com",
                    Password = "P@ssw0rd!"
                }));
        }

        [Fact]
        public async Task Dado_ModoDefaultTenant_Quando_Register_Entao_DeveRetornarCanLoginTrue()
        {
            // Dado
            ConfigurarRegistroBasico();

            // Quando
            var result = await _sut.Register(new RegisterInput
            {
                TenantSelectionMode = TenantSelectionMode.DefaultTenant,
                Name = "Joao",
                Surname = "Silva",
                UserName = "joaosilva",
                EmailAddress = "joao@example.com",
                Password = "P@ssw0rd!"
            });

            // Então
            result.ShouldNotBeNull();
            result.CanLogin.ShouldBeTrue();
            result.TenantId.ShouldBeNull();
        }

        [Fact]
        public async Task Dado_ModoCreateNew_Quando_TenantCreationDesabilitado_Entao_DeveLancarExcecao()
        {
            // Dado
            ConfigurarRegistroBasico();
            ConfigurarSetting(AppSettings.TenantManagement.AllowTenantCreation, false);

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(async () =>
                await _sut.Register(new RegisterInput
                {
                    TenantSelectionMode = TenantSelectionMode.CreateNew,
                    TenancyName = "empresa",
                    Name = "Joao",
                    Surname = "Silva",
                    UserName = "joaosilva",
                    EmailAddress = "joao@example.com",
                    Password = "P@ssw0rd!"
                }));
        }

        [Fact]
        public async Task Dado_ModoJoinExisting_Quando_JoinRequestsDesabilitado_Entao_DeveLancarExcecao()
        {
            // Dado
            ConfigurarRegistroBasico();
            ConfigurarSetting(AppSettings.TenantManagement.AllowJoinRequests, false);

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(async () =>
                await _sut.Register(new RegisterInput
                {
                    TenantSelectionMode = TenantSelectionMode.JoinExisting,
                    ExistingTenantId = 1,
                    Name = "Joao",
                    Surname = "Silva",
                    UserName = "joaosilva",
                    EmailAddress = "joao@example.com",
                    Password = "P@ssw0rd!"
                }));
        }

        [Fact]
        public async Task Dado_ModoJoinExisting_Quando_Register_Entao_DeveCriarSolicitacaoPendente()
        {
            // Dado
            ConfigurarRegistroBasico();
            ConfigurarSetting(AppSettings.TenantManagement.AllowJoinRequests, true);

            var tenant = new Tenant("empresa", "Empresa") { Id = 1, IsActive = true };
            var tenantManager = ManagerTestHelper.CreateTenantManager();
            tenantManager.FindByIdAsync(1).Returns(tenant);
            _sut.TenantManager = tenantManager;

            var request = new TenantJoinRequest { Id = 100, UserId = 2, TenantId = 1, Status = TenantJoinRequestStatus.Pending };
            var tenantUserManager = Substitute.For<ITenantUserManager>();
            tenantUserManager.CreatePendingMembershipAsync(Arg.Any<long>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>()).Returns(request);

            typeof(AccountAppService).GetField("_tenantUserManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_sut, tenantUserManager);

            // Quando
            var result = await _sut.Register(new RegisterInput
            {
                TenantSelectionMode = TenantSelectionMode.JoinExisting,
                ExistingTenantId = 1,
                Name = "Joao",
                Surname = "Silva",
                UserName = "joaosilva",
                EmailAddress = "joao@example.com",
                Password = "P@ssw0rd!"
            });

            // Então
            result.ShouldNotBeNull();
            result.CanLogin.ShouldBeFalse();
            result.TenantId.ShouldBe(1);
            await tenantUserManager.Received(1).CreatePendingMembershipAsync(Arg.Any<long>(), 1, Arg.Any<string>(), Arg.Any<string>());
        }

        #endregion

        private void ConfigurarSetting(string nome, bool valor)
        {
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueAsync(Arg.Any<string>()).Returns(Task.FromResult("true"));
            settingManager.GetSettingValueAsync(nome).Returns(Task.FromResult(valor.ToString().ToLowerInvariant()));
            _sut.SettingManager = settingManager;
        }

        private void ConfigurarRegistroBasico()
        {
            ConfigurarSetting(AppSettings.TenantManagement.AllowSelfRegistration, true);
            _sut.LocalizationManager = Substitute.For<Abp.Localization.ILocalizationManager>();
            _sut.UnitOfWorkManager = ManagerTestHelper.CreateUnitOfWorkManager();
            _sut.UserManager = ManagerTestHelper.CreateUserManager();
        }

        private IObjectMapper CreateObjectMapper()
        {
            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<TenantListDto>>(Arg.Any<object>()).Returns(ci =>
            {
                var source = ci.Arg<object>();
                var count = source is System.Collections.IEnumerable e ? e.Cast<object>().Count() : 1;
                var list = new List<TenantListDto>();
                for (int i = 0; i < count; i++)
                {
                    list.Add(new TenantListDto());
                }
                return list;
            });
            return objectMapper;
        }
    }
}

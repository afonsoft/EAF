using Abp;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization.Accounts;
using Eaf.Middleware.Authorization.Accounts.Dto;
using Eaf.Middleware.Authorization.Impersonation;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.MultiTenancy.Dto;
using Eaf.Middleware.Url;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
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
        private readonly AccountAppService _sut;

        public AccountAppServiceBddTests()
        {
            _userEmailer = Substitute.For<IUserEmailer>();
            _webUrlService = Substitute.For<IWebUrlService>();
            _impersonationManager = Substitute.For<IImpersonationManager>();

            _sut = new AccountAppService(_userEmailer, _webUrlService, _impersonationManager);
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

        #endregion

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
